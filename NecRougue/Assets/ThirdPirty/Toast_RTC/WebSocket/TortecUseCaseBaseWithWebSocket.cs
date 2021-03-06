using System;
using System.Collections;
using System.Collections.Generic;
using Toast.RealTimeCommunication;
using UniRx;
using Unity.WebRTC;
using UnityEngine;

public interface ITortecUseCaseBaseWithWebSocket
{
    bool IsOpen { get; }
    string SelfId { get; }
    void SubscribeMessage<T>(GameObject owner, Action<string, T> res) where T : class;
    void SubscribeMessage<T>(CompositeDisposable owner, Action<string, T> res) where T : class;
    void BroadcastAll<T>(T data) where T : class;
    List<string> ConnectionIds();
    Subject<Unit> OnOpenCallback { get; }
    Subject<string> OnCloseOtherCallback { get; }
}


public abstract class TortecUseCaseBaseWithWebSocket : MonoBehaviour,ITortecUseCaseBaseWithWebSocket
{
    [Serializable]
    protected class Greeting
    {
        public bool needEcho;
    }
    private class SubjectData
    {
        public string connectionId;
        public string data;
    }
    private Dictionary<string,Subject<SubjectData>> _subjects = new Dictionary<string, Subject<SubjectData>>();
    private Subject<string> _onCloseOtherSubject = new Subject<string>();
    public Subject<Unit> OnOpenCallback { get; }  = new Subject<Unit>();
    public Subject<string> OnErrorCallback = new Subject<string>();
    public Subject<Unit> OnCloseCallback = new Subject<Unit>();
    public Subject<string> OnCloseOtherCallback => _onCloseOtherSubject;
    private Dictionary<string,Type> _typeMap = new Dictionary<string, Type>();
    private List<string> _connections = new List<string>();
    private Queue<Unit> _onConnectCallStack = new Queue<Unit>();
    private Queue<string> _onErrorCallStack = new Queue<string>();
    private Queue<WssigPayloadProtocol> _onMessageCallStack = new Queue<WssigPayloadProtocol>();
    private Queue<Unit> _onCloseCallStack = new Queue<Unit>();


    protected WssigClient Peer { get; } = new WssigClient();
    protected string TypeToKey<T>() => typeof(T).ToString();
    public bool IsOpen => Peer.IsOpen;
    public string SelfId => Peer.SelfId;

    protected void Setup()
    {
        Peer.OnOpen = () => _onConnectCallStack?.Enqueue(Unit.Default);
        Peer.OnError = e => _onErrorCallStack?.Enqueue(e);
        Peer.OnMessage = m => _onMessageCallStack?.Enqueue(m);
        Peer.OnClose = () => _onCloseCallStack?.Enqueue(Unit.Default);
    }

    public void SubscribeMessage<T>(GameObject owner,Action<string,T> res) where T : class
    {
        if (!_subjects.ContainsKey(TypeToKey<T>()))
        {
            _subjects.Add(TypeToKey<T>() ,new Subject<SubjectData>());
        }

        if (!_typeMap.ContainsKey(TypeToKey<T>()))
        {
            _typeMap.Add(TypeToKey<T>(),typeof(T));
        }
        var subject = _subjects[TypeToKey<T>()];
        subject.Subscribe(d => res?.Invoke(d.connectionId, JsonUtility.FromJson<T>(d.data))).AddTo(this).AddTo(owner);
    }
    public void SubscribeMessage<T>(CompositeDisposable owner,Action<string,T> res) where T : class
    {
        if (!_subjects.ContainsKey(TypeToKey<T>()))
        {
            _subjects.Add(TypeToKey<T>() ,new Subject<SubjectData>());
        }

        if (!_typeMap.ContainsKey(TypeToKey<T>()))
        {
            _typeMap.Add(TypeToKey<T>(),typeof(T));
        }
        var subject = _subjects[TypeToKey<T>()];
        subject.Subscribe(d => res?.Invoke(d.connectionId, JsonUtility.FromJson<T>(d.data))).AddTo(this).AddTo(owner);
    }
    public void SubscribeOnCloseOther<T>(GameObject owner,Action<string> res) where T : class
    {
        _onCloseOtherSubject.Subscribe(res).AddTo(this).AddTo(owner);
    }

    protected virtual void OnMessage(WssigPayloadProtocol data)
    {
        if (data.command == "sys_greeting")
        {
            var p = JsonUtility.FromJson<Greeting>(data.data);
            if (p.needEcho)
            {
                Peer.Send("sys_greeting",JsonUtility.ToJson(new Greeting()
                {
                    needEcho = false
                }));
            }

            if (!_connections.Contains(data.msgfrom))
            {
                Debug.Log("Greeting from " + data.msgfrom);
                _connections.Add(data.msgfrom);
            }
        }
        else if (data.command == "sys_leave")
        {
            Debug.Log("leave : " + data.msgfrom);
            _connections.Remove(data.msgfrom);
            _onCloseOtherSubject?.OnNext(data.msgfrom);
        }
        else
        {
            if (!_typeMap.ContainsKey(data.command))
            {
                return;
            }

            var d = JsonUtility.FromJson(data.data, _typeMap[data.command]);
            if (!_subjects.ContainsKey(data.command))
            {
                return;
            }
            _subjects[data.command]?.OnNext(new SubjectData()
            {
                connectionId = data.msgfrom,
                data = data.data
            });
        }
    }
    
    public void BroadcastAll<T>(T data) where T : class
    {
        Peer.Send(TypeToKey<T>(),JsonUtility.ToJson(data));
    }

    public List<string> ConnectionIds()
    {
        return _connections;
    }

    void Update()
    {
        //メインスレッド化する
        while (_onConnectCallStack?.Count > 0)
        {
            OnOpenCallback?.OnNext(_onConnectCallStack.Dequeue());
        }
        while (_onErrorCallStack?.Count > 0)
        {
            OnErrorCallback?.OnNext(_onErrorCallStack.Dequeue());
        }
        while (_onMessageCallStack?.Count > 0)
        {
            OnMessage(_onMessageCallStack.Dequeue());
        }
        while (_onCloseCallStack?.Count > 0)
        {
            OnCloseCallback?.OnNext(_onCloseCallStack.Dequeue());
        }
    }

    private void OnDestroy()
    {
        _onCloseOtherSubject?.Dispose();
        OnOpenCallback?.Dispose();
        OnErrorCallback?.Dispose();
        OnCloseCallback?.Dispose();
        Peer?.Dispose();
    }
}

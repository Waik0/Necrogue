using System;
using System.Collections.Generic;
using Toast.RealTimeCommunication;
using UniRx;
using Unity.WebRTC;
using UnityEngine;

public abstract class TortecUseCaseBase : MonoBehaviour
{
    private class SubjectData
    {
        public string connectionId;
        public object data;
    }
    private Dictionary<string,Subject<SubjectData>> _subjects = new Dictionary<string, Subject<SubjectData>>();
    private Dictionary<string,Type> _typeMap = new Dictionary<string, Type>();
    protected abstract TRTCPeer Peer { get; }
    protected string TypeToKey<T>() => typeof(T).ToString();
    public string SelfId => Peer.SelfId;
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
        subject.Subscribe(d=> res?.Invoke(d.connectionId,d.data as T)).AddTo(this).AddTo(owner);
    }

    public List<string> ConnectionIds()
    {
        return Peer.ConnectionIds();
    }
    
    public List<RTCPeerConnection> ConnectionPeers()
    {
        return Peer.ConnectionPeers();
    }

    public Dictionary<RTCPeerConnection, MetaDataModel> PeersMeta()
    {
        return Peer.MetaCache;
    }
    public List<RTCDataChannel> Channels()
    {
        return Peer.Channels();
    }
    public void OpenChannelAll<T>() where T : class
    {
        Peer.CreateChannelAll(TypeToKey<T>());
    }
    public void OpenChannelAll(Type type)
    {
        Debug.Log("Open " + type.ToString());
        Peer.CreateChannelAll(type.ToString());
    }
    public bool CheckAndOpenChannelAll<T>() where T : class
    {
        return Peer.CheckAndOpenChannelAll(TypeToKey<T>());
    }
    public bool CheckAndOpenChannelAll(Type type)
    {
        Debug.Log("Check Open " + type.ToString());
        return Peer.CheckAndOpenChannelAll(type.ToString());
    }
    public void BroadcastAll<T>(T data) where T : class
    {
        Peer.BroadcastMessageChannel(TypeToKey<T>(),JsonUtility.ToJson(data));
    }
    
    public void EndMatching()
    {
        Peer.EndSignaling();
    }
    protected void OnMessage(RTCDataChannel channel, string message)
    {
        var cid = "";
        if (_subjects.ContainsKey(channel.Label))
        {
            var data = JsonUtility.FromJson(message,_typeMap[channel.Label]);
            _subjects[channel.Label] ?.OnNext(new SubjectData(){connectionId =  Peer.ChannelToConnectionId(channel),
                data = data});
        }
    }

}

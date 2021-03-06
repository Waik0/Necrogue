using System;
using System.Collections;
using System.Collections.Generic;
using ModestTree;
using UniRx;
using UnityEngine;
using Zenject;
public interface ICursorMessageReceiver {
    void StartSubscribe(ITortecUseCaseBaseWithWebSocket peer);
    void EndSubscribe();
    Action<CursorData> OnGetCursorData { set; }
}
public class CursorMessageReceiver : ICursorMessageReceiver,ITickable
{
    private CompositeDisposable _compositeDisposable;
    private bool _isActive = false;
    private Dictionary<string,Queue<CursorData>> _cursorQueue = new Dictionary<string, Queue<CursorData>>();
    public Action<CursorData> OnGetCursorData { get; set; }

    public void StartSubscribe(ITortecUseCaseBaseWithWebSocket peer)
    {
        _isActive = true;
        _compositeDisposable = new CompositeDisposable();
        peer.SubscribeMessage<CursorData>(_compositeDisposable,OnReceiveCursorData);
    }
    public void EndSubscribe()
    {
        _isActive = false;
        _compositeDisposable?.Dispose();
    }
    void OnReceiveCursorData(string id,CursorData c)
    {
        //プレイヤーごとにキューにためる
        if (!_cursorQueue.ContainsKey(id))
        {
            _cursorQueue.Add(id, new Queue<CursorData>());
        }
        if(_cursorQueue[id].Count < 20)
            _cursorQueue[id].Enqueue(c);
    }
    void DequeueCursorMessage()
    {
        foreach (var keyValuePair in _cursorQueue)
        {
            if (!keyValuePair.Value.IsEmpty())
            {
                do
                {
                    var c = keyValuePair.Value.Dequeue();
                    if (c != null)
                    {
                        OnGetCursorData?.Invoke(c);
                    }
                } while (keyValuePair.Value.Count > 15);
            }
        }
    }
    public void Tick()
    {
        if(_isActive) 
            DequeueCursorMessage();
    }
}

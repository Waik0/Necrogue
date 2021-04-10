using System;
using System.Collections;
using System.Collections.Generic;
using ModestTree;
using UniRx;
using UnityEngine;

public class CursorDataReceiver //: MonoBehaviour
{
    private CompositeDisposable _compositeDisposable;
    private bool _isActive = false;
    private Dictionary<ulong,Queue<CursorData>> _cursorQueue = new Dictionary<ulong, Queue<CursorData>>();
    public Action<CursorData> OnGetCursorData { get; set; }

    public void StartSubscribe()
    {
        //_isActive = true;
        _compositeDisposable = new CompositeDisposable();
        SteamNetworkManager.SubscribeMessage<CursorData>(OnReceiveCursorData,_compositeDisposable);
    }
    public void EndSubscribe()
    {
        //_isActive = false;
        _compositeDisposable?.Dispose();
    }
    void OnReceiveCursorData(ulong id,CursorData c)
    {
        OnGetCursorData?.Invoke(c);
        return;
        //プレイヤーごとにキューにためる
        if (!_cursorQueue.ContainsKey(id))
        {
            _cursorQueue.Add(id, new Queue<CursorData>());
        }
        if(_cursorQueue[id].Count < 20)
            _cursorQueue[id].Enqueue(c);
    }
#if false
    

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
    void Update()
    {
        if(_isActive) 
            DequeueCursorMessage();
    }
#endif
}
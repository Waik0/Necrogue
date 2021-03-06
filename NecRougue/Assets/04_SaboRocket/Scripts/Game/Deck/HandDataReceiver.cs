using System;
using System.Collections;
using System.Collections.Generic;
using UniRx;
using UnityEngine;
public interface IHandDataReceiver {
    void StartSubscribe(ITortecUseCaseBaseWithWebSocket peer);
    void EndSubscribe();
    Action<HandData> OnGetHandData { set; }
}
public class HandDataReceiver : IHandDataReceiver
{    
    private CompositeDisposable _compositeDisposable;
    public void StartSubscribe(ITortecUseCaseBaseWithWebSocket peer)
    {
        _compositeDisposable?.Dispose();
        _compositeDisposable = new CompositeDisposable();
        peer.SubscribeMessage<HandData>(_compositeDisposable,OnGetHandDataInternal);
    }

    void OnGetHandDataInternal(string id ,HandData data)
    {
        OnGetHandData?.Invoke(data);
    }
    public void EndSubscribe()
    {
        _compositeDisposable?.Dispose();
    }

    public void Dispose()
    {
        _compositeDisposable?.Dispose();
    }

    public Action<HandData> OnGetHandData { get; set; }
}

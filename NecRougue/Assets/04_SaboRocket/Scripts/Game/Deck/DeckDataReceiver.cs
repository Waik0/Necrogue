using System;
using System.Collections;
using System.Collections.Generic;
using UniRx;
using UnityEngine;
public interface IDeckDataReceiver
{
    Action<DeckData> OnGetDeckData { set; }
    void StartSubscribe(ITortecUseCaseBaseWithWebSocket peer);
    void EndSubscribe();
}
public class DeckDataReceiver : IDeckDataReceiver
{ 
    public Action<DeckData> OnGetDeckData { get; set; }
    private CompositeDisposable _compositeDisposable;
    public void StartSubscribe(ITortecUseCaseBaseWithWebSocket peer)
    {
        _compositeDisposable?.Dispose();
        _compositeDisposable = new CompositeDisposable();
        peer.SubscribeMessage<DeckData>(_compositeDisposable,OnGetDeckDataInternal);
    }
    void OnGetDeckDataInternal(string id ,DeckData data)
    {
        OnGetDeckData?.Invoke(data);
    }

    public void EndSubscribe()
    {
        _compositeDisposable?.Dispose();
    }
}

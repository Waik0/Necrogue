using System;
using System.Collections;
using System.Collections.Generic;
using UniRx;
using UnityEngine;

public interface IRollDataReceiver
{
    void StartSubscribe(ITortecUseCaseBaseWithWebSocket peer);
    void EndSubscribe();
    Action<RollData> OnGetRollData { set; }
}
public class RollDataReceiver : IRollDataReceiver
{
    private CompositeDisposable _compositeDisposable;
    public Action<RollData> OnGetRollData { get;set; }
    public void StartSubscribe(ITortecUseCaseBaseWithWebSocket peer)
    {
        EndSubscribe();
        _compositeDisposable = new CompositeDisposable();
        peer.SubscribeMessage<RollData>(_compositeDisposable,OnGetRollDataInternal);
    }

    void OnGetRollDataInternal(string id, RollData rollData)
    {
        OnGetRollData?.Invoke(rollData);
    }
    public void EndSubscribe()
    {
        _compositeDisposable?.Dispose();
    }
}

using System;
using System.Collections;
using System.Collections.Generic;
using UniRx;
using UnityEngine;
using Zenject;
public interface IInputReceiver {
    void StartSubscribe(ITortecUseCaseBaseWithWebSocket peer);
    void EndSubscribe();
    Action<string,InputData> OnGetInputData { set; }
}
public class InputReceiver : IInputReceiver
{

    private CompositeDisposable _disposable;
    public  Action<string,InputData> OnGetInputData { get;set; }

    public void StartSubscribe(ITortecUseCaseBaseWithWebSocket peer)
    {
        EndSubscribe();
        _disposable = new CompositeDisposable();
        peer.SubscribeMessage<InputData>(_disposable,OnReceiveInputData);
    }
    
    public void EndSubscribe()
    {
        _disposable?.Dispose();
    }

    void OnReceiveInputData(string id,InputData inputData)
    {
        OnGetInputData?.Invoke(id,inputData);
    }
}

using System;
using System.Collections;
using System.Collections.Generic;
using UniRx;
using UnityEngine;
using Zenject;
public interface IGameStartReceiver{
    void StartSubscribe(ITortecUseCaseBaseWithWebSocket peer);
    void EndSubscribe();
    Action<GameStartData> OnGameStart { set; }
}
public class GameStartReceiver : IGameStartReceiver
{
    private CompositeDisposable _disposable;
    public Action<GameStartData> OnGameStart { get; set; }
    public void StartSubscribe(ITortecUseCaseBaseWithWebSocket peer)
    {
        EndSubscribe();
        _disposable = new CompositeDisposable();
        peer.SubscribeMessage<GameStartData>(_disposable,OnReceiveGameStart);
    }
    public void EndSubscribe()
    {
        _disposable?.Dispose();
    }

    void OnReceiveGameStart(string id, GameStartData gameStartData)
    {
        OnGameStart?.Invoke(gameStartData);   
    }
}

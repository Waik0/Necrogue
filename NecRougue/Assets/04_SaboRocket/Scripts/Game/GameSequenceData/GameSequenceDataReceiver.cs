using System;
using System.Collections;
using System.Collections.Generic;
using UniRx;
using UnityEngine;
using Zenject;

/// <summary>
/// ゲーム開始、ゲーム
/// ゲーム開始、次のターンのメッセージを受け取る
/// </summary>
public interface IGameSequenceDataReceiver
{
    void StartSubscribe(ITortecUseCaseBaseWithWebSocket peer);
    void EndSubscribe();
    Action<string> OnGetReady { set; }//Readyを送ったプレイヤー
    Action<int,string> OnGetNextTurn { set; }//ターン数、ターンプレイヤー
    Action OnGetGameOver { set; }
}
public class GameSequenceDataReceiver : IGameSequenceDataReceiver
{
    private CompositeDisposable _compositeDisposable;
    public Action<string> OnGetReady { get; set; }//Readyを送ったプレイヤー
    public Action<int,string> OnGetNextTurn { get; set; }//ターン数、ターンプレイヤー
    public Action OnGetGameOver { get; set; }
    public void StartSubscribe(ITortecUseCaseBaseWithWebSocket peer)
    {
        Debug.Log("Subscribe");
        EndSubscribe();
        _compositeDisposable = new CompositeDisposable();
        peer.SubscribeMessage<GameSequenceData>(_compositeDisposable,OnReceiveGameSequenceDataHost);
    }
    public void EndSubscribe()
    {
        _compositeDisposable?.Dispose();
    }
    void OnReceiveGameSequenceDataHost(string id, GameSequenceData gameSequenceData) 
    {
        switch (gameSequenceData.command)
        {
            case GameSequenceData.Command.Ready:
                Debug.Log("Ready");
                OnGetReady?.Invoke(id);
                break;
            case GameSequenceData.Command.NextTurn:
                OnGetNextTurn?.Invoke(gameSequenceData.currentTurn,gameSequenceData.currentPlayer);
                break;
            case GameSequenceData.Command.GameOver:
                break;
        }
    }
  
}

using System;
using System.Collections;
using System.Collections.Generic;
using JetBrains.Annotations;
using Toast;
using Toast.RealTimeCommunication;
using UniRx;
using UnityEngine;
using Zenject;

public class MatchingHostSequence : IDisposable
{
    public enum State
    {
        CreateRoom,
        WaitPlayer,
        PrepareToGame,
        ToGame,
        ToTitle,
    }
    private Statemachine<State> _statemachine;
    private ITortecHostUseCaseWithWebSocket _hostUseCase;
    //receiver
    private IGameSequenceDataReceiver _gameSequenceDataReceiver;
    private IGameStartReceiver _gameStartReceiver;
    //sender
    private IGameStartDataSender _gameStartDataSender;
    //other
    private ReadyStateChecker _readyStateChecker;
    private Ping _ping;
    //events
    private Subject<bool> _onActiveSequence = new Subject<bool>();
    private Subject<string> _onCreateRoom = new Subject<string>();
    
    #region public
    public IObservable<bool> OnActiveSequence => _onActiveSequence;
    public IObservable<string> OnCreateRoom => _onCreateRoom;
    public State CurrentState => _statemachine.Current;
    public void ResetSequence()
    {
        _onActiveSequence?.OnNext(true);
        _statemachine.Next(State.CreateRoom);
    }

    public void StopSequence()
    {
        _onActiveSequence?.OnNext(false);
    }
    public void UpdateSequence()
    {
        _statemachine.Update();
    }
    public void ToGame()
    {
        if (_statemachine.Current == State.WaitPlayer)
        {
            _statemachine.Next(State.PrepareToGame);
        }
    }
    #endregion

    [Inject]
    void Inject(
        ITortecHostUseCaseWithWebSocket hostUseCase,
        IGameSequenceDataReceiver gameSequenceDataReceiver,
        IGameStartReceiver gameStartReceiver,
        ReadyStateChecker readyStateChecker,
        IGameStartDataSender gameStartDataSender,
        Ping ping
    )
    {
        _ping = ping;
        _hostUseCase = hostUseCase;
        _gameSequenceDataReceiver = gameSequenceDataReceiver;
        _gameStartReceiver = gameStartReceiver;
        _gameStartDataSender = gameStartDataSender;
        _readyStateChecker = readyStateChecker;
        _statemachine = new Statemachine<State>();
        _statemachine.Init(this);
        
    }

    void ReceiverInit()
    {
        _readyStateChecker.Init();
        _gameStartReceiver.StartSubscribe(_hostUseCase);
        _gameStartReceiver.OnGameStart = gameStartData =>
        {
            //_readyStateChecker.SetPlayers(gameStartData.players);
            _statemachine.Next(State.ToGame);
        };
        _gameSequenceDataReceiver.StartSubscribe(_hostUseCase);
        _gameSequenceDataReceiver.OnGetReady = s => _readyStateChecker.AddReady(s,true);
        _ping.StartPing(_hostUseCase);
    }
    IEnumerator CreateRoom()
    {
        var roomName = _hostUseCase.CreateRoom();
        _onCreateRoom?.OnNext(roomName);
        _statemachine.Next(State.WaitPlayer);
        yield return null;
    }

    IEnumerator WaitPlayer()
    {
        ReceiverInit();
        yield return null;
    }
    IEnumerator PrepareToGame()
    {
        _gameStartDataSender.SendStart(_hostUseCase);
        yield return null;
    }

    public void Dispose()
    {
        _onActiveSequence?.Dispose();
    }
}

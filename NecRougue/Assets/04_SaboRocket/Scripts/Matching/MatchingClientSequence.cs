using System;
using System.Collections;
using System.Collections.Generic;
using Toast;
using UniRx;
using UnityEngine;
using UnityEngine.UI;
using Zenject;

public class MatchingClientSequence : IDisposable
{

    public enum State
    {
        Wait,
        JoinRoom,
        ToGame,
        ToTitle,
    }

    private Statemachine<State> _statemachine = new Statemachine<State>();
    private ITortecClientUseCaseWithWebSocket _clientUseCase;
    private Subject<bool> _onActiveSequence = new Subject<bool>();
    private string _roomName;
    //receiver
    private PlayerDataUseCase _playerDataUseCase;
    private IGameStartReceiver _gameStartReceiver;
    //other
    private Ping _ping;
    #region public
    public IObservable<bool> OnActiveSequence => _onActiveSequence;
    public State CurrentState => _statemachine.Current;
    public void ResetSequence()
    {
        _onActiveSequence?.OnNext(true);
        _statemachine.Next(State.Wait);
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
        _statemachine.Next(State.ToGame);
    }
    public void Join(string room)
    {
        _roomName = room;
        _statemachine.Next(State.JoinRoom);
    }
    #endregion

    [Inject]
    void Inject(
        ITortecClientUseCaseWithWebSocket clientUseCase,
        IGameStartReceiver gameStartReceiver,
        PlayerDataUseCase playerDataUseCase,
        Ping ping
    )
    {
        _ping = ping;
        _clientUseCase = clientUseCase;
        _statemachine = new Statemachine<State>();
        _statemachine.Init(this);
        _playerDataUseCase = playerDataUseCase;
        _gameStartReceiver = gameStartReceiver;
        ReceiverInit();
    }

    void ReceiverInit()
    {
        _gameStartReceiver.StartSubscribe(_clientUseCase);
        _gameStartReceiver.OnGameStart = gameStartData =>
        {
            //_playerDataUseCase.SetPlayerList(gameStartData.players);
            Debug.Log("プレイヤー" + gameStartData.players.Count+ "人");
            _statemachine.Next(State.ToGame);
        };
        _clientUseCase.OnOpenCallback.Subscribe(_ => _ping.StartPing(_clientUseCase));
    }
    IEnumerator Wait()
    {
        _roomName = PlayerPrefs.GetString("roomNameCache","");
        yield return null;
    }
    IEnumerator JoinRoom()
    {
        if (!_clientUseCase.IsOpen)
        {
            _clientUseCase.JoinRoom(_roomName);
        }
        yield return null;
    }

    public void Dispose()
    {
        _onActiveSequence?.Dispose();
    }
}

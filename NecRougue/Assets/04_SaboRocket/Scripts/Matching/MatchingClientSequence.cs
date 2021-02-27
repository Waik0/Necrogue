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
    private ITortecClientUseCase _clientUseCase;
    private Subject<bool> _onActiveSequence = new Subject<bool>();
    private string _roomName;
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

    public void Join(string room)
    {
        _roomName = room;
        _statemachine.Next(State.JoinRoom);
    }
    #endregion

    [Inject]
    void Inject(
        ITortecClientUseCase clientUseCase
    )
    {
        _clientUseCase = clientUseCase;
        _statemachine = new Statemachine<State>();
        _statemachine.Init(this);
    }

    IEnumerator Wait()
    {
        _roomName = PlayerPrefs.GetString("roomNameCache","");
        yield return null;
    }
    IEnumerator JoinRoom()
    {
        _clientUseCase.JoinRoom(_roomName);
        yield return null;
        _statemachine.Next(State.ToGame);
    }

    public void Dispose()
    {
        _onActiveSequence?.Dispose();
    }
}

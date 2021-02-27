using System;
using System.Collections;
using System.Collections.Generic;
using Toast;
using UniRx;
using UnityEngine;
using Zenject;

public class MatchingHostSequence : IDisposable
{
    public enum State
    {
        CreateRoom,
        ToGame,
        ToTitle,
    }
    private Statemachine<State> _statemachine;
    private ITortecHostUseCase _hostUseCase;
    private Subject<bool> _onActiveSequence = new Subject<bool>();
    
    #region public

    public IObservable<bool> OnActiveSequence => _onActiveSequence;
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
        _statemachine.Next(State.ToGame);
    }
    #endregion

    [Inject]
    void Inject(
        ITortecHostUseCase hostUseCase
    )
    {
        _hostUseCase = hostUseCase;
        _statemachine = new Statemachine<State>();
        _statemachine.Init(this);
    }

    IEnumerator CreateRoom()
    {
        var roomName = _hostUseCase.CreateRoom();
        yield return null;
    }

    public void Dispose()
    {
        _onActiveSequence?.Dispose();
    }
}

 using System;
 using System.Collections;
using System.Collections.Generic;
 using Toast;
 using UniRx;
 using UnityEngine;
 using Zenject;

 public class TitleSequence : IDisposable
{
    public enum State
    {
        Demo,
        ToMatchingHost,
        ToMatchingClient,
    }
    private Statemachine<State> _statemachine;
    private Subject<bool> _onActiveSequence = new Subject<bool>();
    public IObservable<bool> OnActiveSequence => _onActiveSequence;
    #region public

    public State CurrentState => _statemachine.Current;
    public void ResetSequence()
    {
        _onActiveSequence?.OnNext(true);
        _statemachine.Next(State.Demo);
    }
    public void StopSequence()
    {
        _onActiveSequence?.OnNext(false);
    }
    public void UpdateSequence()
    {
        _statemachine.Update();
    }
    public void ToHost()
    {
        _statemachine.Next(State.ToMatchingHost);
    }

    public void ToClient()
    {
        _statemachine.Next(State.ToMatchingClient);
    }
    #endregion
    [Inject]
    void Inject()
    {
        _statemachine = new Statemachine<State>();
        _statemachine.Init(this);
    }
    IEnumerator Demo()
    {
        yield return null;
    }

    public void Dispose()
    {
        _onActiveSequence?.Dispose();
    }
}

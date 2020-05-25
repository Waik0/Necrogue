using System.Collections;
using System.Collections.Generic;
using Toast;
using UnityEngine;

public class EventSequence : ISequence<bool>
{
    enum State
    {
        Init,
        SelectCard,
        SelectButton,
        End
    }
    private Statemachine<State> _statemachine;
    private EventPresenter _eventPresenter = new EventPresenter();
    private DeckEditSequence _deckEditSequence = new DeckEditSequence();
    private BattleDataUseCase _battleDataUseCase = new BattleDataUseCase();
    private BattlePresenter _battlePresenter = new BattlePresenter();
    public EventSequence()
    {
        _statemachine = new Statemachine<State>();
    }

    public void Inject(BattleDataUseCase battleDataUseCase,PlayerDataUseCase playerDataUseCase)
    {
        _deckEditSequence.Inject(battleDataUseCase,_battlePresenter);
    }
    public void ResetSequence()
    {
        _statemachine.Next(State.Init);
    }

    public bool UpdateSequence()
    {
        _statemachine.Update();
        return _statemachine.Current != State.End;
    }

    IEnumerator Init()
    {
        _deckEditSequence.ResetSequence();
        yield return null;
    }

    IEnumerator SelectCard()
    {
        var isContinue = _deckEditSequence.UpdateSequence();
        yield return null;
    }
    IEnumerator End()
    {
        yield return null;
    }
}

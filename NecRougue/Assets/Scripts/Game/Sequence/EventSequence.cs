using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Toast;
using UnityEngine;

public enum GameEventType
{
    None,
    FirstCard,
}
public class EventRootSequence : ISequence<bool>
{
    enum State
    {
        Init,
        Event,
        End
    }
    private Statemachine<State> _statemachine;
    private EventSequence _eventSequence;
    public EventRootSequence(GameEventType type)
    {
        _statemachine = new Statemachine<State>();
        _statemachine.Init(this);
        switch (type)
        {
            case GameEventType.None:
                _eventSequence = null;
                break;
            case GameEventType.FirstCard:
                _eventSequence = new FirstCardEventSequence();
                break;
            default:
                throw new ArgumentOutOfRangeException(nameof(type), type, null);
        }
    }

    public void Inject(PlayerDataUseCase playerDataUseCase)
    {
        
        _eventSequence.Inject(playerDataUseCase);
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
        DebugLog.Function(this,1);
        _eventSequence.ResetSequence();
        _statemachine.Next(State.Event);
        yield return null;
    }

    IEnumerator Event()
    { 
        DebugLog.Function(this,1);
        while (_eventSequence.UpdateSequence())
        {
            yield return null;
        }

        _statemachine.Next(State.End);

    }
    IEnumerator End()
    {
        DebugLog.Function(this,1);
        yield return null;
    }
#if DEBUG
    public void DebugUI1()
    {
        _eventSequence.DebugUI1();
    }
#endif
}

public abstract class EventSequence : ISequence<bool>
{
    
    protected PlayerDataUseCase _playerDataUseCase = new PlayerDataUseCase();
    public virtual void Inject(PlayerDataUseCase playerDataUseCase)
    {
        _playerDataUseCase = playerDataUseCase;
        
    }

    public abstract void ResetSequence();

    public abstract bool UpdateSequence();
#if DEBUG
    public abstract void DebugUI1();

#endif
}

public class FirstCardEventSequence : EventSequence
{
    enum State
    {
        Init,
        SelectCard,
        End
    }

    protected FirstCardEventPresenter _firstCardEventPresenter = new FirstCardEventPresenter();
    private SelectCardUI _selectCardUi = new SelectCardUI();
    private Statemachine<State> _statemachine;
    private int _count;
    public FirstCardEventSequence()
    {
        _statemachine = new Statemachine<State>();
        _statemachine.Init(this);
    }
    public override void Inject( PlayerDataUseCase playerDataUseCase)
    {
        base.Inject(playerDataUseCase);
        _firstCardEventPresenter.Inject(playerDataUseCase);
    }
    public override void ResetSequence()
    {
        _statemachine.Next(State.Init);
    }

    public override bool UpdateSequence()
    {
        _statemachine.Update();
        return _statemachine.Current != State.End;
    }

    IEnumerator Init()
    {
        DebugLog.Function(this,2);
        _statemachine.Next(State.SelectCard);
        yield return null;
    }

    IEnumerator SelectCard()
    {
        DebugLog.Function(this,2);
        _count = 0;
        while (!_firstCardEventPresenter.IsAllSelected())
        {
            _count++;
            _selectCardUi.SetCards(_firstCardEventPresenter.GetRandomId());
            while (!_selectCardUi.IsSelect())
            {
                yield return null;
            }
            _firstCardEventPresenter.AddStock(_selectCardUi.SelectIndex());

        }
        _statemachine.Next(State.End);
    }
    IEnumerator End()
    {
        DebugLog.Function(this,2);
        yield return null;
    }
#if DEBUG
    public override void DebugUI1()
    {
        GUILayout.Label(_count + "枚目");
        _selectCardUi.DebugUI1();
    }
#endif
}

public class SelectCardUI
{
    private CardData[] _ids;
    private int _selectIndex = -1;
    public void SetCards(CardData[] ids)
    {
        _ids = ids;
        _selectIndex = -1;
    }

    public bool IsSelect() => _selectIndex > -1;
    public int SelectIndex() => _selectIndex;
#if DEBUG
    public void DebugUI1()
    {
        GUILayout.Label("取得するカードを選択");
        GUILayout.BeginHorizontal();
        var count = 0;
        if (_ids == null)
        {
            return;
        }
        foreach (var id in _ids)
        {
            GUILayout.BeginVertical("box",GUILayout.Width(100));
            GUILayout.Label(id.Name);
            if (GUILayout.Button("もらう"))
            {
                _selectIndex = count;
            }
            GUILayout.EndVertical();
            count++;
        }
        
        GUILayout.EndHorizontal();
    }
#endif

}
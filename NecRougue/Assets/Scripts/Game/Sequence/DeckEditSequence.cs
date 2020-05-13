using System.Collections;
using System.Collections.Generic;
using Toast;
using UnityEngine;

public class DeckEditSequence : Sequence<bool>
{
    //----------------------------------------------------------------------------------------------------------------------
    // Enum定義
    //----------------------------------------------------------------------------------------------------------------------

    public enum State
    {
        Init,
        WaitForInput,
        ChangeOrder,
        Summon,
        End
    }
    private class ChangeOrderArguments
    {
        public int Select;
        public int Change;
    }
    private class SummonArguments
    {
        public int StockOrder;
        public int DeckOrder;
    }
    //----------------------------------------------------------------------------------------------------------------------
    // フィールド
    //----------------------------------------------------------------------------------------------------------------------


    private Statemachine<State> _statemachine;
    private BattleDataUseCase _battleDataUseCase;
    private BattlePresenter _battlePresenter;
    private DeckEditUI _deckEditUI = new DeckEditUI();

    private ChangeOrderArguments _changeOrderArgumentsCache;
    private SummonArguments _summonArguments;
    //----------------------------------------------------------------------------------------------------------------------
    // パブリックメソッド
    //----------------------------------------------------------------------------------------------------------------------

    public DeckEditSequence()
    {
        _statemachine = new Statemachine<State>();
        _statemachine.Init(this);
     
    }
    public void Inject(
        BattleDataUseCase battleDataUseCase,
        BattlePresenter battlePresenter)
    {
        _battleDataUseCase = battleDataUseCase;
        _deckEditUI.Inject(battleDataUseCase);
        _deckEditUI.OnChangeCard.AddListener(StartChangeOrder);
        _deckEditUI.OnSummonEvent.AddListener(StartSummon);
        _battlePresenter = battlePresenter;
    }
    public override void ResetSequence()
    {
        _statemachine.Next(State.Init);
    }
    public void ResetSelected()
    {
        _deckEditUI.ResetSelect();
    }
    public override bool UpdateSequence()
    {

        _statemachine.Update();
        return _statemachine.Current != State.End;
    }
    //----------------------------------------------------------------------------------------------------------------------
    // プライベートメソッド
    //----------------------------------------------------------------------------------------------------------------------
    private void StartChangeOrder(int a,int b)
    {
        DebugLog.Function(this, 2);
        _changeOrderArgumentsCache = new ChangeOrderArguments()
        {
            Select = a,
            Change = b
        };
        _statemachine.Next(State.ChangeOrder);
    }

    private void StartSummon(int a, int b)
    {
        _summonArguments = new SummonArguments()
        {
            DeckOrder = b,
            StockOrder = a
        };
        _statemachine.Next(State.Summon);
    }
    //----------------------------------------------------------------------------------------------------------------------
    // シーケンス
    //----------------------------------------------------------------------------------------------------------------------

    IEnumerator Init()
    {
        DebugLog.Function(this,2);
        _statemachine.Next(State.WaitForInput);
        yield return null;
    }

    IEnumerator WaitForInput()
    {
        DebugLog.Function(this,2);
        _deckEditUI.ResetUI();
        _battleDataUseCase.ChangeState(BattleState.DeckPrepare);
        _battlePresenter.OnCommand(new BattleCommand().Generate(_battleDataUseCase.GetSnapShot()));

        while (!_deckEditUI.UpdateUI())
        {
            yield return null;
        }
        Debug.Log("End");
        _statemachine.Next(State.End);
    }
    IEnumerator ChangeOrder()
    {
        DebugLog.Function(this,2);
        var result = _battleDataUseCase.ChangeCard(_changeOrderArgumentsCache.Select, _changeOrderArgumentsCache.Change);
        _statemachine.Next(State.WaitForInput);
        yield return null;
        
    }
    IEnumerator Summon()
    {
        DebugLog.Function(this, 2);
 
        _battleDataUseCase.Summon(_summonArguments.StockOrder, _summonArguments.DeckOrder);
        _battleDataUseCase.ChangeState(BattleState.Ability);
        _battleDataUseCase.ResolveAbilityAll(
            AbilityTimingType.SummonOwn,
            ss =>
                _battlePresenter?.OnCommand(new BattleCommand().Generate(ss)),
            _battleDataUseCase.GetOperationPlayerIndex(),
            _summonArguments.DeckOrder);
        _battleDataUseCase.ResolveAbilityAll(AbilityTimingType.SummonRace,
            ss =>
                _battlePresenter?.OnCommand(new BattleCommand().Generate(ss)));
        _statemachine.Next(State.WaitForInput);
        yield return null;
    }
#if DEBUG
    public void DebugUI()
    {
        //GUILayout.Label("[ DECK EDIT ] STATE : " + _statemachine.Current.ToString());
        _deckEditUI.DebugUI();

    }
    public void DebugUI2()
    {
        _deckEditUI.DebugUI2();
    }
#endif
}

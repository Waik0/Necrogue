using System.Collections;
using System.Collections.Generic;
using Toast;
using UnityEngine;

public class ShopSequence : Sequence<bool>
{

    public enum State
    {
        Init,
        WaitForInput,
        End
    }
    //----------------------------------------------------------------------------------------------------------------------
    // フィールド
    //----------------------------------------------------------------------------------------------------------------------


    private Statemachine<State> _statemachine;
    private PlayerDataUseCase _playerDataUseCase;
    private BattleDataUseCase _battleDataUseCase = new BattleDataUseCase();
    private BattlePresenter _battlePresenter = new BattlePresenter();
    private DeckEditSequence _deckEditSequence = new DeckEditSequence();
    private ShopUI _shopUI = new ShopUI();
    private ShopDataUseCase _shopDataUseCase = new ShopDataUseCase();
    //private BattleDataUseCase battleDataUseCase = new BattleDataUseCase();
    //private int _defender;
    //----------------------------------------------------------------------------------------------------------------------
    // パブリックメソッド
    //----------------------------------------------------------------------------------------------------------------------

    public ShopSequence()
    {
        _statemachine = new Statemachine<State>();
        _statemachine.Init(this);
    }
    public void Inject(PlayerDataUseCase playerDataUseCase)
    {
        _playerDataUseCase = playerDataUseCase;
        _battlePresenter.Inject(_battleDataUseCase);
        _deckEditSequence.Inject(_battleDataUseCase, _battlePresenter);
        _shopUI.Inject(_shopDataUseCase,_battleDataUseCase,_playerDataUseCase);
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
    //----------------------------------------------------------------------------------------------------------------------
    // シーケンス
    //----------------------------------------------------------------------------------------------------------------------
    IEnumerator Init()
    {
        _battleDataUseCase.ResetData();
        _battleDataUseCase.SetPlayer(new BattlePlayerData().Generate(_playerDataUseCase.Data));
        _battlePresenter.Reset();
        _deckEditSequence.ResetSequence();
        _shopDataUseCase.LotteryMonsters(_playerDataUseCase.Data.ShopLevel);
        _shopUI.ResetUI();
        _statemachine.Next(State.WaitForInput);
        yield return null;
    }
    IEnumerator WaitForInput()
    {
        
        while (_shopUI.UpdateUI())
        {
            var isNotEndDeckEditSequence = _deckEditSequence.UpdateSequence();
            _battlePresenter.UpdateCommandProcess();
            yield return null;
        }
        _statemachine.Next(State.End);
        //yield return null;
    }
#if DEBUG
    public void DebugUI()
    {
        switch (_statemachine.Current)
        {
            case State.Init:
                break;
            case State.WaitForInput:
                _shopUI.DebugUI();
                _battlePresenter.DebugUI();
                _deckEditSequence.DebugUI();
              
                break;
            case State.End:
                break;
            default:
                break;
        }
    }
    public void DebugUI2()
    {
        _shopUI.DebugUI2();
    }
#endif
}

using System.Collections;
using System.Collections.Generic;
using System.Linq;
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
    private MapDataUseCase _mapDataUseCase;
    private int _order = -1;
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
    public void Inject(PlayerDataUseCase playerDataUseCase,MapDataUseCase mapDataUseCase)
    {
        _mapDataUseCase = mapDataUseCase;
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
    int ShopLevelUpPrice(int shopLevel)
    {
        return Mathf.Max(shopLevel * 10 - _mapDataUseCase.CurrentDepth(), 1);
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
        _shopDataUseCase.ResetData(
            _playerDataUseCase.GetShopLevel(),
             ShopLevelUpPrice(_playerDataUseCase.GetShopLevel()));
        _shopDataUseCase.LotteryMonsters();

        _statemachine.Next(State.WaitForInput);
        yield return null;
    }
    IEnumerator WaitForInput()
    {
        var isEnd = false;
        _shopUI.ResetUI();
        _shopUI.OnBuy = Buy;
        _shopUI.OnSell = Sell;
        _shopUI.OnLevelUp = ShopLevelUp;
        _shopUI.OnReload = Reroll;
        _shopUI.OnEnd = () => isEnd = true;
        while (!isEnd)
        {
            _shopUI.UpdateUI();
            var isNotEndDeckEditSequence = _deckEditSequence.UpdateSequence();
            _battlePresenter.UpdateCommandProcess();
            yield return null;
        }
        _playerDataUseCase.SetDeck(_battleDataUseCase.GetRemainDecks().Select(_ => new CardData().Generate(_)).ToList());
        _playerDataUseCase.SetStock(_battleDataUseCase.GetRemainStocks().Select(_ => new CardData().Generate(_)).ToList());
        _playerDataUseCase.SetShopLevel(_shopDataUseCase.GetShopLevel());
        _statemachine.Next(State.End);
        //yield return null;
    }
    void ShopLevelUp()
    {
        var price = _shopDataUseCase.GetShopLevelUpPrice();
        var canpay = _playerDataUseCase.Pay(price);
        if (canpay)
        {
            _shopDataUseCase.SetShopLevel(_shopDataUseCase.GetShopLevel() + 1);
            _shopDataUseCase.SetShopLevelUpPrice(ShopLevelUpPrice(_shopDataUseCase.GetShopLevel()));
        }

    }
    void Reroll()
    {
        var canpay = _playerDataUseCase.Pay(2);
        if (canpay)
        {
            _shopDataUseCase.LotteryMonsters();
        }
    }
    void Buy(int order)
    {
        var card = _shopDataUseCase.GetBattleCardData(order);
        var canpay = _playerDataUseCase.Pay(3);
        if (canpay)
        {
            _shopDataUseCase.Remove(order);
            _battleDataUseCase.AddStock(card.Id);
        }


    }
    void Sell(int order)
    {
        var card = _battleDataUseCase.GetOperationPlayer().Deck[order];
        var price = 1;
        _playerDataUseCase.AddGold(price);
        _battleDataUseCase.RemoveDeckCard(order);
        _battleDataUseCase.ChangeState(BattleState.Sell);
        _battlePresenter?.OnCommand(new BattleCommand().Generate(_battleDataUseCase.GetSnapShot()));
        _deckEditSequence.ResetSelected();
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
        _battlePresenter.DebugUI2();
        _shopUI.DebugUI2();
    }
#endif
}

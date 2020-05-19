using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class ShopUI : IModalUI
{
    ShopDataUseCase _shopDataUseCase;
    BattleDataUseCase _battleDataUseCase;
    PlayerDataUseCase _playerDataUseCase;

    public Action<int> OnBuy;
    public Action<long> OnSell;
    public Action OnEnd;
    public Action OnLevelUp;
    public Action OnReload;
    public void Inject(ShopDataUseCase shopDataUseCase,
        BattleDataUseCase battleDataUseCase,
        PlayerDataUseCase playerDataUseCase)
    {
        _shopDataUseCase = shopDataUseCase;
        _battleDataUseCase = battleDataUseCase;
        _playerDataUseCase = playerDataUseCase;
    }
    public void ResetUI()
    {
    
    }

    public bool UpdateUI()
    {
        return true;
    }
    void Buy(int order)
    {
        OnBuy?.Invoke(order);
    }
    void Sell(long unique)
    {
        OnSell?.Invoke(unique);
    }
    void End()
    {
        OnEnd?.Invoke();
    }
    void LevelUp()
    {
        OnLevelUp?.Invoke();
    }
    void Reroll()
    {
        OnReload?.Invoke();
    }
    /*
    bool Buy(int order)
    {
        var card = _shopDataUseCase.GetBattleCardData(order);
        var canpay = _playerDataUseCase.Pay(3);
        if (canpay)
        {
            _shopDataUseCase.Remove(order);
            _battleDataUseCase.AddStock(card.Id);
        }
        return canpay;
        

    }
    bool Sell(int order)
    {
        var card = _battleDataUseCase.GetOperationPlayer().Deck[order];
        var price = 1;
        _battleDataUseCase.AddGold(price);
        _battleDataUseCase.RemoveDeckCard(order);
        return true;
    }
    */
#if DEBUG
    public void DebugUI()
    {
        var width = GUILayout.Width(Screen.width / 7);
        if(_shopDataUseCase.Cards() == null){
            return;
        }
        GUILayout.Label("ショップレベル : " + _shopDataUseCase.GetShopLevel());
        GUILayout.BeginHorizontal();
        for (var i = 0;i < _shopDataUseCase.Cards().Count; i ++)
        {
            var data = _shopDataUseCase.GetBattleCardData(i);
            GUILayout.BeginVertical("box",width);
            GUILayout.Label("レア度 : "+data.Rarity.ToString());
            GUILayout.Label(data.Name.ToString());
            GUILayout.Label($"<color=green>H: { data.Hp,-3}</color> <color=red>A: { data.Attack,-3}</color>");
            if (GUILayout.Button("買う"))
            {
                Buy(i);
            }

            GUILayout.EndVertical();
        }
        GUILayout.EndHorizontal();
        GUILayout.BeginHorizontal();
        foreach (var battleCard in _battleDataUseCase.GetOperationPlayer().Deck)
        {
            if (GUILayout.Button("売る", width))
            {
                Sell(battleCard.Unique);
            }

        }

        GUILayout.EndHorizontal();
    }
    public void DebugUI2()
    {
        if(_shopDataUseCase.Data == null)
        {
            return;
        }
        var height = GUILayout.Height(Screen.height / 5);
        if (GUILayout.Button($"レベルアップ\n{_shopDataUseCase.GetShopLevelUpPrice()} G",height))
        {
            LevelUp();
        }
        if (GUILayout.Button("リロード \n 2 G",height))
        {
            Reroll();
        }

        if (GUILayout.Button("終了",height))
        {
            End();
        }
    }
#endif
}

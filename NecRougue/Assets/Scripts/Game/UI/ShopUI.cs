using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ShopUI : IModalUI
{
    ShopDataUseCase _shopDataUseCase;
    BattleDataUseCase _battleDataUseCase;
    PlayerDataUseCase _playerDataUseCase;
    bool _end;
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
        _end = false;
    }

    public bool UpdateUI()
    {
        return !_end;
    }
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
#if DEBUG
    public void DebugUI()
    {
        var width = GUILayout.Width(Screen.width / 7);
        if(_shopDataUseCase.Cards() == null){
            return;
        }
        GUILayout.BeginHorizontal();
        for (var i = 0;i < _shopDataUseCase.Cards().Count; i ++)
        {
            var data = _shopDataUseCase.GetBattleCardData(i);
            GUILayout.BeginVertical("box",width);
            GUILayout.Label("レア度 : "+data.Rarity.ToString());
            GUILayout.Label(data.Name.ToString());
            GUILayout.Label($"<color=green>H: { data.Hp.Current,-3}</color> <color=red>A: { data.Attack.Current,-3}</color>");
            if (GUILayout.Button("買う"))
            {
                Buy(i);
            }

            GUILayout.EndVertical();
        }
        GUILayout.EndHorizontal();
    }
    public void DebugUI2()
    {
        if (GUILayout.Button("終了"))
        {
            _end = true;
        }
    }
#endif
}

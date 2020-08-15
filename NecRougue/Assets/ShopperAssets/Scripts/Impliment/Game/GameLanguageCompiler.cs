using System;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using ShopperAssets.Scripts.Game;
using ShopperAssets.Scripts.Interface.Game;
using UnityEngine;
using Zenject;
using Random = UnityEngine.Random;

public class AbilityResolver
{
    public enum AbilityTiming
    {
        Get,//手札に入れた時
        Use,//手札から使ったとき
        Trash,//すてられたとき
        EndTurn,
    }
    public enum AbilityCommands
    {
        //プラス効果
        GetCard,//ショップから無償で得る
        AttackEnemy,//param1攻撃
        Coin,//param1コイン入手
        AttackDeck,//山札の枚数だけダメージ増加
        //マイナス効果
        //その他
        Remove,//このアビリティを使った段階で手札から除外
    }
    [Inject] private IShopUsecase _shopUsecase;
    [Inject] private IEnemyUsecase _enemyUsecase;
    [Inject] private IPlayerUsecase _playerUsecase;
    public Dictionary<AbilityCommands, Action<string,int,int>> Commands { get; private set; }

    public AbilityResolver()
    {
        InitCommands();
    } 
 
    void InitCommands()
    {
        Commands = new Dictionary<AbilityCommands,  Action<string,int,int>>()
        {
            { AbilityCommands.GetCard, (guid, p1, p2) =>
            {//ランダム購入
                var id = _shopUsecase.Goods[Random.Range(0, _shopUsecase.Goods.Count - 1)].GUID;
                var goods = _shopUsecase.Buy(guid);
                if (goods != null)
                {
                    _playerUsecase.AddHand(goods);
                    UseAbility(AbilityTiming.Get, goods);
                }
            }},
            { AbilityCommands.AttackEnemy, (guid, p1, p2) =>
            {//敵攻撃
                _enemyUsecase.Damage(p2,p1);
            }},
            { AbilityCommands.Remove , (guid, p1, p2) =>
            {
                _playerUsecase.RemoveHand(guid);
            }},
            { AbilityCommands.Coin , (guid, p1, p2) =>
            {
                _playerUsecase.AddCoin(p1);
            }},
            { AbilityCommands.AttackDeck , (guid, p1, p2) =>
            {
                _enemyUsecase.Damage(p2,_playerUsecase.Deck.Count / 5);
            }}
        };
    }
    /// <summary>
    /// Ability発動
    /// </summary>
    /// <param name="nowTiming"></param>
    /// <param name="card"></param>
    public void UseAbility(AbilityResolver.AbilityTiming nowTiming, CardModel card)
    {
        foreach (var abilityModel in card.Abilities)
        {
            if (abilityModel.Timing == nowTiming)
            {
                Commands.TryGetValue(abilityModel.Command, out var command);
                Debug.Log($"{card.Name} , {abilityModel.Name}" );
                command?.Invoke(card.GUID,abilityModel.AbilityParam1,abilityModel.AbilityParam2);
            }
        }
    }

}

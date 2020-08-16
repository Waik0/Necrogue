using System;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using ShopperAssets.Scripts.Game;
using ShopperAssets.Scripts.Interface.Game;
using UnityEngine;
using UnityEngine.Events;
using Zenject;
using Random = UnityEngine.Random;

public class AbilityResolver
{
    public enum AbilityTiming
    {
        Get = 1,//手札に入れた時
        Use = 2,//手札から使ったとき
        Trash= 3 ,//すてられたとき
        PlayerCounter = 4,//敵の行動に対するカウンター
        //敵
        EnemyCounter =101,//プレイヤーに対するカウンター
        EnemyTurn = 102,//敵の行動
        EndTurn  = 201,
    }

    public enum AbilityCommands
    {
        //プラス効果
        GetCard = 1, //ショップから無償で得る
        AttackEnemy = 2, //param1攻撃
        Coin = 3, //param1コイン入手
        AttackDeck = 4, //山札の枚数だけダメージ増加
        Stun = 5, //ランダムな敵を気絶させる
        Wall = 6, //param1ダメージを無効化
        Revive = 7, //生き返り

        //アクションカード
        //マイナス効果
        //敵側効果
        AttackPlayer = 101, //プレイヤーを攻撃
        Curse = 102, //呪いを得る
        DropTwo = 103, //二枚すてさせる
        Buff = 104, //前方の敵を強化

        //その他
        Remove = 201, //このアビリティを使った段階で手札から除外
    }

    [Inject] private IShopUsecase _shopUsecase;
    [Inject] private IEnemyUsecase _enemyUsecase;
    [Inject] private IPlayerUsecase _playerUsecase;
    public UnityEvent OnAbilityResolved = new UnityEvent();
    public Dictionary<AbilityCommands, Action<string,int,int>> Commands { get; private set; }

    public AbilityResolver()
    {
        Debug.Log("[AbilityResolver] Init");
        InitCommands();
    } 
 
    void InitCommands()
    {
        Commands = new Dictionary<AbilityCommands,  Action<string,int,int>>()
        {
            //味方側アビリティ
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
            }},
            //敵側アビリティ
            {AbilityCommands.AttackPlayer , (guid, p1, p2) =>
            {
                if(_enemyUsecase.GetFieldOwnIndex(guid) ==p2)
                    _playerUsecase.Damage(p1);
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
            Debug.Log(abilityModel.Timing);
            if (abilityModel.Timing == nowTiming)
            {
                Commands.TryGetValue(abilityModel.Command, out var command);
                Debug.Log($"{card.Name} , {abilityModel.Name}" );
                command?.Invoke(card.GUID,abilityModel.AbilityParam1,abilityModel.AbilityParam2);
                OnAbilityResolved?.Invoke();
            }
        }
    }
    public void UseAbility( EnemyModel enemyModel,int index)
    {
        if (index < enemyModel.Abilities.Count && index >= 0)
        {
            if (enemyModel.Abilities[index].Timing == AbilityTiming.EnemyTurn)
            {
                Debug.Log($"{enemyModel.Name} , {enemyModel.Abilities[index].Name}" );
                Commands.TryGetValue(enemyModel.Abilities[index].Command, out var command);
                command?.Invoke(enemyModel.GUID,enemyModel.Abilities[index].AbilityParam1,enemyModel.Abilities[index].AbilityParam2);
                OnAbilityResolved?.Invoke();
            }
        }
       
    }

}

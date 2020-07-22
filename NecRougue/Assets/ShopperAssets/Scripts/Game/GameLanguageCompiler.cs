using System;
using System.Collections;
using System.Collections.Generic;
using ShopperAssets.Scripts.Game;
using UnityEngine;
using Zenject;

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
    [Inject] private GameUseCase _gameUseCase;

    public Dictionary<AbilityCommands, Action<string,int,int>> Commands { get; private set; }

    public AbilityResolver()
    {
        InitCommands();
    } 
    void InitCommands()
    {
        Commands = new Dictionary<AbilityCommands,  Action<string,int,int>>()
        {
            { AbilityCommands.GetCard, (guid,p1,p2) => { _gameUseCase.BuyRandom(); }},
            { AbilityCommands.AttackEnemy, (guid,p1,p2) => { _gameUseCase.AttackToEnemy(p2,p1); }},
            { AbilityCommands.Remove , (guid,p1,p2) => {_gameUseCase.RemoveHand(guid);}}
        };
    }
}

using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum MapType
{
    Battle_Common,
    Battle_Elete,//レアモンスター戦
    Battle_Necro,//ポケモンでいうとトレーナー戦
    Batlle_Boss,
    Item,
    Shop,
    Event
}
[Flags, Serializable]
public enum MapOption
{
    None = 0,
    Rescue = 1,
    Test = 1 << 1,
}
public enum PlayerType
{
    None,
    Player,
    Enemy
}
public enum AbilityTimingType
{
    None,
    SummonOwn,//召喚時(自分)
    SummonRace,//召喚時(種族)
    BattleStart,
    TurnStart,
    ConfirmAttacker,
    ConfirmTargetAttack,//ターゲットを決めたとき
    ConfirmTargetDefence,//ターゲットにされた時
    Attack,
    Defence,
    TurnEnd,
}


public enum BattleCommandType
{
    None,
    TurnStart,
    Attack,
    Defence,
    Ability,
    TurnEnd,
    
}
//snapshotによる演出用
public enum BattleCardState
{
    None,
    Attack,
    Damage,
    Ability,
    Dead,
}

public enum BattleState
{
    None,
    DeckPrepare,
    TurnStart,
    Attack,
    Ability,
    TurnEnd,
    Sell,//SHOP用
    End,
}

public enum EnemyDestroyState
{
    None,
    Gold,
    Capture
};
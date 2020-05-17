﻿using System;
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
    Summon,//召喚時(自分)
    //SummonRace,//召喚時(種族)
    BattleStart,
    TurnStart,
    ConfirmAttacker,
    ConfirmTargetAttack,//ターゲットを決めたとき
    ConfirmTargetDefence,//ターゲットにされた時
    Attack,
    Defence,
    Dead,
    TurnEnd,
}
/// <summary>
/// アビリティ発動タイミングの行動を起こしたプレイヤーが自分か敵か味方か両方か
/// </summary>
public enum AbilityEffectConditionType
{
    None,
    Self,//自分
    Ally,//味方
    Enemy,//敵
    AllyRace,//味方種族
    Any,//だれでも
}

/// <summary>
/// 実際に効果を及ぼすターゲットカード
/// </summary>
public enum AbilityEffectTargetType
{
    None,
    MySelf,
    MySide,
    MyLeft,
    DefenderSide,
    Action,//アクションしたモンスター
    AllyAll,
    EnemyAll,
    AllyLeftmost,
    EnemyLeftmost,
    All,
    

}

public enum AbilityEffectType
{
    None,
    AtUp,
    HpUp,
    AtHpUp,
    Union,//超電磁
    Summon,//param1モンスターをparam2体召喚
    SummonRandom,//Rarity:param1のモンスターランダム召喚
    ExtAttack,
    //自分の能力とする場合は、バトル開始時に自分に付与という形で実装
    Blocker,//挑発
    Revive,//よみがえりを付与
    Shield,//聖なる盾付与
    Stun,//攻撃時にスタンさせる能力付与
    AddAbility,//能力を与える
    
    
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
    Triple,//トリプルができた時
    End,
}

public enum EnemyDestroyState
{
    None,
    Gold,
    Capture
};
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
    Summon = 10,//召喚時(自分)
    //SummonRace,//召喚時(種族)
    BattleStart = 20,
    TurnStart = 30,
    //ConfirmAttacker = 40,
    ConfirmTargetAttack = 50,//ターゲットを決めたとき
    ConfirmTargetDefence = 60,//ターゲットにされた時
    Attack = 70,
    Defence = 80,
    Dead = 90,
    TurnEnd = 1000,
}
/// <summary>
/// アビリティ発動タイミングの行動を起こしたプレイヤーが自分か敵か味方か両方か
/// </summary>
public enum AbilityEffectConditionType
{
    None,
    Self = 10,//自分
    Ally = 20,//味方
    Enemy = 30,//敵
    AllyRace = 40,//味方種族
    Any = 1000,//だれでも
}

/// <summary>
/// 実際に効果を及ぼすターゲットカード
/// </summary>
public enum AbilityEffectTargetType
{
    None,
    MySelf = 10,
    MySide = 20,
    MyLeft = 30,
    DefenderSide = 40,
    Action = 50,//アクションしたモンスター
    AllyAll = 60,
    EnemyAll = 70,
    AllyLeftmost =80,
    EnemyLeftmost = 90,
    Random = 100,
    Defender = 110,
    RandomAlly = 120,
    RandomEnemy = 130,
    All = 1000,
    

}

public enum AbilityEffectType
{
    None,
    AtUp = 10,
    HpUp = 20,
    AtHpUp= 30,
    Union= 40,//超電磁
    Summon = 50,//param1モンスターをparam2体召喚
    SummonRandom = 60,//Rarity:param1のモンスターランダム召喚
    ExtAttack = 70,
    //自分の能力とする場合は、バトル開始時に自分に付与という形で実装
    Blocker = 80,//挑発
    Revive = 90,//よみがえりを付与
    Shield= 100,//聖なる盾付与
    Stun = 110,//攻撃時にスタンさせる能力付与
    AddAbility = 120,//能力を与える
    
    
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
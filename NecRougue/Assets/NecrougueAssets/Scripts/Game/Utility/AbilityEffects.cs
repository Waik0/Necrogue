using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
public class AbilityEffectsArgument
{
    public BattleDataUseCase BattleDataUseCase;
    public BattleProcessSequence BattleProcess;
    //能力発動をチェックする者
    public long AbilityCardUnique;
    //public int AbilityPlayerIndex;
    //public int AbilityDeckIndex;
    //アクションを起こしたもの(攻撃、召喚など
    public long ActionCardUnique;
    //public int ActionPlayerIndex = -1;
    //public int ActionDeckIndex = -1;
    //防御者 Attack Defenceの時のみ使用
    //public int DefenderPlayerIndex = -1;
    //public int DefenderDeckIndex = -1;
    public long DefenderCardUnique;
    //死亡時の場合indexの取得方法が異なる
    public bool isDeadEffect;
    //
    public int Level = 1;//能力レベル
    public int Param1;
    public int Param2; 
    public AbilityTimingType TimingType;
    //public bool IsMine = false;
    //あとで入れるやつ
    public List<BattleCard> TargetCards;
}
public static class AbilityEffects
{

    private static Dictionary<AbilityEffectType, Func<AbilityEffectsArgument,bool>> EffectList = new Dictionary<AbilityEffectType,  Func<AbilityEffectsArgument,bool>>()
    {
        {//
            AbilityEffectType.None,
            abilityEffectsArgument =>
            {
                return false;
            }

        },
        {//モンスターは攻撃力+{param1*level}を得る
            AbilityEffectType.AtUp,
            abilityEffectsArgument =>
            {
                if (abilityEffectsArgument.TargetCards.Count <= 0)
                {
                    return false;
                }
                foreach(var card in abilityEffectsArgument.TargetCards)
                {
                    card.Attack += abilityEffectsArgument.Level * abilityEffectsArgument.Param1;
                }

                return true;
            }
            
        },
        {//モンスターはHp+{param1*level}を得る
            AbilityEffectType.HpUp,
            abilityEffectsArgument =>
            {
                if (abilityEffectsArgument.TargetCards.Count <= 0)
                {
                    return false;
                }
                foreach(var card in abilityEffectsArgument.TargetCards)
                {
                    card.Hp += abilityEffectsArgument.Level * abilityEffectsArgument.Param1;
                }

                return true;
            }
            
        },
        {//モンスターは攻撃力+{param1*level}Hp+{param1*level}を得る
            AbilityEffectType.AtHpUp,
            abilityEffectsArgument =>
            {
                if (abilityEffectsArgument.TargetCards.Count <= 0)
                {
                    return false;
                }
                foreach(var card in abilityEffectsArgument.TargetCards)
                {
                    card.Attack += abilityEffectsArgument.Level * abilityEffectsArgument.Param1;
                    card.Hp += abilityEffectsArgument.Level * abilityEffectsArgument.Param1;
                }

                return true;
            }
            
        },
        {//{param1:EnemyId}を{Param2}体召喚する。
            AbilityEffectType.Summon,
            abilityEffectsArgument =>
            {
                //if (abilityEffectsArgument.TargetCards)
                //{
                    
                //}
                //index取得
                var abilityCardIndex = 
                    abilityEffectsArgument.
                        BattleDataUseCase.GetIndex(abilityEffectsArgument.AbilityCardUnique);
                if (abilityCardIndex.pIndex < 0 || abilityCardIndex.dIndex < 0)
                {
                    return false;
                }
                
                for (int i = 0; i < abilityEffectsArgument.Param2; i++)
                {
                    
                    var summoned = abilityEffectsArgument.BattleDataUseCase
                        .SummonDirect(
                            abilityCardIndex.pIndex,
                            abilityCardIndex.dIndex,
                            abilityEffectsArgument.Param1);
                    if (!summoned)
                    {
                        continue;
                    }

                    abilityEffectsArgument.BattleDataUseCase.ChangeState(BattleState.Ability);
                    //召喚時効果発動
                    abilityEffectsArgument.BattleDataUseCase.ResolveAbilityAll(
                        AbilityTimingType.Summon,
                        ss =>
                            abilityEffectsArgument.BattleProcess?.OnCommand.Invoke(new BattleCommand().Generate(ss)),
                        abilityEffectsArgument.ActionCardUnique,
                        abilityEffectsArgument.DefenderCardUnique);
                    //死亡したモンスターがいるかチェック
                    var removed = abilityEffectsArgument.BattleDataUseCase.RemoveDeadCard();

                    abilityEffectsArgument.BattleDataUseCase.ResolveAbilityDead(removed, ss =>
                        abilityEffectsArgument.BattleProcess?.OnCommand.Invoke(new BattleCommand().Generate(ss)));
                }
                return true;
            }
        },
        {// 挑発を付与
            AbilityEffectType.Blocker,
            abilityEffectsArgument =>
            {
                if (abilityEffectsArgument.TargetCards.Count <= 0)
                {
                    return false;
                }
                foreach (var card in abilityEffectsArgument.TargetCards)
                {
                    if (card.AttackPriolity <= 0)
                    {
                        card.AttackPriolity = 1;
                    }
                }

                return true;
            }
        },
        {// 攻撃無効を付与
            AbilityEffectType.Shield,
            abilityEffectsArgument =>
            {
                if (abilityEffectsArgument.TargetCards.Count <= 0)
                {
                    return false;
                }
                foreach (var card in abilityEffectsArgument.TargetCards)
                {
                    if (card.Defence <= 0)
                    {
                        card.Defence = 1;
                    }
                }

                return true;
            }
        },
        {// スタンさせる
            AbilityEffectType.Stun,
            abilityEffectsArgument =>
            {
                if (abilityEffectsArgument.TargetCards.Count <= 0)
                {
                    return false;
                }
                foreach (var card in abilityEffectsArgument.TargetCards)
                {
                    if (card.Stun <= 0)
                    {
                        card.Stun = 1;
                    }
                }

                return true;
            }
        },
        {// 超電磁
            AbilityEffectType.Union,
            abilityEffectsArgument =>
            {
                var union = abilityEffectsArgument.BattleDataUseCase.GetCardRef(abilityEffectsArgument.AbilityCardUnique);
                if (union == null)
                {
                    return false;
                }
                if (abilityEffectsArgument.TargetCards.Count <= 0)
                {
                    return false;
                }
                foreach (var card in abilityEffectsArgument.TargetCards)
                {
                    card.Attack += union.Attack;
                    card.Hp += union.Hp;
                    card.Defence = Mathf.Max(  card.Defence + union.Defence,1);
                    card.Stun = Mathf.Max(  card.Stun + union.Stun,1);
                    card.AttackPriolity = Mathf.Max(  card.AttackPriolity + union.AttackPriolity,1);
                    
                }
                //自分を削除
                abilityEffectsArgument.BattleDataUseCase.ForceRemoveDeckCard(union.Unique);

                return true;
            }
        },
        {// ランダム召喚
            AbilityEffectType.SummonRandom,
            abilityEffectsArgument =>
            {
                //index取得
                var abilityCardIndex = 
                    abilityEffectsArgument.
                        BattleDataUseCase.GetIndex(abilityEffectsArgument.AbilityCardUnique);
                if (abilityCardIndex.pIndex < 0 || abilityCardIndex.dIndex < 0)
                {
                    return false;
                }

                var records = MasterdataManager.Records<MstMonsterRecord>()
                    .Where(_=>_.grade == abilityEffectsArgument.Param1).ToList();
                if (records.Count <= 0)
                {
                    return false;
                }
                for (int i = 0; i < abilityEffectsArgument.Param2; i++)
                {
                    var id = records[UnityEngine.Random.Range(0, records.Count)].id;
                    var summoned = abilityEffectsArgument.BattleDataUseCase
                        .SummonDirect(
                            abilityCardIndex.pIndex,
                            abilityCardIndex.dIndex,
                            id);
                    if (!summoned)
                    {
                        continue;
                    }

                    abilityEffectsArgument.BattleDataUseCase.ChangeState(BattleState.Ability);
                    //召喚時効果発動
                    abilityEffectsArgument.BattleDataUseCase.ResolveAbilityAll(
                        AbilityTimingType.Summon,
                        ss =>
                            abilityEffectsArgument.BattleProcess?.OnCommand.Invoke(new BattleCommand().Generate(ss)),
                        abilityEffectsArgument.ActionCardUnique,
                        abilityEffectsArgument.DefenderCardUnique);
                    //死亡したモンスターがいるかチェック
                    var removed = abilityEffectsArgument.BattleDataUseCase.RemoveDeadCard();

                    abilityEffectsArgument.BattleDataUseCase.ResolveAbilityDead(removed, ss =>
                        abilityEffectsArgument.BattleProcess?.OnCommand.Invoke(new BattleCommand().Generate(ss)));
                }
                return true;
            }
        },
        {// よみがえり
            AbilityEffectType.Revive,
            abilityEffectsArgument =>
            {
                if (abilityEffectsArgument.TargetCards.Count <= 0)
                {
                    return false;
                }
                foreach (var card in abilityEffectsArgument.TargetCards)
                {
                    if (card.Revive <= 0)
                    {
                        card.Revive = 1;
                    }
                }

                return true;

            }
        },
        {// 追加攻撃
            AbilityEffectType.ExtAttack,
            abilityEffectsArgument =>
            {
                if (abilityEffectsArgument.TargetCards.Count <= 0)
                {
                    return false;
                }
                foreach (var card in abilityEffectsArgument.TargetCards)
                {
                    card.Hp -= abilityEffectsArgument.Param1;
                }
                return true;
            }
        },
        {// アビリティ付与
            AbilityEffectType.AddAbility,
            abilityEffectsArgument => {
                if (abilityEffectsArgument.TargetCards.Count <= 0)
                {
                    return false;
                }
                var ability = MasterdataManager.Get<MstAbilityRecord>(abilityEffectsArgument.Param1);

                var addability = false;
                foreach (var card in abilityEffectsArgument.TargetCards)
                {
                    if (card.AbilityList.Any(_ => _.Id == ability.id))
                    { 
                        continue;
                    }

                    addability = true;
                    card.AbilityList.Add(new Ability().Generate(ability));
                }
                return addability;
                
            }
        },
    };

    private static Dictionary<AbilityEffectConditionType, Func<AbilityEffectsArgument, bool>> AbilityConditions = 
        new Dictionary<AbilityEffectConditionType, Func<AbilityEffectsArgument, bool>>()
    {
            {
                AbilityEffectConditionType.None,
                arg =>
                {
                    return false;
                }


            },
            {
                AbilityEffectConditionType.Self,
                arg => arg.ActionCardUnique == arg.AbilityCardUnique
            },
            {
                AbilityEffectConditionType.Ally,
                 arg =>
                 {
                     var pIndex1 = arg.BattleDataUseCase.GetPlayerIndex(arg.AbilityCardUnique);
                     var pIndex2 = arg.BattleDataUseCase.GetPlayerIndex(arg.ActionCardUnique);
                     return pIndex1 == pIndex2;
                 }

            },
            {
                AbilityEffectConditionType.Enemy,
                arg =>
                {
                    var pIndex1 = arg.BattleDataUseCase.GetPlayerIndex(arg.AbilityCardUnique);
                    var pIndex2 = arg.BattleDataUseCase.GetPlayerIndex(arg.ActionCardUnique);
                    return pIndex1 != pIndex2;

                }

            },
            {//ActionCardの種族がId = param1のとき
                AbilityEffectConditionType.AllyRace,
                 arg =>
                {
                    var pIndex1 = arg.BattleDataUseCase.GetPlayerIndex(arg.AbilityCardUnique);
                    var pIndex2 = arg.BattleDataUseCase.GetPlayerIndex(arg.ActionCardUnique);
                    return pIndex1 == pIndex2 &&
                           arg.BattleDataUseCase.GetRaceData(arg.ActionCardUnique)
                               .Any(race => race.Id == arg.Param1);

                }

            },
            {
                AbilityEffectConditionType.Any,
                 arg =>
                {
                    return true;

                }

            }
    };

    private static Dictionary<AbilityTimingType, Func<AbilityEffectConditionType, AbilityEffectsArgument, bool>>
        AbilityTiming =
            new Dictionary<AbilityTimingType, Func<AbilityEffectConditionType, AbilityEffectsArgument, bool>>()
            {
                {AbilityTimingType.BattleStart, (c, arg) => true},
                {AbilityTimingType.TurnStart, (c, arg) => true},
                {AbilityTimingType.TurnEnd, (c, arg) => true},
                {AbilityTimingType.Summon, (c, arg) => AbilityConditions[c](arg)},
                {AbilityTimingType.ConfirmTargetAttack, (c, arg) => AbilityConditions[c](arg)},
                {AbilityTimingType.ConfirmTargetDefence, (c, arg) => AbilityConditions[c](arg)},
                {AbilityTimingType.Attack, (c, arg) => AbilityConditions[c](arg)},
                {AbilityTimingType.Defence, (c, arg) => AbilityConditions[c](arg)},
                {AbilityTimingType.Dead, (c, arg) => AbilityConditions[c](arg)},
                {AbilityTimingType.None, (c, arg) => false},

            };
    
    private static Dictionary<AbilityEffectTargetType, Func<AbilityEffectsArgument, List<BattleCard>>> AbilityTargets =
        new Dictionary<AbilityEffectTargetType, Func<AbilityEffectsArgument, List<BattleCard>>>()
    {
            {//発動者の両隣
                AbilityEffectTargetType.MySide,
                arg =>
                {
                    var cards = new List<BattleCard>();
                    var abilityCardIndex = 
                        arg.
                            BattleDataUseCase.GetIndex(arg.AbilityCardUnique);
                    if (abilityCardIndex.pIndex < 0 || abilityCardIndex.dIndex < 0)
                    {
                        return cards;
                    }
                    var left =   arg.BattleDataUseCase
                    .GetCardByIndex(abilityCardIndex.pIndex,abilityCardIndex.dIndex - 1);
                    var right =  arg.BattleDataUseCase
                        .GetCardByIndex(abilityCardIndex.pIndex,abilityCardIndex.dIndex + 1);
                     if(left != null )cards.Add(left);
                     if(right != null )cards.Add(right);
                    return cards;
                }

            },
            {
                AbilityEffectTargetType.MySelf,
                arg =>
                {
                    var cards = new List<BattleCard>();

                    var self = arg.BattleDataUseCase.GetCardRef(arg.AbilityCardUnique);
                     if(self != null )cards.Add(self);
                    return cards;
                }

            },
            {
                AbilityEffectTargetType.MyLeft,
                arg =>
                {
                    var cards = new List<BattleCard>();
                    var abilityCardIndex = 
                        arg.
                            BattleDataUseCase.GetIndex(arg.AbilityCardUnique);
                    if (abilityCardIndex.pIndex < 0 || abilityCardIndex.dIndex < 0)
                    {
                        return cards;
                    }
                    var left =   arg.BattleDataUseCase
                        .GetCardByIndex(abilityCardIndex.pIndex ,abilityCardIndex.dIndex - 1);
                    if(left != null )cards.Add(left);
                    return cards;
                }

            },
            {
                AbilityEffectTargetType.DefenderSide,
                arg =>
                {
                    var cards = new List<BattleCard>();
                    var index = 
                        arg.
                            BattleDataUseCase.GetIndex(arg.DefenderCardUnique);
                    if (index.pIndex < 0 || index.dIndex < 0)
                    {
                        return cards;
                    }
                    var left =   arg.BattleDataUseCase
                        .GetCardByIndex(index.pIndex,index.dIndex - 1);
                    var right =  arg.BattleDataUseCase
                        .GetCardByIndex(index.pIndex,index.dIndex + 1);
                    if(left != null )cards.Add(left);
                    if(right != null )cards.Add(right);
                    return cards;
                }

            },
            {
                AbilityEffectTargetType.Action,
                arg =>
                {
                    var cards = new List<BattleCard>();
                    var self = arg.BattleDataUseCase.GetCardRef(arg.ActionCardUnique);
                    if(self != null )cards.Add(self);
                    return cards;
                }

            },
            {
                AbilityEffectTargetType.AllyAll,
                arg =>
                {
                    var cards = new List<BattleCard>();
                    var pIndex = arg.BattleDataUseCase.GetPlayerIndex(arg.AbilityCardUnique);
                    for (int i = 0; i < arg.BattleDataUseCase.GetPlayer(pIndex).Deck.Count; i++)
                    {
                        var card = arg.BattleDataUseCase.GetCardByIndex(pIndex, i);
                        if (arg.AbilityCardUnique == card.Unique)
                        {
                            continue;
                        }
                        if(card != null)cards.Add(card);
                    }
                    return cards;
                }

            },
            {
                AbilityEffectTargetType.EnemyAll,
                arg =>
                {
                    var cards = new List<BattleCard>();
                    var pType = arg.BattleDataUseCase.GetPlayerType(arg.AbilityCardUnique);
                    foreach (var battlePlayerData in arg.BattleDataUseCase.GetPlayerList().Where(_=>_.PlayerType != pType))
                    {
                        for (int i = 0; i < battlePlayerData.Deck.Count; i++)
                        {
                            var card = battlePlayerData.Deck[i];
                            if(card != null)cards.Add(card);
                        }
                    }
                   
                    return cards;
                }
            },
            {
                AbilityEffectTargetType.AllyLeftmost,
                arg =>
                {
                    var cards = new List<BattleCard>();
                    var pIndex = arg.BattleDataUseCase.GetPlayerIndex(arg.AbilityCardUnique);
                    var card = arg.BattleDataUseCase.GetCardByIndex(pIndex, 0);
                    if(card != null)cards.Add(card);

                    return cards;
                }

            },
            {
                AbilityEffectTargetType.EnemyLeftmost,
                arg =>
                {
                    var cards = new List<BattleCard>();
                    var pType = arg.BattleDataUseCase.GetPlayerType(arg.AbilityCardUnique);
                    foreach (var battlePlayerData in arg.BattleDataUseCase.GetPlayerList().Where(_=>_.PlayerType != pType))
                    {
                        if (battlePlayerData.Deck.Count <= 0)
                        {
                            continue;
                        }
                        var card = battlePlayerData.Deck[0];
                        if(card != null)cards.Add(card);
                    }
                    return cards;
                }
 
            },
            {
                AbilityEffectTargetType.Random,
                arg =>
                {
                    var cards = new List<BattleCard>();
                    var card = arg.BattleDataUseCase.RandomCard();
                    if(card!=null)cards.Add(card);
                    return cards;
                }

            }
            ,
            {
                AbilityEffectTargetType.All,
                arg =>
                {
                    var cards = new List<BattleCard>();
                    for (int j = 0; j < arg.BattleDataUseCase.Data.PlayerList.Count; j++)
                    {
                        for (int i = 0; i < arg.BattleDataUseCase.GetPlayer(j).Deck.Count; i++)
                        {
                            var c = arg.BattleDataUseCase.GetCardByIndex(j, i);
                            if (arg.AbilityCardUnique == c.Unique)
                            {
                                continue;
                            }
                            if(c != null)cards.Add(c);
                        }
                    }
                    return cards;
                }

            }
            ,
            {
                AbilityEffectTargetType.Defender,
                arg =>
                {
                    var cards = new List<BattleCard>();
                    var self =   arg.BattleDataUseCase
                        .GetCardRef(arg.DefenderCardUnique);
                    if(self != null )cards.Add(self);
                    return cards;
                }

            }
            ,
            {
                AbilityEffectTargetType.RandomAlly,
                arg =>
                {
                    var cards = new List<BattleCard>();
                    var pIndex = arg.BattleDataUseCase.GetPlayerIndex(arg.AbilityCardUnique);
                    var card = arg.BattleDataUseCase.RandomCard(pIndex);
                    if(card!=null)cards.Add(card);
                    return cards;
                }

            }
            ,
            {
                AbilityEffectTargetType.RandomEnemy,
                arg =>
                {
                    var cards = new List<BattleCard>();
                    var ptype = arg.BattleDataUseCase.GetPlayerType(arg.AbilityCardUnique);
                    for (int j = 0; j < arg.BattleDataUseCase.Data.PlayerList.Count; j++)
                    {
                        if (arg.BattleDataUseCase.Data.PlayerList[j].PlayerType == ptype)
                        {
                            continue;
                        }
                        var card = arg.BattleDataUseCase.RandomCard(j);
                        if (card != null) cards.Add(card);
                    }

                    return cards;
                }

            }
            
    };

    public static Func<AbilityEffectsArgument, bool> GetEffect(MstAbilityRecord ability)
    {

        var targetType = ability.targetType;
        var conditionType = ability.conditionType;
        var effectType = ability.effectType;
        var timingType = ability.timingType;
       
        var chooseTaregt = AbilityTargets[targetType];
        var condition = AbilityTiming[timingType];
        var effect = EffectList[effectType];
        
        bool Effect(AbilityEffectsArgument arg)
        {
            arg.Param1 = ability.param1;
            arg.Param2 = ability.param2;
            if (arg.TimingType != timingType)
            {
                return false;
            }
            
            if (!condition(conditionType,arg))
            {
                return false;
            }
            var cards = chooseTaregt(arg);
            if (cards.Count == 0)
            {
                return false;
            }
            arg.TargetCards = cards;
            var result = effect(arg);

            return result;
        }
        return Effect;
       
    }
}
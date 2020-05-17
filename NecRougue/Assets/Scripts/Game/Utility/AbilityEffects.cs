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
    public int AbilityPlayerIndex;
    public int AbilityDeckIndex;
    //アクションを起こしたもの(攻撃、召喚など
    public int ActionPlayerIndex = -1;
    public int ActionDeckIndex = -1;
    //防御者 Attack Defenceの時のみ使用
    public int DefenderPlayerIndex = -1;
    public int DefenderDeckIndex = -1;
    //
    public int Level;//能力レベル
    public int Param1;
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
        {//モンスターは攻撃力+{Level}を得る
            AbilityEffectType.AtUp,
            abilityEffectsArgument =>
            {
                foreach(var card in abilityEffectsArgument.TargetCards)
                {
                    card.Attack += abilityEffectsArgument.Level * abilityEffectsArgument.Param1;
                }

                return true;
            }
            
        },
        {// Level/Levelの{param1:EnemyId}を1体召喚する。
            AbilityEffectType.Summon,
            abilityEffectsArgument =>
            {
                abilityEffectsArgument.BattleDataUseCase
                    .SummonDirect(
                        abilityEffectsArgument.AbilityDeckIndex,
                        abilityEffectsArgument.AbilityDeckIndex,
                        abilityEffectsArgument.Param1);
                
                return true;
            }
        }
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
                arg =>
                {
                    return 
                    arg.ActionDeckIndex == arg.AbilityDeckIndex &&
                    arg.ActionPlayerIndex == arg.AbilityPlayerIndex;
                    
                }


            },
            {
                AbilityEffectConditionType.Ally,
                 arg =>
                {
                    return
                    arg.ActionPlayerIndex == arg.AbilityPlayerIndex &&
                    arg.ActionDeckIndex != arg.AbilityDeckIndex;

                }

            },
            {//ActionCardの種族がId = param1のとき
                AbilityEffectConditionType.AllyRace,
                 arg =>
                {
                    return arg.ActionPlayerIndex == arg.AbilityPlayerIndex &&
                    arg.BattleDataUseCase.GetRaceData(arg.ActionPlayerIndex,arg.ActionDeckIndex).Any(race =>race.Id == arg.Param1 );

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
    private static Dictionary<AbilityEffectTargetType, Func<AbilityEffectsArgument, List<BattleCard>>> AbilityTargets =
        new Dictionary<AbilityEffectTargetType, Func<AbilityEffectsArgument, List<BattleCard>>>()
    {
            {//発動者の両隣
                AbilityEffectTargetType.MySide,
                arg =>
                {
                    var cards = new List<BattleCard>();
                    if(arg.AbilityPlayerIndex == -1 || arg.AbilityDeckIndex == -1)
                    {
                        return cards;
                    }
                    var left =   arg.BattleDataUseCase
                    .GetCardRef(arg.AbilityPlayerIndex,arg.AbilityDeckIndex - 1);
                    var right =  arg.BattleDataUseCase
                    .GetCardRef(arg.AbilityPlayerIndex,arg.AbilityDeckIndex + 1);
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
                    if(arg.AbilityPlayerIndex == -1 || arg.AbilityDeckIndex == -1)
                    {
                        return cards;
                    }
                    var self =   arg.BattleDataUseCase
                    .GetCardRef(arg.AbilityPlayerIndex,arg.AbilityDeckIndex);
                     if(self != null )cards.Add(self);
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
        var condition = AbilityConditions[conditionType];
        var effect = EffectList[effectType];

        bool Effect(AbilityEffectsArgument arg)
        {
            if (arg.TimingType != timingType)
            {
                return false;
            }
            if (!condition(arg))
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
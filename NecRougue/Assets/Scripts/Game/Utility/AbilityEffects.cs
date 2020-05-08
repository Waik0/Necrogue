using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AbilityEffectsArgument
{
    public BattleDataUseCase BattleDataUseCase;
    public BattleProcess BattleProcess;
    public int PlayerIndex;
    public int DeckIndex;
    public int Level;
    public AbilityTimingType TimingType;
    public bool IsMine = false;
}
public static class AbilityEffects
{
    private static Dictionary<int, Func<AbilityEffectsArgument,bool>> EffectList = new Dictionary<int,  Func<AbilityEffectsArgument,bool>>()
    {
        {//ターン開始時: 隣接するモンスターは攻撃力+{Level}を得る
            101,
            abilityEffectsArgument =>
            {
                if (abilityEffectsArgument.TimingType != AbilityTimingType.TurnStart)
                {
                    return false;
                }
                var left = abilityEffectsArgument.BattleDataUseCase
                    .GetCardRef(abilityEffectsArgument.PlayerIndex,abilityEffectsArgument.DeckIndex - 1);
                var right = abilityEffectsArgument.BattleDataUseCase
                    .GetCardRef(abilityEffectsArgument.PlayerIndex,abilityEffectsArgument.DeckIndex + 1);
                if (left != null) left.Attack.Current+= abilityEffectsArgument.Level;
                if (right != null) right.Attack.Current+= abilityEffectsArgument.Level;
                return true;
            }
            
        },
        {//召喚時: Level/Levelの{param1:EnemyId}を1体召喚する。
            102,
            abilityEffectsArgument =>
            {
                if (abilityEffectsArgument.TimingType != AbilityTimingType.SummonOwn)
                {
                    return false;
                }

                if (!abilityEffectsArgument.IsMine)
                {
                    return false;
                }

                return true;
            }
        },
        {//{param1:RaceId}召喚時: {param1:RaceId}を召喚するたびに攻撃力+{Level}
            103,
            abilityEffectsArgument =>
            {
                if (abilityEffectsArgument.TimingType != AbilityTimingType.SummonRace)
                {
                    return false;
                }
                var own = abilityEffectsArgument.BattleDataUseCase
                    .GetCardRef(abilityEffectsArgument.PlayerIndex,abilityEffectsArgument.DeckIndex );
                if (own != null) own.Attack.Current+= abilityEffectsArgument.Level;
                return true;
            }
        },
        {//召喚時: 味方の{param1:RaceId}1体に+1/+1を付与する
            104,
            abilityEffectsArgument =>
            {
                if (abilityEffectsArgument.TimingType != AbilityTimingType.SummonOwn)
                {
                    return false;
                }
                if (!abilityEffectsArgument.IsMine)
                {
                    return false;
                }

                return true;
            }
        },
    };

    public static Func<AbilityEffectsArgument,bool> GetEffect(int id)
    {
        if (EffectList.ContainsKey(id))
        {
            return EffectList[id];
        }

        return null;
    }
}
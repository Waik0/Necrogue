using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class AbilityEffects
{
    private static Dictionary<int, Action<BattleData>> EffectList = new Dictionary<int, Action<BattleData>>()
    {

    };

    public static Action<BattleData> GetEffect(int id)
    {
        if (EffectList.ContainsKey(id))
        {
            return EffectList[id];
        }

        return null;
    }
}

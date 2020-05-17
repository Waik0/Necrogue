using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[Serializable,MasterPath("Master/mst_ability.json")]
public class MstAbilityRecord : IMasterRecord
{
    public int Id
    {
        get => id;
    }
    public int id;
    public string name;
    public AbilityTimingType timingType;
    public AbilityEffectConditionType conditionType;
    public AbilityEffectTargetType targetType;//実際に効果を発揮する対象
    public AbilityEffectType effectType;//
    public int raceId;//汎用的に使用する種族パラメーター
    public int param1;//汎用的に使用するパラメーター
    public int param2;//汎用的に使用するパラメーター
    public string description;
}

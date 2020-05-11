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
    //public AbilityTimingType timingType;
    public int param1;//汎用的に使用するパラメーター
    public string description;
}

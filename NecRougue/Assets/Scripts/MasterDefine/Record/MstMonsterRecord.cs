using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
[MasterPath("Master/mst_monster.json")]
[Serializable]
public class MstMonsterRecord : IMasterRecord
{
    public int Id
    {
        get => id;
    }

    public int id;
    public string name;
    public int attack;
    public int hp;
    public int defence;
    public int raceId1;
    public int raceId2;
    public int abilityId1;
    public int abilityId2;
    public int abilityId3;
    public int abilityId4;

}

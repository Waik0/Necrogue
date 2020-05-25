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
    public int rarity;//出現率
    public int grade;//ショップランク
    public int attack;
    public int hp;
    //public int defence;//聖なる盾
    //public int priority;//挑発
    //public int revival;
    public int raceId1;
    public int raceId2;
    public int abilityId1;
    public int abilityId2;
    public int abilityId3;
    public int abilityId4;

}

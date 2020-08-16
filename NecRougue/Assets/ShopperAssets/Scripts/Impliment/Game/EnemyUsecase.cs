using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using ShopperAssets.Scripts.Game;
using ShopperAssets.Scripts.Master;
using UnityEngine;
using Random = UnityEngine.Random;

public class EnemyUsecase : IEnemyUsecase
{
    private List<int> RankCounts = new List<int>()
    {
        5,
        8,
        8,
        4,
        1
    };

    public List<EnemyModel> Field { get;  private set; }
    public int Level { get;  private set; }
    public int EnemyCount { get; private set; } = 4;

    public void Reset()
    {
        //EnemyDeckCreate();
        FieldInit();
        Level = 0; 
    }

    private void FieldInit()
    {
        Field = new List<EnemyModel>();
        for (int i = 0; i < EnemyCount; i++)
        {
            
            Field.Add(null);
        }
    }

    public int GetFieldOwnIndex(string guid)
    {

        try
        {
            return Field.FindIndex(_ => _!=null && _.GUID == guid);
        }
        catch (Exception e)
        { 
            Debug.LogError(e);
            return -1;
        }
       
    }

    public int EnemyTurn(int index)
    {
        if (Field.Count <= index || Field[index] == null)
            return -1;
        return Random.Range(0, Field[index].Abilities.Count);
        
    }

    public void CheckDead()
    {
        for (var i = 0; i < Field.Count; i++)
        {
            if (Field[i] != null && Field[i].Hp <= 0)
            {
                Field[i] = null;
            }
        }
    }
    public void MoveForward()
    {
        if (Field.Count > 0)
        {
            if (Field[0] == null || Field[0].Hp < 0)
            {
                Field.RemoveAt(0);
            }
        }
        if (Field.Count < EnemyCount)
        {
            if (Field[Field.Count - 1] == null)
            {
                var enemies = MasterdataManager.Records<ShMstEnemyRecord>()
                    .Where(_ => _.Rank == Level).ToList();

                if (enemies.Count > 0)
                {
                    var pop = new EnemyModel().Generate(enemies[Random.Range(0, enemies.Count)]);
                    Field.Add(pop);
                }
            }
            else
            {
                Field.Add(null);
            }


        }

        if (Field.Count >= EnemyCount)
        {
            Debug.Log($"Enemy 3:{Field[3]} 2:{Field[2]}1:{Field[1]}0:{Field[0]}");
        }
    }

    public void Damage(int range, int attack)
    {
        Debug.Log($"Attack Atk:{attack} , Range:{range}");
        if (Field.Count > range && range >= 0)
        {
            if (Field[range] != null)
            {
                Field[range].Hp -= Mathf.Max(0, attack - Field[range].Defence);
            }
        }
    }
}

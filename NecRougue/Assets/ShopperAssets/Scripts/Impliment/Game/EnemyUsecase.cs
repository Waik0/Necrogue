using System.Collections;
using System.Collections.Generic;
using System.Linq;
using ShopperAssets.Scripts.Game;
using ShopperAssets.Scripts.Master;
using UnityEngine;

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

    public List<EnemyModel> Deck { get; private set; }
    public List<EnemyModel> Field { get;  private set; }
    public int Level { get;  private set; }
    public int EnemyCount { get; private set;  }

    public void Reset()
    {
        //EnemyDeckCreate();
        FieldInit();
        Level = 0;
        EnemyCount = 3;
    }

    private void FieldInit()
    {
        Field = new List<EnemyModel>();
        for (int i = 0; i < EnemyCount; i++)
        {
            
            Field.Add(null);
        }
    }
        
    private void EnemyDeckCreate()
    {
        var enemies = MasterdataManager.Records<ShMstEnemyRecord>();
        List<ShMstEnemyRecord> _candidate;
        var targetRank = 0;
        foreach (var c in RankCounts)
        {
            _candidate = enemies.Where(_ => _.Rank == targetRank).ToList();
            for (int i = 0; i < _candidate.Count && i < c; i++)
            {
                if (_candidate.Count > 0)
                {
                    var target = _candidate[UnityEngine.Random.Range(0, _candidate.Count)];
                    Deck.Add(new EnemyModel().Generate(target));
                }
            }
            targetRank++;
        }
        Debug.Log("Enemy:"+Deck.Count);

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
            var enemies = MasterdataManager.Records<ShMstEnemyRecord>()
                .Where(_ => _.Rank == Level).ToList();

            if (enemies.Count > 0)
            {
                var pop = new EnemyModel().Generate(enemies[Random.Range(0,enemies.Count)]);
                Field.Add(pop);
            }
          
                
        }
    }

    public void Damage(int range, int attack)
    {
        if (Field.Count > range && range >= 0)
        {
            Field[range].Hp -= Mathf.Max(0,attack - Field[range].Defence);
        }
    }
}

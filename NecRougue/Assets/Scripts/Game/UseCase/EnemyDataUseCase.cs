using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using Random = UnityEngine.Random;

//プロパティからの変更を防ぐためにEntityはこうぞうたいで定義


public class EnemyDataUseCase : IEntityUseCase<EnemyData>
{
    private EnemyData _enemyData;
    public EnemyData Data
    {
        get => _enemyData;
    }

    public EnemyDataUseCase()
    {
        ResetData();
    }
    public void ResetData()
    {
        _enemyData = new EnemyData();
    }

    public void SetEnemyId(int id)
    {
        _enemyData.Id = id;
    }
}
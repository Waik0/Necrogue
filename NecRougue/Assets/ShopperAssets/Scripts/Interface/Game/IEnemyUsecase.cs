using System.Collections;
using System.Collections.Generic;
using ShopperAssets.Scripts.Game;
using UnityEngine;

public interface IEnemyUsecase
{
    List<EnemyModel> Field  { get; }
     int Level { get; }
     int EnemyCount { get; }
     
    void Reset();

    void CheckDead();
    void MoveForward();

    int GetFieldOwnIndex(string guid);
    int EnemyTurn(int index);
    void Damage(int range,int attack);
}


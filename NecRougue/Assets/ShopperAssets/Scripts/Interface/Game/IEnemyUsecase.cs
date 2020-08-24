using System.Collections;
using System.Collections.Generic;
using ShopperAssets.Scripts.Game;
using UnityEngine;
using UnityEngine.Events;

public interface IEnemyUsecase
{
    List<EnemyModel> Field  { get; }
     int Level { get; }
     int EnemyCount { get; }
     UnityEvent OnDamaged { get; }
    void Reset();

    void CheckDead();
    void MoveForward();

    int GetFieldOwnIndex(string guid);
    string GetGUIDFromIndex(int index);
    int EnemyTurn(int index);
    void Damage(int range,int attack);
    bool Stun(string guid);
    bool Stun(int range);
}


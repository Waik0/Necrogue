using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Zenject;

public class TmsNpcBase : MonoBehaviour, IPoolable<TmsNpcBase.SpawnData, IMemoryPool>
{
    public class TmsNpcBaseFactory : PlaceholderFactory<SpawnData, TmsNpcBase> { }

    [Inject]
    void Inject()
    {
        
    }
    public class SpawnData
    {
        public Vector3 SpawnPos;
    }

    private SpawnData _data;

    public void OnDespawned()
    {
    }

    public void OnSpawned(SpawnData p1, IMemoryPool p2)
    {
        _data = p1;
        transform.position = p1.SpawnPos;
    }
}

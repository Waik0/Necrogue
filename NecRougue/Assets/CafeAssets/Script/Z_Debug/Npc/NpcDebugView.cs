using System.Collections;
using System.Collections.Generic;
using CafeAssets.Script.Interface.Layer_01.Manager;
using CafeAssets.Script.Model;
using UnityEngine;
using Zenject;

public interface INpcDebugUnitSpawner
{
    void Spawn(NpcDebugModel model);
}
public interface INpcDebugFacade
{
    
}
public interface  INpcDebugView{}
public class NpcDebugView : MonoBehaviour,INpcDebugView,INpcSpawnReceiver
{
    private INpcDebugUnitSpawner _spawner;
    [Inject]
    public void Inject(
        INpcDebugUnitSpawner spawner
    )
    {
        _spawner = spawner;
    }


    public void OnSpawnNpc(NpcModel model)
    {
        Debug.Log(" WEFAWFAWCVWAVASDFASFCZAEVAWEFAW");
        _spawner.Spawn(new NpcDebugModel()
        {
            ChaseObject = model.GameObject
        });
    }
}


using System.Collections;
using System.Collections.Generic;
using CafeAssets.Script.Impliment.Game.Layer_01.Factory;
using CafeAssets.Script.Interface.Layer_01.Manager;
using CafeAssets.Script.Interface.Registry;
using CafeAssets.Script.Model;
using CafeAssets.Script.System.GameNpcSystem;
using UnityEngine;
using Zenject;

public class NpcDebugUnitSpawner : INpcDebugUnitSpawner
{

    private NpcDebugUnitCollectionFactory _factory;
    //private List<INpcFacade> _npcFacades = new List<INpcFacade>();
    public NpcDebugUnitSpawner(
        NpcDebugUnitCollectionFactory factory
    )
    {
        _factory = factory;
    }
    public void Spawn(NpcDebugModel model)
    {
        _factory.Create(model);
    }

}

class NpcDebugUnitPool : MonoPoolableMemoryPool<NpcDebugModel, IMemoryPool, NpcDebugUnitFacade>
{
}

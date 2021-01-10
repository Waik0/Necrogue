using System;
using System.Collections;
using System.Collections.Generic;
using CafeAssets.Script.GameComponents.Npc;
using CafeAssets.Script.System.GameNpcSystem;
using UnityEngine;

/// <summary>
/// 特定のNpcのParamに他のNpcがアクセスするためのクラス
/// </summary>
public interface INpcParamCollector
{
    string[] FindParamHolderIds<T>(Func<T, bool> condition) where T : class,INpcParamCollection;

    T GetParam<T>(string id) where T : class,INpcParamCollection;
    //<T> GetParam(string id, T key);
}

public class NpcParamCollector : INpcParamCollector 
{
    private INpcRegistry _npcRegistry;
    public NpcParamCollector(
        INpcRegistry npcRegistry
        )
    {
        _npcRegistry = npcRegistry;
    }
    public T GetParam<T>(string id) where T : class,INpcParamCollection
    {
        foreach (var npcCollection in _npcRegistry.Entity)
        {
            if (npcCollection is INpcFacade facade)
            {
                if (npcCollection.Id == id)
                {
                    return facade.OwnParamRegistry().Find<T>();
                }
            }
        }

        return default;
    }

    public string[] FindParamHolderIds<T>(Func<T,bool> condition) where T : class,INpcParamCollection
    {
        var ret = new List<string>();
        foreach (var npcCollection in _npcRegistry.Entity)
        {
            if (npcCollection is INpcFacade facade)
            {
                var param = facade.OwnParamRegistry().Find<T>();
                var r = condition.Invoke(param);
                if(r){
                    ret.Add(npcCollection.Id);
                }
            }
        }

        return ret.ToArray();
    }
}

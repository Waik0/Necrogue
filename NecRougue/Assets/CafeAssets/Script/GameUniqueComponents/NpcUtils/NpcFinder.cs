using System.Collections;
using System.Collections.Generic;
using CafeAssets.Script.GameComponents.Npc;
using UnityEngine;

public interface INpcFinder
{
    INpcFacade Find(string id);
}
/// <summary>
/// IdからFacade探す
/// </summary>
public class NpcFinder : INpcFinder
{
    private INpcRegistry _npcRegistry;
    public NpcFinder(
        INpcRegistry registry
    )
    {
        _npcRegistry = registry;
    }
    public INpcFacade Find(string id)
    {
        if(_npcRegistry.Find(id) is INpcFacade facade)
        {
            return facade;
        }
        return null;
    }
}

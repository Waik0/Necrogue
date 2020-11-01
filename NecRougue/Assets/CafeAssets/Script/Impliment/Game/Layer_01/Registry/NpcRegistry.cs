using System.Collections;
using System.Collections.Generic;
using CafeAssets.Script.Interface.Registry;
using UnityEngine;

/// <summary>
/// アクティブなNPCを管理
/// </summary>
public class NpcRegistry : INpcRegistry
{
    readonly List<INpcCollection> _entities = new List<INpcCollection>();

    public IEnumerable<INpcCollection> Entity => _entities;

    public void Add(INpcCollection enemy)
    {
        _entities.Add(enemy);
    }

    public void Remove(INpcCollection enemy)
    {
        _entities.Remove(enemy);
    }
}

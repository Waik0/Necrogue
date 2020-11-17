using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NpcDebugUnitRegistry : INpcDebugUnitRegistry
{
    readonly List<INpcDebugUnitCollection> _entities = new List<INpcDebugUnitCollection>();

    public IEnumerable<INpcDebugUnitCollection> Entity => _entities;

    public void Add(INpcDebugUnitCollection enemy)
    {
        _entities.Add(enemy);
    }

    public void Remove(INpcDebugUnitCollection enemy)
    {
        _entities.Remove(enemy);
    }
}

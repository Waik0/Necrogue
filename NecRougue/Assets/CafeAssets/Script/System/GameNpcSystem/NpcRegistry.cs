using System.Collections;
using System.Collections.Generic;
using CafeAssets.Script.System.GameNpcSystem;
using UnityEngine;

/// <summary>
/// アクティブなNPCを管理
/// </summary>
public class NpcRegistry
{
    readonly List<NpcFacade> _entities = new List<NpcFacade>();

    public IEnumerable<NpcFacade> Entity
    {
        get { return _entities; }
    }

    public void Add(NpcFacade enemy)
    {
        _entities.Add(enemy);
    }

    public void Remove(NpcFacade enemy)
    {
        _entities.Remove(enemy);
    }
}

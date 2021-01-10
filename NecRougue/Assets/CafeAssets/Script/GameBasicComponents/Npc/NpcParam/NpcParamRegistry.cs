using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

/// <summary>
/// 他のNpcインスタンスからアクセスできるようにすべてのParamを管理する
/// </summary>
public interface INpcParamCollection
{
    void Reset();
    void Tick();
    string Log();
}
public interface INpcParamRegistry
{
    IEnumerable<INpcParamCollection> Entity { get; }
    T Find<T>() where T :  class, INpcParamCollection;
    void Reset();
    void Tick();
}
/// <summary>
/// アクティブなNPCを管理
/// </summary>
public class NpcParamRegistry : INpcParamRegistry
{
    readonly List<INpcParamCollection> _entities = new List<INpcParamCollection>();

    public IEnumerable<INpcParamCollection> Entity => _entities;

    public NpcParamRegistry(
        List<INpcParamCollection> collections)
    {
        _entities = collections;
    }
    public T Find<T>()  where T : class, INpcParamCollection
    {
        return (T)_entities.FirstOrDefault(ins => ins is T c);
    }
    public void Tick()
    {
        foreach (var npcParamCollection in _entities)
        {
            npcParamCollection.Tick();
        }
    }

    public void Reset()
    {
        foreach (var npcParamCollection in _entities)
        {
            npcParamCollection.Reset();
        }
    }
}

using System.Collections.Generic;
using System.Linq;

/// <summary>
/// レジストリで管理するNpc
/// </summary>
public interface INpcCollection
{
    string Id { get;  }
    void Tick();
}
public interface INpcRegistry
{
    IEnumerable<INpcCollection> Entity { get; }
    void Add(INpcCollection element);
    void Remove(INpcCollection element);
    INpcCollection Find(string id);
    void Update();
}
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

    public INpcCollection Find(string id)
    {
        return _entities.FirstOrDefault(npcCollection => npcCollection.Id == id);
    }

    public void Update()
    {
        foreach (var npcCollection in _entities)
        {
            npcCollection.Tick();
        }
    }
}

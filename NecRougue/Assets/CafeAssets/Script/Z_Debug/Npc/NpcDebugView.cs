using CafeAssets.Script.GameComponents.Npc;
using CafeAssets.Script.GameComponents.NpcCollection;
using UniRx;
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
/// <summary>
/// 各NPC上に表示するデバッグ情報
/// </summary>
public class NpcDebugView : MonoBehaviour,INpcDebugView
{
    private INpcDebugUnitSpawner _spawner;
    private INpcSpawner _npcSpawner;
    [Inject]
    public void Inject(
        INpcDebugUnitSpawner spawner,
        INpcSpawner npcSpawner
    )
    {
        _spawner = spawner;
        _npcSpawner = npcSpawner;
    }

    private void Awake()
    {
        _npcSpawner.OnSpawn.Subscribe(OnSpawnNpc).AddTo(this);
    }

    void OnSpawnNpc(INpcCollection model)
    {
        var facade = model as NpcFacade;
        if (!facade)
        {
            Debug.LogError("facade NULL");
        }
        _spawner.Spawn(new NpcDebugModel()
        {
            ChaseObject = facade.gameObject
        });
    }
}


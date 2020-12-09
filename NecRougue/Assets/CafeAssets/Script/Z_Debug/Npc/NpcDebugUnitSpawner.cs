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
        DebugLog.LogClassName(this,$"NPC生成します");
        _factory.Create(model);
    }

}

class NpcDebugUnitPool : MonoPoolableMemoryPool<NpcDebugModel, IMemoryPool, NpcDebugUnitFacade>
{
}

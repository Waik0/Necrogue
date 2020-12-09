using CafeAssets.Script.GameComponents.Npc;
using Zenject;

namespace CafeAssets.Script.GameComponents.NpcCollection
{

    public class NpcCollectionFactory : PlaceholderFactory<NpcFacadeModel, INpcCollection> { }
    class NpcFacadePool : MonoPoolableMemoryPool<NpcFacadeModel, IMemoryPool, NpcFacade> { }
}
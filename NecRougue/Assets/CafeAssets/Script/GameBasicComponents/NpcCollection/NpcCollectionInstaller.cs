using CafeAssets.Script.GameComponents.Npc;
using UnityEngine;
using Zenject;

namespace CafeAssets.Script.GameComponents.NpcCollection
{
    public class NpcCollectionInstaller : MonoInstaller
    {
        [SerializeField]
        private NpcFacade NpcFacade;
        public override void InstallBindings()
        {
            Container.BindInterfacesTo<NpcRegistry>().AsSingle();
            Container.BindInterfacesAndSelfTo<NpcSpawner>().AsCached().NonLazy();
            Container.BindInterfacesTo<NpcFinder>().AsCached().NonLazy();
            Container.BindInterfacesTo<NpcParamCollector>().AsCached().NonLazy();
            Container.BindFactory<NpcFacadeModel, INpcCollection, NpcCollectionFactory>().To<NpcFacade>().FromPoolableMemoryPool<NpcFacadeModel, NpcFacade, NpcFacadePool>(poolBinder => poolBinder
                .WithInitialSize(20)//スパイクが発生しないように初期化時に20体生成
                .FromSubContainerResolve()
                .ByNewPrefabInstaller<NpcInstaller>(NpcFacade)
                .UnderTransformGroup("NpcList"));
        }
    }
}
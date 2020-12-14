using Zenject;

namespace CafeAssets.Script.GameComponents.TilemapParams
{
    public class TilemapParamsInstaller : MonoInstaller
    {
        public override void InstallBindings()
        {
            Container.BindInterfacesTo<TilemapParamsFacade>().AsCached().NonLazy();
            Container.BindInterfacesTo<TilemapParameterRepositoryInternal>().AsCached().NonLazy();
            Container.BindInterfacesTo<TilemapParamsUseCase>().AsCached();
        }
    }
}

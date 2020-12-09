using CafeAssets.Script.System.GameParameterSystem;
using Zenject;

namespace CafeAssets.Script.GameComponents.TilemapParams
{
    public class TilemapParamsInstaller : MonoInstaller
    {
        public override void InstallBindings()
        {
            Container.BindInterfacesTo<TilemapParameterRepository>().AsCached().NonLazy();
            Container.BindInterfacesTo<TilemapStaticParamsUseCase>().AsCached().NonLazy();
            Container.BindInterfacesTo<TilemapDynamicParamsUseCase>().AsCached().NonLazy();
        }
    }
}

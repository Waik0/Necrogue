using Zenject;

namespace CafeAssets.Script.GameComponents.TilemapPassability

{
    public class TilemapPassabilityInstaller : MonoInstaller
    {
        public override void InstallBindings()
        {
            Container.BindInterfacesTo<TilemapPassabilityUseCase>().AsCached().NonLazy();
        }
    }
}
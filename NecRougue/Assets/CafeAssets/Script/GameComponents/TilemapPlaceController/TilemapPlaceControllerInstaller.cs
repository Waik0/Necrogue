using Zenject;

namespace CafeAssets.Script.GameComponents.TilemapPlaceController
{
    public class TilemapPlaceControllerInstaller : MonoInstaller
    {
        public TileSelectView TileSelect;

        public override void InstallBindings()
        {
            Container.BindInterfacesTo<PlaceTileUseCase>().AsCached().NonLazy();
            Container.BindInterfacesTo<TileSelectView>().FromComponentInNewPrefab(TileSelect).AsCached().NonLazy();
            InstallBehaviours();
        }

        void InstallBehaviours()
        {
            Container.BindInterfacesTo<PlaceTileBehaviourSingle>().AsCached().NonLazy();
            Container.BindInterfacesTo<PlaceTileBehaviourRect>().AsCached().NonLazy();
            Container.BindInterfacesTo<PlaceTileBehaviourRectPreview>().AsCached().NonLazy();
            Container.BindInterfacesTo<PlaceTileBehaviourSinglePreview>().AsCached().NonLazy();
        }
    }
}
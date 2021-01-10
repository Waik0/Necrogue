using CafeAssets.Script.GameComponents.TilemapParams;
using UnityEngine;
using Zenject;

namespace CafeAssets.Script.GameBasicComponents.TilemapParams
{
    public class TilemapParamsInstaller : MonoInstaller
    {
        public override void InstallBindings()
        {
            Debug.Log("[TilemapParamsInstaller]Install");
            Container.BindInterfacesTo<TilemapParamsFacade<TileEffectParams>>().AsCached().NonLazy();
            Container.BindInterfacesTo<TilemapParamsFacade<TileStaticParams>>().AsCached().NonLazy();
            Container.BindInterfacesTo<TilemapParameterRepositoryInternal<TileEffectParams>>().AsCached().NonLazy();
            Container.BindInterfacesTo<TilemapParameterRepositoryInternal<TileStaticParams>>().AsCached().NonLazy();
            Container.BindInterfacesTo<TilemapParamsUseCase<TileEffectParams>>().AsCached();
            Container.BindInterfacesTo<TilemapParamsUseCase<TileStaticParams>>().AsCached();
            
        }
    }
}

using CafeAssets.Script.GameComponents.TilemapPlaceController;
using CafeAssets.Script.GameComponents.TilemapPlacePreview.TilemapAdapter;
using UnityEngine;
using Zenject;

namespace CafeAssets.Script.GameComponents.TilemapPlacePreview
{
    public class TilemapPlacePreviewInstaller : MonoInstaller
    {
        [SerializeField]
        private TilemapAdapterForPreview _tilemapAdapter;
        public override void InstallBindings()
        {
            Container.BindInterfacesTo<TilemapPlacePreviewUseCase>().FromNewComponentOnNewGameObject().AsCached().NonLazy();
            Container.BindInterfacesTo<TilemapAdapterForPreview>().FromComponentInNewPrefab(_tilemapAdapter).AsCached().NonLazy();
        }
    }
}
using UnityEngine;
using Zenject;

namespace CafeAssets.Script.GameComponents.GameInput
{
    public class GameInputInstaller : MonoInstaller
    {
        [SerializeField]
        public GameInputAdapter GameCameraAdapter;
        public override void InstallBindings()
        {
            Container.BindInterfacesTo<GameInputAdapter>().FromComponentInNewPrefab(GameCameraAdapter).AsCached().NonLazy();
            InstallBehaviours();
        }

        void InstallBehaviours()
        {
            Container.BindInterfacesTo<SendCameraInput>().AsCached().NonLazy();
            Container.BindInterfacesTo<SendTilemapInput>().AsCached().NonLazy();
        }
    }
}

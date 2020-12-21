using CafeAssets.Script.System.GameCameraSystem;
using UnityEngine;
using Zenject;

namespace CafeAssets.Script.GameComponents.GameCamera
{
    public class GameCameraInstaller : MonoInstaller
    {
        [SerializeField]
        public GameCameraAdapter GameCameraAdapter;
        public override void InstallBindings()
        {
            Container.BindInterfacesTo<GameCameraAdapter>().FromComponentInNewPrefab(GameCameraAdapter).AsCached().NonLazy();
        }
    }
}

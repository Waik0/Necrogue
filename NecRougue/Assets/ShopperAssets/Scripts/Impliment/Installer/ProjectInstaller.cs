using ShopperAssets.Scripts.Game;
using ShopperAssets.Scripts.Interface.Game;
using UnityEngine;
using Zenject;

namespace ShopperAssets.Scripts.Installer
{
    public class ProjectInstaller : MonoInstaller
    {
        [SerializeField] private GameView _gameViewPrefab;
        public override void InstallBindings()
        {
            Debug.Log("Bind");    
            //MasterData
            Container.Bind<MasterdataManager>().FromNewComponentOnNewGameObject().AsSingle().NonLazy();
            //Game
            Container.Bind<IShopUsecase>().To<ShopUsecase>().AsSingle();
            Container.Bind<IEnemyUsecase>().To<EnemyUsecase>().AsSingle();
            Container.Bind<IPlayerUsecase>().To<PlayerUsecase>().AsSingle();
            Container.Bind<GamePresenter>().AsSingle();
            Container.Bind<GameSequence>().AsSingle();
            Container.Bind<GameView>().FromComponentInNewPrefab(_gameViewPrefab).AsCached();
            Container.Bind<AbilityResolver>().AsSingle();
        }
    }
}

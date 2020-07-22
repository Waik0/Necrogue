using ShopperAssets.Scripts.Game;
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
            Container.Bind<GameUseCase>().AsSingle();
            Container.Bind<GamePresenter>().AsSingle();
            Container.Bind<GameSequence>().AsSingle();
            Container.Bind<GameView>().FromComponentInNewPrefab(_gameViewPrefab).AsSingle();
        }
    }
}

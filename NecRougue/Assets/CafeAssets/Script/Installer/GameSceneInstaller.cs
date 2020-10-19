using System.Collections;
using System.Collections.Generic;
using CafeAssets.Script.System.GameCoreSystem;
using CafeAssets.Script.System.GameMapSystem;
using CafeAssets.Script.System.GameParameterSystem;
using CafeAssets.Script.System.GameTimeSystem;
using UnityEngine;
using Zenject;

public class GameSceneInstaller : MonoInstaller
{
    [SerializeField] private MapView _mapViewPrefab;
    [SerializeField] private GameTimeView _gameTimeView;
    /// <summary>
    /// Gameシーンでインストールされる項目
    /// </summary>
    public override void InstallBindings()
    {
        Debug.Log("[GameScene] Install");
        //UseCase
        Container.Bind<IGameUseCase>().To<GameUseCase>().AsCached().Lazy();
        Container.Bind<IGameParameterUseCase>().To<GameParameterUseCase>().AsCached().Lazy();
        Container.Bind<IGameTimeUseCase>().To<GameTimeUseCase>().AsCached().NonLazy();
        
        //Controller
        Container.Bind<IGameStaticDataController>().To<GameStaticDataController>().AsSingle().NonLazy();
        //View
        Container.Bind<MapView>().FromComponentInNewPrefab(_mapViewPrefab).AsCached().NonLazy();
        Container.BindInterfacesTo<GameTimeView>().FromComponentInNewPrefab(_gameTimeView).AsCached().NonLazy();
        
    }
}

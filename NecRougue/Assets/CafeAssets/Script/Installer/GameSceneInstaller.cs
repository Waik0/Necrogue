using System.Collections;
using System.Collections.Generic;
using CafeAssets.Script.System.GameCoreSystem;
using CafeAssets.Script.System.GameMapSystem;
using UnityEngine;
using Zenject;

public class GameSceneInstaller : MonoInstaller
{
    [SerializeField] private MapView _mapViewPrefab;
    /// <summary>
    /// Gameシーンでインストールされる項目
    /// </summary>
    public override void InstallBindings()
    {
        Debug.Log("[GameScene] Install");
        //Presenter
        Container.Bind<GamePresenter>().AsCached().Lazy();
        Container.Bind<GameParameterPresenter>().AsCached().Lazy();
        Container.Bind<MapView>().FromComponentInNewPrefab(_mapViewPrefab).AsCached().NonLazy();
        //Controller
        Container.Bind<IGameStaticDataController>().To<GameStaticDataController>().AsSingle().NonLazy();
    }
}

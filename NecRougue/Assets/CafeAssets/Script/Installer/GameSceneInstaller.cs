using System.Collections;
using System.Collections.Generic;
using CafeAssets.Script.System.GameCameraSystem;
using CafeAssets.Script.System.GameCoreSystem;
using CafeAssets.Script.System.GameInputSystem;
using CafeAssets.Script.System.GameMapSystem;
using CafeAssets.Script.System.GameParameterSystem;
using CafeAssets.Script.System.GameTimeSystem;
using UnityEngine;
using Zenject;

public class GameSceneInstaller : MonoInstaller
{
    [SerializeField] private MapView _mapViewPrefab;
    [SerializeField] private GameTimeView _gameTimeView;
    [SerializeField] private CameraView _cameraView;
    [SerializeField] private GameInputView _gameInputView;
    /// <summary>
    /// Gameシーンでインストールされる項目
    /// ※Lazyの使用に注意!!!
    /// ※Interfaceがうまく機能しない可能性がある
    /// </summary>
    public override void InstallBindings()
    {
        Debug.Log("[GameScene] Install");
        //UseCase
        Container.Bind<IGameUseCase>().To<GameUseCase>().AsCached().Lazy();
        Container.Bind<IGameParameterUseCase>().To<GameParameterUseCase>().AsCached().Lazy();
        Container.BindInterfacesTo<MapUseCase>().AsCached().NonLazy();
        //manager
        //特殊なinterfaceを管理するclass
        Container.BindInterfacesTo<GameTimeManager>().AsCached().NonLazy();//実装先のTickOnGameを呼び出す
        Container.BindInterfacesTo<GameResettableManager>().AsCached().NonLazy();//実装先のResetOnGameを呼び出す
        Container.BindInterfacesTo<GameScreenInputManager>().AsCached().NonLazy();//実装先のInputOnGameを呼び出す
        Container.BindInterfacesTo<MapPlaceManager>().AsCached().NonLazy();
        //Controller
        Container.BindInterfacesTo<GameStaticDataController>().AsSingle().NonLazy();
        //View
        Container.BindInterfacesTo<MapView>().FromComponentInNewPrefab(_mapViewPrefab).AsCached().NonLazy();
        Container.BindInterfacesTo<GameTimeView>().FromComponentInNewPrefab(_gameTimeView).AsCached().NonLazy();
        Container.BindInterfacesTo<CameraView>().FromComponentInNewPrefab(_cameraView).AsCached().NonLazy();
        Container.BindInterfacesTo<GameInputView>().FromComponentInNewPrefab(_gameInputView).AsCached().NonLazy();

    }
}

using System;
using System.Collections;
using System.Collections.Generic;
using CafeAssets.Script.Impliment.Game.Layer_01.Factory;
using CafeAssets.Script.Impliment.Game.Layer_02.UseCase;
using CafeAssets.Script.Impliment.Game.Layer_04.View;
using CafeAssets.Script.Model;
using CafeAssets.Script.System.GameCameraSystem;
using CafeAssets.Script.System.GameCoreSystem;
using CafeAssets.Script.System.GameInputSystem;
using CafeAssets.Script.System.GameMapSystem;
using CafeAssets.Script.System.GameNpcSystem;
using CafeAssets.Script.System.GameParameterSystem;
using CafeAssets.Script.System.GameTimeSystem;
using UnityEngine;
using Zenject;

public class GameSceneInstaller : MonoInstaller
{
    [Serializable]
    public class Settings
    {
        public TilemapView TilemapViewPrefab;
        public TilemapPassableView TilemapPassableViewPrefab;
        public MapPlacePreviewView MapPlacePreviewViewPrefab;
        public GameTimeView GameTimeView;
        public CameraView CameraView;
        public GameInputView GameInputView;
        public TileSelectView TileSelect;

        public NpcFacade NpcFacade;
    }

    [Inject] private Settings _settings;

    
    /// <summary>
    /// Gameシーンでインストールされる項目
    /// ※Lazyの使用に注意!!!
    /// ※Manager系がうまく機能しない可能性がある （Listだから内容変化してもいけるかもしれないけど
    /// </summary>
    public override void InstallBindings()
    {
        Debug.Log("[GameScene] Install");
        //1層
        //manager
        //特殊なinterfaceを管理するclass
        Container.BindInterfacesTo<GameTimeManager>().AsCached().NonLazy();//実装先のTickOnGameを呼び出す
        Container.BindInterfacesTo<GameResettableManager>().AsCached().NonLazy();//実装先のResetOnGameを呼び出す
        Container.BindInterfacesTo<GameScreenInputManager>().AsCached().NonLazy();//実装先のInputOnGameを呼び出す
        Container.BindInterfacesTo<TilePlaceTileManager>().AsCached().NonLazy();//OnMapPlace
        Container.BindInterfacesTo<TileSelectManager>().AsCached().NonLazy();//OnTileSelect
        Container.BindInterfacesTo<TilemapManager>().AsCached().NonLazy();//OnUpdateTilemap
        Container.BindInterfacesTo<NpcSpawnManager>().AsCached().NonLazy();//OnSpawn
        //Registory
        //アクティブなオブジェクトを管理するクラス
        Container.BindInterfacesTo<NpcRegistry>().AsSingle();
        //Repository
        //データ操作をするクラス
        Container.BindInterfacesTo<TilemapParameterRepository>().AsCached().NonLazy();
        
        //2層
        //UseCase
        //単一のビジネスルール
        //ViewやSequence,Controllerから呼び出される。(Managerからも)
        Container.BindInterfacesTo<GameUseCase>().AsCached().NonLazy();
        Container.BindInterfacesTo<GameParameterUseCase>().AsCached().NonLazy();
        Container.BindInterfacesTo<PlaceTileUseCase>().AsCached().NonLazy();
        Container.BindInterfacesTo<TilemapUseCase>().AsCached().NonLazy();
        Container.BindInterfacesTo<TilemapPassabilityUseCase>().AsCached().NonLazy();
        Container.BindInterfacesTo<TilemapParamUseCase>().AsCached().NonLazy();
        //Spawner
        Container.BindInterfacesAndSelfTo<NpcSpawner>().AsCached().NonLazy();
        //3層
        //Controller
        //Tickイベント駆動であらゆるUseCaseから情報を受け取ってゲームを進行させるやつ
        Container.BindInterfacesTo<DebugNpcSpawnTimingController>().AsCached().NonLazy();

        //Factory
        //プレハブにはサブコンテナを適用
        Container.BindFactory<NpcFacadeModel, INpcCollection, NpcCollectionFactory>().To<NpcFacade>().FromPoolableMemoryPool<NpcFacadeModel, NpcFacade, NpcFacadePool>(poolBinder => poolBinder
            .WithInitialSize(20)//スパイクが発生しないように初期化時に20体生成
            .FromSubContainerResolve()
            .ByNewPrefabInstaller<NpcInstaller>(_settings.NpcFacade)
            .UnderTransformGroup("NpcList"));
        
        //View
        Container.BindInterfacesTo<TilemapView>().FromComponentInNewPrefab(_settings.TilemapViewPrefab).AsCached().NonLazy();
        //Container.BindInterfacesTo<TilemapPassableView>().FromComponentInNewPrefab(_settings.TilemapPassableViewPrefab).AsCached().NonLazy();
        Container.BindInterfacesTo<MapPlacePreviewView>().FromComponentInNewPrefab(_settings.MapPlacePreviewViewPrefab).AsCached().NonLazy();
        Container.BindInterfacesTo<GameTimeView>().FromComponentInNewPrefab(_settings.GameTimeView).AsCached().NonLazy();
        Container.BindInterfacesTo<CameraView>().FromComponentInNewPrefab(_settings.CameraView).AsCached().NonLazy();
        Container.BindInterfacesTo<GameInputView>().FromComponentInNewPrefab(_settings.GameInputView).AsCached().NonLazy();
        Container.BindInterfacesTo<TileSelectView>().FromComponentInNewPrefab(_settings.TileSelect).AsCached().NonLazy();

    }
    class NpcFacadePool : MonoPoolableMemoryPool<NpcFacadeModel, IMemoryPool, NpcFacade>
    {
    }
}

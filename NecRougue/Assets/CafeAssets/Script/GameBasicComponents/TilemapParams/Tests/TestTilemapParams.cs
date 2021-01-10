using System;
using Zenject;
using System.Collections;
using System.Collections.Generic;
using CafeAssets.Script.GameBasicComponents.TilemapParams;
using CafeAssets.Script.GameComponents.TilemapParams;
using NUnit.Framework;
using UniRx;
using UnityEngine;
using UnityEngine.TestTools;
[TestFixture]
public class TestTilemapParams : ZenjectIntegrationTestFixture
{
    [Inject]
    ITilemapParamsFacade<FakeParamType> _target;
    
    public void CommonInstall()
    {
        // Setup initial state by creating game objects from scratch, loading prefabs/scenes, etc
        //var installerGameObject = new GameObject("Test_Installer",new []{typeof(TilemapParamsInstaller)});
        PreInstall();
        //var installer = installerGameObject.GetComponent<MonoInstaller>();
        Container.BindInterfacesTo<TilemapParamsFacade<FakeParamType>>().AsCached().NonLazy();
        Container.BindInterfacesTo<TilemapParameterRepositoryInternal<FakeParamType>>().AsCached().NonLazy();
        Container.BindInterfacesTo<TilemapParamsUseCase<FakeParamType>>().AsCached();
        //Container.Inject(installer);
        //installer.InstallBindings();
        PostInstall();
    }
    [UnityTest]
    public IEnumerator パラメータ追加と取得()
    {
        var disposable = new CompositeDisposable();
        var callNum = 0;
        var keyValuePair = new (Vector3Int pos, List<ITileParamsModelBase<FakeParamType>> models)[]
        {
            (new Vector3Int(1,2,3),
                new List<ITileParamsModelBase<FakeParamType>>()
                {
                    new FakeParamModel() {Key = FakeParamType.FakeA, Param = 1,Size = 0},
                    new FakeParamModel() {Key = FakeParamType.FakeB, Param = 2,Size = 0},
                } 
            ), 
            (new Vector3Int(4,5,6),
                new List<ITileParamsModelBase<FakeParamType>>()
                {
                    new FakeParamModel() {Key = FakeParamType.FakeB, Param = 3,Size = 5},
                    new FakeParamModel() {Key = FakeParamType.FakeC, Param = 4,Size = 0},
                } 
            ), 
        };

        CommonInstall();
        _target.OnUpdateTileParams.Subscribe(_ => CheckCallNum()).AddTo(disposable);
        _target.SetTileParam(keyValuePair);
        var testParams = new (Vector2Int pos,FakeParamType key,int result)[]
        {
            (new Vector2Int(1,2),FakeParamType.FakeA,1),
            (new Vector2Int(1,2),FakeParamType.FakeB,5),
            (new Vector2Int(4,5),FakeParamType.FakeB,3),
            (new Vector2Int(4,5),FakeParamType.FakeC,4),
        };
        foreach (var testParam in testParams)
        {
            Debug.Log($"Key : {testParam.pos} , {testParam.key} Param : {_target.GetTileParam(testParam.pos,testParam.key)})");
            Assert.IsTrue(_target.GetTileParam(testParam.pos,testParam.key) == testParam.result);
        }
        disposable.Dispose();
        yield break;
        void CheckCallNum()
        {
            Debug.Log("OnUpdateCalled");
            Assert.IsTrue(callNum == 0);
            callNum++;
        }
    }

    /// <summary>
    /// NPC 隣接or直接効果
    /// </summary>
    public enum FakeParamType
    {
        FakeA,//座る
        FakeB,//テーブル(飲食物等設置可能)
        FakeC,
    }

    [Serializable]
    public class FakeParamModel : ITileParamsModelBase<FakeParamType>
    {
        public int Param { get; set; }
        public int Size { get; set; }

        public FakeParamType Key { get; set; }
    
        public ITileParamsModelBase<FakeParamType> DeepCopy()
        {
            return new FakeParamModel()
            {
                Param = Param,
                Key = Key
            };
        }
    }
}
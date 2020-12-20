using System;
using Zenject;
using System.Collections;
using System.Collections.Generic;
using CafeAssets.Script.GameComponents.TilemapParams;
using NUnit.Framework;
using UniRx;
using UnityEngine;
using UnityEngine.TestTools;
[TestFixture]
public class TestTilemapParams : ZenjectIntegrationTestFixture
{
    [Inject]
    ITilemapParamsFacade _target;
    
    public void CommonInstall()
    {
        // Setup initial state by creating game objects from scratch, loading prefabs/scenes, etc
        var installerGameObject = new GameObject("Test_Installer",new []{typeof(TilemapParamsInstaller)});
        PreInstall();
        var installer = installerGameObject.GetComponent<MonoInstaller>();
        Container.Inject(installer);
        installer.InstallBindings();
        // Call Container.Bind methods
        PostInstall();
    }
    [UnityTest]
    public IEnumerator 単一パラメータ追加()
    {
        var disposable = new CompositeDisposable();
        var callNum = 0;
        var position = new Vector3Int(1,2,3);
        var param = new List<ITileParamsModelBase>()
        {
            new FakeParamModel() {Key = FakeParamType.FakeA, Param = 1},
            new FakeParamModel() {Key = FakeParamType.FakeB, Param = 2},
        };
        CommonInstall();
        _target.OnUpdateTileParams.Subscribe(_ => CheckCallNum()).AddTo(disposable);
        _target.SetTileParam(position,param);
        Debug.Log($"Key : {position} , {FakeParamType.FakeA} Param : {_target.GetTileParam(position,FakeParamType.FakeA).Param})");
        Assert.IsTrue(_target.GetTileParam(position,FakeParamType.FakeA).Param == 1);
        Debug.Log($"Key : {position} , {FakeParamType.FakeB} Param : {_target.GetTileParam(position,FakeParamType.FakeB).Param})");
        Assert.IsTrue(_target.GetTileParam(position,FakeParamType.FakeB).Param == 2);
        disposable.Dispose();
        yield break;
        void CheckCallNum()
        {
            Debug.Log("OnUpdateCalled");
            Assert.IsTrue(callNum == 0);
            callNum++;
        }
    }
    [UnityTest]
    public IEnumerator 複数パラメータ追加()
    {
        var disposable = new CompositeDisposable();
        var callNum = 0;
        var keyValuePair = new (Vector3Int pos, List<ITileParamsModelBase> models)[]
        {
            (new Vector3Int(1,2,3),
                new List<ITileParamsModelBase>()
                {
                    new FakeParamModel() {Key = FakeParamType.FakeA, Param = 1},
                    new FakeParamModel() {Key = FakeParamType.FakeB, Param = 2},
                } 
                ), 
            (new Vector3Int(4,5,6),
                new List<ITileParamsModelBase>()
                {
                    new FakeParamModel() {Key = FakeParamType.FakeB, Param = 3},
                    new FakeParamModel() {Key = FakeParamType.FakeC, Param = 4},
                } 
            ), 
        };

        CommonInstall();
        _target.OnUpdateTileParams.Subscribe(_ => CheckCallNum()).AddTo(disposable);
        _target.SetTileParam(keyValuePair);
        var testParams = new (Vector3Int pos,FakeParamType key,int result)[]
        {
            (new Vector3Int(1,2,3),FakeParamType.FakeA,1),
            (new Vector3Int(1,2,3),FakeParamType.FakeB,2),
            (new Vector3Int(4,5,6),FakeParamType.FakeB,3),
            (new Vector3Int(4,5,6),FakeParamType.FakeC,4),
        };
        foreach (var testParam in testParams)
        {
            Debug.Log($"Key : {testParam.pos} , {testParam.key} Param : {_target.GetTileParam(testParam.pos,testParam.key).Param})");
            Assert.IsTrue(_target.GetTileParam(testParam.pos,testParam.key).Param == testParam.result);
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
    public class FakeParamModel : ITileParamsModel<FakeParamType>
    {
        public int Param { get; set; }

        public FakeParamType Key { get; set; }
    
        public ITileParamsModelBase DeepCopy()
        {
            return new FakeParamModel()
            {
                Param = Param,
                Key = Key
            };
        }
    }
}
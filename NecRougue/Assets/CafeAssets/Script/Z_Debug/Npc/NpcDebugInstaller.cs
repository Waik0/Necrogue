using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Zenject;

public class NpcDebugInstaller : MonoInstaller
{
    [Serializable]
    public class Settings
    {
        public NpcDebugUnitFacade FacadePrefab;
        public NpcDebugView ViewPrefab;
    }

    [SerializeField] private Settings _settings;

    public override void InstallBindings()
    {
        Container.BindInterfacesTo<NpcDebugUnitRegistry>().AsSingle();
        Container.BindInterfacesAndSelfTo<NpcDebugUnitSpawner>().AsCached().NonLazy();
        Container.BindInterfacesTo<NpcDebugView>().FromComponentInNewPrefab(_settings.ViewPrefab).AsCached().NonLazy();
        //Factory
        //プレハブにはサブコンテナを適用
        Container.BindFactory<NpcDebugModel,INpcDebugUnitCollection, NpcDebugUnitCollectionFactory>().To<NpcDebugUnitFacade>().FromPoolableMemoryPool<NpcDebugModel, NpcDebugUnitFacade, NpcDebugUnitPool>(poolBinder => poolBinder
            .WithInitialSize(20)//スパイクが発生しないように初期化時に20体生成
            .FromSubContainerResolve()
            .ByNewPrefabInstaller<NpcDebugUnitInstaller>(_settings.FacadePrefab)
            .UnderTransformGroup("NpcDebugUnitList"));
        
    }
}

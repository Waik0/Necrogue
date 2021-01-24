using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Zenject;

public class TmsNpcSystemInstaller : MonoInstaller
{
    [SerializeField] private TmsNpcBase _prefab;
    public override void InstallBindings()
    {
        Container.BindInterfacesTo<TmsNpcSpawner>().AsCached().NonLazy();
        Container.BindFactory<TmsNpcBase.SpawnData, TmsNpcBase, TmsNpcBase.TmsNpcBaseFactory>().To<TmsNpcBase>().FromPoolableMemoryPool<TmsNpcBase.SpawnData, TmsNpcBase,TmsNpcBasePool>(poolBinder => poolBinder
            .WithInitialSize(20)//スパイクが発生しないように初期化時に20体生成
            .FromSubContainerResolve()
            .ByNewPrefabInstaller<TmsNpcPrefabInstaller>(_prefab)
            .UnderTransformGroup("TmsNpcList"));
    }
}

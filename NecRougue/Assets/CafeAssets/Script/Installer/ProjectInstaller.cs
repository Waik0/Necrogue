using System.Collections;
using System.Collections.Generic;
using CafeAssets.Script.Sequence;
using CafeAssets.Script.System.PropsSystem;
using UnityEngine;
using Zenject;

public class ProjectInstaller : MonoInstaller
{
    public override void InstallBindings()
    {
        Debug.Log("[PJIns] Install");
        //シーケンス周りをバインド
        Container.Bind<SequenceController>().FromNewComponentOnNewGameObject().AsSingle().NonLazy();
        Container.Bind<RootSequence>().FromNewComponentOnNewGameObject().AsSingle().Lazy();
        //Props
        Container.Bind<Props>().FromNewComponentOnNewGameObject().AsSingle().NonLazy();
    }
}

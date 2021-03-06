using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Zenject;

public class SceneInstaller : MonoInstaller
{
    public override void InstallBindings()
    {
        Sequence();
      
    }
    void Sequence()
    {
        Container.Bind<TitleRootSequence>().FromNewComponentOnNewGameObject().AsSingle().NonLazy();
        Container.BindInterfacesAndSelfTo<TitleSequence>().AsSingle().NonLazy();
        // Container.BindInterfacesAndSelfTo<MatchingHostSequence>().AsSingle().NonLazy();
        // Container.BindInterfacesAndSelfTo<MatchingClientSequence>().AsSingle().NonLazy();
        // Container.BindInterfacesAndSelfTo<InGameHostSequence>().AsSingle().NonLazy();
        // Container.BindInterfacesAndSelfTo<InGameClientSequence>().AsSingle().NonLazy();
    }

 
}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Zenject;

public class ProjectInstaller : MonoInstaller
{
    public override void InstallBindings()
    {
        Container.Bind<SceneProgress>().FromNewComponentOnNewGameObject().AsSingle().NonLazy();
    }
}

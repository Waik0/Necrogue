using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Zenject;

public class GameSampleInstaller : MonoInstaller
{
    public override void InstallBindings()
    {
        Container.Bind<WebSocketSample>().FromNewComponentOnNewGameObject().AsSingle();
    }
}

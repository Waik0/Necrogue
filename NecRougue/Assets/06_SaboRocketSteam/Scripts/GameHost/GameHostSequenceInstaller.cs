using System.Collections;
using System.Collections.Generic;
using SaboRocketSteam.Scripts.GameHost;
using SaboRocketSteam.Scripts.GameHost.Physics;
using SaboRocketSteam.Scripts.GameHost.States;
using UnityEngine;
using Zenject;

public class GameHostSequenceInstaller : MonoInstaller
{
    public override void InstallBindings()
    {
        Container.Bind<GameHostSequence>().FromNewComponentOnNewGameObject().AsSingle().NonLazy();
        Container.Bind<Init>().AsSingle();
        Container.Bind<WaitReady>().AsSingle();
        Container.Bind<WaitInput>().AsSingle();
        Container.Bind<CalcPhysics>().AsSingle();

        Container.Bind<PiecePhysicsUseCase>().AsSingle();
    }
}

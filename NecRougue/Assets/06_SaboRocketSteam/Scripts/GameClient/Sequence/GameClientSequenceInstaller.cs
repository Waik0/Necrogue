using System.Collections;
using System.Collections.Generic;
using SaboRocketSteam.Scripts.GameClient.Sequence.States;
using UnityEngine;
using Zenject;

public class GameClientSequenceInstaller : MonoInstaller
{
    public override void InstallBindings()
    {
        Container.Bind<BridgeProperty>().AsCached().IfNotBound();
        Container.Bind<GameClientSequence>().FromNewComponentOnNewGameObject().AsSingle().NonLazy();
        Container.Bind<Init>().AsSingle();
        Container.Bind<WaitReadyAll>().AsSingle();
        Container.Bind<WaitInputClient>().AsSingle();
        Container.Bind<PieceAnimation>().AsSingle();
        Container.Bind<Reload>().AsSingle();
    }
}

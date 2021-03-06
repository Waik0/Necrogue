using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Zenject;

public class HostInstaller : MonoInstaller
{
    [SerializeField] private Cursor _cursorPrefab;
    public override void InstallBindings()
    {
        Sequence();
        Network();
        Sender();
        Receiver();
        Registry();
        Other();
    }
    void Network()
    {
        Container.BindInterfacesAndSelfTo<TortecHostUseCaseWithWebSocket>().FromNewComponentOnNewGameObject().AsSingle().NonLazy();
    }
    void Sequence()
    { 
        Container.BindInterfacesAndSelfTo<HostRootSequence>().FromNewComponentOnNewGameObject().AsSingle().NonLazy(); 
        Container.BindInterfacesAndSelfTo<MatchingHostSequence>().AsSingle().NonLazy(); 
        Container.BindInterfacesAndSelfTo<InGameHostSequence>().AsSingle().NonLazy();
    }

    void Sender()
    {
        Container.BindInterfacesAndSelfTo<GameStartDataSender>().AsSingle().NonLazy();
        Container.BindInterfacesAndSelfTo<GameSequenceDataSender>().AsSingle().NonLazy();
        Container.BindInterfacesAndSelfTo<CursorMessageSender>().FromNewComponentOnNewGameObject().AsSingle().NonLazy();
        Container.BindInterfacesAndSelfTo<TimelineDataSender>().AsSingle().NonLazy();
        Container.BindInterfacesAndSelfTo<InputSender>().AsSingle().NonLazy();
        Container.BindInterfacesAndSelfTo<HandDataSender>().AsSingle().NonLazy();
    }

    void Receiver()
    {
        Container.BindInterfacesAndSelfTo<GameStartReceiver>().AsSingle().NonLazy();
        Container.BindInterfacesAndSelfTo<GameSequenceDataReceiver>().AsSingle().NonLazy();
        Container.BindInterfacesAndSelfTo<CursorMessageReceiver>().AsSingle().NonLazy();
        Container.BindInterfacesAndSelfTo<TimelineDataReceiver>().AsSingle().NonLazy();
        Container.BindInterfacesAndSelfTo<InputReceiver>().AsSingle().NonLazy();
        Container.BindInterfacesAndSelfTo<HandDataReceiver>().AsSingle().NonLazy();
    }

    void Registry()
    {
        Container.BindInterfacesAndSelfTo<PhysicsPieceRegistry>().FromNewComponentOnNewGameObject().AsSingle().NonLazy();
    }

    void Other()
    {
        Container.BindInterfacesAndSelfTo<PlayerDataUseCase>().AsSingle().NonLazy();
        Container.BindInterfacesAndSelfTo<PlayerTurnUseCase>().AsSingle().NonLazy();
        Container.BindInterfacesAndSelfTo<CursorDataUseCase>().AsSingle().WithArguments(_cursorPrefab).NonLazy();
        Container.BindInterfacesAndSelfTo<PhysicsUseCase>().FromNewComponentOnNewGameObject().AsSingle();
        Container.BindInterfacesAndSelfTo<ReadyStateChecker>().AsSingle().NonLazy();
        Container.BindInterfacesAndSelfTo<PieceViewerUseCase>().FromNewComponentOnNewGameObject().AsSingle();
        Container.BindInterfacesAndSelfTo<Ping>().FromNewComponentOnNewGameObject().AsSingle().NonLazy();
        Container.BindInterfacesAndSelfTo<HandUseCase>().AsSingle().NonLazy();
        Container.BindInterfacesAndSelfTo<DeckUseCase>().AsSingle().NonLazy();
    }
    
}

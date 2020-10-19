using System.Collections;
using System.Collections.Generic;
using CafeAssets.Script.Sequence;
using CafeAssets.Script.System.PropsSystem;
using UnityEngine;
using UnityEngine.EventSystems;
using Zenject;

public class ProjectInstaller : MonoInstaller
{
    [SerializeField] private EventSystem _eventSystem;
    public override void InstallBindings()
    {
        Debug.Log("[PJIns] Install");
        //シーケンス周りをバインド
        Container.Bind<SequenceController>().FromNewComponentOnNewGameObject().AsSingle().NonLazy();
        Container.Bind<RootSequence>().FromNewComponentOnNewGameObject().AsSingle().Lazy();
        //Props
        Container.Bind<Props>().FromNewComponentOnNewGameObject().AsSingle().NonLazy();
        //
        Container.Bind<EventSystem>().FromComponentInNewPrefab(_eventSystem).AsSingle().NonLazy();
    }
}

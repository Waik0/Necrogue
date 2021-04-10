using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Zenject;

public class CursorSystemInstaller : MonoInstaller
{
    [SerializeField] private Cursor _cursorPrefab;
    public override void InstallBindings()
    {
        Container.Bind<CursorDataReceiver>().AsSingle().NonLazy();
        Container.Bind<CursorDataSender>().FromNewComponentOnNewGameObject().AsSingle();
        Container.Bind<CursorViewPresenter>().AsSingle().WithArguments(_cursorPrefab).NonLazy();
    }
}

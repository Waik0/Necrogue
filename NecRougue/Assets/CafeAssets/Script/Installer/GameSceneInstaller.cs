using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Zenject;

public class GameSceneInstaller : MonoInstaller
{
    /// <summary>
    /// Gameシーンでインストールされる項目
    /// </summary>
    public override void InstallBindings()
    {
        Debug.Log("[GameScene] Install");
        Container.Bind<GameParameterPresenter>().AsCached().Lazy();
    }
}

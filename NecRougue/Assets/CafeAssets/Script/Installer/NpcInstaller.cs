using System.Collections;
using System.Collections.Generic;
using CafeAssets.Script.System.GameNpcSystem;
using UnityEngine;
using Zenject;

/// <summary>
/// NPC用のサブコンテナのインストーラー
/// </summary>
public class NpcInstaller : Installer<NpcInstaller>
{
    public override void InstallBindings()
    {
        Container.BindInterfacesTo<NpcAiUseCase>().AsSingle();
        Container.BindInterfacesTo<NpcParamUseCase>().AsSingle();
        //行動系
        //INpcActionUseCaseを継承しているもの
        Container.BindInterfacesTo<NpcStop>().AsCached();
        Container.BindInterfacesTo<NpcMoveToPlace>().AsCached();
    }
}

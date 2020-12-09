using System.Collections;
using System.Collections.Generic;
using CafeAssets.Script.GameComponents.Npc.NpcAi;
using CafeAssets.Script.GameComponents.Npc.NpcMove;
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
        Container.BindInterfacesTo<NpcMoveUseCase>().AsSingle();
        InstallNpcAction();
        InstallNpcCondition();
    }

    private void InstallNpcAction()
    {
        //行動系
        //INpcActionUseCaseを継承しているもの
        Container.BindInterfacesTo<NpcStop>().AsCached();
        Container.BindInterfacesTo<NpcMoveToRandomPlace>().AsCached();
        Container.BindInterfacesTo<NpcMoveToChair>().AsCached();

    }

    private void InstallNpcCondition()
    {
        Container.BindInterfacesTo<NpcStopCondition>().AsCached();
        Container.BindInterfacesTo<NpcMoveToRandomPlaceCondition>().AsCached();
        Container.BindInterfacesTo<NpcMoveToChairCondition>().AsCached();
    }
}

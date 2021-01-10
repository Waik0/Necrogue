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
        Container.BindInterfacesTo<NpcParamRegistry>().AsSingle();
        Container.BindInterfacesTo<NpcMoveUseCase>().AsSingle();
        Container.BindInterfacesTo<NpcAstarMoveUseCase>().AsSingle();
        InstallNpcAction();
        InstallNpcCondition();
        InstallNpcParams();
    }

    private void InstallNpcAction()
    {
        //行動系
        //INpcActionUseCaseを継承しているもの
        Container.BindInterfacesTo<NpcStop>().AsCached();
        Container.BindInterfacesTo<NpcMoveToRandomPlace>().AsCached();
        Container.BindInterfacesTo<NpcMoveToChair>().AsCached();
        Container.BindInterfacesTo<NpcOrder>().AsCached();
        Container.BindInterfacesTo<NpcMoveToOrder>().AsCached();

    }

    private void InstallNpcCondition()
    {
        Container.BindInterfacesTo<NpcStopCondition>().AsCached();
        Container.BindInterfacesTo<NpcMoveToRandomPlaceCondition>().AsCached();
        Container.BindInterfacesTo<NpcMoveToChairCondition>().AsCached();
        Container.BindInterfacesTo<NpcOrderCondition>().AsCached();
        Container.BindInterfacesTo<NpcMoveToOrderCondition>().AsCached();

    }

    private void InstallNpcParams()
    {
        Container.BindInterfacesAndSelfTo<NpcParamSitDown>().AsSingle();
        Container.BindInterfacesAndSelfTo<NpcParamOrder>().AsSingle();
        Container.BindInterfacesAndSelfTo<NpcParamTakeOrder>().AsSingle();
        Container.BindInterfacesAndSelfTo<NpcParamWaitTime>().AsSingle();

    }
}

using System.Collections;
using System.Collections.Generic;
using CafeAssets.Script.System.GameCoreSystem;
using CafeAssets.Script.System.GameTimeSystem;
using UnityEngine;
using Zenject;

public class GameTimeInstaller : MonoInstaller
{
    [SerializeField]
    public GameTimeView GameTimeView;
    public override void InstallBindings()
    {
        Container.BindInterfacesTo<GameTimeUseCase>().AsCached().NonLazy();
        Container.BindInterfacesTo<GameTimeView>().FromComponentInNewPrefab(GameTimeView).AsCached().NonLazy();
    }
}
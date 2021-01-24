using System.Collections;
using System.Collections.Generic;
using TileParamSystem.Source.Core.Script;
using UnityEngine;
using Zenject;
/// <summary>
///  todo いらないかもしれないので消す
/// </summary>
public class TileParamInstaller : MonoInstaller
{
    public override void InstallBindings()
    {
        Container.BindInterfacesTo<TileParamRegistry>().AsSingle();
    }
}
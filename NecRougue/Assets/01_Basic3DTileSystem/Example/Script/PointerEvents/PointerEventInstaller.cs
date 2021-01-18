using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Zenject;

public class PointerEventInstaller : MonoInstaller
{
    public override void InstallBindings()
    {
        Container.BindInterfacesTo<MoveCamera>().AsSingle();
        Container.BindInterfacesTo<TilePlaceSingle>().AsSingle();
        Container.BindInterfacesTo<TilePlaceRect>().AsSingle();
        Container.BindInterfacesTo<PointerEventSelecter>().AsSingle();
    }
}

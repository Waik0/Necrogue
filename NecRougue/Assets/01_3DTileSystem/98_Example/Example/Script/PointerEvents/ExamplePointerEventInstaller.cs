using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Zenject;

public class ExamplePointerEventInstaller : MonoInstaller
{
    public override void InstallBindings()
    {
        Container.BindInterfacesTo<PointerEventSelecter>().AsSingle();
        
        Container.BindInterfacesTo<MoveCamera>().AsSingle();
        Container.BindInterfacesTo<TilePlaceSingle>().AsSingle();
        Container.BindInterfacesTo<TilePlaceRect>().AsSingle();
        Container.BindInterfacesTo<MoveGetParamCursor>().AsSingle();
        Container.BindInterfacesTo<NpcSpawn>().AsSingle();
    }
}

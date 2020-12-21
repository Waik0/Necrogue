using System.Collections;
using System.Collections.Generic;
using CafeAssets.Script.GameUniqueComponents.FindChair;
using UnityEngine;
using Zenject;

public class TilemapUtilityInstaller : MonoInstaller
{
    public override void InstallBindings()
    {
        Container.BindInterfacesTo<FindChair>().AsCached();
    }
}

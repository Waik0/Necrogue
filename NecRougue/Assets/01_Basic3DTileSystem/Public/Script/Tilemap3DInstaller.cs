using System.Collections;
using System.Collections.Generic;
using Basic3DTileSystem.Source.Core.Script;
using Basic3DTileSystem.Source.Interface.Script;
using UnityEngine;
using Zenject;

/// <summary>
/// 外部で利用するためのインストーラー
/// </summary>
public class Tilemap3DInstaller : MonoInstaller
{
    [SerializeField] private Tilemap3DCameraControl _cameraControl;
    [SerializeField] private Tilemap3DEditFacade _facade;
    [SerializeField] private Tilemap3DTouchToPosition _touchToPosition;
    public override void InstallBindings()
    {
        Container.BindInterfacesTo<Tilemap3DInterfaceFacade>().AsSingle();
        Container.BindInterfacesTo<Tilemap3DCameraControl>().FromInstance(_cameraControl);
        Container.Bind<Tilemap3DSelectTile>().AsSingle();
        Container.BindInterfacesTo<Tilemap3DEditFacade>().FromInstance(_facade);
        Container.Bind<Tilemap3DTouchToPosition>().FromInstance(_touchToPosition);
    }
}

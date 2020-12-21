using System.Collections;
using System.Collections.Generic;
using CafeAssets.Script.GameComponents.Tilemap;
using CafeAssets.Script.GameComponents.TilemapAdapter;
using UnityEngine;
using Zenject;

public class TilemapInstaller : MonoInstaller
{
    [SerializeField] private TilemapAdapter _tilemapAdapter;
    public override void InstallBindings()
    {
        Container.BindInterfacesTo<TilemapUseCase>().AsCached().NonLazy();
        Container.BindInterfacesTo<TilemapAdapter>().FromComponentInNewPrefab(_tilemapAdapter).AsCached().NonLazy();
    }
}
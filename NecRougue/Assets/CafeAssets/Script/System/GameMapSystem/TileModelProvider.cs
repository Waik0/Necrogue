using System.Collections;
using System.Collections.Generic;
using CafeAssets.Script.System.GameMapSystem;
using CafeAssets.Script.System.GameMapSystem.TileInheritance;
using UnityEngine;
using Zenject;
[CreateAssetMenu(fileName = "TileModelProvider",menuName = "ScriptableObject/TileModelProvider")]
public class TileModelProvider : ScriptableObjectInstaller
{
    public FloorTileModel[] FloorTileModels;
    public FurnitureTileModel[] FurnitureTileModels;
    public GoodsTileModel[] GoodsTileModels;

    public override void InstallBindings()
    {
        Container.BindInstance(this);
    }
}

using System;
using System.Linq;
using CafeAssets.Script.GameComponents.Tilemap;
using CafeAssets.Script.GameComponents.TilemapAdapter.ScriptableObjects;
using CafeAssets.Script.GameComponents.TilemapModelAdapter.ScriptableObjects;
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

public static class TileModelProviderExtensions
{
    public static TileModel[] GetTileModelList(this TileModelProvider prov, TileType type)
    {
        switch (type)
        {
            case TileType.Floor: return prov.FloorTileModels;
            case TileType.Furniture: return prov.FurnitureTileModels;
            case TileType.Goods: return prov.GoodsTileModels;
            default:
                throw new ArgumentOutOfRangeException(nameof(type), type, null);
        }
    }
    public static ITileModel GetTileModel(this TileModelProvider prov,string name,TileType type)
    {
        switch (type)
        {
            case TileType.Floor:return prov.FloorTileModels.FirstOrDefault(_=>_.name == name);
            case TileType.Furniture:return prov.FurnitureTileModels.FirstOrDefault(_=>_.name == name);
            case TileType.Goods:return prov.GoodsTileModels.FirstOrDefault(_=>_.name == name);
        }

        return null;
    }
}


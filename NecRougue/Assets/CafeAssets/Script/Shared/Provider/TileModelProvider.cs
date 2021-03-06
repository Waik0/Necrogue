﻿using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using CafeAssets.Script.Model;
using CafeAssets.Script.System.GameMapSystem;
using CafeAssets.Script.System.GameMapSystem.TileInheritance;
using UnityEngine;
using UnityEngine.Tilemaps;
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
            case TileType.None:
            default:
                throw new ArgumentOutOfRangeException(nameof(type), type, null);
        }
    }
    public static FloorTileModel GetFloorTileModel(this TileModelProvider prov,string name)
    {
        return prov.FloorTileModels.FirstOrDefault(_=>_.name == name);
    }
    public static TileModel GetTileModel(this TileModelProvider prov,string name,TileType type)
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

public static class TileBaseExtensions
{
    public static TileModel GetTileModel(this TileBase self, TileModelProvider prov,string name)
    {
        TileModel t = prov.FloorTileModels.FirstOrDefault(_=>_.name == name);
        if (t == null) t = prov.FurnitureTileModels.FirstOrDefault(_ => _.name == name);
        if (t == null) t = prov.GoodsTileModels.FirstOrDefault(_ => _.name == name);
        Debug.Log($"Null {name}");
        return t;
    }
}
﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface ITilemap3DFacade
{
    void SetTileList(TileModel3DList list);
    ITileModel3D GetTileNewInstance(int index);
    ITileModel3D GetTileNewInstance(string fileName);
    void ClearAllTiles();
    void SetTile(Vector3Int pos, int index);
    void SetTiles(Vector3Int[] pos, int index);
    void SetTile(Vector3Int pos, string fileName);
    void SetTiles(Vector3Int[] pos, string fileName);
    void SetTile(Vector3Int pos, ITileModel3D model);
    ITileModel3D GetTile(Vector3Int pos);
    void RemoveTile(Vector3Int pos);
    Vector3Int WorldToCell(Vector3 world);
    Vector3 CellToWorld(Vector3Int cell);
    BoundsInt CellBounds { get; }
}
/// <summary>
/// 
/// </summary>
public class Tilemap3DFacade : MonoBehaviour,ITilemap3DFacade
{
    [SerializeField] private Tilemap3D _tilemap;
    [SerializeField] private TileModel3DLoader _tileModel3DLoader;


    public void SetTileList(TileModel3DList list)
    {
        _tileModel3DLoader.SetTileList(list);
    }

    public ITileModel3D GetTileNewInstance(int index)
    {
        return _tileModel3DLoader.GetTileInstance(index);
    }

    public ITileModel3D GetTileNewInstance(string fileName)
    {
        return _tileModel3DLoader.GetTileInstance(fileName);
    }

    public void ClearAllTiles()
    {
        _tilemap.ClearAllTiles();
    }

    public void SetTile(Vector3Int pos, int index)
    {
        var ins = _tileModel3DLoader.GetTileInstance(index);
        _tilemap.SetTile(pos,ins);
    }

    public void SetTiles(Vector3Int[] pos, int index)
    {
        foreach (var vector3Int in pos)
        {
            var ins = _tileModel3DLoader.GetTileInstance(index);
            _tilemap.SetTile(vector3Int,ins);
        }
    }

    public void SetTile(Vector3Int pos, string fileName)
    {
        var ins = _tileModel3DLoader.GetTileInstance(fileName);
        _tilemap.SetTile(pos,ins);
    }

    public void SetTiles(Vector3Int[] pos, string fileName)
    {
        foreach (var vector3Int in pos)
        {
            var ins = _tileModel3DLoader.GetTileInstance(fileName);
            _tilemap.SetTile(vector3Int,ins);
        }
    }

    public void SetTile(Vector3Int pos, ITileModel3D model)
    {
        _tilemap.SetTile(pos, model);
    }
    public ITileModel3D GetTile(Vector3Int pos)
    {
        return _tilemap.GetTile(pos);
    }

    public void RemoveTile(Vector3Int pos)
    {
        _tilemap.RemoveTile(pos);
    }

    public Vector3Int WorldToCell(Vector3 world)
    {
        return _tilemap.WorldToCell(world);
    }

    public Vector3 CellToWorld(Vector3Int cell)
    {
        return _tilemap.CellToWorld(cell);
    }

    public BoundsInt CellBounds => _tilemap.CellBounds;
}

﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface ITileModel3DLoader
{
    void SetTileList(TileModel3DList list);
    ITileModel3D GetTileInstance(int index);
    ITileModel3D GetTileInstance(string fileName);
}
public class TileModel3DLoader : MonoBehaviour
{
    private TileModel3DList _tileBase3DList;

    public void SetTileList(TileModel3DList list)
    {
        _tileBase3DList = list;
    }
    public ITileModel3D GetTileInstance(int index)
    {
        if (_tileBase3DList == null) return null;
        if (_tileBase3DList.Tiles.Length > index &&
            index >= 0)
        {
            var ins = Instantiate(_tileBase3DList.Tiles[index]);
            return ins;
        }
        return null;
    }

    public ITileModel3D GetTileInstance(string fileName)
    {
        if (_tileBase3DList == null) return null;
        foreach (var tileModel3D in _tileBase3DList.Tiles)
        {
            if (tileModel3D.gameObject.name == fileName)
            {
                var ins = Instantiate(tileModel3D);
                return ins;
            }
        }
        return null;
    }
}

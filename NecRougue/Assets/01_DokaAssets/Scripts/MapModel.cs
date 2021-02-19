using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;
[CreateAssetMenu(fileName = "MapModel", menuName = "Doka/MapModel")]
public class MapModel : ScriptableObject
{
    [SerializeField] private TileModel[] _models;

    public TileBase GetTile(int id)
    {
        foreach (var tileModel in _models)
        {
            if (tileModel.Id == id)
            {
                return tileModel.TileBase;
            }
        }
        return null;
    }
}

[Serializable]
public class TileModel
{
    public int Id;
    [SerializeField] private TileBase _tileBase;
    public TileBase TileBase => _tileBase;
}



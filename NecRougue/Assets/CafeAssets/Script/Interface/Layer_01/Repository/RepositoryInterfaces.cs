using System.Collections;
using System.Collections.Generic;
using CafeAssets.Script.Model;
using UnityEngine;

public interface ITilemapParameterRepository
{
    Dictionary<Vector2Int,TilemapPropsModel> Entity { get; }
    void Add(Vector2Int key, TilemapPropsModel model);
    TilemapPropsModel Get(Vector2Int key);
    void Remove(Vector2Int key);
}
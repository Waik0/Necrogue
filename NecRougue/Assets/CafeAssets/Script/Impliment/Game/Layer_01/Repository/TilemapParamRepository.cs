﻿using System.Collections;
using System.Collections.Generic;
using CafeAssets.Script.Model;
using UnityEngine;

/// <summary>
/// タイルマップに関する
/// </summary>
public class TilemapParameterRepository : ITilemapParameterRepository
{

    private Dictionary<Vector2Int, TilemapPropsModel> _entity = new Dictionary<Vector2Int, TilemapPropsModel>(); 
    public Dictionary<Vector2Int, TilemapPropsModel> Entity => _entity;

    public void Add(Vector2Int key, TilemapPropsModel model)
    {
        if(!_entity.ContainsKey(key))
            _entity.Add(key,null);
        _entity[key] = model;
    }

    public TilemapPropsModel Get(Vector2Int key)
    {
        if (!_entity.ContainsKey(key))
            return null;
        return _entity[key];
    }

    public void Remove(Vector2Int key)
    {
        if (!_entity.ContainsKey(key)) return;
        _entity.Remove(key);
    }
}

using System;
using System.Collections;
using System.Collections.Generic;
using CafeAssets.Script.System.GameMapSystem;
using UnityEngine;

public class GamePresenter : IDisposable
{
    private GameParameterPresenter _parameter;
    private MapView _mapView;

    public GamePresenter(
        MapView mapView)
    {
        _parameter = new GameParameterPresenter();
        _mapView = mapView;
    }
    /// <summary>
    /// セーブデータよみこみ
    /// </summary>
    /// <param name="path"></param>
    public void LoadData(string path)
    {
        
    }

    public void ResetParams()
    {
        _parameter.Reset();
    }

    public void ResetMap()
    {
        _mapView.Reset();
    }

    public void Dispose()
    {
    }
}

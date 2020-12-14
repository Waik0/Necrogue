
using System;
using System.Collections.Generic;
using CafeAssets.Script.GameComponents.TilemapParams;
using UnityEngine;

/// <summary>
/// タイルが供給する変更不可な情報
/// </summary>
public enum TileStaticParams
{
    American,
}

[Serializable]
public class TileStaticParamModelList : TileParamModelList<TileStaticParamModel>
{
}
[Serializable]
public class TileStaticParamModel : ITileParamsModel<TileStaticParams>
{
    [SerializeField]
    private TileStaticParams _key;
    [SerializeField]
    private int _param;


    public int Param { get => _param; set => _param = value; }

    public TileStaticParams Key { get => _key; set => _key = value; }
    
    public ITileParamsModelBase DeepCopy()
    {
        return new TileStaticParamModel()
        {
            _key = _key,
            _param = _param
        };
    }
}

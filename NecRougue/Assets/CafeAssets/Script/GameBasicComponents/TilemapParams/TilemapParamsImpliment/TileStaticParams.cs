
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
/// <summary>
/// お店の雰囲気を表すパラメータ
/// todo リネーム
/// </summary>
[Serializable]
public class TileStaticParamModel : ITileParamsModelBase<TileStaticParams>
{
    
    public int Param { get; set; }
    public int Size { get; set; }

    public TileStaticParams Key { get; set; }

}
[Serializable]
public class TileStaticParamModelInitial
{
    [SerializeField] private TileStaticParams _key;
    [SerializeField] private int _size;
    [SerializeField] private int _param;
    
    public TileStaticParamModel CreateCopy() {
        return new TileStaticParamModel()
        {
            Key = _key,
            Param = _param,
            Size =  _size
        };
    }
}
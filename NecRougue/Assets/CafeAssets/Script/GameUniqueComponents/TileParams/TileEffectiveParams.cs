using System;
using System.Collections.Generic;
using CafeAssets.Script.GameComponents.TilemapParams;
using UnityEngine;

/// <summary>
/// NPC 隣接or直接効果
/// </summary>
public enum TileEffectParams
{
    Sit,//座る
    Table,//テーブル(飲食物等設置可能)
    Order,//注文
    EffectRadius,//効果範囲
    ActionTime//行動にかかる時間
}

[Serializable]
public class TileEffectiveParamModel : ITileParamsModelBase<TileEffectParams>
{

    public int Param { get; set; }
    public int Size { get; set; }
    public TileEffectParams Key { get; set; }
    
}

[Serializable]
public class TileEffectiveParamModelInitial
{
    [SerializeField] private TileEffectParams _key;
    [SerializeField] private int _param;
    
    public TileEffectiveParamModel CreateCopy() {
        return new TileEffectiveParamModel()
        {
            Key = _key,
            Param = _param
        };
    }
}

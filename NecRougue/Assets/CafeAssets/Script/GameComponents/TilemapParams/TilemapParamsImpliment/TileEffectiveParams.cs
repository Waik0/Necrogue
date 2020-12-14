
using System;
using System.Collections.Generic;
using CafeAssets.Script.GameComponents.TilemapParams;
using UnityEngine;

/// <summary>
/// NPC 隣接or直接効果
/// </summary>
public enum TileEffectType
{
    Sit,//座る
    Table,//テーブル(飲食物等設置可能)
    Order,//注文
    EffectRadius,//効果範囲
    ActionTime//行動にかかる時間
}

[Serializable]
public class TileEffectiveParamModelList : TileParamModelList<TileEffectiveParamModel>
{
}
[Serializable]
public class TileEffectiveParamModel : ITileParamsModel<TileEffectType>
{
    [SerializeField]
    private TileEffectType _key;
    [SerializeField]
    private int _param;


    public int Param { get => _param; set => _param = value; }

    public TileEffectType Key { get => _key; set => _key = value; }
    
    public ITileParamsModelBase DeepCopy()
    {
        return new TileEffectiveParamModel()
        {
            _key = _key,
            _param = _param
        };
    }
}

using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEditor.VersionControl;
using UnityEngine;

namespace CafeAssets.Script.System.GameMapSystem
{
    public enum TileType
    {
        Floor,
        Furniture,
        Goods,
        None,
    }
    public enum PlaceTileMode
    {
        PlaceTileSingle,//一度だけ置く
        PlaceTileDraw,//ドラッグしたところにも置いていく
        PlaceTileRect,//四角形に置く
    
    }
    public class TileModel : IsometricRuleTile
    {
        public string Unique => this.name;
        [Header("表示情報")]
        public string Name;
        public string Description;
        [Header("入手に必要なパラメータ")]
        public NeedParameterModelSet[] NeedParameterForAppear;
        [Header("設置コスト")]
        public UseParameterModelSet[] UseParameterForPlace;
        [Header("設置効果")]
        public ProvideParameterModelSet[] ProvideParameter;
        [Header("設置可能レイヤ")]
        public int MinLayer;
        public int MaxLayer;
        [Header("通行可否(壁かどうか)")] 
        //同じ座標のすべてのレイヤがIsWall == falseじゃないと通行できない
        public bool IsWall;

        [Header("ブラシサイズ(単一の大きさ)")] 
        public Vector2Int Brush;
        
        [Header("デフォルトの配置モード")] 
        public PlaceTileMode PlaceMode = PlaceTileMode.PlaceTileSingle;

    }

    [Serializable]
    public class NeedParameterModelSet
    {
        public string Name;
        public int Num;
    }
    [Serializable]
    public class UseParameterModelSet
    {
        public string Name;
        public int Num;
    }
    [Serializable]
    public class ProvideParameterModelSet
    {
        public string Name;
        public GameParameterOperations Operation;
        public int Num;
    }
}


/// <summary>
/// ローカライズ対応
/// ここではない場所に置く
/// </summary>

public enum Region
{
    Ja = 0,
    En = 1,
}
[Serializable]
public class RegionTextSet
{
    
    public string Text;
    public Region Region;
}

public static class RegionTextSetExtensions
{
    public static string GetText(this RegionTextSet[] self,Region region)
    {
        return self.FirstOrDefault(_=>_.Region == region)?.Text;
    }
}
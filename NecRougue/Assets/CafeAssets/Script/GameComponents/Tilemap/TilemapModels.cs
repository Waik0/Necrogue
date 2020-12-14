using System;
using System.Linq;
using CafeAssets.Script.GameComponents.TilemapParams;
using UnityEngine;

namespace CafeAssets.Script.GameComponents.Tilemap
{
    
    /// <summary>
    /// ローカライズ対応
    /// ここではない場所に置く
    /// </summary>

    public enum Region
    {
        Ja = 0,
        En = 1,
    }
    /// <summary>
    /// タイルの種類
    /// </summary>
    public enum TileType
    {
        Floor,
        Furniture,
        Goods,
    }
    public enum PlaceTileMode
    {
        PlaceTileSingle,//一度だけ置く
        PlaceTileDraw,//ドラッグしたところにも置いていく
        PlaceTileRect,//四角形に置く

    }





    [Serializable]
    public class ProvideParameterModelSet
    {
        public string Name;
        public TilemapParameterOperations Operation;
        public int Num;
    }
    /// <summary>
    /// 各タイルの情報（静的）
    /// </summary>
    public interface ITileModel
    {
        TileStaticParamModelList StaticParams { get; }
        string GetName();//フローリングとか
        string GetSystemName();//Tile_0000とか
        bool GetIsWall();
        PlaceTileMode GetDefaultPlaceMode();
        ProvideParameterModelSet[] GetProvideParameter();
        Vector2Int BrushSize();
        int ZMin();
        int ZMax();
    }
    /// <summary>
    /// ここをImplに移動
    /// </summary>
    public interface ITileEffectiveModel : ITileModel
    {
        TileEffectiveParamModelList EffectiveParams { get; }
    }
    
    //ローカライズ
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
}
using System;
using System.Collections;
using System.Collections.Generic;
using CafeAssets.Script.GameComponents.Tilemap;
using CafeAssets.Script.GameComponents.TilemapParams;
using UnityEngine;

namespace CafeAssets.Script.GameComponents.TilemapAdapter.ScriptableObjects
{
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
        
        public virtual TileType Type {  get; }
    }
    
    public class BasicTileModel : TileModel,ITileModel
    {
        
        [Header("供給パラメータ")]
        [SerializeField]
        public TileStaticParamModelList _staticParams;

        public TileStaticParamModelList StaticParams => _staticParams;

        public string GetName()
        {
            return Name;
        }

        public string GetSystemName()
        {
            return name;
        }

        public bool GetIsWall()
        {
            return IsWall;
        }

        public PlaceTileMode GetDefaultPlaceMode()
        {
            return PlaceMode;
        }

        public ProvideParameterModelSet[] GetProvideParameter()
        {
            return ProvideParameter;
        }

        public Vector2Int BrushSize()
        {
            return Brush;
        }

        public int ZMin()
        {
            return MinLayer;
        }

        public int ZMax()
        {
            return MaxLayer;
        }
    }
    
//キャラクターが乗るか隣接すると効果が発揮されるタイル
//IsWall = trueなら隣接 falseなら乗る
//テーブル、レジ、コーヒーメーカーなど想定
    public class EffectiveTileModel : TileModel,ITileEffectiveModel
    {
        [Header("効果")] 
        private TileEffectiveParamModelList _effectiveParams;

        public TileEffectiveParamModelList EffectiveParams => _effectiveParams;

        private TileStaticParamModelList _staticParams;

        public TileStaticParamModelList StaticParams => _staticParams;

        public string GetName()
        {
            return Name;
        }

        public string GetSystemName()
        {
            return name;
        }

        public bool GetIsWall()
        {
            return IsWall;
        }

        public ProvideParameterModelSet[] GetProvideParameter()
        {
            return ProvideParameter;
        }
        public PlaceTileMode GetDefaultPlaceMode()
        {
            return PlaceMode;
        }
        public int GetEffectRadius()
        {
            return 0;//EffectRadius;
        }
        public Vector2Int BrushSize()
        {
            return Brush;
        }
        public int ZMin()
        {
            return MinLayer;
        }

        public int ZMax()
        {
            return MaxLayer;
        }

       
    }
}
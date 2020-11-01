using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.Tilemaps;

namespace CafeAssets.Script.Model
{ 
    #region NpcSystem
    public class NpcActionModel
    {
        public string Param;
    }
    public class NpcAiModel
    {
        
    }
    public class NpcFacadeModel
    {
        public string Name;
        public NpcType Type;
        public NpcAiModel Ai;
    }
    public class NpcModel
    {
    
    }
    public class NpcParamModel
    {
        public Dictionary<ParameterStyle, int> Param;
        public NpcParamModel()
        {
            Param = new Dictionary<ParameterStyle, int>();
            foreach (ParameterStyle key in Enum.GetValues(typeof(ParameterStyle)))
            {
                Param.Add(key,0);
            }
        }

        public void Set(ParameterStyle key, int num)
        {
            Param[key] = num;
        }

        public int Get(ParameterStyle key)
        {
            return Param[key];
        }

        public int GetSum()
        {
            return Param.Sum(keyValuePair => keyValuePair.Value);
        }
    }
    #endregion

    #region Input

    public class GameInputModel
    {
        public Vector3 CurrentPos;
        public Vector3 WorldCurrentPos;
        public Vector3 Delta;
        public Vector3 WorldDelta;
        public Vector3 DownPos;
        public Vector3 WorldDownPos;
        public InputMode InputMode;
        public GameInputState State;
    }

    #endregion

    #region GameTime

     public class GameTimeModel
        {
            public GameTimeModel(long first)
            {
                TotalMinutes = first;
            }
    
            public long TotalMinutes; //最小単位は分
            public long Minutes => TotalMinutes % 60;
            public long Hour => (TotalMinutes / 60) % 24;
            public long Day => (TotalMinutes / 1440) % 7 + 1;
            public long Week => (TotalMinutes / 10080) % 4 + 1;
            public long Month => (TotalMinutes / 40320) % 12 + 1;
            public long Year => (TotalMinutes / 483840) + 1;
        }

    #endregion


    #region TileSystem

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
    
    public class BasicTileModel : TileModel
    {
    }
    
    //キャラクターが乗るか隣接すると効果が発揮されるタイル
    //IsWall = trueなら隣接 falseなら乗る
    //テーブル、レジ、コーヒーメーカーなど想定
    public class EffectiveTileModel : TileModel
    {
        [Header("効果種類")] 
        public TileEffectType EffectType;
        [Header("作業時間")]
        //注文や飲食物作成にかかる時間
        //todo キャラのパラメーターで短縮かのうにする
        public float ActionTime;
        
        
        
    }
    public class TilePlaceModel
    {
        public TileModel Model;
        public PlaceTileMode PlaceMode;
        public Vector3 StartWorldPos;
        public Vector3 EndWorldPos;
        public int Z;
    }

    public class TilemapModel
    {
        public Tilemap Tilemap;
    }
    #endregion



    
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
    public class GameParamModel
    {
        public Dictionary<ParameterStyle, int> Param;
        public GameParamModel()
        {
            Param = new Dictionary<ParameterStyle, int>();
            foreach (ParameterStyle key in Enum.GetValues(typeof(ParameterStyle)))
            {
                Param.Add(key,0);
            }
        }

        public void Set(ParameterStyle key, int num)
        {
            Param[key] = num;
        }

        public int Get(ParameterStyle key)
        {
            return Param[key];
        }

        public int GetSum()
        {
            return Param.Sum(keyValuePair => keyValuePair.Value);
        }
    }
    
}
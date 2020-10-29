using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using CafeAssets.Script.System.GameMapSystem;
using CafeAssets.Script.System.GameMapSystem.TileInheritance;
using UnityEngine;
using UnityEngine.Tilemaps;

public enum ParameterStyle
{
    Fixable,
    Fluid
    
}
namespace CafeAssets.Script.System.GameParameterSystem
{
    public interface IGameParameterUseCase
    {
        void Reset();
        string[] GetKeys();
        int Get(string key);
        int Set(ParameterStyle style, string key, int num);
        int Operation(ParameterStyle style, string key, GameParameterOperations operation, int num);
    }
    public class GameParameterUseCase : IGameParameterUseCase,IDisposable,ITilemapReceiver
    {
        private Dictionary<string, GameParamModel> IntParams;//固定資産
        private TileModelProvider _tileModelProvider;
        public GameParameterUseCase(
            TileModelProvider tileModelProvider
            )
        {
            _tileModelProvider = tileModelProvider;
        }
        public void Reset()
        {
            if (IntParams == null)
            {
                IntParams = new Dictionary<string, GameParamModel>();
            }
            IntParams.Clear();
        }

        public string[] GetKeys()
        {
            if (IntParams == null)
            {
                IntParams = new Dictionary<string, GameParamModel>();
            }
            return IntParams.Keys.ToArray();
        }

        void AddModel(string key)
        {
            if (!IntParams.ContainsKey(key))
            {
                IntParams.Add(key,new GameParamModel());
            }
        }

     
        public int Get(string key)
        {
            AddModel(key);
            return IntParams[key].GetSum();
        }

        void Clear(ParameterStyle style)
        {
            foreach (var gameParamModel in IntParams)
            {
                gameParamModel.Value.Set(style,0);
            }
        }
        public int Set(ParameterStyle style,string key, int num)
        {
            AddModel(key);
            IntParams[key].Set(style,num);
            return IntParams[key].GetSum();
        }

        public int Operation(ParameterStyle style,string key, GameParameterOperations operation, int num)
        {
            AddModel(key);
            switch (operation)
            {
                case GameParameterOperations.Add:
                    IntParams[key].Set(style, IntParams[key].Get(style) + num);
                    break;
                case GameParameterOperations.Times:
                    IntParams[key].Set(style, IntParams[key].Get(style) * num);
                    break;
                case GameParameterOperations.Division:
                    IntParams[key].Set(style, IntParams[key].Get(style) / num);
                    break;
            }
            return IntParams[key].GetSum();
        }

        public void Dispose()
        {
            Debug.Log("[GameParam]Dispose");
        }

        public void OnUpdateTile(TilemapModel model)
        { 
            CalcTileParam(model.Tilemap);
        }
        //todo 重くなりそうなので色々考えたい
        void CalcTileParam(Tilemap tilemap)
        {
            Clear(ParameterStyle.Fixable);
            List<ProvideParameterModelSet> models = new List<ProvideParameterModelSet>();
            foreach (var vector3Int in tilemap.cellBounds.allPositionsWithin)
            {
                var tile = tilemap.GetTile<TileModel>(vector3Int);
                if(tile == null)continue;
                foreach (var provideParameterModelSet in tile.ProvideParameter)
                {
                    models.Add(provideParameterModelSet);
                }
                /*
                switch (tile)
                {
                    case FloorTileModel floorTileModel:
                        break;
                    case BasicTileModel basicTileModel:
                        break;
                    case FurnitureTileModel furnitureTileModel:
                        break;
                    case GoodsTileModel goodsTileModel:
                        break;
                    case EffectiveTileModel effectiveTileModel:
                        break;
                    default:
                        throw new ArgumentOutOfRangeException(nameof(tile));
                }
                */
               
            }

            Debug.Log(models.Count);
            foreach (var provideParameterModelSet in models.OrderBy(_ => _.Operation))
            {
                Debug.Log($"{ provideParameterModelSet.Name} { provideParameterModelSet.Operation} {provideParameterModelSet.Num}");
                Operation(ParameterStyle.Fixable, provideParameterModelSet.Name, provideParameterModelSet.Operation,provideParameterModelSet.Num);
            }
           
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
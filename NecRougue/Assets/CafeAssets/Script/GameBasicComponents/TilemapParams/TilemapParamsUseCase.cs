using System.Collections;
using System.Collections.Generic;
using CafeAssets.Script.GameComponents.Tilemap;
using CafeAssets.Script.GameComponents.TilemapAdapter.ScriptableObjects;
using UnityEngine;
using UnityEngine.Tilemaps;
using NotImplementedException = System.NotImplementedException;

namespace CafeAssets.Script.GameComponents.TilemapParams
{
    public interface ITilemapParamsUseCase<T> where T : struct
    {
        Dictionary<Vector3Int,List<ITileParamsModelBase<T>>> Entity { get; }
        void SetTileParam(Vector3Int key,List<ITileParamsModelBase<T>> tileModel);
        int GetTileParam(Vector2Int pos, T key);
        Dictionary<T,int> GetTileParams(Vector2Int pos);
        void UpdateTileParam(Vector3Int pos, T key, int Param);
        void RemoveTileParam(Vector3Int pos);
        //void SetTileParams(Vector2Int key,ITileModel[] tileModel);
    }
    public sealed class TilemapParamsUseCase<T> : ITilemapParamsUseCase<T> where T : struct
    {
        private ITilemapParameterRepository<T> _repository;
        public TilemapParamsUseCase(
            ITilemapParameterRepository<T> repository
        )
        {
            _repository = repository;
        }

        public Dictionary<Vector3Int, List<ITileParamsModelBase<T>>> Entity => _repository.Entity;
        public void SetTileParam(Vector3Int pos,List<ITileParamsModelBase<T>> tileModel)
        {
            _repository.Add(pos,tileModel);
        }

        public int GetTileParam(Vector2Int pos, T key)
        {
            int param = 0;
            //全パラメータを探索し、効果範囲にposが含まれるパラメータを取得
            foreach (var keyValuePair in _repository.Entity)
            {
                var rect = new RectInt(new Vector2Int(keyValuePair.Key.x,keyValuePair.Key.y), Vector2Int.zero);
               
                foreach (var tileParamsModelBase in keyValuePair.Value)
                {
                    if (!tileParamsModelBase.Key.Equals(key))
                    {
                        continue;
                    }

                    rect.position = new Vector2Int(keyValuePair.Key.x - tileParamsModelBase.Size,
                        keyValuePair.Key.y - tileParamsModelBase.Size);
                    rect.size = new Vector2Int(tileParamsModelBase.Size * 2  + 1, tileParamsModelBase.Size * 2 + 1);
                    //Debug.Log(rect);
                    if (rect.Contains(pos))
                    {
                        param += tileParamsModelBase.Param;
                    }
                }
            }
            return param;
        }

        public Dictionary<T,int> GetTileParams(Vector2Int pos)
        {
            Dictionary<T,int> param = new Dictionary<T, int>();
            //全パラメータを探索し、効果範囲にposが含まれるパラメータを取得
            foreach (var keyValuePair in _repository.Entity)
            {
                var rect = new RectInt(new Vector2Int(keyValuePair.Key.x,keyValuePair.Key.y), Vector2Int.zero);
                foreach (var tileParamsModelBase in keyValuePair.Value)
                {
      
                    rect.position = new Vector2Int(keyValuePair.Key.x - tileParamsModelBase.Size,
                        keyValuePair.Key.y - tileParamsModelBase.Size);
                    rect.size = new Vector2Int(tileParamsModelBase.Size * 2  + 1, tileParamsModelBase.Size * 2 + 1);
                    if (!rect.Contains(pos)) continue;
                    if (!param.ContainsKey(tileParamsModelBase.Key))
                    {
                        param.Add(tileParamsModelBase.Key,0);
                    }
                    param[tileParamsModelBase.Key] = tileParamsModelBase.Param;
                }
            }
            return param;
        }


        public void UpdateTileParam(Vector3Int pos, T key, int Param)
        {
            var data = _repository.Get(pos);
            foreach (var tilemapParamsModelBase in data)
            {
                if (tilemapParamsModelBase is ITileParamsModelBase<T> c && c.Key.Equals(key))
                {
                    Debug.Log($" {pos} {key} 値更新");
                    c.Param = Param;
                }
            }
        }

        public void RemoveTileParam(Vector3Int pos)
        {
            _repository.Remove(pos);
        }
    }
}
using System.Collections;
using System.Collections.Generic;
using CafeAssets.Script.GameComponents.Tilemap;
using CafeAssets.Script.GameComponents.TilemapAdapter.ScriptableObjects;
using UnityEngine;
using UnityEngine.Tilemaps;
using NotImplementedException = System.NotImplementedException;

namespace CafeAssets.Script.GameComponents.TilemapParams
{
    internal interface ITilemapParamsUseCase
    {
        Dictionary<Vector3Int,List<ITileParamsModelBase>> Entity { get; }
        void SetTileParam(Vector3Int key,List<ITileParamsModelBase> tileModel);
        ITileParamsModel<T> GetTileParam<T>(Vector3Int pos, T key) where T : struct;
        void UpdateTileParam<T>(Vector3Int pos, T key, int Param) where T : struct;
        void RemoveTileParam(Vector3Int pos);
        //void SetTileParams(Vector2Int key,ITileModel[] tileModel);
    }
    sealed class TilemapParamsUseCase : ITilemapParamsUseCase
    {
        private ITilemapParameterRepository _repository;
        public TilemapParamsUseCase(
            ITilemapParameterRepository repository
        )
        {
            _repository = repository;
        }

        public Dictionary<Vector3Int, List<ITileParamsModelBase>> Entity => _repository.Entity;
        public void SetTileParam(Vector3Int pos,List<ITileParamsModelBase> tileModel)
        {
            _repository.Add(pos,tileModel);
        }

        public ITileParamsModel<T> GetTileParam<T>(Vector3Int pos, T key) where T : struct
        {
            foreach (var tileParamsModelBase in _repository.Get(pos))
            {
                if (tileParamsModelBase is ITileParamsModel<T> c && c.Key.Equals(key))
                {
                    return c;
                }
            }
            return null;
        }


        public void UpdateTileParam<T>(Vector3Int pos, T key, int Param) where T : struct
        {
            var data = _repository.Get(pos);
            foreach (var tilemapParamsModelBase in data)
            {
                if (tilemapParamsModelBase is ITileParamsModel<T> c && c.Key.Equals(key))
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
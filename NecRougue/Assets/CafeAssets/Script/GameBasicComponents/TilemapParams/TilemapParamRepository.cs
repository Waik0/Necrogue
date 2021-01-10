using System.Collections.Generic;
using UnityEngine;

namespace CafeAssets.Script.GameComponents.TilemapParams
{
    public interface ITileParamsModelBase<T>
    {
        int Param { get; set; }
        int Size { get; set; }
        T Key { get; }
        //Vector2Int Size { get; set; }
    }
    public interface ITilemapParameterRepository<T> where T : struct
    {
        Dictionary<Vector3Int,List<ITileParamsModelBase<T>>> Entity { get; }
        void Add(Vector3Int key, List<ITileParamsModelBase<T>> model);

        List<ITileParamsModelBase<T>> Get(Vector3Int key);
        void Remove(Vector3Int key);
    }
    /// <summary>
    /// タイルマップに関するデータを保持
    /// </summary>
    public sealed class TilemapParameterRepositoryInternal<T> : ITilemapParameterRepository<T> where T : struct
    {

        private Dictionary<Vector3Int, List<ITileParamsModelBase<T>>> _entity = new Dictionary<Vector3Int,List<ITileParamsModelBase<T>>>(); 
        public Dictionary<Vector3Int, List<ITileParamsModelBase<T>>> Entity => _entity;

        public void Add(Vector3Int key, List<ITileParamsModelBase<T>> model)
        {
            if(!_entity.ContainsKey(key))
                _entity.Add(key,null);
            _entity[key] = model;
        }
        public List<ITileParamsModelBase<T>> Get(Vector3Int key)
        {
            if (!_entity.ContainsKey(key))
                return null;
            return _entity[key];
        }

        public void Remove(Vector3Int key)
        {
            if (!_entity.ContainsKey(key)) return;
            _entity.Remove(key);
        }
        
    }
}
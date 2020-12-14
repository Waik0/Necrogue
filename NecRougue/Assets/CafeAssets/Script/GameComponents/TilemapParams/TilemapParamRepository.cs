using System.Collections.Generic;
using UnityEngine;

namespace CafeAssets.Script.GameComponents.TilemapParams
{
    interface ITilemapParameterRepository 
    {
        Dictionary<Vector3Int,List<ITileParamsModelBase>> Entity { get; }
        void Add(Vector3Int key, List<ITileParamsModelBase> model);
        
        List<ITileParamsModelBase> Get(Vector3Int key);
        void Remove(Vector3Int key);
    }
    /// <summary>
    /// タイルマップに関するデータを保持
    /// </summary>
    sealed class TilemapParameterRepositoryInternal : ITilemapParameterRepository
    {

        private Dictionary<Vector3Int, List<ITileParamsModelBase>> _entity = new Dictionary<Vector3Int,List<ITileParamsModelBase>>(); 
        public Dictionary<Vector3Int, List<ITileParamsModelBase>> Entity => _entity;

        public void Add(Vector3Int key, List<ITileParamsModelBase> model)
        {
            if(!_entity.ContainsKey(key))
                _entity.Add(key,null);
            _entity[key] = model;
        }
        public List<ITileParamsModelBase> Get(Vector3Int key)
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
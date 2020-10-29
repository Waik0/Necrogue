using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;
using Zenject;

namespace CafeAssets.Script.System.GameMapSystem
{
    public interface ITilemapReceiver
    {
        void OnUpdateTile(TilemapModel model);

    }
    public interface ITilemapManager
    {
        void OnUpdateTile(TilemapModel model);

    }
    /// <summary>
    /// タイルマップが更新されたことを通知する
    /// タイルマップのインスタンスの参照を渡す
    /// </summary>
    public class TilemapManager : ITilemapManager,IManager<ITilemapReceiver>
    {
        private List<ITilemapReceiver> _receivers = new List<ITilemapReceiver>();
        public TilemapManager(
            [InjectOptional]
            List<ITilemapReceiver> receivers
        )
        {
            if(receivers != null)
                _receivers = receivers;
        }
        public void Add(ITilemapReceiver element)
        {
            _receivers.Add(element);
        }

        public void RemoveNull()
        {
            _receivers.RemoveAll(_ => _ == null);
        }

        public void Dispose()
        {
            _receivers.Clear();
        }
        
        public void OnUpdateTile(TilemapModel model)
        {
            foreach (var mapPlaceReceiver in _receivers)
            {
                mapPlaceReceiver?.OnUpdateTile(model);
            }
        }
        
    }

    public class TilemapModel
    {
        public Tilemap Tilemap;
    }
}
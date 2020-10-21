using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Zenject;

namespace CafeAssets.Script.System.GameMapSystem
{
    public interface ITileSelectReceiver
    {
        void OnSelectTile(TileSelectModel model);
    }
    public interface ITileSelectManager
    {
        void OnSelectTile(TileSelectModel model);
    }
    public class TileSelectManager : ITileSelectManager,IManager<ITileSelectReceiver>
    {
        private List<ITileSelectReceiver> _tileSelectReceivers = new List<ITileSelectReceiver>();
        public TileSelectManager(
            [InjectOptional]
            List<ITileSelectReceiver> mapPlaceReceivers
        )
        {
            if(mapPlaceReceivers != null)
                _tileSelectReceivers = mapPlaceReceivers;
        }
        public void Add(ITileSelectReceiver element)
        {
            _tileSelectReceivers.Add(element);
        }

        public void RemoveNull()
        {
            _tileSelectReceivers.RemoveAll(_ => _ == null);
        }

        public void Dispose()
        {
            _tileSelectReceivers.Clear();
        }
        
        public void OnSelectTile(TileSelectModel model)
        {
            foreach (var tileSelectReceiver in _tileSelectReceivers)
            {
                tileSelectReceiver?.OnSelectTile(model);
            }
        }
        
    }

    public class TileSelectModel
    {
        public TileModel Model;
    }
}
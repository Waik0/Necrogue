using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Zenject;
using NotImplementedException = System.NotImplementedException;

namespace CafeAssets.Script.System.GameMapSystem
{
    public interface IMapPlaceReceiver
    {
        void OnPlaceTile(MapPlaceModel model);
        void OnRemoveTile(MapPlaceModel model);
    }
    public interface IMapPlaceManager
    {
        void OnPlaceTile(MapPlaceModel model);
        void OnRemoveTile(MapPlaceModel model);
    }
    public class MapPlaceManager : IMapPlaceManager,IManager<IMapPlaceReceiver>
    {
        private List<IMapPlaceReceiver> _mapPlaceReceivers = new List<IMapPlaceReceiver>();
        public MapPlaceManager(
            [InjectOptional]
            List<IMapPlaceReceiver> mapPlaceReceivers
        )
        {
            if(mapPlaceReceivers != null)
                _mapPlaceReceivers = mapPlaceReceivers;
        }
        public void Add(IMapPlaceReceiver element)
        {
            _mapPlaceReceivers.Add(element);
        }

        public void RemoveNull()
        {
            _mapPlaceReceivers.RemoveAll(_ => _ == null);
        }

        public void Dispose()
        {
            _mapPlaceReceivers.Clear();
        }
        
        public void OnPlaceTile(MapPlaceModel model)
        {
            foreach (var mapPlaceReceiver in _mapPlaceReceivers)
            {
                mapPlaceReceiver?.OnPlaceTile(model);
            }
        }

        public void OnRemoveTile(MapPlaceModel model)
        {
            foreach (var mapPlaceReceiver in _mapPlaceReceivers)
            {
                mapPlaceReceiver?.OnRemoveTile(model);
            }
        }
    }

    public class MapPlaceModel
    {
        public TileModel Model;
        public Vector3Int Pos;
    }
}
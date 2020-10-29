using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Zenject;
using NotImplementedException = System.NotImplementedException;

namespace CafeAssets.Script.System.GameMapSystem
{
    public interface IMapPlaceReceiver
    {
        void OnPlaceTile(TilePlaceModel model);
        void OnRemoveTile(TilePlaceModel model);
    }
    public interface IMapPlaceManager
    {
        void OnPlaceTile(TilePlaceModel model);
        void OnRemoveTile(TilePlaceModel model);
    }
    /// <summary>
    /// タイルが置かれたことを通知する
    /// </summary>
    public class TilePlaceManager : IMapPlaceManager,IManager<IMapPlaceReceiver>
    {
        private List<IMapPlaceReceiver> _mapPlaceReceivers = new List<IMapPlaceReceiver>();
        public TilePlaceManager(
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
        
        public void OnPlaceTile(TilePlaceModel model)
        {
            foreach (var mapPlaceReceiver in _mapPlaceReceivers)
            {
                mapPlaceReceiver?.OnPlaceTile(model);
            }
        }

        public void OnRemoveTile(TilePlaceModel model)
        {
            foreach (var mapPlaceReceiver in _mapPlaceReceivers)
            {
                mapPlaceReceiver?.OnRemoveTile(model);
            }
        }
    }

    public class TilePlaceModel
    {
        public TileModel Model;
        public PlaceTileMode PlaceMode;
        public Vector3 StartWorldPos;
        public Vector3 EndWorldPos;
        public int Z;
    }
}
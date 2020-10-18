using System;
using CafeAssets.Script.System.GameMapSystem;
using UnityEngine;

namespace CafeAssets.Script.System.GameCoreSystem
{
    public interface IGamePresenter
    {
        
    }
    public class GamePresenter : IGamePresenter,IDisposable
    {
        private GameParameterPresenter _parameter;
        private IGameStaticDataController _staticData;
        private MapView _mapView;

        public GamePresenter(
            MapView mapView,
            IGameStaticDataController staticData)
        {
            _parameter = new GameParameterPresenter();
            _mapView = mapView;
            _staticData = staticData;
        }
        /// <summary>
        /// セーブデータよみこみ
        /// </summary>
        /// <param name="path"></param>
        public void LoadData(string path)
        {
        
        }

        public void ResetParams()
        {
            _parameter.Reset();
        }

        public void ResetMap()
        {
            _mapView.Reset();
        }

        public void SetTileDebug()
        {
            var data = _staticData.GetFloorTileModel("FloorTile_0000");
            Debug.Log(data.Name);
            _mapView.SetTile(data,Vector3Int.zero);
        }

        public void Dispose()
        {
        }
    }
}

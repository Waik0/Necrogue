using System;
using CafeAssets.Script.System.GameMapSystem;
using CafeAssets.Script.System.GameParameterSystem;
using CafeAssets.Script.System.GameTimeSystem;
using UnityEngine;
using Zenject;

namespace CafeAssets.Script.System.GameCoreSystem
{
    public interface IGameUseCase
    {
        //セーブデータ
        void LoadData(string path);
        void ResetParams();
        void ResetMap();
        void Tick();

    }
    public class GameUseCase : IGameUseCase,IDisposable
    {
        private IGameParameterUseCase _parameter;
        private IGameStaticDataController _staticData;
        private IGameTimeUseCase _gameTimeUseCase;
        private MapView _mapView;

        public GameUseCase(
            MapView mapView,
            IGameParameterUseCase parameterUseCase,
            IGameStaticDataController staticData,
            IGameTimeUseCase gameTimeUseCase)
        {
            _parameter = parameterUseCase;
            _mapView = mapView;
            _staticData = staticData;
            _gameTimeUseCase = gameTimeUseCase;
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

        public void Tick()
        {
            _gameTimeUseCase.Tick();
        }
    }
}

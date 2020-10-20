using System.Collections;
using System.Collections.Generic;
using CafeAssets.Script.System.GameCoreSystem;
using UnityEngine;
using NotImplementedException = System.NotImplementedException;

namespace CafeAssets.Script.System.GameMapSystem
{
    public interface IMapUseCase
    {
        
    }
    public class MapUseCase : IGameScreenInputReceiver,IMapUseCase
    {
        private TileModel _currentSelectedTile;
        private IMapPlaceManager _mapPlaceManager;
        private IGameStaticDataController _gameStaticDataController;
        public MapUseCase(
            IMapPlaceManager mapPlaceManager,
            IGameStaticDataController dataController)
        {
            _mapPlaceManager = mapPlaceManager;
            _gameStaticDataController = dataController;
            _currentSelectedTile = _gameStaticDataController.GetFloorTileModel("FloorTile_0000");
        }
        void PlaceTile(Vector3Int pos)
        {
            _mapPlaceManager?.OnPlaceTile(new MapPlaceModel()
            {
                Pos = pos,
                Model = _currentSelectedTile
            });
        }

        public void GameInput(GameInputModel model)
        {
            PlaceTile(Vector3Int.zero);
        }
    }
}
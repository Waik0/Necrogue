using System.Collections;
using System.Collections.Generic;
using CafeAssets.Script.System.GameCoreSystem;
using UnityEngine;
using NotImplementedException = System.NotImplementedException;

namespace CafeAssets.Script.System.GameMapSystem
{
    public interface IMapPlaceUseCase
    {
        TileModel SelectedTile { get; set; }
    }
    public class MapPlaceUseCase : IGameScreenInputReceiver,IMapPlaceUseCase,ITileSelectReceiver
    {
        public TileModel SelectedTile { get; set; }
        private IMapPlaceManager _mapPlaceManager;
        private IGameStaticDataController _gameStaticDataController;
        public MapPlaceUseCase(
            IMapPlaceManager mapPlaceManager)
        {
            _mapPlaceManager = mapPlaceManager;
      
        }
        void PlaceTile(Vector3 pos)
        {
            if (SelectedTile == null) return;
            Debug.Log(pos);
            _mapPlaceManager?.OnPlaceTile(new MapPlaceModel()
            {
                WorldPos = pos,
                Model = SelectedTile
            });
        }

        public void GameInput(GameInputModel model)
        {
            if(model.IsPlaceTileMode) PlaceTile(new Vector3(model.WorldCurrentPos.x,model.WorldCurrentPos.y,0));
        }

        public void OnSelectTile(TileSelectModel model)
        {
            SelectedTile = model.Model;
        }
    }
}
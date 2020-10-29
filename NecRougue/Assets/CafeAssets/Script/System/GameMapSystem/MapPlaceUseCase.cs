using System;
using System.Collections;
using System.Collections.Generic;
using CafeAssets.Script.System.GameCoreSystem;
using CafeAssets.Script.System.GameTimeSystem;
using UnityEngine;
using NotImplementedException = System.NotImplementedException;

namespace CafeAssets.Script.System.GameMapSystem
{
    public interface IMapPlaceUseCase
    {
        TileModel SelectedTile { get; set; }
        PlaceTileMode PlaceTileMode { get; set; }
    }
    /// <summary>
    /// 入力情報をタイル設置情報へと変換するクラス
    /// </summary>
    public class MapPlaceUseCase : IGameScreenInputReceiver,IMapPlaceUseCase,ITileSelectReceiver
    {
       
        private IMapPlaceManager _mapPlaceManager;
        private IGameStaticDataController _gameStaticDataController;
        public TileModel SelectedTile { get; set; }
        public PlaceTileMode PlaceTileMode { get; set; } //基本的にタイル固有の設定で一応変更可能に
        private Vector3 _rectStartPos;
        public MapPlaceUseCase(
            IMapPlaceManager mapPlaceManager)
        {
            _mapPlaceManager = mapPlaceManager;
        }
        /// <summary>
        /// タイル設置
        /// </summary>
        void PlaceTile(TilePlaceModel model)
        {
            
            _mapPlaceManager?.OnPlaceTile(model);
        }

        void PlaceTileSingle(GameInputModel model)
        {
            if (model.State == GameInputState.PointerUp)
            {
                var placeModel = model.ToPlaceModel(PlaceTileMode.PlaceTileSingle);
                placeModel.Model = SelectedTile;
                PlaceTile(placeModel);
                SelectedTile = null;
            }
        }

        void PlaceTileDraw(GameInputModel model)
        {
            var placeModel = model.ToPlaceModel(PlaceTileMode.PlaceTileDraw);
            placeModel.Model = SelectedTile;
            PlaceTile(placeModel);
            if (model.State == GameInputState.PointerUp) SelectedTile = null;
        }
        void PlaceTileRect(GameInputModel model)
        {
            if (model.State == GameInputState.PointerUp)
            {
                var placeModel = model.ToPlaceModel(PlaceTileMode.PlaceTileRect);
                placeModel.Model = SelectedTile;
                PlaceTile(placeModel);
                SelectedTile = null;
            }
        }
        /// <summary>
        /// フィールドに対する入力
        /// </summary>
        /// <param name="model"></param>
        public void GameInput(GameInputModel model)
        {
            if (model.InputMode == InputMode.PlaceTile)
            {
                if (SelectedTile == null) return;
                PlaceTileMode = SelectedTile.PlaceMode;
                switch (PlaceTileMode)
                {
                    case PlaceTileMode.PlaceTileSingle:
                        PlaceTileSingle(model);
                        break;
                    case PlaceTileMode.PlaceTileDraw:
                        PlaceTileDraw(model);
                        break;
                    case PlaceTileMode.PlaceTileRect:
                        PlaceTileRect(model);
                        break;
                    default:
                        throw new ArgumentOutOfRangeException();
                }
                
            }
            
        }

        /// <summary>
        /// タイルが選択されたとき
        /// </summary>
        /// <param name="model"></param>
        public void OnSelectTile(TileSelectModel model)
        {
            SelectedTile = model.Model;
        }
    }

    public static class GameInputModelExtensions
    {
        public static TilePlaceModel ToPlaceModel(this GameInputModel model,PlaceTileMode mode)
        {
            switch (mode)
            {
                case PlaceTileMode.PlaceTileSingle:
                    return model.PlaceTileSingle();
                    break;
                case PlaceTileMode.PlaceTileDraw:
                    return PlaceTileDraw(model);
                    break;
                case PlaceTileMode.PlaceTileRect:
                    return PlaceTileRect(model);
                    break;
                default:
                    throw new ArgumentOutOfRangeException();
            }
        }
        private static TilePlaceModel ToPlaceModelInternal(this GameInputModel model,Vector3 spos,Vector3 epos,PlaceTileMode mode)
        {
            return new TilePlaceModel()
            {
                StartWorldPos = spos,
                EndWorldPos = epos,
                PlaceMode = mode,
                Model = null
            };
        }
        public static TilePlaceModel PlaceTileSingle(this GameInputModel model)
        {
            var p = new Vector3(model.WorldCurrentPos.x, model.WorldCurrentPos.y, 0);
            return model.ToPlaceModelInternal(p,p,PlaceTileMode.PlaceTileSingle);
        }
        public static TilePlaceModel PlaceTileDraw(this GameInputModel model)
        {
            var p = new Vector3(model.WorldCurrentPos.x, model.WorldCurrentPos.y, 0);
            return model.ToPlaceModelInternal(p, p, PlaceTileMode.PlaceTileDraw);
        }
        public static TilePlaceModel PlaceTileRect(this GameInputModel model)
        {
            return model.ToPlaceModelInternal(
                new Vector3(model.WorldCurrentPos.x,model.WorldCurrentPos.y,0),
                new Vector3(model.WorldDownPos.x,model.WorldDownPos.y,0),
                PlaceTileMode.PlaceTileRect);
        }
       
    }
}
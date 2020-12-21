using System;
using System.Collections.Generic;
using System.Linq;
using CafeAssets.Script.GameComponents.GameInput;
using CafeAssets.Script.GameComponents.Tilemap;
using UnityEngine;

namespace CafeAssets.Script.GameComponents.TilemapPlaceController
{
    public class TilePlaceModel
    {
        //public ITileModel Model;
        //public PlaceTileMode PlaceMode;
        public Vector3 StartWorldPos;
        public Vector3 CurrentWorldPos;

        public Vector3 EndWorldPos;
        //public int Z;
    }

    /// <summary>
    /// タイルを設置する
    /// </summary>
    public interface IPlaceTileUseCase
    {
        ITileModel SelectedTile { get; set; }

        PlaceTileMode PlaceTileMode { get; set; }

        //void PlaceTileModel(TilePlaceModel model);
        void StartPlace(Vector3 pos);
        void UpdatePlace(Vector3 pos);
        void EndPlace(Vector3 pos);

        /// <summary>
        /// タイル選択されたとき
        /// </summary>
        /// <param name="model"></param>
        void OnSelectTile(ITileModel model);
        /// <summary>
        /// 
        /// </summary>
        /// <param name="mode"></param>
        void SetPlaceTileMode(PlaceTileMode mode);
    }

    public interface IPlaceTileBehaviour
    {
        PlaceTileMode TargetPlaceMode { get; }
        void StartPlace(Vector3 pos,ITileModel model);
        void UpdatePlace(Vector3 pos,ITileModel model);
        void EndPlace(Vector3 pos,ITileModel model);
    }

    /// <summary>
    /// 入力情報をタイル設置情報へと変換するクラス
    /// </summary>
    public class PlaceTileUseCase : IPlaceTileUseCase
    {
        //private ITilemapUseCase _tilemapUseCase;
        public ITileModel SelectedTile { get; set; }
        public PlaceTileMode PlaceTileMode { get; set; } //基本的にタイル固有の設定で一応変更可能に

        private List<IPlaceTileBehaviour> _placeTileBehaviours;

        public PlaceTileUseCase(
            //ITilemapUseCase tilemapUseCase,
            List<IPlaceTileBehaviour> tileBehaviours)
        {
            //_tilemapUseCase = tilemapUseCase;
            _placeTileBehaviours = tileBehaviours;
        }


        public void StartPlace(Vector3 pos)
        {
            DebugLog.LogClassName(this, $"配置開始 {PlaceTileMode}");
            DebugLog.LogClassName(this,$"該当モード {_placeTileBehaviours.Count(_=>_.TargetPlaceMode == PlaceTileMode)} / {_placeTileBehaviours.Count}");
            foreach (var placeTileBehaviour in _placeTileBehaviours)
            {
                if (placeTileBehaviour.TargetPlaceMode == PlaceTileMode)
                {
                    placeTileBehaviour.StartPlace(pos,SelectedTile);
                }
            }
        }

        public void UpdatePlace(Vector3 pos)
        {
            foreach (var placeTileBehaviour in _placeTileBehaviours)
            {
                if (placeTileBehaviour.TargetPlaceMode == PlaceTileMode)
                {
                    placeTileBehaviour.UpdatePlace(pos,SelectedTile);
                }
            }
        }

        public void EndPlace(Vector3 pos)
        {
            DebugLog.LogClassName(this, $"配置終了 {PlaceTileMode}");
            foreach (var placeTileBehaviour in _placeTileBehaviours)
            {
                if (placeTileBehaviour.TargetPlaceMode == PlaceTileMode)
                {
                    placeTileBehaviour.EndPlace(pos,SelectedTile);
                }
            }
        }

        /// <summary>
        /// タイルが選択されたとき
        /// </summary>
        /// <param name="model"></param>
        public void OnSelectTile(ITileModel model)
        {
            DebugLog.LogClassName(this, $"タイル選択 {model.GetName()}");
            SelectedTile = model;
        }

        public void SetPlaceTileMode(PlaceTileMode mode)
        {
            PlaceTileMode = mode;
        }
    }
    

    // void PlaceTile(TilePlaceModel model)
    // {
    //     //_placeTileManager?.OnPlaceTile(model);
    //     _tilemapUseCase.SetTile(model, model.Model);
    // }
    // void PlaceTileSingle(GameInputModel model)
    // {
    //     if (model.State == GameInputState.PointerUp)
    //     {
    //         var placeModel = model.ToPlaceModel(PlaceTileMode.PlaceTileSingle);
    //         placeModel.Model = SelectedTile;
    //         PlaceTile(placeModel);
    //         SelectedTile = null;
    //     }
    // }
    //
    // void PlaceTileDraw(GameInputModel model)
    // {
    //     var placeModel = model.ToPlaceModel(PlaceTileMode.PlaceTileDraw);
    //     placeModel.Model = SelectedTile;
    //     PlaceTile(placeModel);
    //     if (model.State == GameInputState.PointerUp) SelectedTile = null;
    // }
    // void PlaceTileRect(GameInputModel model)
    // {
    //     if (model.State == GameInputState.PointerUp)
    //     {
    //         var placeModel = model.ToPlaceModel(PlaceTileMode.PlaceTileRect);
    //         placeModel.Model = SelectedTile;
    //         PlaceTile(placeModel);
    //         SelectedTile = null;
    //     }
    // }
    /// <summary>
    /// フィールドに対する入力
    /// </summary>
    /// <param name="model"></param>
    // public void GameInput(GameInputModel model)
    // {
    //     if (model.InputMode == InputMode.PlaceTile)
    //     {
    //         if (SelectedTile == null) return;
    //         PlaceTileMode = SelectedTile.PlaceMode;
    //         switch (PlaceTileMode)
    //         {
    //             case PlaceTileMode.PlaceTileSingle:
    //                 PlaceTileSingle(model);
    //                 break;
    //             case PlaceTileMode.PlaceTileDraw:
    //                 PlaceTileDraw(model);
    //                 break;
    //             case PlaceTileMode.PlaceTileRect:
    //                 PlaceTileRect(model);
    //                 break;
    //             default:
    //                 throw new ArgumentOutOfRangeException();
    //         }
    //         
    //     }
    //     
    // }

    // public static class GameInputModelExtensions
    // {
    //     public static TilePlaceModel ToPlaceModel(this GameInputModel model,PlaceTileMode mode)
    //     {
    //         switch (mode)
    //         {
    //             case PlaceTileMode.PlaceTileSingle:
    //                 return model.PlaceTileSingle();
    //                 break;
    //             case PlaceTileMode.PlaceTileDraw:
    //                 return PlaceTileDraw(model);
    //                 break;
    //             case PlaceTileMode.PlaceTileRect:
    //                 return PlaceTileRect(model);
    //                 break;
    //             default:
    //                 throw new ArgumentOutOfRangeException();
    //         }
    //     }
    //     private static TilePlaceModel ToPlaceModelInternal(this GameInputModel model,Vector3 spos,Vector3 epos,PlaceTileMode mode)
    //     {
    //         return new TilePlaceModel()
    //         {
    //             StartWorldPos = spos,
    //             EndWorldPos = epos,
    //         };
    //     }
    //     public static TilePlaceModel PlaceTileSingle(this GameInputModel model)
    //     {
    //         var p = new Vector3(model.WorldCurrentPos.x, model.WorldCurrentPos.y, 0);
    //         return model.ToPlaceModelInternal(p,p,PlaceTileMode.PlaceTileSingle);
    //     }
    //     public static TilePlaceModel PlaceTileDraw(this GameInputModel model)
    //     {
    //         var p = new Vector3(model.WorldCurrentPos.x, model.WorldCurrentPos.y, 0);
    //         return model.ToPlaceModelInternal(p, p, PlaceTileMode.PlaceTileDraw);
    //     }
    //     public static TilePlaceModel PlaceTileRect(this GameInputModel model)
    //     {
    //         return model.ToPlaceModelInternal(
    //             new Vector3(model.WorldCurrentPos.x,model.WorldCurrentPos.y,0),
    //             new Vector3(model.WorldDownPos.x,model.WorldDownPos.y,0),
    //             PlaceTileMode.PlaceTileRect);
    //     }
    //
    //    
    //}
}
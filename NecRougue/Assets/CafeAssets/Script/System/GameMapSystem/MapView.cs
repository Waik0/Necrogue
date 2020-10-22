﻿using System;
using System.Collections;
using System.Collections.Generic;
using CafeAssets.Script.System.GameCoreSystem;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.Tilemaps;
using UnityEngine.UI;
using NotImplementedException = System.NotImplementedException;

namespace CafeAssets.Script.System.GameMapSystem
{
    public interface IMapView
    {
        void SetTile(MapPlaceModel model);
        void RemoveTile(Vector3Int pos);
    }
    /// <summary>
    /// タイルマップを管理
    /// ViewだがTilemapのインスタンスはTileMapManagerを経由して他のUsecaseたちに共有する予定
    /// </summary>
    public class MapView : MonoBehaviour, IMapView, IGameResettable,IMapPlaceReceiver
    {
        [SerializeField] private Tilemap _tilemap;
        [SerializeField] private TilemapRenderer _tilemapRenderer;

        public void SetTile(MapPlaceModel model)
        {
            _tilemap.SetTileModel(model);
        }

        public void RemoveTile(Vector3Int pos)
        {
            _tilemap.SetTile(pos, null);
        }
        
        public void ResetOnGame()
        {
            Debug.Log("[MapView]Reset");
            _tilemap.ClearAllTiles();
        }

        public void OnPlaceTile(MapPlaceModel model)
        {
            SetTile(model);
        }

        public void OnRemoveTile(MapPlaceModel model)
        {
            throw new NotImplementedException();
        }
    }

    public static class TilemapExtensions
    {
        public static void SetTileModel(this Tilemap self, MapPlaceModel model)
        {
            switch (model.PlaceMode)
            {
                //1個置き
                case PlaceTileMode.PlaceTileSingle:
                case PlaceTileMode.PlaceTileDraw:
                    //ブラシサイズ対応
                    if (model.Model.Brush.sqrMagnitude > 1)
                    {
                        self.SetBrushTiles(model);
                    }
                    else
                    {
                        var pos = self.WorldToCell(model.StartWorldPos);
                        pos.z = model.Z;
                        self.SetTile(pos, model.Model);
                    }
                    break;
                case PlaceTileMode.PlaceTileRect:
                    self.SetRectTiles(model);
                    break;
                default:
                    throw new ArgumentOutOfRangeException();
            }

        }

        private static void SetBrushTiles(this Tilemap self,MapPlaceModel model)
        {
            Debug.Log("ブラシモード");
            Debug.Log(model.Model);
            var positions = new Vector3Int[model.Model.Brush.x * model.Model.Brush.y];
            var tiles = new TileBase[model.Model.Brush.x * model.Model.Brush.y];
            var origin = self.WorldToCell(model.StartWorldPos);
            int xhalf = (model.Model.Brush.x + 1 - (model.Model.Brush.x % 2)) / 2;
            int yhalf = (model.Model.Brush.y + 1 - (model.Model.Brush.y % 2)) / 2;
            for (var i = 0; i < model.Model.Brush.x; i++)
            {
                for (var j = 0; j < model.Model.Brush.y; j++)
                {
                    
                    positions[i * model.Model.Brush.y + j] = new Vector3Int(origin.x - xhalf + i,origin.y - yhalf + j,model.Z);
                    tiles[i * model.Model.Brush.y + j] = model.Model;
                }
            }
            self.SetTiles(positions,tiles);
        }

        private static void SetRectTiles(this Tilemap self,MapPlaceModel model)
        {
            Debug.Log("矩形モード");
            Debug.Log(model.Model);
     
            var start = self.WorldToCell(model.StartWorldPos);
            var end = self.WorldToCell(model.EndWorldPos);
            var sx = Mathf.Min(start.x, end.x);
            var ex = Mathf.Max(start.x, end.x);
            var sy = Mathf.Min(start.y, end.y);
            var ey = Mathf.Max(start.y, end.y);
            var lx = ex - sx;
            var ly = ey - sy;
            var positions = new Vector3Int[ lx * ly ];
            var tiles = new TileBase[ lx * ly ];
            for (var i = 0; i < lx; i++)
            {
                for (var j = 0; j < ly; j++)
                {
                    
                    positions[i * ly + j] = new Vector3Int(sx + i,sy + j,model.Z);
                    tiles[i * ly + j] = model.Model;
                }
            }
            self.SetTiles(positions,tiles);
        }
    }
}
﻿using System.Collections.Generic;
using System.Linq;
using CafeAssets.Script.Interface.Layer_01.Manager;
using CafeAssets.Script.Interface.Layer_02.UseCase;
using CafeAssets.Script.Model;
using UnityEngine;
using UnityEngine.Tilemaps;
using NotImplementedException = System.NotImplementedException;

namespace CafeAssets.Script.Impliment.Game.Layer_02.UseCase
{
    /// <summary>
    /// タイルマップ系の操作をするクラス
    /// 個別のタイルに保存する情報の管理は別クラス。
    /// 不変のタイル情報のみ管理
    /// </summary>
    public class TilemapUseCase : ITilemapUseCase
    {
        // private TilemapCollider2D _tilemapCollider;
        private Vector2Int _fieldSize = new Vector2Int(100,100);

        public Vector2Int MaxSize => _fieldSize;
        public Tilemap Tilemap { private get; set; }

        public T GetTileModel<T>(Vector3Int pos) where T : TileModel
        {
            if (Tilemap == null) return null;
            var tile = Tilemap.GetTile<T>(pos);
            return tile;
        }

        public Vector3Int WorldToCell(Vector3 world)
        {
            return Tilemap.WorldToCell(world);
        }

        public Vector3 CellToWorld(Vector3Int cell)
        {
            return Tilemap.CellToWorld(cell);
        }

        public BoundsInt CellBounds => Tilemap.cellBounds;
        /// <summary>
        /// 特定の種類のタイルを集計
        /// todo 最適化 Linqと毎回集計をやめる。
        /// </summary>
        /// <param name="type"></param>
        /// <returns></returns>
        /// <exception cref="NotImplementedException"></exception>
        public TileModel[] AggregateTileModels(TileType type)
        {
            return Tilemap.GetTilesBlock(Tilemap.cellBounds).Select(tileBase => tileBase as TileModel).Where(at => at != null && at.Type == type).ToArray();
        }
        /// <summary>
        /// 特定の効果のタイルを集計
        /// todo 最適化 Linqと毎回集計をやめる。
        /// </summary>
        /// <param name="type"></param>
        /// <returns></returns>
        /// <exception cref="NotImplementedException"></exception>
        public EffectiveTileModel[] AggregateEffectiveTileModels(TileEffectType type)
        {
            return Tilemap.GetTilesBlock(Tilemap.cellBounds).Select(tileBase => tileBase as EffectiveTileModel).Where(at => at != null && at.EffectType == type).ToArray();
        }

        /// <summary>
        /// 通行可否
        /// </summary>
        /// <param name="cell"></param>
        /// <returns></returns>
        public bool GetPassable(Vector3Int cell)
        {
            var zPos = Tilemap.cellBounds.position.z;
            var zSize = Tilemap.cellBounds.size.z;
            var count = 0;
            var passable = true;
            foreach (var tileBase in Tilemap.GetTilesBlock(new BoundsInt(cell.x, cell.y, zPos, 1, 1, zSize)))
            {
                var at = tileBase as TileModel;
                if (at == null)
                {
                    if (count == 0)
                    {
                        //床がnullなら通行不可能
                        return false;
                    }
                }
                else
                {
                    passable &= at.IsWall == false;
                }
                count++;
            }

            return passable;
            // return Tilemap.GetTilesBlock(new BoundsInt(cell.x, cell.y, zPos, 1, 1, zSize)).All(_ =>
            // {
            //    
            //     var at = _ as TileModel;
            //     return at != null && at.IsWall == false;
            // });
        }
    }
}
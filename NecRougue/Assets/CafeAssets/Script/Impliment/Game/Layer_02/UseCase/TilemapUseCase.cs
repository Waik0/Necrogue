using System.Linq;
using CafeAssets.Script.Interface.Layer_01.Manager;
using CafeAssets.Script.Interface.Layer_02.UseCase;
using CafeAssets.Script.Model;
using UnityEngine;
using UnityEngine.Tilemaps;
using NotImplementedException = System.NotImplementedException;

namespace CafeAssets.Script.Impliment.Game.Layer_02.UseCase
{
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
        /// 通行可否
        /// </summary>
        /// <param name="cell"></param>
        /// <returns></returns>
        public bool GetPassable(Vector3Int cell)
        {
            var zPos = Tilemap.cellBounds.position.z;
            var zSize = Tilemap.cellBounds.size.z;
            return Tilemap.GetTilesBlock(new BoundsInt(cell.x, cell.y, zPos, 1, 1, zSize)).All(_ =>
            {
                var at = _ as TileModel;
                return at != null && at.IsWall == false;
            });
        }
    }
}
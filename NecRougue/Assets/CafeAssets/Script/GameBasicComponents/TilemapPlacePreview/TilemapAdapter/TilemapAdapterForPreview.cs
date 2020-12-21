using System.Collections;
using System.Collections.Generic;
using System.Linq;
using CafeAssets.Script.GameComponents.Tilemap;
using CafeAssets.Script.GameComponents.TilemapPlaceController;
using UnityEngine;
using UnityEngine.Tilemaps;
using NotImplementedException = System.NotImplementedException;

namespace CafeAssets.Script.GameComponents.TilemapPlacePreview.TilemapAdapter
{
    /// <summary>
    /// マップの配置プレビュー用タイルマップ操作クラス
    /// 上位: TilemapPlacePreviewUseCase
    /// 方式: 継承による依存
    /// </summary>
    public class TilemapAdapterForPreview : MonoBehaviour, ITilemapAdapterForPreview
    {
        [SerializeField] private UnityEngine.Tilemaps.Tilemap _tilemap;
        [SerializeField] private TilemapRenderer _tilemapRenderer;

        private void Awake()
        {
            ResetOnGame();
        }

        public void SetTile(Vector3Int pos, ITileModel model)
        {
            var tileBase = model as TileBase;
            if (tileBase != null)
                _tilemap.SetTile(pos, tileBase);
        }

        public void SetTiles(Vector3Int[] pos, ITileModel model)
        {
            var tileBase = model as TileBase;
            if (tileBase != null)
            {
                TileBase[] tiles = new TileBase[pos.Length];
                for (var i = 0; i < tiles.Length; i++)
                {
                    tiles[i] = tileBase;
                }

                _tilemap.SetTiles(pos, tiles);
            }
        }

        public void ClearTiles()
        {
            _tilemap.ClearAllTiles();
        }

        public Vector3Int WorldToCell(Vector3 world)
        {
            return _tilemap.WorldToCell(world);
        }

        public Vector3 CellToWorld(Vector3Int cell)
        {
            return _tilemap.CellToWorld(cell);
        }

        public void ResetOnGame()
        {
            Debug.Log("[MapView]Reset");
            _tilemap.ClearAllTiles();
        }
    }
}
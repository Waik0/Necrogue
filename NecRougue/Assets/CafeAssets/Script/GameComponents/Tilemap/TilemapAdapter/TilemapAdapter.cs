using System.Linq;
using CafeAssets.Script.GameComponents.Tilemap;
using CafeAssets.Script.GameComponents.TilemapPlaceController;
using UnityEngine;
using UnityEngine.Tilemaps;
using Zenject;
using NotImplementedException = System.NotImplementedException;

namespace CafeAssets.Script.GameComponents.TilemapAdapter
{


    /// <summary>
    /// タイルマップを管理
    /// </summary>
    public class TilemapAdapter : MonoBehaviour, ITilemapAdapter, IInitializable
    {
        [SerializeField] private UnityEngine.Tilemaps.Tilemap _tilemap;
        [SerializeField] private TilemapRenderer _tilemapRenderer;
        //[SerializeField] private TilemapCollider2D _tilemapCollider;

        private void Awake()
        {
            ResetOnGame();
        }
        public void Initialize()
        {
            
        }

       

        public void SetTile(Vector3Int pos, ITileModel model)
        {
            var tileBase = model as TileBase;
            if(tileBase != null) 
                _tilemap.SetTile(pos,tileBase);
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

        public ITileModel GetTile(Vector3Int pos)
        {
            return _tilemap.GetTile(pos) as ITileModel;
        }

        public void RemoveTile(Vector3Int pos)
        {
            _tilemap.SetTile(pos, null);
        }

        public Vector3Int WorldToCell(Vector3 world)
        {
            return _tilemap.WorldToCell(world);
        }

        public Vector3 CellToWorld(Vector3Int cell)
        {
            return _tilemap.CellToWorld(cell);
        }

        public BoundsInt CellBounds { get; }
        public ITileModel[] GetTileModelsBlock(BoundsInt bounds)
        {
            return _tilemap.GetTilesBlock(bounds).OfType<ITileModel>().ToArray();
        }
        

        public void ResetOnGame()
        {
            Debug.Log("[MapView]Reset");
            _tilemap.ClearAllTiles();
        }

        public void OnPlaceTile(TilePlaceModel model)
        {
            throw new NotImplementedException();
        }

        public void OnRemoveTile(TilePlaceModel model)
        {
            throw new NotImplementedException();
        }
    }

    // public static class TilemapExtensions
    // {
    //     public static void SetTileModel(this Tilemap self, TilePlaceModel model)
    //     {
    //         switch (model.PlaceMode)
    //         {
    //             //1個置き
    //             case PlaceTileMode.PlaceTileSingle:
    //             case PlaceTileMode.PlaceTileDraw:
    //                 //ブラシサイズ対応
    //                 if (model.Model.Brush.sqrMagnitude > 1)
    //                 {
    //                     self.SetBrushTiles(model,model.Model);
    //                 }
    //                 else
    //                 {
    //                     var pos = self.WorldToCell(model.StartWorldPos);
    //                     pos.z = model.Model.MinLayer;
    //                     self.SetTile(pos, model.Model);
    //                 }
    //                 break;
    //             case PlaceTileMode.PlaceTileRect:
    //                 self.SetRectTiles(model,model.Model);
    //                 break;
    //             default:
    //                 throw new ArgumentOutOfRangeException();
    //         }
    //
    //     }
    //
    //     private static Vector3Int[] SetBrushTiles(this Tilemap self,TilePlaceModel model,TileBase tile)
    //     {
    //         Debug.Log("ブラシモード");
    //         Debug.Log(model.Model);
    //         var positions = new Vector3Int[model.Model.Brush.x * model.Model.Brush.y];
    //         var tiles = new TileBase[model.Model.Brush.x * model.Model.Brush.y];
    //         var origin = self.WorldToCell(model.StartWorldPos);
    //         int xhalf = (model.Model.Brush.x + 1 - (model.Model.Brush.x % 2)) / 2;
    //         int yhalf = (model.Model.Brush.y + 1 - (model.Model.Brush.y % 2)) / 2;
    //         for (var i = 0; i < model.Model.Brush.x; i++)
    //         {
    //             for (var j = 0; j < model.Model.Brush.y; j++)
    //             {
    //                 
    //                 positions[i * model.Model.Brush.y + j] = new Vector3Int(origin.x - xhalf + i,origin.y - yhalf + j,model.Model.MinLayer);
    //                 tiles[i * model.Model.Brush.y + j] = tile;
    //             }
    //         }
    //         self.SetTiles(positions,tiles);
    //         return positions;
    //     }
    //
    //     private static Vector3Int[] SetRectTiles(this Tilemap self,TilePlaceModel model,TileBase tile)
    //     {
    //         Debug.Log("矩形モード");
    //         Debug.Log(model.Model);
    //  
    //         var start = self.WorldToCell(model.StartWorldPos);
    //         var end = self.WorldToCell(model.EndWorldPos);
    //         var sx = Mathf.Min(start.x, end.x);
    //         var ex = Mathf.Max(start.x, end.x);
    //         var sy = Mathf.Min(start.y, end.y);
    //         var ey = Mathf.Max(start.y, end.y);
    //         var lx = ex - sx;
    //         var ly = ey - sy;
    //         var positions = new Vector3Int[ lx * ly ];
    //         var tiles = new TileBase[ lx * ly ];
    //         for (var i = 0; i < lx; i++)
    //         {
    //             for (var j = 0; j < ly; j++)
    //             {
    //                 
    //                 positions[i * ly + j] = new Vector3Int(sx + i,sy + j,model.Model.MinLayer);
    //                 tiles[i * ly + j] = tile;
    //             }
    //         }
    //         self.SetTiles(positions,tiles);
    //         return positions;
    //     }
    // }
}
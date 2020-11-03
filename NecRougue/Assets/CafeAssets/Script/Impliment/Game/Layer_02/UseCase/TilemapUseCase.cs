using CafeAssets.Script.Interface.Layer_01.Manager;
using CafeAssets.Script.Interface.Layer_02.UseCase;
using CafeAssets.Script.Model;
using UnityEngine;
using UnityEngine.Tilemaps;
namespace CafeAssets.Script.Impliment.Game.Layer_02.UseCase
{
    public class TilemapUseCase : ITilemapUseCase,ITilemapReceiver
    {
        private Tilemap _tilemap;
       // private TilemapCollider2D _tilemapCollider;
        private Vector2Int _fieldSize = new Vector2Int(100,100);
        public void OnUpdateTile(TilemapModel model)
        {
            if (_tilemap == null)
            {
                //_tilemapCollider = model.TilemapCollider2D;
                _tilemap = model.Tilemap;
            }
        }

        public Vector2Int MaxSize => _fieldSize;
        public Tilemap Tilemap => _tilemap;

        public T GetTileModel<T>(Vector3Int pos) where T : TileModel
        {
            if (_tilemap == null) return null;
            var tile = _tilemap.GetTile<T>(pos);
            return tile;
        }
    }
}
using UnityEngine;

namespace CafeAssets.Script.GameComponents.Tilemap
{
    /// <summary>
    /// タイルマップ制御
    /// 外部コンポーネントに継承させる
    /// </summary>
    public interface ITilemapAdapter
    {
        void SetTile(Vector3Int pos,ITileModel model);
        void SetTiles(Vector3Int[] pos, ITileModel model);
        void ClearTiles();
        ITileModel GetTile(Vector3Int pos);
        void RemoveTile(Vector3Int pos);
        
        Vector3Int WorldToCell(Vector3 world);
        
        Vector3 CellToWorld(Vector3Int cell);
        BoundsInt CellBounds { get; }

        //TileModel[] AggregateTileModels(TileType type);
        //EffectiveTileModel[] AggregateEffectiveTileModels(TileEffectType type);
        ITileModel[] GetTileModelsBlock(BoundsInt bounds);
    }
}
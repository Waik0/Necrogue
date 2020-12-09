using UnityEngine;

//TilemapUseCaseを構成する部品を定義
namespace CafeAssets.Script.GameComponents.Tilemap
{
    /// <summary>
    /// タイルを設置する
    /// </summary>
    public interface ISetTile
    {
        void SetTile(Vector3Int pos, ITileModel model);
        void SetTiles(Vector3Int[] pos, ITileModel model);
    }
    /// <summary>
    /// タイルが設置されている範囲を取得
    /// </summary>
    public interface IGetTileBounds
    {
        BoundsInt CellBounds { get; }
    }
    /// <summary>
    /// 座標を変換
    /// </summary>
    public interface ITilePositionTransform
    {
        Vector3Int WorldToCell(Vector3 world);

        Vector3 CellToWorld(Vector3Int cell);
    }
}
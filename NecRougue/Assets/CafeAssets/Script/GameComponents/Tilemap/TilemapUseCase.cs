using System;
using UniRx;
using UnityEngine;

namespace CafeAssets.Script.GameComponents.Tilemap
{
    /// <summary>
    /// タイルマップ系の操作をするクラス
    /// 個別のタイルに保存する情報の管理は別クラス。
    /// 不変のタイル情報のみ管理
    /// </summary>
    public interface ITilemapUseCase : ISetTile,IGetTileBounds,ITilePositionTransform
    {
        IObservable<Unit> OnUpdateTiles { get; }
        //T GetTileModel<T>(Vector3Int pos) where T : ITileModel;
        
        ITileModel[] AggregateAllTileModels();

        //EffectiveTileModel[] AggregateEffectiveTileModels(TileEffectType type);
        bool GetPassable(Vector3Int cell);
    }
    /// <summary>
    /// タイルマップ系の操作をするクラス
    /// 個別のタイルに保存する情報の管理は別クラス。
    /// 不変のタイル情報のみ管理
    /// </summary>
    public class TilemapUseCase : ITilemapUseCase
    {
        private ITilemapAdapter _tilemapAdapter;

        /// <summary>
        /// Tileが上書きされたとき
        /// </summary>
        private ISubject<Unit> _onUpdateTiles;

        public IObservable<Unit> OnUpdateTiles => _onUpdateTiles ?? (_onUpdateTiles = new Subject<Unit>());

        public TilemapUseCase(
            ITilemapAdapter tilemapAdapter
        )
        {
            _tilemapAdapter = tilemapAdapter;
        }

        //public T GetTileModel<T>(Vector3Int pos) where T : ITileModel
        //{
        //    return (T) _tilemapAdapter.GetTile(pos);
        //}

        public Vector3Int WorldToCell(Vector3 world)
        {
            return _tilemapAdapter.WorldToCell(world);
        }

        public Vector3 CellToWorld(Vector3Int cell)
        {
            return _tilemapAdapter.CellToWorld(cell);
        }

        public BoundsInt CellBounds => _tilemapAdapter.CellBounds;

        // /// <summary>
        // /// タイルを集計
        // /// todo 最適化 Linqと毎回集計をやめる。
        // /// </summary>
        // /// <param name="type"></param>
        // /// <returns></returns>
        // /// <exception cref="NotImplementedException"></exception>
        public ITileModel[] AggregateAllTileModels()
        {
            return _tilemapAdapter.GetTileModelsBlock(_tilemapAdapter.CellBounds);
            //return Tilemap.GetTilesBlock(Tilemap.cellBounds).Select(tileBase => tileBase as ITileModel).Where(at => at != null).ToArray();
        }
        
        public void SetTile(Vector3Int pos, ITileModel tileModel)
        {
            DebugLog.LogClassName(this,$"タイルを設置します。 {pos} {tileModel.GetName()}");
            _tilemapAdapter.SetTile(pos, tileModel);
            _onUpdateTiles?.OnNext(Unit.Default);
        }

        public void SetTiles(Vector3Int[] pos, ITileModel tileModel)
        {
            DebugLog.LogClassName(this,$"タイルを設置します。 {pos.Length}箇所 {tileModel.GetName()}");
            _tilemapAdapter.SetTiles(pos, tileModel);
            _onUpdateTiles?.OnNext(Unit.Default);
        }

        /// 通行可否
        /// </summary>
        /// <param name="cell"></param>
        /// <returns></returns>
        public bool GetPassable(Vector3Int cell)
        {
            var zPos = CellBounds.position.z;
            var zSize = CellBounds.size.z;
            var count = 0;
            var passable = true;
            foreach (var tileModel in _tilemapAdapter.GetTileModelsBlock(new BoundsInt(cell.x, cell.y, zPos, 1, 1,
                zSize)))
            {
                if (tileModel == null)
                {
                    Debug.LogError("CONTAINS NULL");
                    if (count == 0)
                    {
                        //床がnullなら通行不可能
                        return false;
                    }
                }
                else
                {
                    passable &= tileModel.GetIsWall() == false;
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
        // /// <summary>
        // /// 特定の効果のタイルを集計
        // /// todo 最適化 Linqと毎回集計をやめる。
        // /// </summary>
        // /// <param name="type"></param>
        // /// <returns></returns>
        // /// <exception cref="NotImplementedException"></exception>
        // public EffectiveTileModel[] AggregateEffectiveTileModels(TileEffectType type)
        // {
        //     return Tilemap.GetTilesBlock(Tilemap.cellBounds).Select(tileBase => tileBase as EffectiveTileModel).Where(at => at != null && at.EffectType == type).ToArray();
        // }
        //
        // /// <summary>
    }
}
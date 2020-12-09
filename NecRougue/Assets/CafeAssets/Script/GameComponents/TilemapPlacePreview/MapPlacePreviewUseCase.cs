using CafeAssets.Script.GameComponents.Tilemap;
using CafeAssets.Script.GameComponents.TilemapPlacePreview.ScriptableObject;
using UnityEngine;
using Zenject;
using NotImplementedException = System.NotImplementedException;

namespace CafeAssets.Script.GameComponents.TilemapPlacePreview
{
    public interface ITilePreviewModel : ITileModel 
    {
        
    }
    public interface IMapPlacePreviewView
    {
    
    }
    /// <summary>
    /// タイルマップ制御
    /// 外部コンポーネントに継承させる
    /// </summary>
    public interface ITilemapAdapterForPreview
    {
        void SetTile(Vector3Int pos,ITileModel model);
        void SetTiles(Vector3Int[] pos, ITileModel model);
        void ClearTiles();
        //ITileModel GetTile(Vector3Int pos);
        //void RemoveTile(Vector3Int pos);
        
        Vector3Int WorldToCell(Vector3 world);
        
        Vector3 CellToWorld(Vector3Int cell);
        //BoundsInt CellBounds { get; }

        //TileModel[] AggregateTileModels(TileType type);
        //EffectiveTileModel[] AggregateEffectiveTileModels(TileEffectType type);
        //ITileModel[] GetTileModelsBlock(BoundsInt bounds);
    }
    public interface ITilemapPlacePreviewUseCase : ITilePositionTransform
    {

        void SetTile(Vector3Int pos);
        void SetTiles(Vector3Int[] pos);

        void ClearAllTile();
    }
    public class TilemapPlacePreviewUseCase : MonoBehaviour,IMapPlacePreviewView,ITilemapPlacePreviewUseCase
    {
        private ITilemapAdapterForPreview _tilemapAdapter;
        private ITilePreviewModel _allow;
        private ITilePreviewModel _deny;
        [Inject]
        public void Inject(
            ITilemapAdapterForPreview tilemapAdapter,
            TilePreviewModelProvider provider
        )
        {
            _tilemapAdapter = tilemapAdapter;//newされる
            _allow = provider.Allow;
            _deny = provider.Deny;
        }

        private void Awake()
        {
            ClearAllTile();
        }
        public void SetTile(Vector3Int pos)
        {
            _tilemapAdapter.SetTile(pos,_allow);
        }

        public void SetTiles(Vector3Int[] pos)
        {
            _tilemapAdapter.SetTiles(pos,_allow);
        }

        public void ClearAllTile()
        {
            _tilemapAdapter.ClearTiles();
        }

        public Vector3Int WorldToCell(Vector3 world)
        {
            return _tilemapAdapter.WorldToCell(world);
        }

        public Vector3 CellToWorld(Vector3Int cell)
        {
            return _tilemapAdapter.CellToWorld(cell);
        }
    }
}
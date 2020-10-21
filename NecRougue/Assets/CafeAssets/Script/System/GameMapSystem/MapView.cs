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
        void SetTile(TileModel tile, Vector3Int pos);
        void RemoveTile(Vector3Int pos);
    }

    public class MapView : MonoBehaviour, IMapView, IGameResettable,IMapPlaceReceiver
    {
        [SerializeField] private Tilemap _tilemap;
        [SerializeField] private TilemapRenderer _tilemapRenderer;

        public void SetTile(TileModel tile, Vector3Int pos)
        {
            _tilemap.SetTile(pos, tile);
        }

        public void RemoveTile(Vector3Int pos)
        {
            _tilemap.SetTile(pos, null);
        }

        void PlaceTile(Vector2 pos)
        {
            
        }
        

        public void ResetOnGame()
        {
            Debug.Log("[MapView]Reset");
            _tilemap.ClearAllTiles();
        }

        public void OnPlaceTile(MapPlaceModel model)
        {
            SetTile(model.Model, _tilemap.WorldToCell(model.WorldPos));
        }

        public void OnRemoveTile(MapPlaceModel model)
        {
            throw new NotImplementedException();
        }
    }
}
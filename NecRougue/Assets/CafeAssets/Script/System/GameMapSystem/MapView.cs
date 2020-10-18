using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;
using NotImplementedException = System.NotImplementedException;

namespace CafeAssets.Script.System.GameMapSystem
{
    public interface IMapView
    {
        void Reset();
        void SetTile(TileModel tile, Vector3Int pos);
        void RemoveTile(Vector3Int pos);
    }
    public class MapView : MonoBehaviour,IMapView
    {
        [SerializeField] private Tilemap _tilemap;
        [SerializeField] private TilemapRenderer _tilemapRenderer;
        public void Reset()
        {
            Debug.Log("[MapView]Reset");
            _tilemap.ClearAllTiles();
           
        }

        public void SetTile(TileModel tile, Vector3Int pos)
        {
            _tilemap.SetTile(pos,tile);
        }

        public void RemoveTile(Vector3Int pos)
        {
            _tilemap.SetTile(pos,null);
        }
    }
}
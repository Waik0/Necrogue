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

    public class MapView : MonoBehaviour, IMapView, IGameResettable
    {
        [SerializeField] private Tilemap _tilemap;
        [SerializeField] private TilemapRenderer _tilemapRenderer;
        [SerializeField] private Text _debugText;
        private bool _placeTileMode = false;
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

        void MoveCamera(Vector2 move)
        {
            var cam = Camera.main;
            cam.gameObject.transform.Translate(move);
        }
        public void OnPointerDown(BaseEventData e)
        {
            var pe = (PointerEventData) e;
            if (_placeTileMode)
            {
                PlaceTile(pe.position);
            }
           

        }

        public void OnPointerUp()
        {
            
        }

        public void OnDrag(BaseEventData e)
        {
            var pe = (PointerEventData) e;
            if (!_placeTileMode)
            {
                MoveCamera(pe.delta);
            }
        }

        public void ResetOnGame()
        {
            Debug.Log("[MapView]Reset");
            _tilemap.ClearAllTiles();
        }
    }
}
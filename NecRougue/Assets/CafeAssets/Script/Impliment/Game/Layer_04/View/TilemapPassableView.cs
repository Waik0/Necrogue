using CafeAssets.Script.Interface.Layer_01.Manager;
using CafeAssets.Script.Interface.View;
using CafeAssets.Script.Model;
using CafeAssets.Script.System.GameMapSystem;
using UnityEngine;
using UnityEngine.Tilemaps;

namespace CafeAssets.Script.Impliment.Game.Layer_04.View
{
    public class TilemapPassableView : MonoBehaviour,ITilemapPassableView,IPlaceTileReceiver
    { 
        [SerializeField] private Tilemap _passable;
        [SerializeField] private AstarNodeTile _tile;

        public void SetTile(TilePlaceModel model)
        {
            _passable.SetTilePassable(model,_tile);
        }

        public void RemoveTile(Vector3Int pos)
        {
            throw new global::System.NotImplementedException();
        }

        public void OnPlaceTile(TilePlaceModel model)
        {
            SetTile(model);
        }

        public void OnRemoveTile(TilePlaceModel model)
        {
            throw new global::System.NotImplementedException();
        }
    }
}

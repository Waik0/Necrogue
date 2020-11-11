using CafeAssets.Script.Interface.Layer_01.Manager;
using CafeAssets.Script.Interface.Layer_02.UseCase;
using CafeAssets.Script.Interface.View;
using CafeAssets.Script.Model;
using CafeAssets.Script.System.GameMapSystem;
using UnityEngine;
using UnityEngine.Tilemaps;
using Zenject;
using NotImplementedException = System.NotImplementedException;

namespace CafeAssets.Script.Impliment.Game.Layer_04.View
{
    public class TilemapPassableView : MonoBehaviour,ITilemapPassableView,IPlaceTileReceiver
    { 
        [SerializeField] private Tilemap _passable;
        [SerializeField] private AstarNodeTile _tile;

        [Inject]
        private void Inject(
            ITilemapPassabilityUseCase tilemapPassability
            )
        {
            //tilemapPassability.PassableTilemap = _passable;
        }
        private void Awake(){}
        public void SetTile(TilePlaceModel model)
        {
        }

        public void RemoveTile(Vector3Int pos)
        {
            throw new global::System.NotImplementedException();
        }

        public BoundsInt CanReachBounds()
        {
            return _passable.cellBounds;
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

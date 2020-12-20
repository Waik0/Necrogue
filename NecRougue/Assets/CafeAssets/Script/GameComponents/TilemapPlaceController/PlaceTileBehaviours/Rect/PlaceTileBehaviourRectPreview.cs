using CafeAssets.Script.GameComponents.Tilemap;
using CafeAssets.Script.GameComponents.TilemapPlacePreview;
using UnityEngine;

namespace CafeAssets.Script.GameComponents.TilemapPlaceController
{
    public class PlaceTileBehaviourRectPreview : IPlaceTileBehaviour
    {
        private Vector3Int _rectStartPos;

        private ITilemapPlacePreviewUseCase _tilemapUseCase;

        public PlaceTileBehaviourRectPreview(
            ITilemapPlacePreviewUseCase tilemapUseCase)
        {
            _tilemapUseCase = tilemapUseCase;
        }

        public PlaceTileMode TargetPlaceMode => PlaceTileMode.PlaceTileRect;

        public void StartPlace(Vector3 pos,ITileModel model)
        {
            var z = model.ZMin();
            z = z >= 1 ? z - 1 : z;
           // pos.z = z;
            _rectStartPos = _tilemapUseCase.WorldToCell(pos);
            _rectStartPos.z = z;
        }

        public void UpdatePlace(Vector3 pos,ITileModel model)
        {
            _tilemapUseCase.ClearAllTile();
            var start = _rectStartPos;
            var z = model.ZMin();
            z = z >= 1 ? z - 1 : z;
            //pos.z = z;
            var end = _tilemapUseCase.WorldToCell(pos);
            end.z = z;
            var sx = Mathf.Min(start.x, end.x);
            var ex = Mathf.Max(start.x, end.x);
            var sy = Mathf.Min(start.y, end.y);
            var ey = Mathf.Max(start.y, end.y);
            var lx = ex - sx;
            var ly = ey - sy;
            var positions = new Vector3Int[lx * ly];
            for (var i = 0; i < lx; i++)
            {
                for (var j = 0; j < ly; j++)
                {

                    positions[i * ly + j] = new Vector3Int(sx + i, sy + j, end.z);
                }
            }
            _tilemapUseCase.SetTiles(positions);
        }

        public void EndPlace(Vector3 pos,ITileModel model)
        {
            _tilemapUseCase.ClearAllTile();
        }
    }
}
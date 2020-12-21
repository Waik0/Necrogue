
using CafeAssets.Script.GameComponents.Tilemap;
using UnityEngine;

namespace CafeAssets.Script.GameComponents.TilemapPlaceController
{
    public class PlaceTileBehaviourRect : IPlaceTileBehaviour
    {
        private Vector3Int _rectStartPos;
        private ITilemapUseCase _tilemapUseCase;

        public PlaceTileBehaviourRect(
            ITilemapUseCase tilemapUseCase)
        {
            _tilemapUseCase = tilemapUseCase;
        }

        public PlaceTileMode TargetPlaceMode => PlaceTileMode.PlaceTileRect;

        public void StartPlace(Vector3 pos,ITileModel model)
        {
            var z = model.ZMin();
            //pos.z = z;
            _rectStartPos = _tilemapUseCase.WorldToCell(pos);
            _rectStartPos.z = z;
        }

        public void UpdatePlace(Vector3 pos,ITileModel model)
        {

        }

        public void EndPlace(Vector3 pos,ITileModel model)
        {
            //Zをmodelに合わせる
            DebugLog.LogClassName(this,"矩形配置");
            var start = _rectStartPos;
            var z = model.ZMin();
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
            _tilemapUseCase.SetTiles(positions, model);
        }
    }
}
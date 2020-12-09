using CafeAssets.Script.GameComponents.Tilemap;
using CafeAssets.Script.GameComponents.TilemapPlacePreview;
using UnityEngine;

namespace CafeAssets.Script.GameComponents.TilemapPlaceController
{
    public class PlaceTileBehaviourSinglePreview : IPlaceTileBehaviour
    {
        private ITilemapPlacePreviewUseCase _tilemapUseCase;

        public PlaceTileBehaviourSinglePreview(
            ITilemapPlacePreviewUseCase tilemapUseCase)
        {
            _tilemapUseCase = tilemapUseCase;
        }

        public PlaceTileMode TargetPlaceMode => PlaceTileMode.PlaceTileSingle;

        public void StartPlace(Vector3 pos,ITileModel model)
        {
 
        }

        public void UpdatePlace(Vector3 pos,ITileModel model)
        {
            _tilemapUseCase.ClearAllTile();
            //Previewの2段目以降は1段下げる
            var z = model.ZMin();
            z = z >= 1 ? z - 1 : z;
            //pos.z = z;
            var origin = _tilemapUseCase.WorldToCell(pos);
            origin.z = z;
            var brush = model.BrushSize();
            
            if (brush.sqrMagnitude <= 2)
            {
                Debug.Log(brush.sqrMagnitude);
                _tilemapUseCase.SetTile(origin);
            }
            else
            {
                var positions = new Vector3Int[brush.x * brush.y];
                int xhalf = (brush.x + 1 - (brush.x % 2)) / 2;
                int yhalf = (brush.y + 1 - (brush.y % 2)) / 2;

                for (var i = 0; i < brush.x; i++)
                {
                    for (var j = 0; j < brush.y; j++)
                    {

                        positions[i * brush.y + j] = new Vector3Int(origin.x - xhalf + i, origin.y - yhalf + j,
                            origin.z);
                    }
                }
                _tilemapUseCase.SetTiles(positions);
            }
        }

        public void EndPlace(Vector3 pos,ITileModel model)
        {
            _tilemapUseCase.ClearAllTile();
        }
    }
}
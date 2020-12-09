using CafeAssets.Script.GameComponents.Tilemap;
using UnityEngine;
using UnityEngine.Tilemaps;
using NotImplementedException = System.NotImplementedException;

namespace CafeAssets.Script.GameComponents.TilemapPlacePreview.ScriptableObject
{
    [CreateAssetMenu(fileName = "PreviewTile_0000",menuName = "ScriptableObject/PreviewTileModel")]
    public class TilePreviewModel : Tile,ITilePreviewModel
    {


        public string GetName()
        {
            return "";
        }

        public string GetSystemName()
        {
            return name;
        }

        public bool GetIsWall()
        {
            return false;
        }

        public ProvideParameterModelSet[] GetProvideParameter()
        {
            return new ProvideParameterModelSet[]{};
        }

        public Vector2Int BrushSize()
        {
            return Vector2Int.one;
        }

        public int ZMin()
        {
            return 0;
        }

        public int ZMax()
        {
            return 0;
        }

        public PlaceTileMode GetDefaultPlaceMode()
        {
            return PlaceTileMode.PlaceTileSingle;
        }
        public int GetEffectRadius()
        {
            return 0;
        }
    }
}
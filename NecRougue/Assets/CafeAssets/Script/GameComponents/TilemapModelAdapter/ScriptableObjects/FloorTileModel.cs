using CafeAssets.Script.GameComponents.Tilemap;
using CafeAssets.Script.GameComponents.TilemapAdapter.ScriptableObjects;
using UnityEngine;

namespace CafeAssets.Script.GameComponents.TilemapModelAdapter.ScriptableObjects
{
    [CreateAssetMenu(fileName = "FloorTile_0000",menuName = "ScriptableObject/FloorTileModel")]
    public class FloorTileModel : BasicTileModel
    {
        public override TileType Type => TileType.Floor;
    }
}

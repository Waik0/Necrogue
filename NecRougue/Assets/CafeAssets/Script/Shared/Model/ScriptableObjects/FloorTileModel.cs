using CafeAssets.Script.Model;
using UnityEngine;

namespace CafeAssets.Script.System.GameMapSystem.TileInheritance
{
    [CreateAssetMenu(fileName = "FloorTile_0000",menuName = "ScriptableObject/FloorTileModel")]
    public class FloorTileModel : BasicTileModel
    {
        public override TileType Type => TileType.Floor;
    }
}

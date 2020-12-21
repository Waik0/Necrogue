using CafeAssets.Script.GameComponents.Tilemap;
using CafeAssets.Script.GameComponents.TilemapAdapter.ScriptableObjects;
using UnityEngine;

namespace CafeAssets.Script.System.GameMapSystem.TileInheritance
{
    [CreateAssetMenu(fileName = "FurnitureTile_0000",menuName = "ScriptableObject/FurnitureTile")]
    public class FurnitureTileModel : EffectiveTileModel
    {
        public override TileType Type => TileType.Furniture;
    }
}

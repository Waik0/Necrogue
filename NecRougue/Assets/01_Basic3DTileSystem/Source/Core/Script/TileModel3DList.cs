using UnityEngine;

namespace Basic3DTileSystem.Source.Core.Script
{
    [CreateAssetMenu(fileName = "TileList_0000",menuName = "ScriptableObject/TileBase3DList")]
    public class TileModel3DList : ScriptableObject
    {
        [SerializeField] private TileModel3D[] _tiles;
        public TileModel3D[] Tiles => _tiles;
    }
}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
[CreateAssetMenu(fileName = "TileList_0000",menuName = "ScriptableObject/TileBase3DList")]
public class TileModel3DList : ScriptableObject
{
    [SerializeField] private TileModel3D[] _tiles;
    public TileModel3D[] Tiles => _tiles;
}

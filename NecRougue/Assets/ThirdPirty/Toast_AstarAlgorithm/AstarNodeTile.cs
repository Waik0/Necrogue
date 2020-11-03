using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;
[CreateAssetMenu(fileName = "Passable_00",menuName = "Tiles/AstarNodeTile")]
public class AstarNodeTile : TileBase
{
    public Vector2Int Pos;
    public int Step;
    public float Distance;
    public float Weight;
    public Vector2Int Prev;
    public bool IsAlreadyPassed;
    public bool Passable;
}

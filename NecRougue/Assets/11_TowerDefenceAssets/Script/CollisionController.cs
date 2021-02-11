using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

public class CollisionController : MonoBehaviour
{
    [SerializeField] private Tilemap _tilemap;
    [SerializeField] private Tile _tile;
    void Start()
    {
        Debug();
    }
    void Debug()
    {
        _tilemap.SetTile(new Vector3Int(1,1,0),_tile);
        _tilemap.SetTile(new Vector3Int(1,2,0),_tile);
        _tilemap.SetTile(new Vector3Int(1,3,0),_tile);
        _tilemap.SetTile(new Vector3Int(2,1,0),_tile);
        _tilemap.SetTile(new Vector3Int(2,2,0),_tile);
    }
}

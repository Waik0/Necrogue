using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TilemapSystem3DExample : MonoBehaviour
{

    [SerializeField] private Tilemap3DFacade _facade;
    [SerializeField] private TileModel3DList _model3DList;

    void Start()
    {
        _facade.SetTileList(_model3DList);
        _facade.SetTile(new Vector3Int(0,0,0),0);
        _facade.SetTile(new Vector3Int(1,0,0),0);
        _facade.SetTile(new Vector3Int(0,1,0),0);
        _facade.SetTile(new Vector3Int(0,0,1),0);
        Debug.Log("CellBounds");
        Debug.Log(_facade.CellBounds);
    }
    
}

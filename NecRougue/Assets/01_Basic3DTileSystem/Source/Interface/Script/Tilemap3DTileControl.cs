using System.Collections;
using System.Collections.Generic;
using Basic3DTileSystem.Source.Core.Script;
using UnityEngine;

public class Tilemap3DTileControl : MonoBehaviour
{

    private ITilemap3DEditFacade _editFacade;
    void SetTile(Vector3Int pos,int index)
    {
        _editFacade.SetTile(pos,index);
    }
}

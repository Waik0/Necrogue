using System.Collections;
using System.Collections.Generic;
using Basic3DTileSystem.Example.Script;
using Basic3DTileSystem.Source.Core.Script;
using Basic3DTileSystem.Source.Interface.Script;
using UnityEngine;
using Zenject;

public interface ISelectInputCurrentType
{
    TilemapSystem3DExample.InputType CurrentType { get; }
}
public class PointerEventSelecter : ISelectInputType,ISelectInputCurrentType
{
    private ITilemap3DEditFacade _editFacade;
    private Tilemap3DSelectTile _tilemap3DSelectTile;
    public TilemapSystem3DExample.InputType CurrentType { get; set; }

    [Inject]
    void Inject(
        ITilemap3DEditFacade editFacade
    )
    {
        _editFacade = editFacade;
    }
    public void Select(ITilemap3DInterfaceFacade interfaceFacade,TouchData pos)
    {
        if (interfaceFacade.TileSelected())
        {
            //タイル設置開始
            var tile = _editFacade.GetTilePrefab(interfaceFacade.GetSelectedIndex());
          
            if (tile != null)
            {
                Debug.Log(tile.PlaceType);
                switch (tile.PlaceType)
                {
                    case PlaceType.Single:
                        CurrentType = TilemapSystem3DExample.InputType.PlaceTileSingle;
                        return;
                    case PlaceType.Rect:
                        CurrentType = TilemapSystem3DExample.InputType.PlaceTileRect;
                        return;
                }
            }
        }

        //カメラ移動
        CurrentType = TilemapSystem3DExample.InputType.MoveCamera;

    }

   
}

using System.Collections;
using System.Collections.Generic;
using Basic3DTileSystem.Example.Script;
using Basic3DTileSystem.Source.Core.Script;
using Basic3DTileSystem.Source.Interface.Script;
using UnityEngine;
using Zenject;

public class TilePlaceSingle : ITilemap3DPointerEvent
{
    private ITilemap3DEditFacade _editFacade;
    private int _index = -1;
    private ISelectInputCurrentType _currentType;
    [Inject]
    public void Initialize(ITilemap3DEditFacade editFacade,
        ISelectInputCurrentType currentType)
    {
        _editFacade = editFacade;
        _currentType = currentType;
    }

    public bool CanProcess()
    {
        return _currentType.CurrentType == TilemapSystem3DExample.InputType.PlaceTileSingle;
    }

    public void PointerDown(ITilemap3DInterfaceFacade interfaceFacade,TouchData pos)
    {
        
    }

    public void Drug(ITilemap3DInterfaceFacade interfaceFacade,TouchData pos)
    {
       
    }

    public void PointerUp(ITilemap3DInterfaceFacade interfaceFacade,TouchData pos)
    {
        if (_index >= 0)
        {
            _editFacade.SetTile(pos.OriginTilemapPos,_index);
        }
        interfaceFacade.SelectTile(-1);
    }

    public void SelectTile(int index)
    {
        _index = index;
    }
}
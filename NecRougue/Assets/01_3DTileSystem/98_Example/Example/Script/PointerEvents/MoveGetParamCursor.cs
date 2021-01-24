
using System.Collections.Generic;
using Basic3DTileSystem.Example.Script;
using Basic3DTileSystem.Source.Core.Script;
using Basic3DTileSystem.Source.Interface.Script;
using UnityEngine;
using Zenject;

public class MoveGetParamCursor : ITilemap3DPointerEvent
{
    private ITilemap3DEditFacade _editFacade;
    private int _index = -1;
    private TouchData _down;
    private ISelectInputCurrentType _currentType;
    private ParamGetCursor _paramGetCursor;
    [Inject]
    public void Initialize(
        ITilemap3DEditFacade editFacade,
        ISelectInputCurrentType currentType,
        ParamGetCursor paramGetCursor)
    {
        _editFacade = editFacade;
        _currentType = currentType;
        _paramGetCursor = paramGetCursor;
    }

    public bool CanProcess()
    {
        return _currentType.CurrentType == TilemapSystem3DExample.InputType.GetParam;
    }

    public void PointerDown(ITilemap3DInterfaceFacade interfaceFacade,TouchData pos)
    {
        _paramGetCursor.SetPosition(pos.RayPos);
    }

    public void Drug(ITilemap3DInterfaceFacade interfaceFacade,TouchData pos)
    {
        _paramGetCursor.SetPosition(pos.RayPos);
    }

    public void PointerUp(ITilemap3DInterfaceFacade interfaceFacade,TouchData pos)
    {
        _paramGetCursor.SetPosition(pos.RayPos);
    }

    public void SelectTile(int index)
    {
        _index = index;
    }
}
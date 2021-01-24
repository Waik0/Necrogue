using System.Collections;
using System.Collections.Generic;
using Basic3DTileSystem.Example.Script;
using Basic3DTileSystem.Source.Core.Script;
using Basic3DTileSystem.Source.Interface.Script;
using UnityEngine;
using Zenject;

public class MoveCamera : ITilemap3DPointerEvent
{
    private ITilemap3DCameraControl _cameraControl;
    private ISelectInputCurrentType _currentType;
    private Vector3 _pos;
    private Vector3 _touchPos;
    private Vector3 _buffer;
    [Inject]
    public void Initialize(ITilemap3DCameraControl cameraControl,
        ISelectInputCurrentType currentType)
    {
        _cameraControl = cameraControl;
        _currentType = currentType;
    }

    public bool CanProcess()
    {
        return _currentType.CurrentType == TilemapSystem3DExample.InputType.MoveCamera;
    }

    public void PointerDown(ITilemap3DInterfaceFacade interfaceFacade,TouchData pos)
    {
        _pos = _cameraControl.Position();
        _touchPos = pos.RayPos;
    }

    public void Drug(ITilemap3DInterfaceFacade interfaceFacade,TouchData pos)
    {
        var currentMove = _cameraControl.Position() - _pos;
        var p1 = pos.RayPos - currentMove;
        var p2 = _touchPos ;
        var p = _pos + p2-p1;
        //Debug.Log($"{p1}, {p2}");

        p.y = 0;
        _cameraControl.MoveAbsolute(p);
    }

    public void PointerUp(ITilemap3DInterfaceFacade interfaceFacade,TouchData pos)
    {
        // var p1 = _cameraControl.ScreenToWorld(pos.ScreenPos);
        // var p2 = _cameraControl.ScreenToWorld(_touchPos);
        //
        // _cameraControl.MoveAbsolute(_pos + p2-p1);
    }

    public void SelectTile(int index)
    {
        
    }
}

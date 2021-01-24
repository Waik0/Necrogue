using System.Collections;
using System.Collections.Generic;
using Basic3DTileSystem.Example.Script;
using Basic3DTileSystem.Source.Core.Script;
using Basic3DTileSystem.Source.Interface.Script;
using UnityEngine;
using Zenject;

public class TilePlaceRect : ITilemap3DPointerEvent
{
    private ITilemap3DEditFacade _editFacade;
    private int _index = -1;
    private TouchData _down;
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
        return _currentType.CurrentType == TilemapSystem3DExample.InputType.PlaceTileRect;
    }

    public void PointerDown(ITilemap3DInterfaceFacade interfaceFacade,TouchData pos)
    {
        _down = pos;
    }

    public void Drug(ITilemap3DInterfaceFacade interfaceFacade,TouchData pos)
    {
       
    }

    public void PointerUp(ITilemap3DInterfaceFacade interfaceFacade,TouchData pos)
    {
        if (_index >= 0)
        {
            int sx = Mathf.Min(pos.OriginTilemapPos.x, _down.OriginTilemapPos.x);
            int sy = Mathf.Min(pos.OriginTilemapPos.y, _down.OriginTilemapPos.y);
            int sz = Mathf.Min(pos.OriginTilemapPos.z, _down.OriginTilemapPos.z);
            int ex = Mathf.Max(pos.OriginTilemapPos.x, _down.OriginTilemapPos.x);
            int ey = Mathf.Max(pos.OriginTilemapPos.y, _down.OriginTilemapPos.y);
            int ez = Mathf.Min(pos.OriginTilemapPos.z, _down.OriginTilemapPos.z);
            //todo 高速化
            List<Vector3Int> p = new List<Vector3Int>();

            for (int x = sx; x < ex; x++)
            {
                for (int y = sy; y < ey; y++)
                {
                    for (int z = sz; z < ez; z++)
                    {
                        p.Add(new Vector3Int(x, y, z));
                    }
                }
            }

            _editFacade.SetTiles(p.ToArray(), _index);
        }
        interfaceFacade.SelectTile(-1);
    }

    public void SelectTile(int index)
    {
        _index = index;
    }
}
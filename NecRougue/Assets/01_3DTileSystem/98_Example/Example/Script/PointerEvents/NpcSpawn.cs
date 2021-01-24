using System.Collections;
using System.Collections.Generic;
using Basic3DTileSystem.Example.Script;
using Basic3DTileSystem.Source.Core.Script;
using Basic3DTileSystem.Source.Interface.Script;
using UnityEngine;
using Zenject;

public class NpcSpawn : ITilemap3DPointerEvent
{
    private ITilemap3DEditFacade _editFacade;
    private ISelectInputCurrentType _currentType;
    private ITmsNpcSpawner _spawner;
    [Inject]
    public void Initialize(ITilemap3DEditFacade editFacade,
        ISelectInputCurrentType currentType,
        ITmsNpcSpawner spawner)
    {
        _editFacade = editFacade;
        _currentType = currentType;
        _spawner = spawner;
    }

    public bool CanProcess()
    {
        return _currentType.CurrentType == TilemapSystem3DExample.InputType.PlaceNpc;
    }


    public void PointerDown(ITilemap3DInterfaceFacade interfaceFacade, TouchData pos)
    {
        _spawner.Spawn(new TmsNpcBase.SpawnData()
        {
            SpawnPos = pos.RayPos
        });
    }

    public void Drug(ITilemap3DInterfaceFacade interfaceFacade, TouchData pos)
    {

    }

    public void PointerUp(ITilemap3DInterfaceFacade interfaceFacade, TouchData pos)
    {
     
    }

    public void SelectTile(int index)
    {

    }
}

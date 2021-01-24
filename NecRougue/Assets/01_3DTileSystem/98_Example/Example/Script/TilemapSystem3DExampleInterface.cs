using System.Collections;
using System.Collections.Generic;
using Basic3DTileSystem.Example.Script;
using Basic3DTileSystem.Source.Core.Script;
using Basic3DTileSystem.Source.Interface.Script;
using UnityEngine;
using Zenject;

public class TilemapSystem3DExampleInterface : MonoBehaviour
{
    private ITilemap3DInterfaceFacade _interfaceFacade;
    private ITilemap3DEditFacade _editFacade;
    private ISelectInputCurrentType _currentType;
    [Inject]
    void Inject(
        ISelectInputCurrentType currentType,
        ITilemap3DInterfaceFacade interfaceFacade,
        ITilemap3DEditFacade editFacade
    )
    {
        _editFacade = editFacade;
        _currentType = currentType;
        _interfaceFacade = interfaceFacade;
    }
    void OnGUI()
    {
        GUILayout.BeginArea(new Rect(Screen.width - 250,0,250,Screen.height));
        GUILayout.BeginVertical("box");
        GUILayout.Label($"Current : {_currentType.CurrentType}");
        var list = _editFacade.GetTilePrefabList();
        var c = 0;
        var si = _interfaceFacade.GetSelectedIndex();
        if (GUILayout.Button("TryGetParam"))
        {
            _currentType.CurrentType = TilemapSystem3DExample.InputType.GetParam;
        }

        if (GUILayout.Button("MoveCamera"))
        {
            _currentType.CurrentType = TilemapSystem3DExample.InputType.MoveCamera;
        }
        if (GUILayout.Button("SpawnNpc"))
        {
            _currentType.CurrentType = TilemapSystem3DExample.InputType.PlaceNpc;
        }
        GUILayout.Label($"Name : {_editFacade.GetTilePrefab(si)?.Name ?? null}");
        GUILayout.Label($"Size : {_editFacade.GetTilePrefab(si)?.Size ?? null}");
        GUILayout.Label($"Type : {_editFacade.GetTilePrefab(si)?.PlaceType ?? null}");
        foreach (var tileModel3DReadOnly in list)
        {
            if (GUILayout.Button(tileModel3DReadOnly.Name))
            {
                var i = c;
                _interfaceFacade.SelectTile(i);
            }
            c++;
        }
        GUILayout.EndVertical(); 
        GUILayout.EndArea();
        
    }

}

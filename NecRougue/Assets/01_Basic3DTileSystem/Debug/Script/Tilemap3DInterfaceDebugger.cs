using System;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using Basic3DTileSystem.Source.Interface.Script;
using UnityEngine;
using Zenject;

public class Tilemap3DInterfaceDebugger : MonoBehaviour
{
    private TouchData _currentTouchData;

    private ITilemap3DInterfaceFacade _interfaceFacade;
    [Inject]
    void Inject(
        [InjectOptional]
        ITilemap3DInterfaceFacade facade
        )
    {
        _interfaceFacade = facade;
    }

    private void Awake()
    {
        if(_interfaceFacade != null) 
            _interfaceFacade.Debug = Debug;
    }

    void OnGUI()
    {
        if (_currentTouchData != null)
        {
            GUILayout.BeginVertical("box");
            if (_currentTouchData?.HitObject != null)
            {
                GUILayout.Label($"SelectObject : {_currentTouchData?.HitObject?.name ?? null}");
            }
            GUILayout.Label($"ObjectPos : {_currentTouchData?.HitObjectTilemapPos ?? null }");
            GUILayout.Label($"OriginPos : {_currentTouchData?.OriginTilemapPos ?? null }");
            GUILayout.EndVertical();
        }
    }
    void Debug(TouchData pos)
    {
        _currentTouchData = pos;
    }
}

using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using Zenject;

public abstract class DebuggerBase : MonoBehaviour
{
    public abstract void Debug();
}
public class Tile3DSystemDebugView : MonoBehaviour
{
    private static Tile3DSystemDebugView _instance;
    public static Tile3DSystemDebugView Instance
    {
        get => _instance;
        set
        {
            if (_instance == null)
            {
                _instance = value;
            }
        }
    } 
    private List<DebuggerBase> _debuggers;

    void Awake()
    {
        Instance = this;
        if (Instance != this)
        {
            Destroy(this);
            return;
        }
        Debug.Log("Debugger起動");
        Debug.Log(FindObjectsOfType<DebuggerBase>().ToList().Count);
        _debuggers= FindObjectsOfType<DebuggerBase>().ToList();
    }
    private void OnGUI()
    {
        GUILayout.BeginVertical();
        foreach (var debugger in _debuggers)
        {
            debugger.Debug();
        }
        GUILayout.EndVertical();
    }
}

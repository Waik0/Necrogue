using System;
using System.Collections;
using System.Collections.Generic;
using System.Globalization;
using UnityEngine;

public class LogHandler : MonoBehaviour
{
    private string _log;
    // Start is called before the first frame update
    void Awake()
    {
        Application.logMessageReceived += HandleLog;
    }
    private void OnApplicationQuit()
    {
        Application.logMessageReceived -= HandleLog;
    }

    void HandleLog(string condition, string stackTrace, LogType type)
    {
        _log += type + condition + "\n";
    }

    void Update()
    {
        if (Input.GetMouseButtonDown(0))
        {
            
        }
    }
    // Update is called once per frame
    void OnGUI()
    {
        GUI.skin.label.fontSize =  Screen.width / 64;
        GUI.skin.button.fontSize = Screen.width / 64;
        GUILayout.BeginVertical("box");
        GUILayout.Label(_log);
        GUILayout.EndVertical();
    }
}

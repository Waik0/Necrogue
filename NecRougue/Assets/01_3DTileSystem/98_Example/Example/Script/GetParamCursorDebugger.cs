using System.Collections;
using System.Collections.Generic;
using Basic3DTileSystem.Source.Interface.Script;
using UnityEngine;
using Zenject;

public class GetParamCursorDebugger : DebuggerBase
{
    private ParamGetCursor _cursor;
    [Inject]
    void Inject(
        ParamGetCursor cursor
    )
    {
        _cursor = cursor;
    }

    public override void Debug()
    {
        GUILayout.BeginVertical("box");
        GUILayout.Label("CursorPos : " +_cursor.gameObject.transform.position );
        GUILayout.Label("Params : ");
        foreach (var cursorParam in _cursor.Params)
        {
            GUILayout.Label(cursorParam.ToString());
        }
        
        GUILayout.EndVertical();
    }
}

using System.Collections;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using UnityEngine;

public class DebugLog
{
    /// <summary>
    /// デバッグログ拡張
    /// あらゆる箇所から依存されるが、Stringクラスの例同様、破壊的変更がほぼなく安定性が高くても問題ないクラス
    /// </summary>
    public static void Function(object self,int indent = 0,[CallerMemberName]string name = "",[CallerLineNumber]int lineNumber = 0)
    {
#if DEBUG
        string idt = "";
        for (int i = 0; i < indent; i++)
        {
            idt += "-- ";
        }
        string str = $"[<color=lime> {self.GetType().Name} </color>] ";
        Debug.Log($"{idt}{str,-45}{name} ");
        //"(Line : {lineNumber})");
#endif
    }
    public static void LogClassName(object self,string text)
    {
        //defineで出しわける
        Debug.Log($"<color=lime>[{self.GetType().Name}]</color> {text}");
    }
}

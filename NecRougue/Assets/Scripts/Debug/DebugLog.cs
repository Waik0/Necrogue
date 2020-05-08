using System.Collections;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using UnityEngine;

public class DebugLog
{
    public static void Function(object self,int indent = 0,[CallerMemberName]string name = "",[CallerLineNumber]int lineNumber = 0)
    {
        #if DEBUG
        string idt = "";
        for (int i = 0; i < indent; i++)
        {
            idt += "    ";
        }

        Debug.Log($"{idt}[ {self.GetType().Name} ] {name} ");
        //"(Line : {lineNumber})");
#endif
    }
}

using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Text;
using UnityEngine;
using UnityEditor;
public class EditorUtil
{
    public static string DragAndDropRect()
    {
        var dropArea = GUILayoutUtility.GetRect(0.0f, 50.0f, GUILayout.ExpandWidth(true));

        GUI.Box(dropArea, "Drag & Drop");
        string[] m_paths = new string[0];
        var evt = Event.current;
        switch (evt.type)
        {
            case EventType.DragUpdated:
            case EventType.DragPerform:
                if (!dropArea.Contains(evt.mousePosition)) break;


                //マウスの形状
                DragAndDrop.visualMode = DragAndDropVisualMode.Copy;
                
                if (evt.type == EventType.DragPerform)
                {
                    DragAndDrop.AcceptDrag();

                    m_paths = DragAndDrop.paths;

                    foreach (string path in DragAndDrop.paths)
                    {
                        //GetFullPath(path);
                    }

                    foreach (var draggedObject in DragAndDrop.objectReferences)
                    {
                        Debug.Log(draggedObject);
                    }
                    DragAndDrop.activeControlID = 0;
                }
                Event.current.Use();
                break;
        }


        foreach (string path in m_paths)
        {
            //EditorGUILayout.TextField("Path",path);
            return path;
        }

        return "";
    }

    public static void SaveJsonDialog(string data)
    {
        var path = EditorUtility.SaveFilePanel("save", Application.dataPath, "mst_.json", "json");

        if (!string.IsNullOrEmpty(path))
        {
            File.WriteAllText(path, data, Encoding.UTF8);
            AssetDatabase.Refresh();

        }
    }

    public static string LoadDialog(string ext)
    {
        var path = EditorUtility.OpenFilePanel("open", Application.dataPath, ext);

        if (!string.IsNullOrEmpty(path))
        {
            
            return File.ReadAllText(path, Encoding.UTF8);
            //AssetDatabase.Refresh();

        }

        return "";
    }
    public static void SaveScriptDialog(string data)
    {
        var path = EditorUtility.SaveFilePanel("save", Application.dataPath, "Mst.cs", "cs");

        if (!string.IsNullOrEmpty(path))
        {
            File.WriteAllText(path, data, Encoding.UTF8);
            AssetDatabase.Refresh();

        }
    }
}


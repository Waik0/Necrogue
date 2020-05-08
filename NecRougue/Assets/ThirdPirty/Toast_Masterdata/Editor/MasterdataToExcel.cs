
using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using UnityEditor;
using UnityEngine;

public class MasterdataToExcel : EditorWindow
{
    private string _log = "";
    private List<Type> _classes = new List<Type>();
    private string Log
    {
        get => _log;
        set => _log += value + "\n";
    }

    [MenuItem("Toast/MasterdataExport")]
    static void Open()
    {
        var window = CreateInstance<MasterdataToExcel>();
        window.Show();
    }

    private Vector2 scr;
    void OnGUI()
    {
        if (GUILayout.Button("Export"))
        {
            LoadClassList();
            ExportDefines();
        }
        scr = GUILayout.BeginScrollView(scr);
        GUILayout.Label(Log);
        GUILayout.EndScrollView();
    }

    //-

    private void LoadClassList()
    {
        _classes.Clear();
        foreach (Type type in
            Assembly.GetAssembly(typeof(IMasterRecord)).GetTypes()
                .Where(myType =>
                    myType.IsClass && typeof(IMasterRecord).IsAssignableFrom(myType)))
        {
            Debug.Log(type.ToString());
            _classes.Add(type);
        }
    }

    private Dictionary<string,string> GetConf(Type t)
    {
        Attribute[] attributes = Attribute.GetCustomAttributes(t, typeof(MasterPath));
        if (attributes == null || attributes.Length == 0)
        {
            throw new InvalidOperationException("The provided object is not serializable");

        }

        var path = (attributes[0] as MasterPath).Path;
        //desc
        Attribute[] dsc = Attribute.GetCustomAttributes(t, typeof(MasterDescription));
        var description = "";
        if (dsc != null && dsc.Length != 0)
        {
            description = (dsc[0] as MasterDescription).Msg;
        }

        return new Dictionary<string, string>()
        {
            {"path", path},
            {"description", description}
        };
    }
    private void ExportDefines()
    {
        var book = new Dictionary<string,Dictionary<string, string>>();
        var defSheet = new Dictionary<string, Dictionary<string,string>>();
        var confs = new Dictionary<string, Dictionary<string,string>>();
        foreach (var t in _classes)
        {
            FieldInfo[] fields
                = t.GetFields();
            //Debug.Log(t.Name);
            //Log += t.Name;
            var sheet = new Dictionary<string,string>();
            foreach (var fieldInfo in fields)
            {
                //Log += " - " + fieldInfo.Name;
                //Log += "   " + fieldInfo.FieldType;

                if (fieldInfo.FieldType.IsEnum)
                {
                    if (!defSheet.ContainsKey(fieldInfo.FieldType.ToString()))
                    {
                        defSheet.Add(fieldInfo.FieldType.ToString(),new Dictionary<string,string>());
                        foreach (var value in Enum.GetValues(fieldInfo.FieldType))
                        {
                            defSheet[fieldInfo.FieldType.ToString()].Add(((int)value).ToString(), value.ToString());
                            //Log += "   - " + value;

                        }
                    }

                }
                
                sheet.Add(fieldInfo.Name,fieldInfo.FieldType.ToString());
            }
            book.Add(t.Name,sheet);
            confs.Add(t.Name, GetConf(t));

        }
        var pathBook = "/master.json";
        var pathDef = "/master_def.json";
        var pathConf = "/master_conf.json";
        var jsonmst = MiniJSON.Json.Serialize(book);
        var jsondef = MiniJSON.Json.Serialize(defSheet);
        var jsonconf = MiniJSON.Json.Serialize(confs);
        SaveJson(pathBook, jsonmst);
        SaveJson(pathDef,jsondef);
        SaveJson(pathConf,jsonconf);

    }

    private void SaveJson(string path,string json)
    {
        var unionPath = "ExportedMasterDefine";
        SafeCreateDirectory(unionPath);
        Debug.Log("Write Data : " + path);
        File.WriteAllText(unionPath + path, json);
    }
    private static DirectoryInfo SafeCreateDirectory(string path)
    {
        if (Directory.Exists(path))
        {
            return null;
        }
        return Directory.CreateDirectory(path);
    }
}

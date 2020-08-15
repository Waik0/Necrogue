using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text.RegularExpressions;
using UnityEditor;
using UnityEngine;

public class MasterdataExport : EditorWindow
{
    [MenuItem("Toast/MasterdataExporter")]
    static void Open()
    {
        var classList = LoadClassList();
        SaveMetaJson(classList);
        SaveToJson( classList);
    }
    public static List<Type> LoadClassList()
    {
        var classes = new List<Type>();
        foreach (Type type in
            Assembly.GetAssembly(typeof(IMasterRecord)).GetTypes()
                .Where(myType =>
                    myType.IsClass && typeof(IMasterRecord).IsAssignableFrom(myType)))
        {
            Debug.Log(type.ToString());
            classes.Add(type);
        }

        return classes;

    }

    private static void SaveMetaJson(List<Type> classes)
    {
        string savepath = "";
        var root = new List<Dictionary<string, string>>();
        foreach (var type in classes)
        {
            Attribute[] attributes = Attribute.GetCustomAttributes(type, typeof(MasterPath));
            if (attributes == null || attributes.Length == 0)
            {
                throw new InvalidOperationException("The provided object is not serializable");

            }
            var path = (attributes[0] as MasterPath).Path;
            if (savepath == "")
            {
                savepath =  "Assets/" + path.Replace(Path.GetFileName(path), "");
                savepath += "_meta_mst.json";
            }

            var data = new Dictionary<string, string>();
            data.Add(type.Name,path);
            root.Add(data);
        }
                    
        var json = MiniJSON.Json.Serialize(root);
        Debug.Log("meta " + savepath);
        File.WriteAllText(savepath,json);
        AssetDatabase.Refresh();
    }
    private static void SaveToJson(List<Type> classes)   
    {
      
        Debug.Log("Save");
        foreach (var t in classes)
        {
            Attribute[] attributes = Attribute.GetCustomAttributes(t, typeof(MasterPath));
            if (attributes == null || attributes.Length == 0)
            {
                throw new InvalidOperationException("The provided object is not serializable");

            }
            var path = "Assets/" + (attributes[0] as MasterPath).Path;
            var d = AssetDatabase.LoadAssetAtPath<TextAsset>(path);

            var root = new Dictionary<string,object>(); 
            var meta = new Dictionary<string, object>();
            var fields = new Dictionary<string, object>();
            var enums = new Dictionary<string,Dictionary<int,string>>();
            var records = new List<Dictionary<string,object>>();
            FieldInfo[] fieldsraw
                = t.GetFields();

            foreach (var fieldInfo in fieldsraw)
            {
                fields.Add(fieldInfo.Name,fieldInfo.FieldType.ToString());
                if (fieldInfo.FieldType.IsEnum)
                {
                    if (!enums.ContainsKey(fieldInfo.FieldType.ToString()))
                    {
                        var e = new Dictionary<int, string>();
                       
                        foreach (var value in Enum.GetValues(fieldInfo.FieldType))
                        {
                            e.Add((int)value,value.ToString());
                        }
                        enums.Add(fieldInfo.FieldType.ToString(),e);
                    }

                }
                var enumType = fieldInfo.FieldType.GetElementType();
                Debug.Log(fieldInfo.FieldType);
                if (enumType != null)
                {
                    Debug.Log("Array of " + enumType.ToString());
                    if (enumType.IsEnum)
                    {
                        if (!enums.ContainsKey(enumType.ToString()))
                        {
                            var e = new Dictionary<int, string>();

                            foreach (var value in Enum.GetValues(enumType))
                            {
                                e.Add((int) value, value.ToString());
                            }

                            enums.Add(enumType.ToString(), e);
                        }

                    }
                }
            }
      
            meta.Add("Enums",enums);
            meta.Add("Fields",fields);
            root.Add("Meta",meta);
                  
            if (d != null)
            {
                var data = MiniJSON.Json.Deserialize(d.text) as Dictionary<string,object>;
                
                if (data != null && data.ContainsKey("Records"))
                {
                    Debug.Log("RecordExist");
                    if (data["Records"] is List<object> rec)
                    {
                        Debug.Log("ParseRecord");
                        foreach (var dictionary in rec)
                        {
                            var dic = new Dictionary<string, object>();
                            foreach (var keyValuePair in (Dictionary<string, object>) dictionary)
                            {
                                if (fields.ContainsKey(keyValuePair.Key))
                                {
                                    var typeName = fields[keyValuePair.Key].ToString();
                                    Debug.Log(typeName);
                                    if (Regex.IsMatch(typeName, @"^(.*?)\[\]"))
                                    {
                                        var match = Regex.Match(typeName, @"^(?<type>.*?)\[\]");
                                        if (match.Success)
                                        {
                                            var lt = match.Groups["type"].Value;
                                            Debug.Log($"Array of {lt}");
                                            var li = keyValuePair.Value as List<object>;
                                            dic.Add(keyValuePair.Key,li);
                                        }
                                    }
                                    // if (Regex.IsMatch(typeName, @"System.Collections.Generic.List`1\[(.*)\]"))
                                    // {
                                    //     var match = Regex.Match(typeName, @"System.Collections.Generic.List`1\[(?<type>.*)\]");
                                    //     if (match.Success == true)
                                    //     {
                                    //         var lt = match.Groups["type"].Value;
                                    //         Debug.Log($"List of {lt}");
                                    //         var li = keyValuePair.Value as List<object>;
                                    //         dic.Add(keyValuePair.Key,li);
                                    //     }
                                    // }
                                    else if( enums.ContainsKey(typeName))
                                    {
                                        
                                        dic.Add(keyValuePair.Key,keyValuePair.Value.ToString());
                                    }
                                    else
                                    {
                                        dic.Add(keyValuePair.Key,keyValuePair.Value.ToString());
                                    }
                                    
                                    
                                    
                                }
                                
                            }
                            records.Add(dic);
                        }
                    }
                }
             
            }

            root.Add("Records",records);
            
            
            var json = MiniJSON.Json.Serialize(root);
            Debug.Log("Write Data : " + path);
            File.WriteAllText(path,json);
            
        } 
        AssetDatabase.Refresh();
        //var data = AssetDatabase.LoadAssetAtPath<TextAsset>(path);
        //TableToJson.MakeJson(list);
    }
    
    

}

using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using UnityEditor;
using UnityEngine;

namespace Toast.Masterdata.Editor
{


    public class TableData
    {
        
        public Dictionary<string, (List<string> Data,Type Define, int Width)> Data = new Dictionary<string, (List<string> Data, Type Define, int Width)>();
        public string Description = "";
        private List<Type> _typeList = new List<Type>();
        private string labelCache = "";
        private int defineCache = 0;
        private int selectedIndexCache = 0;
        private int selectedClassCache = 0;
        private List<Type> _classes = new List<Type>();
        private int _index = -1;
        private enum Styles
        {
            Base,
            Odd,
            Even,
            Select,
            Title,
            Odd_Enum,
            Even_Enum
        }
        private Dictionary<Styles, Texture2D> _textures = new Dictionary<Styles, Texture2D>()
        {
            { Styles.Base, new Texture2D(15, 15)},
            { Styles.Odd, new Texture2D(15, 15)},
            { Styles.Even, new Texture2D(15, 15)},
            { Styles.Select, new Texture2D(15, 15)},
            { Styles.Title, new Texture2D(15, 15)},
            { Styles.Odd_Enum, new Texture2D(15, 15)},
            { Styles.Even_Enum, new Texture2D(15, 15)},
        };

        private Dictionary<Styles, GUIStyle> _styles = new Dictionary<Styles, GUIStyle>()
        {
            {Styles.Base, new GUIStyle()},
            {Styles.Odd, new GUIStyle()},
            {Styles.Even, new GUIStyle()},
            {Styles.Select, new GUIStyle()},
            {Styles.Title, new GUIStyle()},
            {Styles.Odd_Enum, new GUIStyle()},
            {Styles.Even_Enum, new GUIStyle()},

        };


        public TableData()
        {
           Init();
        }

        void Init()
        {
            TextureInit(Styles.Base, 1, Color.white, Color.black,true);
            TextureInit(Styles.Odd, 1,Color.white,Color.black);
            TextureInit(Styles.Even,1, new Color(0.9f,0.9f,0.9f), Color.black);
            TextureInit(Styles.Select, 2, Color.white, new Color(0,0.4f,0),true);
            TextureInit(Styles.Title, 1, Color.gray, Color.black);
            TextureInit(Styles.Odd_Enum, 1, new Color(0.95f, 0.95f, 0.95f), Color.black);
            TextureInit(Styles.Even_Enum, 1, new Color(0.8f, 0.8f, 0.8f), Color.black);
            StylesInit();
            _styles[Styles.Base].padding = new RectOffset(0,1,0,0);
            _styles[Styles.Base].fixedHeight = 0;
            _styles[Styles.Base].stretchHeight = false;
            _styles[Styles.Base].stretchWidth = false;
        } 

        void StylesInit()
        {
            foreach (Styles style in _styles.Keys)
            {
                _styles[style].border = new RectOffset(3, 3, 2, 2);
                _styles[style].normal.textColor = Color.black;
                _styles[style].padding = new RectOffset(2,2,2,2);
                _styles[style].wordWrap = false;
                _styles[style].stretchHeight = false;
                _styles[style].margin = new RectOffset(0,0,0,0);
                _styles[style].fixedHeight = 18f;
                _styles[style].normal.background = _textures[style];
                //_styles[style].onFocused.background = _textures[Styles.Select];
            }
        }
        void TextureInitPullDown(Styles index, int tick, Color c, Color bc, bool rightBorder = false)
        {

            for (int i = 0; i < _textures[index].width; i++)
            {
                for (int y = 0; y < _textures[index].height; y++)
                {
                    if (i < tick + 6 ||
                        (i >= _textures[index].width - tick && rightBorder) ||
                        y < tick ||
                        y >= _textures[index].height - tick)
                    {
                        _textures[index].SetPixel(i, y, bc);
                    }
                    else
                    {
                        _textures[index].SetPixel(i, y, c);
                    }
                }
            }

            var arrowColor = Color.white;
            _textures[index].SetPixel(1, 7, arrowColor);
            _textures[index].SetPixel(2, 7, arrowColor);
            _textures[index].SetPixel(3, 7, arrowColor);
            _textures[index].SetPixel(4, 7, arrowColor);
            _textures[index].SetPixel(2, 6, arrowColor);
            _textures[index].SetPixel(3, 6, arrowColor);
            _textures[index].Apply();
        }
        void TextureInit(Styles index,int tick,Color c, Color bc, bool rightBorder = false)
        {
            for(int i = 0; i < _textures[index].width; i++)
            {
                for (int y = 0; y < _textures[index].height; y++)
                {
                    if(i < tick || 
                       (i >= _textures[index].width - tick && rightBorder) ||
                       y < tick  ||
                       y > _textures[index].height - tick)
                    {
                        _textures[index].SetPixel(i, y, bc);
                    }
                    else
                    {
                        _textures[index].SetPixel(i, y, c);
                    }
                }
            }
            _textures[index].Apply();
        }
        public void AddData(string label,Type type)
        {
            if (label != "" && !Data.ContainsKey(label))
            {
                Data.Add(label,( new List<string>() , type , 50 ));
            }

            CheckDataLength();
        }

        void RemoveData(string label)
        {
            if (label != "" && Data.ContainsKey(label))
            {
                Data.Remove(label);
               
            }

        }

        void AddRow()
        {
            var count = 0;
            
            foreach (var list in Data)
            {
                if (count == 0 && list.Value.Define == typeof(int) && list.Value.Data.Count > 0)
                {
                    list.Value.Data.Add( (int.Parse(list.Value.Data.Last()) + 1).ToString());
                }
                else
                {
                    list.Value.Data.Add("");
                }

                count++;
            }

            CheckDataLength();
        }
        void RemoveRow(int row)
        {
            foreach (var list in Data)
            {
                list.Value.Data.RemoveAt(row);
            }

        }

        public void LoadClassList()
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
        public void ViewClasses()
        {
            GUILayout.BeginVertical(GUI.skin.box,GUILayout.Width(400));

            selectedClassCache = EditorGUILayout.Popup("Class", selectedClassCache, _classes.ConvertAll(_=>_.ToString()).ToArray());
            if (GUILayout.Button("Go",GUILayout.Width(200)))
            {
                LoadFromClass(selectedClassCache);
            }
            GUILayout.EndVertical();
        }
        public void View()
        {

            Color defaultColor = GUI.backgroundColor;
            GUI.backgroundColor = new Color(1, 1, 1);
            GUILayout.BeginVertical();
            GUILayout.BeginVertical("box");
            GUILayout.Label(Description);
            GUILayout.EndVertical();
            ViewData(Data);

            AddRowButton();
            AddSaveToJsonButton();
            //AddLoadButton();
            GUILayout.EndVertical();
            GUI.backgroundColor = defaultColor;
        }
        

        private void LabelRightClickEvent(string label, Rect rect)
        {
            var ev = Event.current;
            if (ev.type != EventType.MouseDown || ev.button != 1 || !rect.Contains(ev.mousePosition))
            {
                return;
            }

            var menu = new GenericMenu();
            menu.AddItem(new GUIContent("Delete " + label), false, () => { RemoveData(label); });
            menu.ShowAsContext();
            ev.Use();


        }

        private void CheckDataLength()
        {
            var max = 0;
            foreach (var list in Data)
            {
                max = Math.Max(list.Value.Data.Count ,max) ;
            }

            foreach (var list in Data)
            {
                var d = max - list.Value.Data.Count;
                if (d > 0)
                {
                    var l = Enumerable.Repeat<string>("", d).ToArray();

                    list.Value.Data.AddRange(l);
                }
            }
        }

        private void ViewData(Dictionary<string, (List<string> Data, Type Define, int Width)> data)
        {
            
            GUILayout.BeginHorizontal(_styles[Styles.Base]);

            foreach (var list in data)
            {
                
                GUILayout.BeginVertical(GUILayout.MinWidth(list.Value.Width));
                GUILayout.Label(list.Key, _styles[Styles.Title]);
                //公式によるとリペイント時に限るらしいがこれでうまくいってるので…
                var rect = GUILayoutUtility.GetLastRect();
                LabelRightClickEvent(list.Key, rect);
               
                GUILayout.Label((list.Value.Define).ToString() , _styles[Styles.Title]);
                Func<string,int, string> view = null;
                switch (list.Value.Define)
                {
                    default:
                        if (list.Value.Define.IsEnum)
                        {
                           
                            view = (str,cnt) =>
                            {
                                var style = cnt % 2 == 0 ? _styles[Styles.Even_Enum] : _styles[Styles.Odd_Enum];
                                var parse = 0;
                                int.TryParse(str, out parse);
                                var valueList = new List<int>();
                                foreach (int value in Enum.GetValues(list.Value.Define))
                                {
                                    valueList.Add(value);
                                }

                                var index = 0;
                                if (valueList.Contains(parse))
                                {
                                    index = valueList.IndexOf(parse);
                                   
                                }
                                return valueList[EditorGUILayout.Popup(
                                    index,
                                    Enum.GetNames(list.Value.Define),
                                    style,
                                    GUILayout.MaxHeight(style.fixedHeight))].ToString();
                            };
                        }
                        else
                        {
                            
                            view = (str, cnt) =>
                            {
                                var style = cnt % 2 == 0 ? _styles[Styles.Even] : _styles[Styles.Odd];
                                return GUILayout.TextField(str , style);
                            };
                        }
                        break;
                }
                if (view != null)
                {

                    for (var i = 0; i < list.Value.Data.Count; i++)
                    {

                        list.Value.Data[i] = view(list.Value.Data[i],i);
                    }
                }
                GUILayout.EndVertical();
  
            }
            // AddColumnButton(100);
            GUILayout.EndHorizontal();
        }
        //足りないところを埋める
        private void FillData()
        {

        }

        private void PullDown(string[] types)
        {

        }
        //列追加
        private void AddColumnButton(int width)
        {
            GUILayout.BeginVertical(GUI.skin.box, GUILayout.Width(width));
           // GUILayout.BeginHorizontal();
            GUILayout.Label("name:");
            labelCache = GUILayout.TextField(labelCache);
          
            //GUILayout.EndHorizontal();
            //GUILayout.BeginHorizontal();
            GUILayout.Label("type:");
            defineCache = EditorGUILayout.Popup(defineCache, _typeList.ConvertAll(_=>_.ToString()).ToArray());
            //GUILayout.EndHorizontal();
            if (GUILayout.Button("+"))
            {
                AddData(labelCache, _typeList[defineCache]);
                labelCache = "";
                defineCache = 0;
            }
            GUILayout.EndVertical();
        }
        //要素追加
        private void AddRowButton()
        {
            if (GUILayout.Button("+",GUILayout.Width(100)))
            {
                AddRow();
            }

        }

        private void AddLoadButton()
        {
            if (GUILayout.Button("LOAD", GUILayout.Width(100)))
            {
                LoadFromJson();
            }
        }
        private void AddSaveToJsonButton()
        {
            //var str = EditorUtil.DragAndDropRect();
            if (GUILayout.Button("SAVE", GUILayout.Width(100)))
            {
                if (_index < 0)
                {
                    return;
                }
                SaveToJson(_index);
            }

        }

        private void LoadFromJson()
        {
            var json = TableToJson.LoadFromJson();
            if (json == null || json.Count == 0)
            {
                return;
                
            }
            Data.Clear();
            
            foreach (var dictionary in json)
            {
                foreach (var keyValuePair in dictionary)
                {
                    if (!Data.ContainsKey(keyValuePair.Key))
                    {
                        Data.Add(keyValuePair.Key,(new List<string>(),typeof(string),50));
                    }
                    Data[keyValuePair.Key].Data.Add(keyValuePair.Value.ToString());
                }
            }
        }

        private void LoadFromClass(int index)
        {
            Debug.Log("Load");
            var t = _classes[index];
            FieldInfo[] fields 
                = t.GetFields();

            Data.Clear();
            foreach (var fieldInfo in fields)
            {
                if (!Data.ContainsKey(fieldInfo.Name))
                {
                    Data.Add(fieldInfo.Name, (new List<string>(), fieldInfo.FieldType, 50));
                }

            }
            Attribute[] attributes = Attribute.GetCustomAttributes(t, typeof(MasterPath));
            if (attributes == null || attributes.Length == 0)
            {
                throw new InvalidOperationException("The provided object is not serializable");

            }

            var path = "Assets/Resources/" + (attributes[0] as MasterPath).Path;
            //desc
            Attribute[] dsc = Attribute.GetCustomAttributes(t, typeof(MasterDescription));
            Description = "";
            if (dsc != null && dsc.Length != 0)
            {
                Description = (dsc[0] as MasterDescription).Msg;
            }

            var d = AssetDatabase.LoadAssetAtPath<TextAsset>(path);
            Debug.Log(path);
            if (d == null)
            {
                _index = index;
                return;
            }
            
            var list = new List<Dictionary<string, object>>();
            var json = (IList)MiniJSON.Json.Deserialize(d.text);
            var dicList = new List<IDictionary>();
            //sort
            
            
                
            
            foreach (var o in json)
            {
                var dic = (IDictionary)o;
                var ret = new Dictionary<string, object>();
                dicList.Add(dic);
            
            }

            foreach (var dictionary in dicList.OrderBy(_ =>
            {
                foreach (DictionaryEntry elem in _)
                {
                    if (elem.Key.ToString() == "id")
                    {
                        try
                        {
                            return int.Parse(elem.Value.ToString());
                        }
                        catch
                        {
                            return int.MaxValue;
                        }
                    }
                }

                return int.MaxValue;
            }))
            {
                foreach (DictionaryEntry o1 in dictionary)
                {

                    if (Data.ContainsKey(o1.Key.ToString()))
                    {
                        Data[o1.Key.ToString()].Data.Add(o1.Value.ToString());
                    }
                }
            }
            var datalength = 0;
            foreach (var keyValuePair in Data)
            {
                datalength = Math.Max(keyValuePair.Value.Data.Count, datalength);
            }
            foreach (var keyValuePair in Data)
            {
                var num = datalength - keyValuePair.Value.Data.Count;
                for (int i = 0; i < num; i++)
                {
                    keyValuePair.Value.Data.Add("0");
                }
            }
            _index = index;
            


        }
        private void SaveToJson(int index)
        {
            Debug.Log("Save");
            var max = 0;
            foreach (var l in Data)
            {
                max = Math.Max(l.Value.Data.Count, max);
            }
            var list = new List<Dictionary<string, string>>();
            
            for (int i = 0;i < max; i++)
            {
                var dic = new Dictionary<string, string>();
                foreach (var d in Data)
                {
                    dic.Add(d.Key, d.Value.Data[i]);

                }
                list.Add(dic);
            }
            var t = _classes[index];

            Attribute[] attributes = Attribute.GetCustomAttributes(t, typeof(MasterPath));
            if (attributes == null || attributes.Length == 0)
            {
                throw new InvalidOperationException("The provided object is not serializable");

            }

            var path = "Assets/Resources/" + (attributes[0] as MasterPath).Path;
            var json = MiniJSON.Json.Serialize(list);
            Debug.Log("Write Data : " + path);
            File.WriteAllText(path,json);
            AssetDatabase.Refresh();
            //var data = AssetDatabase.LoadAssetAtPath<TextAsset>(path);
            //TableToJson.MakeJson(list);
        }
        
        private void SaveToClass()
        {

        }


    }

    class DefineToClass
    {
        public static void MakeScript(string name ,List<Dictionary<string, string>> def)
        {

            var json = MiniJSON.Json.Serialize(def);
        }
        static string GetTemplatePath()
        {
            return "";
        }
    }

    class TableToJson
    {
        public static void MakeJson( List<Dictionary<string, string>> list)
        {
            var json = MiniJSON.Json.Serialize(list);
            Debug.Log(json);
            EditorUtil.SaveJsonDialog(json);

        }

        public static List<Dictionary<string, object>> LoadFromJson()
        {
            var d = EditorUtil.LoadDialog("json");
            Debug.Log(d);
            if (string.IsNullOrEmpty(d))
            {
                return new List<Dictionary<string, object>>();
            }

            var list = new List<Dictionary<string, object>>();
            var json = (IList)MiniJSON.Json.Deserialize(d);
            foreach (var o in json)
            {
                var dic = (IDictionary)o;
                var ret = new Dictionary<string, object>();
                foreach (DictionaryEntry o1 in dic)
                {
                    ret.Add(o1.Key.ToString(),o1.Value);
                }
                list.Add(ret);
            }
            Debug.Log(list);
            return list;
        }
        public static void MakeDefJson()
        {

        }
    }

}
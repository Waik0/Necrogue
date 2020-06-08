using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using UnityEditor;
using UnityEngine;

public class FontToRgf : EditorWindow
{
    // Start is called before the first frame update
    [MenuItem("Toast/RgfGenerator")]
    static void Open()
    {
        var window = CreateInstance<FontToRgf>();
        window.Show();
    }

    private Texture2D _texture;
    private List<FontSet> _glyph;
    //private List<char> _letters;
    void OnGUI()
    {
        GUILayout.BeginHorizontal(GUILayout.Width(100));
        GUILayout.BeginVertical();
        if (GUILayout.Button("LoadPng"))
        {
           
           OpenBmp();
           //TestPixels();
        }
        if (GUILayout.Button("LoadGlyph"))
        {
            OpenGlyph();
        }
        //if (GUILayout.Button("LoadTxt"))
        //{
        //    OpenLetterText();
        //}

        if (_glyph != null &&  _texture != null)
        {
            if (GUILayout.Button("Generate", GUILayout.Height(100)))
            {
               Generate();
                
            }
        }

        GUILayout.EndVertical();
        GUILayout.BeginVertical();
        if (_texture != null)
        {
            EditorGUI.DrawTextureAlpha(new Rect(100, 10, 96, 96 * _texture.height / _texture.width), _texture);
        }

        GUILayout.EndVertical();
        GUILayout.EndHorizontal();
    }

    void TestPixels()
    {
        if (_texture == null)
        {
            return;
        }
        var list = new List<Color32>();
        foreach (var color32 in _texture.GetPixels32())
        {
            var res = list.Any(_ => _.r == color32.r && _.g == color32.g && _.b == color32.b);
            if (!res)
            {
                list.Add(color32);
                Debug.Log(color32);
            }
        }
    }
    void Generate()
    {
        
        if (_glyph == null || _texture == null)
        {
            Debug.LogError("FAIL");
            return;
        }

        var path = EditorUtility.SaveFilePanel("保存先", "Assets","font","asset");
        var saveDist = Regex.Replace(path,@"^.*?Assets/","Assets/");
        var rgf = ScriptableObject.CreateInstance<RetroGraphicsFont>();
        rgf.Buffer = new bool[_texture.width * _texture.height];
        rgf.Stride = _texture.width;
        var pixels = _texture.GetPixels32();
        for (var i = 0; i < _texture.GetPixels32().Length; i++)
        {
            rgf.Buffer[i] = pixels[i].r <= 128 || pixels[i].b <= 128 || pixels[i].g <= 128;

        }

       
        // var g = new List<FontSet>();
        // for (int i = 0; i < _glyph.Count; i++)
        // {
        //     g.Add(new FontSet(){
        //         Key = _letters[i],
        //         Rect = _glyph[i]});
        // }

        rgf.Glyphs = _glyph.ToArray();
        AssetDatabase.CreateAsset(rgf, saveDist);
        AssetDatabase.Refresh();
    }

    // void OpenLetterText()
    // {
    //     _letters = new List<char>();
    //     var txt = LoadText("txt");
    //     for (var i = 0; i < txt.Length; i++)
    //     {
    //         _letters.Add(txt[i]);
    //     }
    //     Debug.Log("Load Letter : Count " + _letters.Count);
    // }
    void OpenGlyph()
    {
        _glyph =new List<FontSet>();
        var data = LoadFromJson();
        if (data.ContainsKey("map"))
        {
            //map以下
            //try
            {
                var val = data["map"] as Dictionary<string, object>;
                foreach (var val2 in val)
                {
                    var glyph = val2.Value as Dictionary<string, object>;
                    int key = int.Parse(val2.Key);
                    int x = (int) (long) glyph["x"];
                    int y = (int) (long) glyph["y"];
                    int width = (int) (long) glyph["width"];
                    int height = (int) (long) glyph["height"];


                    _glyph.Add(
                        new FontSet()
                        {
                            Key = Convert.ToChar(key),
                            Rect = new FontRect()
                            {
                                sX = x,
                                sY = y,
                                eX = x + width - 1,
                                eY = y + height - 1,
                            }
                        });
                }
            }
            //catch (Exception e)
            {
                //    Console.WriteLine(e);
                //    throw;
            }
        }
        Debug.Log("Load Glyph : Count " + _glyph.Count);
    }
    public string LoadText(string ext)
    {
        var path = EditorUtility.OpenFilePanel("open", Application.dataPath, ext);

        if (!string.IsNullOrEmpty(path))
        {
            
            return File.ReadAllText(path, Encoding.UTF8);
            //AssetDatabase.Refresh();

        }

        return "";
    }
    public Dictionary<string, object> LoadFromJson()
    {
        var d = LoadText("json");
        if (string.IsNullOrEmpty(d))
        {
            return new Dictionary<string, object>();
        }

        //var list = new List<Dictionary<string, object>>();
        var json = MiniJSON.Json.Deserialize(d) as Dictionary<string,object>;
        // foreach (var o in json)
        // {
        //     var dic = (IDictionary)o;
        //     var ret = new Dictionary<string, object>();
        //     foreach (DictionaryEntry o1 in dic)
        //     {
        //         ret.Add(o1.Key.ToString(),o1.Value);
        //     }
        //     list.Add(ret);
        // }
        //Debug.Log(list);
        return json;
    }
    void OpenBmp()
    {
        var target = EditorUtility.OpenFilePanel("PNG", "Assets","png");
        if (string.IsNullOrEmpty(target))
        {
            return;
        }

        var png = ReadPngFile(target);
        var tex = ConvertPngFile(png);
        var pixels32 = tex.GetPixels32();
        var pixels = tex.GetPixels();
        for (var i = 0; i < pixels32.Length; i++)
        {
            if (pixels32[i].a == 0)
            {
                pixels[i] = Color.white;
                pixels[i].a = 1;
            }
            else
            {
                pixels[i] = Color.black;
            }
        }
        tex.SetPixels(pixels);
        tex.filterMode = FilterMode.Point;
        _texture = tex;
    }
    Texture2D ConvertPngFile(byte[] readBinary)
    {

        int pos = 16; // 16バイトから開始

        int width = 0;
        for (int i = 0; i < 4; i++)
        {
            width = width * 256 + readBinary[pos++];
            Debug.Log(width);
        }

        int height = 0;
        for (int i = 0; i < 4; i++)
        {
            height = height * 256 + readBinary[pos++];
        }
        Debug.Log(width +","+ height);
        Texture2D texture = new Texture2D(width, height);
        texture.LoadImage(readBinary);

        return texture;
    }

    byte[] ReadPngFile(string path){
        FileStream fileStream = new FileStream(path, FileMode.Open, FileAccess.Read);
        BinaryReader bin = new BinaryReader(fileStream);
        byte[] values = bin.ReadBytes((int)bin.BaseStream.Length);

        bin.Close();

        return values;
    }
}

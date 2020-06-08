using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;
using NUnit.Framework;
using UnityEditor;
using UnityEngine;

public class PngToRgs : EditorWindow
{
    [MenuItem("Toast/PngToRgs")]
    static void Open()
    {
        var window = CreateInstance<PngToRgs>();
        window.Show();
    }

    private string _saveDist = "";
    private string _target = "";
    private void OnGUI()
    {
        
        if (GUILayout.Button("SetSavePath"))
        {
            _saveDist = EditorUtility.OpenFolderPanel("保存先", "Assets","");
        }

        if (_saveDist == "")
        {
            return;
        }
        GUILayout.Label("SaveTo : "+  _saveDist);
        if (GUILayout.Button("Select PNG"))
        {
            _target = EditorUtility.OpenFilePanel("PNG", "Assets","png");
        }

        if (_target != null)
        {
            if (GUILayout.Button("Save"))
            {
                var fileName = Path.GetFileNameWithoutExtension(_target) + ".asset";
                SaveToRgs(ConvertPngFile(ReadPngFile(_target)),_saveDist + "/"+fileName);
            }
        }

    }
    byte[] ReadPngFile(string path){
        FileStream fileStream = new FileStream(path, FileMode.Open, FileAccess.Read);
        BinaryReader bin = new BinaryReader(fileStream);
        byte[] values = bin.ReadBytes((int)bin.BaseStream.Length);

        bin.Close();

        return values;
    }

    Texture2D ConvertPngFile(byte[] readBinary)
    {

        int pos = 16; // 16バイトから開始

        int width = 0;
        for (int i = 0; i < 4; i++)
        {
            width = width * 256 + readBinary[pos++];
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

    byte RGBAToGray(Color32 col)
    {
        if (col.a == 0)
        {
            return 255;
        }

        return (byte)(.2 * col.r + .7 * col.g + .7 * col.b);
    }
    void SaveToRgs(Texture2D tex,string path)
    {
        var dic = new List<byte>();
        var value = (byte)0;
        var bytes = new List<byte>();
        var stride = tex.width;
        foreach (var pixel in tex.GetPixels32())
        {
            var index = RGBAToGray(pixel);
            if (dic.Contains(index))
            {
                //bytes.Add(dic[index]);
            }
            else
            {
                dic.Add(index);
                //bytes.Add(value);
                value++;
            }
        }
        Debug.Log("colors : "+ dic.Count);
        dic = dic.OrderBy(_=>_).ToList();
        foreach (var pixel in tex.GetPixels32())
        {
            var index = RGBAToGray(pixel);
            if (index == 255)
            {
                bytes.Add(255);//透過対応
            }
            else
            {

                var data = dic.IndexOf(index);
                bytes.Add((byte) data);
            }
        }
        var rgs = ScriptableObject.CreateInstance<RetroGraphicsSprite>();
        rgs.Buffer = bytes.ToArray();
        rgs.Stride = stride;
        var path2 = Regex.Replace(path,@"^.*?Assets/","Assets/");
        AssetDatabase.CreateAsset(rgs, path2);
        AssetDatabase.Refresh();
        
    }
}

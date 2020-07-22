using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum FontType
{
    Number,
    Symbol,
    English,//12x8
    Japanese_Hira,//12x12
    Japanese_Kata,//12x12
}
[Serializable]
public class FontRect
{
    public int sX;
    public int sY;
    public int eX;
    public int eY;
}
[Serializable]
public class FontSet
{
    public char Key;
    public FontRect Rect;
}
public class RetroGraphicsFont : ScriptableObject
{
    //public FontType Type;
    public bool[] Buffer;
    public int Stride;
    public FontSet[] Glyphs;

    public DeployRetroGraphicsFont Deploy()
    {
        var ret = new Dictionary<char,FontRect>();
        foreach (var fontSet in Glyphs)
        {
            if (ret.ContainsKey(fontSet.Key)) continue;
            ret.Add(fontSet.Key,fontSet.Rect);
        }
        return new DeployRetroGraphicsFont(Stride,Buffer,ret);
    }
}

public class DeployRetroGraphicsFont
{
    private Dictionary<char, FontRect> _glyph;
    private bool[] _buffer;
    private int _stride;
    private int _ymax;
    public DeployRetroGraphicsFont(int Stride ,bool[] Buffer, Dictionary<char, FontRect> Glyph)
    {
        _stride = Stride;
        _glyph = Glyph;
        _buffer = Buffer;
        //反転用
        _ymax = _buffer.Length / _stride;
    }

    public (int stx, int sty) TryGetBuffer(char key,out bool[] data)
    {
        data = null;
        if (_glyph.ContainsKey(key))
        {
            var g = _glyph[key];
            data = new bool[(g.eX - g.sX) * (g.eY - g.sY)];
            for (int i = 0; i < (g.eX - g.sX) * (g.eY - g.sY); i++)
            {
                var inverseY = _stride * (_ymax - g.eY); 
                var ind = g.sX + inverseY + (i / (g.eX - g.sX)) * _stride + i % (g.eX - g.sX);
                data[i] = _buffer[ind];
            }
            return ((g.eX - g.sX),(g.eY - g.sY));
        }

        return (-1, -1);
    }
}
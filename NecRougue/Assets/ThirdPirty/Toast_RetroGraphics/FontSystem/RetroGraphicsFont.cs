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

    public Dictionary<char, FontRect> Deploy()
    {
        return null;
    }
}
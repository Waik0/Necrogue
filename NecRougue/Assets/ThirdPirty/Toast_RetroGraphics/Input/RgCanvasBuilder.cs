using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq.Expressions;
using UnityEngine;

public abstract class IRetroGraphicsUIElement
{
    public RetroGraphicsUIAlign Align;
    public Vector2Int Position;
    public Vector2Int Size;
}

public class RgTextUI : IRetroGraphicsUIElement
{
    public string Text;
    public int WaitFrame = 0;
}
public class RgButtonUI : IRetroGraphicsUIElement
{
    public string Text;
    public int WaitFrame = 0;
    public Action OnClick;
}
public enum  RetroGraphicsUIAlign
{
    Left,
    Right,
}
public class RgCanvasKey
{
    public string Key;
    public IRetroGraphicsUIElement Element => RetroGraphicsUIBuilder.GetElement(this);
    public override int GetHashCode()
    {
        return Key.GetHashCode();
    }

    public override bool Equals(object obj)
    {
        return Key.Equals(obj);
    }
}
public static class RetroGraphicsUIBuilder
{

    private static int _hash = 0;
    private static Dictionary<RgCanvasKey, IRetroGraphicsUIElement> _elements = new Dictionary<RgCanvasKey, IRetroGraphicsUIElement>();
    public static Dictionary<RgCanvasKey, IRetroGraphicsUIElement> Elements => _elements;
    public static bool IsDirty => _dirty;
    private static bool _dirty;
    public static IRetroGraphicsUIElement GetElement(RgCanvasKey key)
    {
        if (_elements.ContainsKey(key))
        {
            return _elements[key];
        }
        return null;
    }

    public static RgCanvasKey AddElement(IRetroGraphicsUIElement element)
    {
        Debug.Log(_hash);
        _hash++;
        var key = new RgCanvasKey()
        {
            Key = _hash.ToString()
        };
        _elements.Add(key , element);
        return key;
    }
    public static void Clear()
    {
        _elements.Clear();
        _dirty = true;
    }
    
}

using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class RetroGraphicsUI : MonoBehaviour
{
    [SerializeField] private RetroScreenRenderer _renderer;
    private static RetroGraphicsUI _instance;
    //Singleton
    public static RetroGraphicsUI Instance
    {
        get {
            if (_instance == null)
            {
                _instance = GameObject.FindObjectOfType<RetroGraphicsUI>();
            }
            return _instance;
        }
    }

    void BuildUI()
    {
        _renderer.SetClearBufferOnce(true,RetroScreenRenderer.LayerList.UI);
        foreach (var retroGraphicsUiElement in RetroGraphicsUIBuilder.Elements)
        {
            Vector2Int s = Vector2Int.zero;
            Vector2Int e = Vector2Int.zero;
            switch (retroGraphicsUiElement.Value.Align)
            {
                case RetroGraphicsUIAlign.Left:
                    s = retroGraphicsUiElement.Value.Position;
                    e = retroGraphicsUiElement.Value.Position + retroGraphicsUiElement.Value.Size;
                  
                    break;
                case RetroGraphicsUIAlign.Right:
                    s = new Vector2Int(_renderer.Width,_renderer.Height) - (retroGraphicsUiElement.Value.Position + retroGraphicsUiElement.Value.Size);
                    e = new Vector2Int(_renderer.Width,_renderer.Height) - (retroGraphicsUiElement.Value.Position);
                    break;
            }
            DrawRect(s,e);
        }
    }

    private void LateUpdate()
    {
        
    }

    private int _stride = 4;
    private byte[] _bytes = new byte[]
    {
        3,0,0,0,
        0,3,3,3,
        0,3,0,0,
        0,3,0,3,
    };
    void DrawRect(Vector2Int s,Vector2Int e)
    {
        for (int x = s.x; x < e.x; x++)
        {
            for (int y = s.y; x < e.y; y++)
            {
                int ix = Mathf.Min(x,_stride,e.x - x);
                int iy =  Mathf.Min(y,_bytes.Length/_stride,e.y - y);
                _renderer.DrawPixel(x,y,_bytes[ix + iy * _stride],RetroScreenRenderer.LayerList.UI);
            }
        }
    }
}

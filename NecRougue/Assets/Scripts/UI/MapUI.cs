using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MapUI : MonoBehaviour, IModalUI
{
    private MapDataUseCase _mapRef;
    private int _selectedMap;
    public void Inject(MapDataUseCase map)
    {
        _mapRef = map;
        _selectedMap = -1;
        //UI初期化
    }

    public int GetSelectedMap()
    {
        return _selectedMap;
    }

    public void ResetUI()
    {
        _selectedMap = -1;
    }

    public bool UpdateUI()
    {
        if (_mapRef == null)
        {
            return false;
        }

        return _selectedMap != -1;
    }
#if DEBUG

    public void DebugUI()
    {
        if (_mapRef == null)
        {
            return;
        }

        var nowDepth = _mapRef.CurrentDepth();
        var mapDepth = -1;
        var first = true;
        GUILayout.BeginVertical();
        foreach (var mapRefNode in _mapRef.Data.Nodes)
        {
            
            if (mapDepth != mapRefNode.Depth)
            {
                if (!first)
                {
                 GUILayout.EndHorizontal();   
                }

                first = false;
                mapDepth = mapRefNode.Depth;
                GUILayout.BeginHorizontal();
            }

            string text = mapRefNode.NodeId + " : \n" + mapRefNode.MapType.ToString();
            if (mapRefNode.Depth == nowDepth)
            {
                if (GUILayout.Button(text,GUILayout.Width(100),GUILayout.Height(100)))
                {
                    _selectedMap = mapRefNode.SerialNumber;
                }
            }
            else
            {
                GUILayout.Label(text,GUILayout.Width(100));
            }
        }
        GUILayout.EndVertical();

    }
#endif
}

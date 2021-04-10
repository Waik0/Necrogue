using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using Zenject;

public class CursorViewPresenter :IInitializable
{

    private Cursor _prefab;
    public Dictionary<ulong, Cursor> CursorDatas { get; } = new Dictionary<ulong, Cursor>();

    public CursorViewPresenter(
        Cursor cursorPrefab)
    {
        _prefab = cursorPrefab;
    }
    public void SetCursorPos(CursorData cursorData)
    {
        if (CursorDatas.ContainsKey(cursorData.id))
        {
            CursorDatas[cursorData.id]?.SetPos(cursorData);
        }
        else
        {
            CursorDatas.Add(cursorData.id, Object.Instantiate(_prefab));
            //CursorDatas[cursorData.id].SetName(cursorData.id);
            CursorDatas[cursorData.id].SetPos(cursorData);  
        }
    }

    public void Initialize()
    {
        foreach (var s in CursorDatas.Keys.ToList())
        {
            Object.Destroy(CursorDatas[s].gameObject);
        }
        CursorDatas.Clear();
    }
}

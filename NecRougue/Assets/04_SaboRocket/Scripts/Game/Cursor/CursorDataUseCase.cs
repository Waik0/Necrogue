using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class CursorDataUseCase
{

    private Cursor _prefab;
    public Dictionary<string, Cursor> CursorDatas { get; } = new Dictionary<string, Cursor>();

    public CursorDataUseCase(
        Cursor cursorPrefab)
    {
        _prefab = cursorPrefab;
    }
    public void Init()
    {
        foreach (var s in CursorDatas.Keys.ToList())
        {
            Object.Destroy(CursorDatas[s]);
        }
        CursorDatas.Clear();
    }
    public void SetCursorPos( CursorData cursorData)
    {
        if (CursorDatas.ContainsKey(cursorData.id))
        {
            CursorDatas[cursorData.id]?.SetPos(cursorData);
        }
        else
        {
            CursorDatas.Add(cursorData.id, Object.Instantiate(_prefab));
            CursorDatas[cursorData.id].SetName(cursorData.id);
            CursorDatas[cursorData.id].SetPos(cursorData);  
        }
    }
}

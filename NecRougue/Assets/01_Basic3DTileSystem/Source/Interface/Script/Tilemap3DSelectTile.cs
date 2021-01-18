using System;
using System.Collections;
using System.Collections.Generic;
using Basic3DTileSystem.Source.Core.Script;
using UnityEngine;


public class Tilemap3DSelectTile
{
    public Action<int> OnSelectTile = null;

    private int _currentSelectIndex = -1;
    public bool CanPlace => _currentSelectIndex >= 0;
    public void SelectTile(int index)
    {
        _currentSelectIndex = index;
        OnSelectTile?.Invoke(index);
    }

    public int GetIndex() => _currentSelectIndex;
    public void ClearIndex() => _currentSelectIndex = -1;

}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BuildElementModel<T> where T : struct
{
    public int Id;
    public T Type;
}
public class BuildFieldModel<T> where T : struct
{
    public BuildElementModel<T>[,] Map { get; private set; }
    public BuildFieldModel(int w,int h)
    {
        Map = new BuildElementModel<T>[w, h];
    }
}

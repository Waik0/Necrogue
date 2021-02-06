using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class FloorFieldModel<T> where T : struct
{
    public T[,] Map { get; private set; }
    public FloorFieldModel(int w,int h)
    {
        Map = new T[w, h];
    }
}
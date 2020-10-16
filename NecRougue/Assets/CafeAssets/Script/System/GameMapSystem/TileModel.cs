using System;
using System.Collections;
using System.Collections.Generic;
using UnityEditor.VersionControl;
using UnityEngine;
[CreateAssetMenu(fileName = "Tile_0000",menuName = "ScriptableObject/TileModel")]
public class TileModel : IsometricRuleTile
{
    public int MinLayer;
    public int MaxLayer;
    public int Price;
    public NeedParameterModelSet[] NeedParameter;
    public ProvideParameterModelSet[] ProvideParameter;
}
[Serializable]
public class NeedParameterModelSet
{
    public string Name;
    public int Num;
}
[Serializable]
public class ProvideParameterModelSet
{
    public string Name;
    public GameParameterOperations Operation;
    public int Num;
}
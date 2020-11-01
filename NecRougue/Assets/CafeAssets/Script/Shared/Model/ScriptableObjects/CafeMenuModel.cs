using System.Collections;
using System.Collections.Generic;
using CafeAssets.Script.Model;
using CafeAssets.Script.System.GameMapSystem;
using UnityEngine;
[CreateAssetMenu(fileName = "Menu_0000",menuName = "ScriptableObject/MenuModel")]
public class CafeMenuModel : ScriptableObject
{
    public RegionTextSet Name;
    public RegionTextSet Description;
    public int Price;
    public NeedParameterModelSet[] NeedParameter;
    public ProvideParameterModelSet[] ProvideParameter;
}

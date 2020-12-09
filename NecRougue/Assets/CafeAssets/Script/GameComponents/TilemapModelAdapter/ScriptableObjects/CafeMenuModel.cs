using CafeAssets.Script.GameComponents.Tilemap;
using CafeAssets.Script.GameComponents.TilemapAdapter.ScriptableObjects;
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

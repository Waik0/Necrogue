﻿

//いくつかの小物は重ね置き可能

using UnityEngine;

namespace CafeAssets.Script.System.GameMapSystem.TileInheritance
{
    [CreateAssetMenu(fileName = "GoodsTile_0000",menuName = "ScriptableObject/GoodsTileModel")]
    public class GoodsTileModel : EffectiveTileModel
    {
        [Header("重ね置き設定(同一グループのみ重ね置き可能)")]
        public int[] OverrideGroup = new []{0};
        
    }
}

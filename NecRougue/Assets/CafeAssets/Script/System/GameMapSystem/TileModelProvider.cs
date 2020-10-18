using System.Collections;
using System.Collections.Generic;
using CafeAssets.Script.System.GameMapSystem;
using UnityEngine;
using Zenject;
[CreateAssetMenu(fileName = "TileModelProvider",menuName = "ScriptableObject/TileModelProvider")]
public class TileModelProvider : ScriptableObjectInstaller
{
    public BasicTileModel[] BasicTileModels;
    public EffectiveTileModel[] EffectiveTileModels;

    public override void InstallBindings()
    {
        Container.BindInstance(this);
    }
}

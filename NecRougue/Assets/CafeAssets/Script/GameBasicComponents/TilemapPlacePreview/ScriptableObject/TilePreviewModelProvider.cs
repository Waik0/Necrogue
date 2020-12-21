using System.Collections;
using System.Collections.Generic;
using CafeAssets.Script.GameComponents.TilemapPlacePreview.ScriptableObject;
using UnityEngine;
using Zenject;

[CreateAssetMenu(fileName = "TileModelProvider",menuName = "ScriptableObject/TilePreviewModelProvider")]
public class TilePreviewModelProvider : ScriptableObjectInstaller
{
    public TilePreviewModel Allow;
    public TilePreviewModel Deny;
    public override void InstallBindings()
    {
        Container.BindInstance(this);
    }
}
using System;
using System.Collections;
using System.Collections.Generic;
using CafeAssets.Script.Impliment.Game.Layer_04.View;
using CafeAssets.Script.System.GameCameraSystem;
using CafeAssets.Script.System.GameInputSystem;
using CafeAssets.Script.System.GameMapSystem;
using CafeAssets.Script.System.GameNpcSystem;
using CafeAssets.Script.System.GameTimeSystem;
using UnityEngine;
using Zenject;

/// <summary>
/// パラメーター系のデバッグ表示
/// </summary>
public class ParamDebugInstaller : MonoInstaller
{
    [Serializable]
    public class Settings
    {
        public DebugView Debug;
    }

    [SerializeField] private Settings _settings;

    public override void InstallBindings()
    {
                
        //Debug
        //todo 別Installerに分けてdefine切り分けしてReleaseに入らないように
        Container.BindInterfacesTo<DebugView>().FromComponentInNewPrefab(_settings.Debug).AsCached().NonLazy();
    }
}

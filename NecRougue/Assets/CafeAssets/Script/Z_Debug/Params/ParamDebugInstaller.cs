using System;
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

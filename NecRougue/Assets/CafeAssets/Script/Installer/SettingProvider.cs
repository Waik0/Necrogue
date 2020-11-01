using System;
using UnityEngine;
using Zenject;

namespace CafeAssets.Script.SceneInstaller
{
    [CreateAssetMenu(menuName = "ScriptableObject/Setting")]
    public class SettingProvider :  ScriptableObjectInstaller<SettingProvider>
    {
        [Header("GameSetting")]
        [SerializeField] private GameSceneInstaller.Settings _settings;
        public override void InstallBindings()
        {
            Container.BindInstance(_settings).IfNotBound();
        }
    }

}

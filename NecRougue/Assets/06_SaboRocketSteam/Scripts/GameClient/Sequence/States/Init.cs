using System.Collections;
using UnityEngine;
using Zenject;

namespace SaboRocketSteam.Scripts.GameClient.Sequence.States
{
    public class Init : State
    {
        [Inject] private BridgeProperty _bridgeProperty;
        [Inject] private SceneProgress _sceneProgress;
        public override void OnEnterState()
        {
            Debug.Log("Enter");
        }

        public override IEnumerator Update()
        {
            _bridgeProperty.IsHost = true;
            if (_bridgeProperty.IsHost)
            {
                yield return _sceneProgress.LoadSceneAdditiveAsync("SteamHost",_bridgeProperty);
            }
            Debug.Log("Initialize");
            yield return null;
            Exit<WaitReadyAll>();
        }

        public override void OnExitState()
        {
            Debug.Log("Exit");
        }
    }
}
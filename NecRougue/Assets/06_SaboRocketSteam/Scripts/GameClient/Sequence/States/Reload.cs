using System.Collections;
using UnityEngine.SceneManagement;
using Zenject;

namespace SaboRocketSteam.Scripts.GameClient.Sequence.States
{
    public class Reload : State
    {
        [Inject] private SceneProgress _sceneProgress;
        [Inject] private BridgeProperty _bridgeProperty;
        public override void OnEnterState()
        {
            _sceneProgress.LoadScene("SteamClient",_bridgeProperty);
        }

        public override IEnumerator Update()
        {
            yield return null;
        }
    }
}

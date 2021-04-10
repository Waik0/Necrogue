using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Zenject;

namespace SaboRocketSteam.Scripts.GameHost.States
{ 
    public class WaitReady : State
    {
        [Inject] private ReadyStateCheckerInteractor _readyStateChecker;
        public override void OnEnterState()
        {
            Debug.Log("Enter WaitReady");
        }

        public override IEnumerator Update()
        {
            Debug.Log("Update WaitReady");
            while (!_readyStateChecker.IsReadyAll())
            {
                yield return null;
            }
            Exit<WaitInput>();
            yield break;
        }

        public override void OnExitState()
        {
            Debug.Log("Exit");
        }
    }
}
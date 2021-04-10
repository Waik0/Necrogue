using System.Collections;
using System.Runtime.InteropServices;
using SaboRocketSteam.Scripts.GameHost.Physics;
using UnityEngine;
using Zenject;

namespace SaboRocketSteam.Scripts.GameHost.States
{
    public class Init : State
    {
        [Inject] private PiecePhysicsUseCase _physicsUseCase;
        [Inject] private ReadyStateCheckerInteractor _readyStateChecker;
        [Inject] private BridgeProperty _bridgeProperty;
        public override void OnEnterState()
        { 
            Debug.Log("Enter");
        }

        public override IEnumerator Update()
        {
            Debug.Log("Initialize");
            _physicsUseCase.Init();
            _readyStateChecker.Init();
            yield return null;
            Exit<WaitReady>();
        }

        public override void OnExitState()
        {
            Debug.Log("Exit");
        }
    }
}
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
        public override void OnEnterState()
        { 
            Debug.Log("Enter");
        }

        public override IEnumerator Update()
        {
            Debug.Log("Initialize");
            _physicsUseCase.Init();
            Exit<WaitReady>();
            yield break;
        }

        public override void OnExitState()
        {
            Debug.Log("Exit");
        }
    }
}
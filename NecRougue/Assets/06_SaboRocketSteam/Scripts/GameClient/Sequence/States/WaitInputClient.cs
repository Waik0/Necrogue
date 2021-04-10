using System.Collections;
using System.Collections.Generic;
using SaboRocketSteam.Scripts.GameHost.Physics;
using SaboRocketSteam.Scripts.GameHost.States;
using UnityEngine;
using Zenject;

namespace SaboRocketSteam.Scripts.GameClient.Sequence.States
{
    public class WaitInputClient : State
    {
        public override void OnEnterState()
        { 
            Debug.Log("Enter");
        }

        public override IEnumerator Update()
        {

            yield return null;
        }

        public override void OnExitState()
        {
            Debug.Log("Exit");
        }
    }
}
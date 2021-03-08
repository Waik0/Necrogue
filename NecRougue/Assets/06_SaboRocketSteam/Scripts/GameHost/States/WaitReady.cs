using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace SaboRocketSteam.Scripts.GameHost.States
{ 
    public class WaitReady : State
    {
        public override void OnEnterState()
        {
            Debug.Log("Enter WaitReady");
        }

        public override IEnumerator Update()
        {
            Debug.Log("Update WaitReady");
            yield break;
        }

        public override void OnExitState()
        {
            Debug.Log("Exit");
        }
    }
}
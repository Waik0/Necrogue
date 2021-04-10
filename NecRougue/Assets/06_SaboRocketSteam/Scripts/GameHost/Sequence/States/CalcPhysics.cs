using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace SaboRocketSteam.Scripts.GameHost.States
{
    public class CalcPhysics : State
    {
        public override void OnEnterState()
        { 
            Debug.Log("Enter");
        }

        public override IEnumerator Update()
        {
            Debug.Log("Update");
            yield break;
        }

        public override void OnExitState()
        {
            Debug.Log("Exit");
        }
    }
}
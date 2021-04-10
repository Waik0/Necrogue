using System.Collections;
using UnityEngine;

namespace SaboRocketSteam.Scripts.GameClient.Sequence.States
{
    public class WaitReadyAll : State
    {
        
        public override void OnEnterState()
        { 
            Debug.Log("Enter");
        }

        public override IEnumerator Update()
        {
            Debug.Log("Initialize");
            yield return null;

        }

        public override void OnExitState()
        {
            Debug.Log("Exit");
        }
    }
}
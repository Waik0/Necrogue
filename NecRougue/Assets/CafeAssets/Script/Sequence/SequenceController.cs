using System;
using UnityEngine;
using Zenject;

namespace CafeAssets.Script.Sequence
{
    public class SequenceController : MonoBehaviour
    {
        [Inject] private RootSequence _rootSequence;

        private void Update()
        {
            _rootSequence?.UpdateState();
        }
    }
}

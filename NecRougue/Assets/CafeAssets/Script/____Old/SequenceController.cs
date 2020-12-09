using System;
using UnityEngine;
using Zenject;

namespace CafeAssets.Script.Sequence
{
    /// <summary>
    /// Injectされないので注意
    /// </summary>
    public class SequenceController : MonoBehaviour
    {
        [SerializeField]
        private SequenceRoot _rootSequence;

        private void Awake()
        {
            DontDestroyOnLoad(this.gameObject);
        }
        private void Update()
        {
            _rootSequence?.UpdateState();
        }
    }
}

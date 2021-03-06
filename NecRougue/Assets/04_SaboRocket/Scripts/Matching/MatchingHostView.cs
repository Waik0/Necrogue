using System;
using System.Collections;
using System.Collections.Generic;
using UniRx;
using UnityEngine;
using UnityEngine.UI;
using Zenject;

namespace Toast.RealTimeCommunication
{

    public class MatchingHostView : MonoBehaviour
    {
        [SerializeField] private Transform _viewRoot;
        [SerializeField] private Text _roomName;
        [SerializeField] private Text _peers;
        private MatchingHostSequence _matchingSequence;
        private ITortecHostUseCaseWithWebSocket _tortecHost;

        [Inject]
        void Inject(
            MatchingHostSequence matchingSequence,
            ITortecHostUseCaseWithWebSocket tortecHost
        )
        {
            _tortecHost = tortecHost;
            _matchingSequence = matchingSequence;
            _matchingSequence.OnActiveSequence.Subscribe(OnChangeActive).AddTo(this);
            matchingSequence.OnCreateRoom.Subscribe(OnCreateRoom).AddTo(this);
            _viewRoot.gameObject.SetActive(false);
        }

        void OnChangeActive(bool active)
        {
            _viewRoot.gameObject.SetActive(active);
        }

        void OnCreateRoom(string name)
        {
            _roomName.text = name;
        }

        private void Update()
        {
       
        }
    }
}
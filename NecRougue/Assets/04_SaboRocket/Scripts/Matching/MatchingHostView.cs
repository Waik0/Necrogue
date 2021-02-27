using System;
using System.Collections;
using System.Collections.Generic;
using UniRx;
using UnityEngine;
using UnityEngine.UI;
using Zenject;

public class MatchingHostView : MonoBehaviour
{
    [SerializeField] private Transform _viewRoot;
    [SerializeField] private Text _roomName;
    [SerializeField] private Text _peers;
    private MatchingHostSequence _matchingSequence;
    private ITortecHostUseCase _tortecHost;
    [Inject]
    void Inject(
        MatchingHostSequence matchingSequence,
        ITortecHostUseCase tortecHost
        )
    {
        _tortecHost = tortecHost;
        _matchingSequence = matchingSequence;
        _matchingSequence.OnActiveSequence.Subscribe(OnChangeActive).AddTo(this);
        tortecHost.OnCreateRoom.Subscribe(OnCreateRoom).AddTo(this);
        _viewRoot.gameObject.SetActive(false);
    }

    public void EndMatching()
    {
        _matchingSequence.ToGame();
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
        _peers.text = "";
        foreach (var rtcPeerConnection in _tortecHost.ConnectionPeers())
        {
            _peers.text += rtcPeerConnection.ToString() + "\n";
        }
    }
}

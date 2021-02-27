using System;
using System.Collections;
using System.Collections.Generic;
using Toast.RealTimeCommunication;
using UnityEngine;
using UnityEngine.UI;

[Serializable]
public class TestChannel
{
    public int a;
}
[RequireComponent(typeof(TortecHostUseCase))]
public class NetworkHostInfoView : MonoBehaviour
{
    private TortecHostUseCase _peer;
    [SerializeField] private Text _linkPeerList;
    [SerializeField] private Text _metaList;
    [SerializeField] private Text _channels;
    private void Awake()
    {
        _peer = GetComponent<TortecHostUseCase>();
    }
    public void CreateRoom()
    {
        _peer.CreateRoom();
    }
    public void OpenChannel()
    {
        _peer.OpenChannelAll<TestChannel>();
    }
    void ViewUpdate()
    {
        _linkPeerList.text = "Peer : \n";
        _metaList.text = "Meta : \n";
        _channels.text = "Channels : \n";
        foreach (var metaDataModel in _peer.PeersMeta())
        {
            _metaList.text += metaDataModel.Value?.connectionState + "\n";
        }
        foreach (var connectionId in _peer.ConnectionIds())
        {
            _linkPeerList.text += connectionId + "\n";
        }
        foreach (var rtcDataChannel in _peer.Channels())
        {
            _channels.text += rtcDataChannel.Label+":"+ rtcDataChannel.ReadyState.ToString() + "\n";
        }
    }

    void FixedUpdate()
    {
        ViewUpdate();
    }
}

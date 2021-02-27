using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
[RequireComponent(typeof(TortecClientUseCase))]
public class NetworkClientInfoView : MonoBehaviour
{
    private TortecClientUseCase _peer;
    [SerializeField] private Text _linkPeerList;
    [SerializeField] private Text _metaList;
    [SerializeField] private Text _channels;
    [SerializeField] private InputField _inputField;
    private void Awake()
    {
        _peer = GetComponent<TortecClientUseCase>();
    }
    public void JoinRoom()
    {
        _peer.JoinRoom(_inputField.text);
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

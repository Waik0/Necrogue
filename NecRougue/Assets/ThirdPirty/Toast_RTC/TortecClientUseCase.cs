using System;
using System.Collections;
using System.Collections.Generic;
using Toast.RealTimeCommunication;
using UniRx;
using Unity.WebRTC;
using UnityEngine;
public interface ITortecClientUseCase
{
    string SelfId { get; }
    IObservable<string> OnJoinRoom { get; }
    void JoinRoom(string room);
    void OpenChannelAll<T>() where T : class;
    bool CheckAndOpenChannelAll<T>() where T : class;
    void SubscribeMessage<T>(GameObject owner, Action<string,T> res) where T : class;
    void BroadcastAll<T>(T data) where T : class;
    List<string> ConnectionIds();
    List<RTCPeerConnection> ConnectionPeers();
    Dictionary<RTCPeerConnection, MetaDataModel> PeersMeta();
}
public class TortecClientUseCase :  TortecUseCaseBase,ITortecClientUseCase
{
    private Subject<string> _onJoinRoom = new Subject<string>();
    public IObservable<string> OnJoinRoom => _onJoinRoom;
    private TORTECClient _tortecClient;
    protected override TRTCPeer Peer => _tortecClient;

    public void JoinRoom(string room)
    {
        _tortecClient.JoinRoom(room);
    }
    
    private void Awake()
    {
        gameObject.AddComponent<TORTECClient>();
        _tortecClient = GetComponent<TORTECClient>();
        _tortecClient.OnMessage = OnMessage;
    }
}

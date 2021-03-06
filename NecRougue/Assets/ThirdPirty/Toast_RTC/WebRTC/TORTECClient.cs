using System;
using System.Collections;
using System.Collections.Generic;
using UniRx;
using Unity.WebRTC;
using UnityEngine;
using UnityEngine.UI;

namespace Toast.RealTimeCommunication
{
    public class TORTECClient : TRTCPeer
    {
        // //debug
        // [SerializeField] private Text _peers;
        // private void FixedUpdate()
        // {
        //     _peers.text = "";
        //     _peers.text = _currentMeta?.connectionState.ToString() ?? "null";
        //     _peers.text += "\nSignalling : " + IsConnectingSignalingServer() + "\n";
        //     foreach (var rtcPeerConnection in _peerConnections)
        //     {
        //         _peers.text += rtcPeerConnection.Key + "\n";
        //     }
        // }
        
        
        private string _roomIdCache;
        public Action<string> OnError;
        public Action OnDisconnect;
        public IDisposable _retry;
        public void JoinRoom(string roomName)
        {
            _metaCache.Clear();
            _roomIdCache = roomName;
            StartSignaling(roomName);
          
        }

        public override void EndSignaling()
        {
            base.EndSignaling();
            _retry?.Dispose();
        }

        private void ReJoinRoomDelay(float retryTime)
        {
            _retry = Observable.Timer(TimeSpan.FromSeconds(retryTime)).Subscribe(_=>{}, () =>
            {
                JoinRoom(_roomIdCache);
                Debug.Log("ReJoin");
            });
        }
        //接続完了
        protected override void OnConnected(RTCPeerConnection connection,RTCDataChannel channel)
        {
            Debug.Log("connect "+ channel.Label);
            if (channel.Label == "meta")
            {
                EndSignaling();
                SendMetaMessageAsync(channel, new MetaDataModel()
                {
                    isHost = false,
                    needMeta = true,
                });
            }
        }
        protected override void OnGetMeta(RTCDataChannel channel,MetaDataModel meta)
        {
            var peer = GetConnectionFromChannel(channel);
            if (!_metaCache.ContainsKey(peer))
            {
                _metaCache.Add(peer,meta);
            }

            if (meta.isHost && !string.IsNullOrEmpty(meta.clientIdentifer))
            {
                //hostが割り振ってくれたID
                SelfId = meta.clientIdentifer;
                Debug.Log("I am " + SelfId);
            }

            _metaCache[peer] = meta;
            switch (meta.connectionState)
            {
                case MetaDataModel.ConnectionState.Success:
                    Debug.Log("Client Connect to host");
                    break;//正常終了
                case MetaDataModel.ConnectionState.Reconnect:
                    Debug.Log("Client Connect to Client Reconnect");
                    ReJoinRoomDelay(2f);
                    return;
            }

            if (meta.needMeta || meta.connectionState == MetaDataModel.ConnectionState.None)
            {
                var echo = new MetaDataModel()
                {
                    isHost = false,
                    needMeta = meta.connectionState == MetaDataModel.ConnectionState.None
                };
                if (meta.isHost)
                {
                    Debug.Log("Client Send Success");
                    echo.connectionState = MetaDataModel.ConnectionState.Success;
                    SendMetaMessage(channel,echo);
                }
                else
                {
                    Debug.Log("Client Send Reconnect");
                    echo.connectionState = MetaDataModel.ConnectionState.Reconnect;
                    SendMetaMessage(channel,echo);
                    Stop();
                    ReJoinRoomDelay(1.5f);
                }
            }
        }

        protected override void OnDisconnected(RTCPeerConnection connection,RTCDataChannel channel)
        {
            Debug.Log("Disconnect");
            if (channel.Label == "meta" &&
                _metaCache != null &&
                _metaCache.ContainsKey(connection))
            {
                _metaCache.Remove(connection);
            }
            OnDisconnect?.Invoke();
        }

        protected override void OnCloseSocket(int code, string reason)
        {
            if (_metaCache.Count <= 1)
            {
                if (reason == "Conflict")
                {
                    Debug.Log("Client Conflict Reconnect");
                    ReJoinRoomDelay(1f);
                }
            }
        }
    }
}
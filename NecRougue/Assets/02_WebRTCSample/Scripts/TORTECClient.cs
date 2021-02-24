using System;
using System.Collections;
using System.Collections.Generic;
using UniRx;
using UnityEngine;
using UnityEngine.UI;

namespace Toast.RealTimeCommunication
{
    public class TORTECClient : TRTCPeer
    {
        //debug
        [SerializeField] private Text _peers;
        private void FixedUpdate()
        {
            _peers.text = "";
            _peers.text = _currentMeta?.connectionState.ToString() ?? "null";
            foreach (var rtcPeerConnection in _peerConnections)
            {
                _peers.text += rtcPeerConnection.Key + "\n";
            }
        }
        
        
        private string _roomIdCache;
        private MetaDataModel _currentMeta = null;
        public Action<string> OnError;
        public Action OnDisconnect;
        public void JoinRoom(string roomName)
        {
            _currentMeta = null;
            _roomIdCache = roomName;
            StartSignaling(roomName);
          
        }

        private void ReJoinRoomDelay(float retryTime)
        {
            _currentMeta = null;
            Observable.Timer(TimeSpan.FromSeconds(retryTime)).Subscribe(_=>{}, () =>
            {
                JoinRoom(_roomIdCache);
                Debug.Log("ReJoin");
            });
        }
        //接続完了
        protected override void OnConnected()
        {
            EndSignaling();
            SendMetaMessage(new MetaDataModel()
            {
                isHost = false
            });
        }

        protected override void OnMessage(string label,string message)
        {
            base.OnMessage(label,message);
            if (label == "meta")
            {
                var meta = JsonUtility.FromJson<MetaDataModel>(message);
                OnGetMeta(meta);
            }
        }

        private void OnGetMeta(MetaDataModel meta)
        {
            _currentMeta = meta;
            switch (meta.connectionState)
            {
                case MetaDataModel.ConnectionState.Success:
                    Debug.Log("Client Connect to host");
                    return;//正常終了
                case MetaDataModel.ConnectionState.Reconnect:
                    Debug.Log("Client Connect to Client Reconnect");
                    ReJoinRoomDelay(3f);
                    return;
            }

            var echo = new MetaDataModel()
            {
                isHost = false,
                needMeta =  true
            };
            if (meta.isHost)
            {
                Debug.Log("Client Send Success");
                echo.connectionState = MetaDataModel.ConnectionState.Success;
                SendMetaMessage(echo);
            }
            else
            {
                Debug.Log("Client Send Reconnect");
                echo.connectionState = MetaDataModel.ConnectionState.Reconnect;
                SendMetaMessage(echo);
                Stop();
                ReJoinRoomDelay(1.5f);
            }
        }

        protected override void OnDisconnected()
        {
            Debug.Log("Disconnect");
            if (_currentMeta == null)
            {
                OnError?.Invoke("JoinError");
                return;
            }
            OnDisconnect?.Invoke();
        }

        protected override void OnCloseSocket(int code, string reason)
        {
            if (_currentMeta == null)
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
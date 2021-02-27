using System;
using System.Collections;
using System.Collections.Generic;
using UniRx;
using Unity.WebRTC;
using UnityEngine;
using UnityEngine.UI;

namespace Toast.RealTimeCommunication
{
    /*
     TORTEC
     WebRTCベースで複数ピア間通信するための仕様(オリジナル)
     シグナリングはAyameLaboを使用することを想定
      接続の流れは
      //旧式
      1. Host-Client間でシグナリング (Connect)
      2. Clientが接続したらHostが部屋を指定しClient同士でシグナリング (Greeting)
      3. 接続があるたびに1~2を繰り返す。
      のようになる。
      接続が切れた場合、
      Host->残っているピアから無作為にホストを選択&残っているピアがピア情報を破棄
      Client->残っているピアがピア情報を破棄
      
      トラブルシューティング
      Q : 最初のシグナリングでクライアント同士が接続したらどうする？
      A : データチャンネルでお互いの情報を交換後、お互い何秒後にリトライするかを示し合わせて接続リトライする。
      Q : ホスト同士は？
      A : データチャンネ[ルでお互いの情報を交換後、isInitiator側はそのままリトライし、もう一方はエラーで処理する。
      
    */
    public class TORTECHost : TRTCPeer
    {
        //debug
        // [SerializeField] private Text _peers;
        // private void FixedUpdate()
        // {
        //     _peers.text = "";
        //     foreach (var metaDataModel in _metaCache)
        //     {
        //         _peers.text += metaDataModel.Value?.connectionState.ToString() ?? "null" ;
        //         _peers.text += "\n";
        //     }
        //
        //     _peers.text += "\nSignulling : " + IsConnectingSignalingServer() + "\n";
        //     foreach (var rtcPeerConnection in _peerConnections)
        //     {
        //         _peers.text += rtcPeerConnection.Key + "\n";
        //     }
        // }

        private string _roomIdCache;
        private bool _matchingContinue = false;
        public IDisposable _retry;
        //private MetaDataModel _currentMeta = null;
        public Action<string> OnError;
        public void CreateRoom(string roomName,bool clearCache = true)
        {
            SelfId = "HOST";
            if (clearCache)
            {
                _metaCache?.Clear();
            }
            _matchingContinue = true;
            _roomIdCache = roomName;
            var r = StartSignaling(roomName);
            Debug.Log(r);
        }
        public override void EndSignaling()
        {
            base.EndSignaling();
            _retry?.Dispose();
        }

        private void ReCreateRoomDelay(float retryTime)
        {
            _retry = Observable.Timer(TimeSpan.FromSeconds(retryTime)).Subscribe(_=>{}, () =>
            {
                CreateRoom(_roomIdCache,false);
                Debug.Log("ReCreate");
            });
        }
        //接続完了
        protected override void OnConnected(RTCPeerConnection connection,RTCDataChannel channel)
        {
            Debug.Log(connection);
            if (channel.Label == "meta")
            {
                EndSignaling();
                SendMetaMessageAsync(channel, new MetaDataModel()
                {
                    isHost = true,
                    needMeta = true,
                });
            }
        }


        protected override void OnGetMeta(RTCDataChannel channel,MetaDataModel meta)
        {
            var peer = GetConnectionFromChannel(channel);
            var cid = ChannelToConnectionId(channel);
            if (!_metaCache.ContainsKey(peer))
            {
                _metaCache?.Add(peer, meta);
            }
            _metaCache[peer] = meta;
            switch (meta.connectionState)
            {
                case MetaDataModel.ConnectionState.Success:
                    Debug.Log("Host Connect to Client");
                    if (_matchingContinue)
                    {
                        ReCreateRoomDelay(0.1f);
                    }
                    break;//正常終了
                case MetaDataModel.ConnectionState.Error: 
                case MetaDataModel.ConnectionState.Reconnect:
                    Debug.Log("Host Connect to Host Error");
                    HostConflictResolveLogic();
                    return;
            }

            if (meta.needMeta || meta.connectionState == MetaDataModel.ConnectionState.None)
            {
                var echo = new MetaDataModel()
                {
                    clientIdentifer = cid,
                    isHost = true,
                    needMeta =  meta.connectionState == MetaDataModel.ConnectionState.None
                };
                if (meta.isHost)
                {
                    Debug.Log("Host Send Error");
                    echo.connectionState = MetaDataModel.ConnectionState.Error;
                    SendMetaMessage(channel,echo);
                    HostConflictResolveLogic();

                }
                else
                {
                    Debug.Log("Host Send Success");
                    echo.connectionState = MetaDataModel.ConnectionState.Success;
                    SendMetaMessage(channel,echo);

                }
            }

            void HostConflictResolveLogic()
            {
                if (_metaCache == null || _metaCache.Count <= 1)
                {//まだホスト化してないほうが停止する
                    Stop();
                    _matchingContinue = false;
                    OnError?.Invoke("CreateError");
                } else if (_matchingContinue)
                {
                    ReCreateRoomDelay(0);
                }
            }
        }
        protected override void OnDisconnected(RTCPeerConnection connection, RTCDataChannel channel)
        {
            Debug.Log("Host Disconnected");
            if (channel.Label == "meta" &&
                _metaCache != null && 
                _metaCache.ContainsKey(connection))
            {
                _metaCache.Remove(connection);
            }
        }

        protected override void OnCloseSocket(int code, string reason)
        {
            if (reason == "Conflict")
            {
                Debug.Log("Host Conflict");
                ReCreateRoomDelay(0.5f);
                return;
            }
        }
    }
}
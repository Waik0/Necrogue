using System;
using System.Collections;
using System.Collections.Generic;
using UniRx;
using UnityEngine;
using UnityEngine.UI;

namespace Toast.RealTimeCommunication
{
    /*
     TORTEC
     WebRTCベースで複数ピア間通信するための仕様(オリジナル)
     シグナリングはAyameLaboを使用することを想定
      接続の流れは
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
        public void CreateRoom(string roomName)
        {
            _currentMeta = null;
            _roomIdCache = roomName;
            StartSignaling(roomName);
        }

        private void ReCreateRoomDelay(int retryTime)
        {
            _currentMeta = null;
            Observable.Timer(TimeSpan.FromSeconds(retryTime)).Subscribe(_=>{}, () =>
            {
                CreateRoom(_roomIdCache);
                Debug.Log("ReCreate");
            });
        }
        //接続完了
        protected override void OnConnected()
        {
            EndSignaling();
            SendMetaMessage(new MetaDataModel()
            {
                isHost = true,
                needMeta =  true,
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
                    Debug.Log("Host Connect to Client");
                    break;//正常終了
                case MetaDataModel.ConnectionState.Error: 
                case MetaDataModel.ConnectionState.Reconnect:
                    Debug.Log("Host Connect to Host Error");
                    Stop();
                    OnError?.Invoke("CreateError");
                    return;
            }

            if (meta.needMeta)
            {
                var echo = new MetaDataModel()
                {
                    isHost = false
                };
                if (meta.isHost)
                {
                    Debug.Log("Host Send Error");
                    echo.connectionState = MetaDataModel.ConnectionState.Error;
                    SendMetaMessage(echo);
                    Stop();
                    OnError?.Invoke("CreateError");
                }
                else
                {
                    Debug.Log("Host Send Success");
                    echo.connectionState = MetaDataModel.ConnectionState.Success;
                    SendMetaMessage(echo);

                }
            }
        }
        protected override void OnDisconnected()
        {
            Debug.Log("Host Disconnected");
        }

        protected override void OnCloseSocket(int code, string reason)
        {
            if (_currentMeta == null)
            {
                if (reason == "Conflict")
                {
                    Debug.Log("Host Conflict");
                    OnError?.Invoke("CreateError");
                }
            }
        }
        //private const string ROOM_CHARS = "0123456789abcdefghijklmnopqrstuvwxyz";
        // string GenerateRoomName(int length)
        // {
        //     var sb  = new System.Text.StringBuilder( length );
        //     var r   = new System.Random();
        //     for ( int i = 0; i < length; i++ )
        //     {
        //         int     pos = r.Next( ROOM_CHARS.Length );
        //         char    c   = ROOM_CHARS[ pos ];
        //         sb.Append( c );
        //     }
        //
        //     return sb.ToString();
        // }
    }
}
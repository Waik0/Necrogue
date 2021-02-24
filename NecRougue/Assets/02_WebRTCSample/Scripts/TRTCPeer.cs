using System;
using System.Collections;
using System.Collections.Generic;
using System.Security.Cryptography;
using Toast.WebRTCUtil;
using UnityEngine;
using UnityEngine.UI;

/*
ELECTRICAL COMMUNICATION
引き裂かれてる IMAGINATION
誰 に も 邪 魔 さ せ な い
 */
namespace Toast.RealTimeCommunication
{
    //初回接続データ
    [Serializable]
    public class MetaDataModel
    {
        public enum ConnectionState
        {
            None,
            Success,
            Reconnect, //送った側は1秒後切断してリトライ 送られた側は受け取ってから1秒後リトライ
            Error,
        }
        public string from;
        public bool isHost;
        public bool needMeta;
        public ConnectionState connectionState;
    }
    public class TRTCPeer : TRTCBase
    {
        
        protected string _signalingURL = "wss://ayame-labo.shiguredo.jp/signaling";
        protected string _signalingKey = "hqP9UNNTNlUIbAHLLdFGGXO7a1Ui1WzlAiNz9qKfPazaCOFB";
        protected readonly string _roomIdBase = "Waik0";
        protected readonly string _roomIdSplit1 = "@";
        protected string _roomId = "hoge";
        protected readonly char _roomPad = '*';
        //protected readonly string _roomIdSplit2 = "__";
        private int _maxPeerNum = 6;
        public string StartSignaling(string room)
        {
            _roomId = room;//GenerateRoomName(5);
            var roomFullName = _roomIdBase + _roomIdSplit1 + _roomId.PadRight(8, _roomPad);
            StartSignaling(_signalingURL,_signalingKey,roomFullName);
            return roomFullName;
        }

        protected void SendMetaMessage(MetaDataModel metaDataModel)
        {
            SendMessage("meta", JsonUtility.ToJson(metaDataModel));
        }

        protected override void OnConnected()
        {
            EndSignaling();
        }

    }
}
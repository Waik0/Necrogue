using System;
using System.Collections;
using System.Collections.Generic;
using System.Security.Cryptography;
using Toast.WebRTCUtil;
using UniRx;
using Unity.WebRTC;
using UnityEngine;
using UnityEngine.Profiling.Memory.Experimental;
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
        public string clientIdentifer;
        public ConnectionState connectionState;
    }
    public class TRTCPeer : TRTCBase
    {
        protected Dictionary<RTCPeerConnection, MetaDataModel> _metaCache = new Dictionary<RTCPeerConnection, MetaDataModel>();
        protected string _signalingURL = "wss://ayame-labo.shiguredo.jp/signaling";
        protected string _signalingKey = "hqP9UNNTNlUIbAHLLdFGGXO7a1Ui1WzlAiNz9qKfPazaCOFB";
        protected readonly string _roomIdBase = "Waik0";
        protected readonly string _roomIdSplit1 = "@";
        protected string _roomId = "hoge";
        protected readonly char _roomPad = '*';
        //protected readonly string _roomIdSplit2 = "__";
        private int _maxPeerNum = 6;
        public string SelfId { get; protected set; }
        public Action<RTCDataChannel, string> OnMessage;
        public string StartSignaling(string room)
        {
            _roomId = room;//GenerateRoomName(5);
            var roomFullName = _roomIdBase + _roomIdSplit1 + _roomId;// + _roomId.PadRight(8, _roomPad);
            StartSignaling(_signalingURL,_signalingKey,roomFullName);
            return roomFullName;
        }
        public Dictionary<RTCPeerConnection, MetaDataModel> MetaCache => _metaCache;
        protected void SendMetaMessage(RTCDataChannel channel,MetaDataModel metaDataModel)
        {
            SendMessageChannel(channel, JsonUtility.ToJson(metaDataModel));
        }

        protected void SendMetaMessageAsync(RTCDataChannel channel, MetaDataModel metaDataModel)
        {
            StartCoroutine(SendMetaMessageCoroutine(channel,metaDataModel));
        }

        IEnumerator SendMetaMessageCoroutine(RTCDataChannel channel, MetaDataModel metaDataModel)
        {
            while (channel.ReadyState != RTCDataChannelState.Open)
            {
                yield return null;
            }
            SendMessageChannel(channel,JsonUtility.ToJson(metaDataModel));
        }
        protected override void OnMessageInternal(RTCDataChannel channel,string message)
        {
            base.OnMessageInternal(channel,message);
            if (channel.Label == "meta")
            {
                var meta = JsonUtility.FromJson<MetaDataModel>(message);
                OnGetMeta(channel,meta);
            }
            OnMessage?.Invoke(channel,message);
        }

        protected virtual void OnGetMeta(RTCDataChannel channel, MetaDataModel message)
        {
            
        }
        
    }
}
using System;
using System.Collections.Generic;
using Toast.RealTimeCommunication;
using UniRx;
using Unity.WebRTC;
using UnityEngine;

namespace Toast.RealTimeCommunication
{

    public interface ITortecHostUseCase
    {
        //events
        IObservable<string> OnCreateRoom { get; }
        string SelfId { get; }
        string CreateRoom();
        void EndMatching();
        void OpenChannelAll<T>() where T : class;
        void OpenChannelAll(Type t);
        bool CheckAndOpenChannelAll<T>() where T : class;
        bool CheckAndOpenChannelAll(Type t);
        void SubscribeMessage<T>(GameObject owner, Action<string, T> res) where T : class;
        void BroadcastAll<T>(T data) where T : class;
        List<string> ConnectionIds();
        List<RTCDataChannel> Channels();
        List<RTCPeerConnection> ConnectionPeers();
        Dictionary<RTCPeerConnection, MetaDataModel> PeersMeta();
    }

    public class TortecHostUseCase : TortecUseCaseBase, ITortecHostUseCase
    {
        private Subject<string> _onCreateRoom = new Subject<string>();
        private const string ROOM_CHARS = "abcdefghijklmnopqrstuvwxyz";
        private TORTECHost _tortecHost;
        protected override TRTCPeer Peer => _tortecHost;
        public IObservable<string> OnCreateRoom => _onCreateRoom;

        public string CreateRoom()
        {
            var roomName = GenerateRoomName(6);
            _tortecHost.CreateRoom(roomName);
            _onCreateRoom?.OnNext(roomName);
            return roomName;
        }


        private void Awake()
        {
            gameObject.AddComponent<TORTECHost>();
            _tortecHost = GetComponent<TORTECHost>();
            _tortecHost.OnMessage = OnMessage;
        }


        string GenerateRoomName(int length)
        {
            var sb = new System.Text.StringBuilder(length);
            var r = new System.Random();
            for (int i = 0; i < length; i++)
            {
                int pos = r.Next(ROOM_CHARS.Length);
                char c = ROOM_CHARS[pos];
                sb.Append(c);
            }

            Debug.Log(sb);
            return sb.ToString();
        }


    }
}
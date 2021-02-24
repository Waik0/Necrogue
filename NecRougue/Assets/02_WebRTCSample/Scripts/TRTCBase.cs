using System.Collections;
using System.Collections.Generic;
using Cysharp.Threading.Tasks;
using Toast.WebRTCUtil;
using Unity.WebRTC;
using UnityEngine;

namespace Toast.RealTimeCommunication
{
    using DataChannelDictionary = Dictionary<int, RTCDataChannel>;
    public class TRTCBase : MonoBehaviour
    {
  

        protected int _interval = 10;
        private Signaling _signaling = null;
        private RTCConfiguration _rtcConfiguration;
        protected readonly Dictionary<string, RTCPeerConnection> _peerConnections =
            new Dictionary<string, RTCPeerConnection>();
        protected readonly Dictionary<RTCPeerConnection, DataChannelDictionary> m_mapPeerAndChannelDictionary =
            new Dictionary<RTCPeerConnection, DataChannelDictionary>();
        
        public virtual void Stop()
        {
            if (_signaling != null)
            {
                _signaling.Stop();
                _signaling = null;
            }
            foreach (var rtcPeerConnection in _peerConnections)
            {
                rtcPeerConnection.Value?.Close();
                rtcPeerConnection.Value?.Dispose();
            }
            _peerConnections.Clear();
            m_mapPeerAndChannelDictionary.Clear();
            _rtcConfiguration = new RTCConfiguration();
        }

        protected void StartSignaling(string signalingURL, string key,string id)
        {
            WebRTCStateManager.Instance.Init();
            _rtcConfiguration = new RTCConfiguration();
            _signaling?.Stop();
            _signaling = new Signaling(signalingURL, key, id, _interval);
            _signaling.OnAccept += OnAccept;
            _signaling.OnAnswer += OnAnswer;
            _signaling.OnOffer += OnOffer;
            _signaling.OnClose += OnCloseSocket;
            _signaling.OnIceCandidate += OnIceCandidate;
            _signaling.Start();
        }
        protected void SendOfferAsync(AcceptMessage acceptMessage)
        {
            SendOffer(acceptMessage.connectionId, this._rtcConfiguration).Forget();
        }
        protected void EndSignaling()
        {
            _signaling?.Stop(); 
        }

        protected void CloseChannel()
        {
            
        }
        protected void EndRTC()
        {
            WebRTCStateManager.Instance.Dispose();
            OnDisconnected();
        }
        private void OnDestroy()
        {
            EndSignaling();
            EndRTC();
        }

        protected virtual void OnCloseSocket(int code,string reason)
        {
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="ayameSignaling"></param>
        protected virtual void OnAccept(Signaling ayameSignaling)
        {
            Debug.Log("OnAccept");
            AcceptMessage acceptMessage = ayameSignaling.m_acceptMessage;

            bool shouldSendOffer = acceptMessage.isExistClient;

            this._rtcConfiguration.iceServers = acceptMessage.ToRTCIceServers();

            // 相手からのOfferを待つ
            if (!shouldSendOffer) return;

            SendOffer(acceptMessage.connectionId, this._rtcConfiguration).Forget();
        }
        async UniTask<bool> SendOffer(string connectionId, RTCConfiguration configuration)
        {
            var pc = new RTCPeerConnection(ref configuration);
            _peerConnections.Add(connectionId, pc);

            // create data chennel
            RTCDataChannelInit dataChannelOptions = new RTCDataChannelInit();

            RTCDataChannel dataChannel = pc.CreateDataChannel("meta", dataChannelOptions);
            dataChannel.OnMessage = bytes => OnMessage(dataChannel, bytes);
            dataChannel.OnOpen = () => OnOpenChannel(connectionId, dataChannel);
            dataChannel.OnClose = () => OnCloseChannel(connectionId, dataChannel);

            pc.OnDataChannel = new DelegateOnDataChannel(channel => { OnDataChannel(pc, channel); });
            pc.OnIceCandidate = new DelegateOnIceCandidate(candidate =>
            {
                _signaling.SendCandidate(connectionId, candidate);
            });
            pc.OnIceConnectionChange = new DelegateOnIceConnectionChange(state =>
            {
                if (state == RTCIceConnectionState.Disconnected)
                {
                    pc.Close();
                    _peerConnections.Remove(connectionId);
                    OnDisconnected();
                }
            });
            RTCOfferOptions options = new RTCOfferOptions();
            options.iceRestart = false;
            options.offerToReceiveAudio = false;
            options.offerToReceiveVideo = false;

            var offer = pc.CreateOffer(ref options);
            await offer;
            if (offer.IsError) return false;
            var desc = offer.Desc;
            var localDescriptionOperation = pc.SetLocalDescription(ref desc); 
            _signaling.SendOffer(connectionId, pc.LocalDescription);

            return true;
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="signaling"></param>
        /// <param name="e"></param>
        void OnOffer(ISignaling signaling, DescData e)
        {
            
            Debug.Log("OnOffer");
            if (_peerConnections.ContainsKey(e.connectionId)) return;

            RTCSessionDescription sessionDescriotion;
            sessionDescriotion.type = RTCSdpType.Offer;
            sessionDescriotion.sdp = e.sdp;

            this.SendAnswer(e.connectionId, sessionDescriotion).Forget();
        }

        async UniTask<bool> SendAnswer(string connectionId, RTCSessionDescription sessionDescriotion)
        {
            var pc = new RTCPeerConnection(ref _rtcConfiguration);
            _peerConnections.Add(connectionId, pc);

            pc.OnDataChannel = new DelegateOnDataChannel(channel => { OnDataChannel(pc, channel); });
            pc.OnIceCandidate = new DelegateOnIceCandidate(candidate =>
            {
                _signaling.SendCandidate(connectionId, candidate);
            });

            pc.OnIceConnectionChange = new DelegateOnIceConnectionChange(state =>
            {
                if (state == RTCIceConnectionState.Disconnected)
                {
                    pc.Close();
                    if (m_mapPeerAndChannelDictionary.ContainsKey(pc))
                    {
                        m_mapPeerAndChannelDictionary.Remove(pc);
                    }
                    _peerConnections.Remove(connectionId);

                    this.OnDisconnected();
                }
            });

            var remoteDescriptionOperation = pc.SetRemoteDescription(ref sessionDescriotion);
            await remoteDescriptionOperation;
            if (remoteDescriptionOperation.IsError) return false;

            RTCAnswerOptions options = default;

            var answer = pc.CreateAnswer(ref options);
            await answer;

            var desc = answer.Desc;
            var localDescriptionOperation = pc.SetLocalDescription(ref desc);
            await localDescriptionOperation;
            if (localDescriptionOperation.IsError) return false;

            _signaling.SendAnswer(connectionId, desc);

            return true;
        }

        void OnAnswer(ISignaling signaling, DescData e)
        {
            
            Debug.Log("OnAnswer");
            RTCSessionDescription desc = new RTCSessionDescription();
            desc.type = RTCSdpType.Answer;
            desc.sdp = e.sdp;

            RTCPeerConnection pc = _peerConnections[e.connectionId];
            pc.SetRemoteDescription(ref desc);
        }

        void OnIceCandidate(ISignaling signaling, CandidateData e)
        {
            if (!_peerConnections.TryGetValue(e.connectionId, out var pc)) return;


            RTCIceCandidateInit rtcIceCandidateInit = new RTCIceCandidateInit();
            rtcIceCandidateInit.candidate = e.candidate;
            rtcIceCandidateInit.sdpMLineIndex = e.sdpMLineIndex;
            rtcIceCandidateInit.sdpMid = e.sdpMid;
            RTCIceCandidate iceCandidate = new RTCIceCandidate(rtcIceCandidateInit);

            pc.AddIceCandidate(iceCandidate);
        }

        /// <summary>
        /// 他方のピアで作成されたDataChannelが接続されたときに呼ばれる。
        /// </summary>
        /// <param name="pc"></param>
        /// <param name="channel"></param>
        void OnDataChannel(RTCPeerConnection pc, RTCDataChannel channel)
        {
            
            Debug.Log("OnDataChannel");
            Debug.Log("DataChannel Connect");
            if (!m_mapPeerAndChannelDictionary.TryGetValue(pc, out var channels))
            {
                channels = new DataChannelDictionary();
                m_mapPeerAndChannelDictionary.Add(pc, channels);
            }

            channels.Add(channel.Id, channel);

            channel.OnMessage = bytes => OnMessage(channel, bytes);
            channel.OnClose = () => OnCloseChannel(_signaling.m_acceptMessage.connectionId, channel);

            this.OnConnected();
        }

        /// <summary>
        /// 自分のピアで作成したDataChannelの接続が確立されたときに呼ばれる。
        /// </summary>
        /// <param name="channel"></param>
        void OnOpenChannel(string connectionId, RTCDataChannel channel)
        {
            
            Debug.Log("OnOpenChannel");
            var pc = _peerConnections[connectionId];

            if (!m_mapPeerAndChannelDictionary.TryGetValue(pc, out var channels))
            {
                channels = new DataChannelDictionary();
                m_mapPeerAndChannelDictionary.Add(pc, channels);
            }

            channels.Add(channel.Id, channel);

            channel.OnMessage = bytes => OnMessage(channel, bytes);
            channel.OnClose = () => OnCloseChannel(connectionId, channel);

            this.OnConnected();
        }

        /// <summary>
        /// 自分のピアで作成したDataChannelの接続が切れたとき
        /// </summary>
        /// <param name="channel"></param>
        void OnCloseChannel(string connectionId, RTCDataChannel channel)
        {
            
            Debug.Log("OnCloseChannel");
            this.OnDisconnected();
        }

        protected virtual void OnMessage(RTCDataChannel channel, byte[] bytes)
        {
            string text = System.Text.Encoding.UTF8.GetString(bytes);
          
            this.OnMessage(  channel.Label,text);
        }

        protected virtual void OnMessage(string label,string message)
        {
            Debug.LogFormat("OnMessage {0} {1}", label,message);
        }

        public void SendMessage(string channel, string message)
        {
            RTCDataChannel dataChannel = this.GetDataChannel(channel);
            if (dataChannel == null) return;

            if (dataChannel.ReadyState != RTCDataChannelState.Open)
            {
                Debug.LogError("Not Open.");
                return;
            }

            dataChannel.Send(message);

            Debug.Log("Send Message");
        }

        // protected bool SendMessage(byte[] bytes)
        // {
        //     RTCDataChannel dataChannel = this.GetDataChannel("dataChannel");
        //     if (dataChannel == null) return false;
        //
        //     if (dataChannel.ReadyState != RTCDataChannelState.Open)
        //     {
        //         Debug.LogError("Not Open.");
        //         return false;
        //     }
        //
        //     dataChannel.Send(bytes);
        //
        //     Debug.Log("Send Message");
        //
        //     return true;
        // }

        protected virtual void OnConnected()
        {

        }

        protected virtual void OnDisconnected()
        {

        }

        protected bool IsConnected()
        {
            RTCDataChannel dataChannel = GetDataChannel("dataChannel");
            if (dataChannel == null) return false;
            return dataChannel.ReadyState == RTCDataChannelState.Open;
        }

        RTCDataChannel GetDataChannel(string label)
        {
            foreach (var dictionary in m_mapPeerAndChannelDictionary.Values)
            {
                foreach (RTCDataChannel dataChannel in dictionary.Values)
                {
                    if (dataChannel.Label == label) return dataChannel;
                }
            }

            return null;
        }
    }
}

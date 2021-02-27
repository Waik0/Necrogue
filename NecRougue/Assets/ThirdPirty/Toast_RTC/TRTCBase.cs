
using System;
using System.Collections.Generic;
using System.Linq;
using Cysharp.Threading.Tasks;
using Toast.WebRTCUtil;
using Unity.WebRTC;
using UnityEngine;

namespace Toast.RealTimeCommunication
{
    using DataChannelDictionary = Dictionary<string, RTCDataChannel>;
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
        public void CreateChannelAll(string label)
        {
            foreach (var rtcPeerConnection in _peerConnections)
            {
                if (rtcPeerConnection.Value != null)
                {
                    RTCDataChannelInit dataChannelOptions = new RTCDataChannelInit();
                    dataChannelOptions.ordered = true;
                    CreateChannel(rtcPeerConnection.Value,rtcPeerConnection.Key,label,dataChannelOptions);
                }
            }
        }

        /// <summary>
        /// チャンネルが空いているすべてのピアに送信
        /// </summary>
        /// <param name="label"></param>
        /// <param name="message"></param>
        public void BroadcastMessageChannel(string label, string message)
        {
            foreach (var keyValuePair in m_mapPeerAndChannelDictionary)
            {
                if (keyValuePair.Value.ContainsKey(label))
                {
                    keyValuePair.Value[label].Send(message);
                }
            }
        }
        /// <summary>
        /// すべてのピアでチャンネルが空いてるか
        /// </summary>
        /// <param name="label"></param>
        /// <returns></returns>
        public bool CheckAndOpenChannelAll(string label)
        {
            var isOpenAll = true;
            foreach (var key in m_mapPeerAndChannelDictionary.Keys.ToList())
            {
                if (!m_mapPeerAndChannelDictionary[key].ContainsKey(label))
                {
                    var id = _peerConnections.FirstOrDefault(k => k.Value == key).Key;
                    if (id != null)
                    {
                        RTCDataChannelInit dataChannelOptions = new RTCDataChannelInit();
                        dataChannelOptions.ordered = true;
                        CreateChannel(key,id, label,dataChannelOptions);
                    }
                }
                isOpenAll &= m_mapPeerAndChannelDictionary[key][label].ReadyState == RTCDataChannelState.Open;
            }
            return isOpenAll;
        }
        public void SendMessageChannel(RTCDataChannel dataChannel, string message)
        {
            
            if (dataChannel == null) return;

            if (dataChannel.ReadyState != RTCDataChannelState.Open)
            {
                Debug.LogError("Not Open.");
                return;
            }

            dataChannel.Send(message);

            Debug.Log("Send Message");
        }

        public void SendMessageChannel(string id,string label, string message)
        {
            if (_peerConnections.ContainsKey(id))
            {
                if (m_mapPeerAndChannelDictionary.ContainsKey(_peerConnections[id]))
                {
                    if (m_mapPeerAndChannelDictionary[_peerConnections[id]].ContainsKey(label))
                    {
                        m_mapPeerAndChannelDictionary[_peerConnections[id]][label].Send(message);
                    }
                }
            }
        }

        public string ChannelToConnectionId(RTCDataChannel channel)
        {
            foreach (var val in m_mapPeerAndChannelDictionary)
            {
                foreach (var rtcDataChannel in val.Value)
                {
                    if (rtcDataChannel.Value == channel)
                    {
                        foreach (var rtcPeerConnection in _peerConnections)
                        {
                            if (rtcPeerConnection.Value == val.Key)
                            {
                                return rtcPeerConnection.Key;
                            }
                        }

                    }
                }
            }

            return "";
        }
        public List<string> ConnectionIds()
        {
            return _peerConnections.Keys.ToList();
        }
        public List<RTCPeerConnection> ConnectionPeers()
        {
            return _peerConnections.Values.ToList();
        }
        public virtual void EndSignaling()
        {
            Debug.Log("EndSignaling");
            _signaling?.Stop(); 
        }

        public List<RTCDataChannel> Channels()
        {
            var li = new List<RTCDataChannel>();
            foreach (var keyValuePair in m_mapPeerAndChannelDictionary)
            {
                foreach (var rtcDataChannel in keyValuePair.Value)
                {
                    li.Add(rtcDataChannel.Value);
                }
            }
            //Debug.Log(li.Count);
            return li;
        }
        
        /// <summary>
        /// 初期化&シグナリング開始
        /// </summary>
        /// <param name="signalingURL"></param>
        /// <param name="key"></param>
        /// <param name="id"></param>
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

        protected RTCPeerConnection GetConnectionFromChannel(RTCDataChannel channel)
        {
            var p = m_mapPeerAndChannelDictionary.FirstOrDefault(_=>_.Value.FirstOrDefault(c=>c.Value.Id == channel.Id).Value != null);
            return p.Key ?? null;
        }

        protected void CloseChannel()
        {
            
        }
        protected void EndRTC()
        {
            WebRTCStateManager.Instance.Dispose();
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

        RTCDataChannel CreateChannel(RTCPeerConnection pc, string connectionId, string label,RTCDataChannelInit dataChannelOption)
        {
            RTCDataChannel dataChannel = pc.CreateDataChannel(label, dataChannelOption);
            dataChannel.OnMessage = bytes => OnMessage(dataChannel, bytes);
            dataChannel.OnOpen = () => OnOpenChannel(connectionId, dataChannel);
            dataChannel.OnClose = () => OnCloseChannel(connectionId, dataChannel);
            if (!m_mapPeerAndChannelDictionary.TryGetValue(pc, out var channels))
            {
                channels = new DataChannelDictionary();
                m_mapPeerAndChannelDictionary.Add(pc, channels);
            }

            if (!channels.ContainsKey(dataChannel.Label))
            {
                channels.Add(dataChannel.Label, dataChannel);
            }
            else
            {
                channels[dataChannel.Label] = dataChannel;
            }
            Debug.Log("CreateChannel");
            return dataChannel;
            
        }
        async UniTask<bool> SendOffer(string connectionId, RTCConfiguration configuration)
        {
            var pc = new RTCPeerConnection(ref configuration);
            this._peerConnections.Add(connectionId, pc);

            // create data chennel
            RTCDataChannelInit dataChannelOptions = new RTCDataChannelInit();
            dataChannelOptions.ordered = true;
            //RTCDataChannel dataChannel = pc.CreateDataChannel("meta", dataChannelOptions);
            // dataChannel.OnMessage = bytes => OnMessage(dataChannel, bytes);
            // dataChannel.OnOpen = () => OnOpenChannel(connectionId, dataChannel);
            // dataChannel.OnClose = () => OnCloseChannel(connectionId, dataChannel);
            RTCDataChannel dataChannel = CreateChannel(pc, connectionId, "meta",dataChannelOptions);
            pc.OnDataChannel = new DelegateOnDataChannel(channel => { OnDataChannel(connectionId,pc, channel); });
            pc.OnIceCandidate = new DelegateOnIceCandidate(candidate =>
            {
                this._signaling.SendCandidate(connectionId, candidate);
            });

            pc.OnIceConnectionChange = new DelegateOnIceConnectionChange(state =>
            {
                if (state == RTCIceConnectionState.Disconnected)
                {
                    Debug.Log("IceDisconnected");
                    pc.Close();
                    this._peerConnections.Remove(connectionId);

                    this.OnDisconnected(pc,dataChannel);
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

            this._signaling.SendOffer(connectionId, pc.LocalDescription);

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
            //CreateChannel(pc, "meta");
            pc.OnDataChannel = new DelegateOnDataChannel(channel =>
            {
                Debug.Log("OnDataChannel");
                OnDataChannel(connectionId,pc, channel);
            });
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
                        foreach (var rtcDataChannel in m_mapPeerAndChannelDictionary[pc])
                        {
                            this.OnDisconnected(pc,rtcDataChannel.Value);
                        }
                        m_mapPeerAndChannelDictionary.Remove(pc);
                    }
                    _peerConnections.Remove(connectionId);
                    
                    
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

        private RTCDataChannel _c;

        void Update()
        {
     
        }
        void OnAnswer(ISignaling signaling, DescData e)
        {
            
            Debug.Log("OnAnswer");
            RTCSessionDescription desc = new RTCSessionDescription();
            desc.type = RTCSdpType.Answer;
            desc.sdp = e.sdp;

            RTCPeerConnection pc = _peerConnections[e.connectionId];
            RTCDataChannelInit dataChannelOptions = new RTCDataChannelInit();
            
            pc.SetRemoteDescription(ref desc);
        }

        void OnIceCandidate(ISignaling signaling, CandidateData e)
        {
            Debug.Log("OnIceCandidate");
            if (!_peerConnections.TryGetValue(e.connectionId, out var pc)) return;

            Debug.Log("Process");
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
        void OnDataChannel(string connectionId,RTCPeerConnection pc, RTCDataChannel channel)
        {
            
            Debug.Log("OnDataChannel");
            Debug.Log("DataChannel Connect");
            if (!m_mapPeerAndChannelDictionary.TryGetValue(pc, out var channels))
            {
                channels = new DataChannelDictionary();
                m_mapPeerAndChannelDictionary.Add(pc, channels);
            }
            if (!channels.ContainsKey(channel.Label))
            {
                channels.Add(channel.Label, channel);
            } else
            {
                channels[channel.Label] = channel;
            }
            channel.OnMessage = bytes => OnMessage(channel, bytes);
            channel.OnClose = () => OnCloseChannel(connectionId, channel);

            this.OnConnected(pc,channel);
        }

        /// <summary>
        /// 自分のピアで作成したDataChannelの接続が確立されたときに呼ばれる。
        /// </summary>
        /// <param name="channel"></param>
        void OnOpenChannel(string conId, RTCDataChannel channel)
        {
            if (!_peerConnections.ContainsKey(conId))
                return;
            var pc = _peerConnections[conId];
            Debug.Log("OnOpenChannel");
            //var pc = _peerConnections[connectionId];

            if (!m_mapPeerAndChannelDictionary.TryGetValue(pc, out var channels))
            {
                channels = new DataChannelDictionary();
                m_mapPeerAndChannelDictionary.Add(pc, channels);
            }

            if (!channels.ContainsKey(channel.Label))
            {
                channels.Add(channel.Label, channel);
            }
            else
            {
                channels[channel.Label] = channel;
            }

            channel.OnMessage = bytes => OnMessage(channel, bytes);
            channel.OnClose = () => OnCloseChannel(conId, channel);

            this.OnConnected(pc, channel);
        }

        /// <summary>
        /// 自分のピアで作成したDataChannelの接続が切れたとき
        /// </summary>
        /// <param name="channel"></param>
        void OnCloseChannel(string cid, RTCDataChannel channel)
        {
            if (_peerConnections.ContainsKey(cid))
            {
                this.OnDisconnected(_peerConnections[cid],channel);
            }
            Debug.Log("OnCloseChannel");
        }

        protected virtual void OnMessage(RTCDataChannel channel, byte[] bytes)
        {
            string text = System.Text.Encoding.UTF8.GetString(bytes);
            this.OnMessageInternal(  channel,text);
        }

        protected virtual void OnMessageInternal(RTCDataChannel channel,string message)
        {
            Debug.LogFormat("OnMessage {0} {1}", channel.Label,message);
        }



        // public void SendMessage(RTCPeerConnection connection,string channel, string message)
        // {
        //     if (!m_mapPeerAndChannelDictionary.ContainsKey(connection))
        //     {
        //         return;
        //     }
        //
        //     var c = m_mapPeerAndChannelDictionary[connection].FirstOrDefault(_ => _.Value.Label == channel).Value;
        //     if (c == null)
        //     {
        //         return;
        //     }
        //
        //     SendMessage(c,message);
        //     RTCDataChannel dataChannel = c;
        // }

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

        protected virtual void OnConnected(RTCPeerConnection connection, RTCDataChannel channel)
        {

        }

        protected virtual void OnDisconnected(RTCPeerConnection connection,RTCDataChannel channel)
        {

        }

        protected bool IsConnectingSignalingServer()
        {
            return _signaling != null && _signaling.IsConnecting();
        }
    }
}

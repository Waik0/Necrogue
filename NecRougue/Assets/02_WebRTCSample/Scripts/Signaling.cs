using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Authentication;
using System.Text;
using System.Threading;
using Toast.WebRTCUtil;
using Unity.WebRTC;
using UnityEngine;
using WebSocketSharp;

namespace Toast.WebRTCUtil
{
    [Serializable]
    public class DescData
    {
        public string connectionId;
        public string sdp;
        public string type;
    }

    [Serializable]
    public class CandidateData
    {
        public string connectionId;
        public string candidate;
        public string sdpMid;
        public int sdpMLineIndex;
    }

    public delegate void OnOfferHandler(ISignaling signaling, DescData e);

    public delegate void OnAnswerHandler(ISignaling signaling, DescData e);

    public delegate void OnIceCandidateHandler(ISignaling signaling, CandidateData e);

    public interface ISignaling
    {
        void Start();
        void Stop();

        event OnOfferHandler OnOffer;
        event OnAnswerHandler OnAnswer;
        event OnIceCandidateHandler OnIceCandidate;
        event Action<int,string> OnClose;

        void SendOffer(string connectionId, RTCSessionDescription offer);
        void SendAnswer(string connectionId, RTCSessionDescription answer);
        void SendCandidate(string connectionId, RTCIceCandidate candidate);
    }

    public class Message
    {
        public string type;
    }

    public class AcceptMessage
    {
        public string type = "accept";
        public string connectionId;
        public List<IceServer> iceServers;
        public bool isExistClient;
        public bool isExistUser;
        public bool isInitiator;

        public RTCIceServer[] ToRTCIceServers()
        {
            RTCIceServer[] servers = new RTCIceServer[this.iceServers.Count];
        
            for (int i = 0; i < this.iceServers.Count; i++)
            {
                Debug.Log( this.iceServers[i]);
                servers[i] = this.iceServers[i].ToRTCIceServer();
            }

            return servers;
        }
    }

    [System.Serializable]
    public class IceServer
    {
        public List<string> urls;
        public string username;
        public string credential;

        public RTCIceServer ToRTCIceServer()
        {
            RTCIceServer rtcIceServer = new RTCIceServer();
            rtcIceServer.credential = this.credential;
            rtcIceServer.username = this.username;
            rtcIceServer.urls = this.urls.ToArray();

            return rtcIceServer;
        }

        public override string ToString()
        {
            var u = this.urls.Aggregate((c, s) => s += c + "\n");
            return $"user : {username} credential : {credential} urls : " + u;
        }
    }

    public class RegisterMessage
    {
        public string type = "register";
        public string roomId;
        public string clientId;
        public string signalingKey;
        public string authnMetadata;
    }

    public class AnswerMessage
    {
        public string type = "answer";
        public string sdp;
    }

    public class OfferMessage
    {
        public string type = "offer";
        public string sdp;
    }

    public class CandidateMessage
    {
        public string type = "candidate";
        public Ice ice;
    }

    [System.Serializable]
    public class Ice
    {
        public string candidate;
        public string sdpMid;
        public int sdpMLineIndex;
    }

    public class PongMessage
    {
        public string type = "pong";
    }


    public class Signaling : ISignaling
    {
        public delegate void OnAcceptHandler(Signaling signaling);

        private string m_url;
        private float m_timeout;
        private bool m_running;
        private Thread m_signalingThread;
        private AutoResetEvent m_wsCloseEvent;
        private WebSocket m_webSocket;
        private string clientId;

        public string m_signalingKey { get; private set; }
        public string m_roomId { get; private set; }
        public AcceptMessage m_acceptMessage { get; private set; } = null;

        public Signaling(string url, string signalingKey, string roomId, float timeout)
        {
            this.m_url = url;
            this.m_signalingKey = signalingKey;
            this.m_roomId = roomId;
            this.m_timeout = timeout;
            this.m_wsCloseEvent = new AutoResetEvent(false);

            this.clientId = RandomString(17);
        }

        string RandomString(int strLength)
        {
            string result = "";
            string charSet = "0123456789";

            System.Random rand = new System.Random();
            while (strLength != 0)
            {
                result += charSet[Mathf.FloorToInt(rand.Next(0, charSet.Length - 1))];
                strLength--;
            }

            return result;
        }

        public void Start()
        {
            this.m_running = true;
            m_signalingThread = new Thread(WSManage);
            m_signalingThread.Start();
        }

        public void Stop()
        {
            m_running = false;
            m_webSocket?.Close();
            m_signalingThread.Abort();
        }

        public event OnAcceptHandler OnAccept;
        public event OnOfferHandler OnOffer;
#pragma warning disable 0067
        public event OnAnswerHandler OnAnswer;
#pragma warning restore 0067
        public event OnIceCandidateHandler OnIceCandidate;
        public event Action<int,string> OnClose;


        public void SendOffer(string connectionId, RTCSessionDescription offer)
        {
            Debug.Log("Signaling: SendOffer");

            OfferMessage offerMessage = new OfferMessage();
            offerMessage.sdp = offer.sdp;

            this.WSSend(JsonUtility.ToJson(offerMessage));
        }

        public void SendAnswer(string connectionId, RTCSessionDescription answer)
        {
            Debug.Log("Signaling: SendAnswer");
            AnswerMessage answerMessage = new AnswerMessage();
            answerMessage.sdp = answer.sdp;

            this.WSSend(JsonUtility.ToJson(answerMessage));
        }

        public void SendCandidate(string connectionId, RTCIceCandidate candidate)
        {
            Debug.Log("Signaling: SendCandidate");

            CandidateMessage candidateMessage = new CandidateMessage();

            Ice ice = new Ice();
            ice.candidate = candidate.Candidate;
            ice.sdpMid = candidate.SdpMid;
            ice.sdpMLineIndex = candidate.SdpMLineIndex ?? 0;

            candidateMessage.ice = ice;
            this.WSSend(JsonUtility.ToJson(candidateMessage));
        }

        public void WSManage()
        {
            while (m_running)
            {
                WSCreate();

                m_wsCloseEvent.WaitOne();

                Thread.Sleep((int) (m_timeout * 1000));
            }

            Debug.Log("Signaling: WS managing thread ended");
        }

        private void WSCreate()
        {
            m_webSocket = new WebSocket(m_url);
            if (m_url.StartsWith("wss"))
            {
                m_webSocket.SslConfiguration.EnabledSslProtocols =
                    SslProtocols.Tls | SslProtocols.Tls11 | SslProtocols.Tls12;
            }

            m_webSocket.OnOpen += WSConnected;
            m_webSocket.OnMessage += WSProcessMessage;
            m_webSocket.OnError += WSError;
            m_webSocket.OnClose += WSClosed;

            Monitor.Enter(m_webSocket);

            Debug.Log($"Signaling: Connecting WS {m_url}");
            m_webSocket.ConnectAsync();
        }

        private void WSProcessMessage(object sender, MessageEventArgs e)
        {
            var content = Encoding.UTF8.GetString(e.RawData);
            Debug.Log($"Signaling: Receiving message: {content}");

            try
            {
                var message = JsonUtility.FromJson<Message>(content);
                string type = message.type;

                switch (type)
                {
                    case "accept":
                    {
                        AcceptMessage acceptMessage = JsonUtility.FromJson<AcceptMessage>(content);
                        this.m_acceptMessage = acceptMessage;
                        this.OnAccept?.Invoke(this);
                        break;
                    }

                    case "offer":
                    {
                        OfferMessage offerMessage = JsonUtility.FromJson<OfferMessage>(content);
                        DescData descData = new DescData();
                        descData.connectionId = this.m_acceptMessage.connectionId;
                        descData.sdp = offerMessage.sdp;

                        this.OnOffer?.Invoke(this, descData);

                        break;
                    }

                    case "answer":
                    {
                        AnswerMessage answerMessage = JsonUtility.FromJson<AnswerMessage>(content);
                        DescData descData = new DescData();
                        descData.connectionId = this.m_acceptMessage.connectionId;
                        descData.sdp = answerMessage.sdp;

                        this.OnAnswer?.Invoke(this, descData);

                        break;
                    }

                    case "candidate":
                    {
                        CandidateMessage candidateMessage = JsonUtility.FromJson<CandidateMessage>(content);

                        CandidateData candidateData = new CandidateData();
                        candidateData.connectionId = this.m_acceptMessage.connectionId;
                        candidateData.candidate = candidateMessage.ice.candidate;
                        candidateData.sdpMLineIndex = candidateMessage.ice.sdpMLineIndex;
                        candidateData.sdpMid = candidateMessage.ice.sdpMid;

                        this.OnIceCandidate?.Invoke(this, candidateData);

                        break;
                    }

                    case "ping":
                    {
                        PongMessage pongMessage = new PongMessage();
                        this.WSSend(JsonUtility.ToJson(pongMessage));

                        break;
                    }

                    case "bye":
                    {
                        // TODO:
                        break;
                    }

                    default:
                    {
                        Debug.LogError("Signaling: Received message from unknown peer");
                        break;
                    }
                }
            }
            catch (Exception ex)
            {
                Debug.LogError("Signaling: Failed to parse message: " + ex);
            }
        }

        private void WSConnected(object sender, EventArgs e)
        {
            RegisterMessage registerMessage = new RegisterMessage();
            registerMessage.roomId = this.m_roomId;
            registerMessage.signalingKey = this.m_signalingKey;
            registerMessage.clientId = this.clientId;

            Debug.Log("Signaling: WS connected.");
            this.WSSend(JsonUtility.ToJson(registerMessage));
        }

        private void WSError(object sender, ErrorEventArgs e)
        {
            Debug.LogError($"Signaling: WS connection error: {e.Message}");
        }

        private void WSClosed(object sender, CloseEventArgs e)
        {
            Debug.Log($"Signaling: WS connection closed, code: {e.Code}");
            Debug.Log(e.Reason);
            m_wsCloseEvent.Set();
            m_webSocket = null;
            OnClose?.Invoke(e.Code,e.Reason);
        }

        private void WSSend(object data)
        {
            if (m_webSocket == null || m_webSocket.ReadyState != WebSocketState.Open)
            {
                Debug.LogError("Signaling: WS is not connected. Unable to send message");
                return;
            }

            if (data is string s)
            {
                Debug.Log("Signaling: Sending WS data: " + s);
                m_webSocket.Send(s);
            }
            else
            {
                string str = JsonUtility.ToJson(data);
                Debug.Log("Signaling: Sending WS data: " + str);
                m_webSocket.Send(str);
            }
        }

    }
}
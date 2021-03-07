using System;
using System.Collections;
using System.Collections.Generic;
using Steamworks;
using UniRx;
using UnityEngine;

public class SteamNetworkManager : MonoBehaviour
{
    protected class SteamNetworkPacket
    {
        public string command;//基本クラス名
        public string payload;
    }
    private class SubjectData
    {
        public ulong fromId;
        public string data;
    }
    protected static SteamNetworkManager _instance;
    protected static SteamNetworkManager Instance {
        get {
            if (_instance == null) {
                _instance = new GameObject("SteamNetworkManager").AddComponent<SteamNetworkManager>();
                return _instance;
            }
            else {
                return _instance;
            }
        }
    }
    // protected Callback<P2PSessionRequest_t> _P2PSessionRequest;
    // protected Callback<P2PSessionConnectFail_t> _P2PSessionConnectFail;
    // protected Callback<SocketStatusCallback_t> _SocketStatusCallback;
 
    private Dictionary<string,Subject<SubjectData>> _subjects = new Dictionary<string, Subject<SubjectData>>();
    private Dictionary<string,Type> _typeMap = new Dictionary<string, Type>();
    private Coroutine _receiveMessageCoroutine;
    void Awake()
    {
        DontDestroyOnLoad(this);
        _receiveMessageCoroutine = StartCoroutine(ReadP2PPacket());
    }
    IEnumerator ReadP2PPacket()
    {
        var startTime = Time.realtimeSinceStartup;
        while (true)
        {
            uint msgSize;
            bool result = SteamNetworking.IsP2PPacketAvailable(out msgSize);
            if (!result)
            {
                yield return null;
                startTime = Time.realtimeSinceStartup;
                continue;
            }
            byte[] bytes = new byte[msgSize];
            uint newMsgSize;
            CSteamID steamIdRemote;
            result = SteamNetworking.ReadP2PPacket(bytes, msgSize, out newMsgSize, out steamIdRemote);
            print($"== GetMessage {msgSize} bytes from {steamIdRemote} ");
            InvokeCallback(ByteToCallbackData(bytes), steamIdRemote);
            if (Time.realtimeSinceStartup - startTime > .014f)
            {
                Debug.Log("TooManyMessage");
                yield return null;
                startTime = Time.realtimeSinceStartup;
            }
        }
    }

    void InvokeCallback(SteamNetworkPacket packet, CSteamID from)
    {
        if (_subjects.ContainsKey(packet.command))
        {
            _subjects[packet.command].OnNext(new SubjectData() {fromId = from.m_SteamID, data = packet.payload});
        }
    }

    SteamNetworkPacket ByteToCallbackData(byte[] bytes)
    {
        var p = System.Text.Encoding.UTF8.GetString(bytes);
        var packet = JsonUtility.FromJson<SteamNetworkPacket>(p);
        return packet;
    }
    protected static string TypeToKey<T>() => typeof(T).ToString();
    //Jsonで送る
    //MessagePack使いたいけど導入めんどいからあと
    public static void SendTo<TData>(CSteamID steamId,TData data)
    {
        var i = Instance;//オブジェクト生成するためだけの参照
        var s= JsonUtility.ToJson(data);
        var p = JsonUtility.ToJson(new SteamNetworkPacket()
        {
            command = TypeToKey<TData>(),
            payload = s
        });
        byte[] bytes = System.Text.Encoding.UTF8.GetBytes(p);
        SteamNetworking.SendP2PPacket(steamId, bytes, (uint)bytes.Length, EP2PSend.k_EP2PSendReliable);
    }
    //メッセージ受け取り時コールバック設定
    public static void SubscribeMessage<T>(Action<ulong,T> res,GameObject owner) where T : class
    {
        var subjects = Instance._subjects;
        var typeMap = Instance._typeMap;
        if (!subjects.ContainsKey(TypeToKey<T>()))
        {
            subjects.Add(TypeToKey<T>() ,new Subject<SubjectData>());
        }

        if (!typeMap.ContainsKey(TypeToKey<T>()))
        {
            typeMap.Add(TypeToKey<T>(),typeof(T));
        }
        var subject = subjects[TypeToKey<T>()];
        subject.Subscribe(d => res?.Invoke(d.fromId, JsonUtility.FromJson<T>(d.data))).AddTo(_instance).AddTo(owner);
    }
    public static void SubscribeMessage<T>(Action<ulong,T> res,CompositeDisposable owner) where T : class
    {
        var subjects = Instance._subjects;
        var typeMap = Instance._typeMap;
        if (!subjects.ContainsKey(TypeToKey<T>()))
        {
            subjects.Add(TypeToKey<T>() ,new Subject<SubjectData>());
        }

        if (!typeMap.ContainsKey(TypeToKey<T>()))
        {
            typeMap.Add(TypeToKey<T>(),typeof(T));
        }
        var subject = subjects[TypeToKey<T>()];
        subject.Subscribe(d => res?.Invoke(d.fromId, JsonUtility.FromJson<T>(d.data))).AddTo(_instance).AddTo(owner);
    }
}


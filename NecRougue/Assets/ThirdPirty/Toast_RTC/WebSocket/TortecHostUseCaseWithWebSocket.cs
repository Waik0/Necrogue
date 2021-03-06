using System;
using System.Collections;
using System.Collections.Generic;
using Toast.RealTimeCommunication;
using UniRx;
using Unity.WebRTC;
using UnityEngine;
public interface ITortecHostUseCaseWithWebSocket : ITortecUseCaseBaseWithWebSocket
{
    string CreateRoom();
}
public class TortecHostUseCaseWithWebSocket : TortecUseCaseBaseWithWebSocket, ITortecHostUseCaseWithWebSocket
{
    private const string ROOM_CHARS = "abcdefghijklmnopqrstuvwxyz";
    private void Awake()
    {
        OnOpenCallback.Subscribe(_=>GreetOnConnect()).AddTo(this);
        Setup();
    }
    public string CreateRoom()
    {
        //スケーラブルにするためにここで自動サーバー選択したい
        var roomName = GenerateRoomName(6);
        Peer.ConnectHost(roomName);
        return roomName;
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
    void GreetOnConnect()
    {
        Debug.Log("Greeting");
        Peer.Send("sys_greeting",JsonUtility.ToJson(new Greeting(){needEcho = true}));
    }
}

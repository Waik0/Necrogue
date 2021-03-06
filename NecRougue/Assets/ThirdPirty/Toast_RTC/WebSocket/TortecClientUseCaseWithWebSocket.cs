using System;
using System.Collections;
using System.Collections.Generic;
using UniRx;
using UnityEngine;
public interface ITortecClientUseCaseWithWebSocket : ITortecUseCaseBaseWithWebSocket
{
    void JoinRoom(string room );
}
public class TortecClientUseCaseWithWebSocket : TortecUseCaseBaseWithWebSocket,ITortecClientUseCaseWithWebSocket
{
    private void Awake()
    {
        OnOpenCallback.Subscribe(_=>GreetOnConnect()).AddTo(this);
        Setup();
    }

    public void JoinRoom(string room)
    {
        Peer.ConnectJoin(room);
    }

    void GreetOnConnect()
    {
        
        Peer.Send("sys_greeting",JsonUtility.ToJson(new Greeting(){needEcho = true}));
    }
}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Zenject;

public interface IGameStartDataSender
{
    void SendStart(ITortecUseCaseBaseWithWebSocket peer);
}
public class GameStartDataSender : IGameStartDataSender
{
    /// <summary>
    /// ゲーム開始を送る
    /// </summary>
    public void SendStart(ITortecUseCaseBaseWithWebSocket peer)
    {
        Debug.Log("プレイヤー" + peer.ConnectionIds().Count + "人");
        //送信時点で接続されていたプレイヤーでゲーム開始
        if (peer.IsOpen)
        {
            peer.BroadcastAll(new GameStartData()
            {
                //players = peer.ConnectionIds(),
            });
        }
    }
}

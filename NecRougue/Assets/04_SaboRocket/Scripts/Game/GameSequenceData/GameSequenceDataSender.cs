﻿using System;
using System.Collections;
using System.Collections.Generic;
using UniRx;
using UnityEngine;
using Zenject;

public interface IGameSequenceDataSender
{
    void SendReadyData(ITortecUseCaseBaseWithWebSocket peer);
    void SendNextTurnData(ITortecUseCaseBaseWithWebSocket peer, int turn, string player);
}
public class GameSequenceDataSender : IGameSequenceDataSender
{
    public void SendReadyData(ITortecUseCaseBaseWithWebSocket peer)
    {
        var data = new GameSequenceData();
        data.command = GameSequenceData.Command.Ready;
        peer.BroadcastAll(data);
    }

    public void SendNextTurnData(ITortecUseCaseBaseWithWebSocket peer,int turn,string player)
    {
        var data = new GameSequenceData();
        data.command = GameSequenceData.Command.NextTurn;
        data.currentTurn = turn;
        data.currentPlayer = player;
        peer.BroadcastAll(data);
    }
    
}

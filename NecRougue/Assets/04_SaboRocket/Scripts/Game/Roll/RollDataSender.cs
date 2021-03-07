using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RollDataSender
{
    public void SendRollDataAll(Dictionary<string, RollData.Roll> rollData, ITortecUseCaseBaseWithWebSocket peer)
    {
        foreach (var keyValuePair in rollData)
        {
            peer.BroadcastAll(new RollData()
            {
                roll = keyValuePair.Value,
                playerId = keyValuePair.Key
            });
        }
    }
}

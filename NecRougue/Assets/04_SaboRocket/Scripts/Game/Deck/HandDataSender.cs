using System.Collections;
using System.Collections.Generic;
using UnityEngine;
public interface IHandDataSender
{
    void SendHandData(HandData handData, ITortecUseCaseBaseWithWebSocket peer);
}
public class HandDataSender : IHandDataSender
{
    public void SendHandData(HandData handData,ITortecUseCaseBaseWithWebSocket peer)
    {
        peer.BroadcastAll(handData);
        
    }
    public void SendHandDataAll(Dictionary<string,List<int>> hands,ITortecUseCaseBaseWithWebSocket peer)
    {
        foreach (var keyValuePair in hands)
        {
            peer.BroadcastAll(new HandData()
            {
                hand = keyValuePair.Value,
                playerId = keyValuePair.Key
            });
        }
    }
}


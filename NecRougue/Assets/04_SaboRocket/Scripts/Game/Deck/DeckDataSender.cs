using System.Collections;
using System.Collections.Generic;
using UnityEngine;
public interface IDeckDataSender
{
    void SendDeckData(DeckData deckData, ITortecUseCaseBaseWithWebSocket peer);
}
public class DeckDataSender : IDeckDataSender
{
    public void SendDeckData(DeckData deckData, ITortecUseCaseBaseWithWebSocket peer)
    {
        peer.BroadcastAll(deckData);
    }
}

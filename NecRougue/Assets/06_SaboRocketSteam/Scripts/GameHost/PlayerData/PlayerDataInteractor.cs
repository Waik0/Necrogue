using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class PlayerDataInteractor : MonoBehaviour
{
    public Dictionary<ulong, PlayerData> PlayerDatas { get; } = new Dictionary<ulong, PlayerData>();
    public void SetPlayerList(List<ulong> playerIDs)
    {
        PlayerDatas.Clear();
        foreach (var playerID in playerIDs)
        {
            PlayerDatas.Add(playerID,null);
        }
    }

    public List<ulong> Players => PlayerDatas.Keys.ToList();
    public ulong GetTurnPlayer(int turn)
    {
        return PlayerDatas.Keys.ToList()[turn % PlayerDatas.Keys.Count];
    }
}

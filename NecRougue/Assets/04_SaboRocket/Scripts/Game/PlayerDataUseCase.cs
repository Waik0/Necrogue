using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

/// <summary>
/// ゲームに参加しているプレイヤーIDとそれに紐づいた情報を管理
/// </summary>
public class PlayerDataUseCase
{
    public Dictionary<string, PlayerData> PlayerDatas { get; } = new Dictionary<string, PlayerData>();
    public void SetPlayerList(List<string> playerIDs)
    {
        PlayerDatas.Clear();
        foreach (var playerID in playerIDs)
        {
            PlayerDatas.Add(playerID,null);
        }
    }

    public List<string> Players => PlayerDatas.Keys.ToList();
    public string GetTurnPlayer(int turn)
    {
        return PlayerDatas.Keys.ToList()[turn % PlayerDatas.Keys.Count];
    }
}

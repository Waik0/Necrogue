using System;
using System.Collections;
using System.Collections.Generic;
using Random = UnityEngine.Random;

public class RollUseCase
{
    private Dictionary<string, RollData.Roll> _rolls = new Dictionary<string, RollData.Roll>();
    public Action<Dictionary<string, RollData.Roll>> OnUpdateRoll = null;

    public void Init()
    {
        _rolls.Clear();
    }

    public void SetRoll(string playerId, RollData.Roll roll)
    {
        if (!_rolls.ContainsKey(playerId))
        {
            _rolls.Add(playerId,RollData.Roll.Blue);
        }
        _rolls[playerId] = roll;
        OnUpdateRoll?.Invoke(_rolls);
    }

    public void RandomInitRoll(List<string> players)
    {
        _rolls.Clear();
        var red = Random.Range(0, players.Count);
        var count = 0;
        foreach (var player in players)
        { 
            SetRoll(player,count == red ? RollData.Roll.Red : RollData.Roll.Blue);
            count++;
        }
        //OnUpdateRoll?.Invoke(_rolls);
    }
}


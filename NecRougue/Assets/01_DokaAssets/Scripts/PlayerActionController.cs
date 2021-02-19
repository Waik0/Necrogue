using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// プレイヤーの行動制御
/// </summary>
public interface IPlayerActionController
{
    string CurrentPlayerId { get; set; }
}

public class PlayerActionController : IPlayerActionController
{
    public string CurrentPlayerId { get; set; }
    
}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// 行動パターン
/// </summary>
public enum NpcActionPattern
{
    //共通
    Stop,
    Talk,
    MoveToNpc,
    Paid,
    //客
    Call,
    Order,
    Pay,
    MoveToChair,
    //店員
    MoveToRandomPlace,
    MoveToOrder,
    Cook,
    Carry,
    Clean,
    Fortune,
}
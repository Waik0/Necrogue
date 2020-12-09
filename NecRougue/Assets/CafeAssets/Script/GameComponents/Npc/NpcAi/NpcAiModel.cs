using System.Collections.Generic;
using UnityEngine;

namespace CafeAssets.Script.GameComponents.Npc.NpcAi
{
    /// <summary>
    /// 行動パターン
    /// </summary>
    public enum NpcActionPattern
    {
        Stop,
        MoveToRandomPlace,
        MoveToChair,
        MoveToNpc,
        Order,
        Talk,
        Cook,
        Carry,
        Pay,
        Clean,
        Fortune,
    }
    /// <summary>
    /// 行動のステータス
    /// </summary>
    public enum NpcActionStatus
    {
        Sleep,
        Doing,
        Complete,
    }
    
    public class NpcMoveModel
    {
        public Vector3 Position;
        public GameObject Self;
    }
    public class NpcActionModel
    {
        public string Param;
    }
    public class NpcAiModel
    {
        public List<NpcActionPattern> Priority;
        
    }
}
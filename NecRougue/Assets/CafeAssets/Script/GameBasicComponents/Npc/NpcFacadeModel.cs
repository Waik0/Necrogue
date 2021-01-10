using System;
using System.Collections.Generic;
using System.Linq;
using CafeAssets.Script.GameComponents.Npc.NpcAi;
using CafeAssets.Script.GameComponents.Npc.NpcParam;
using UnityEngine;

namespace CafeAssets.Script.GameComponents.Npc
{
    public enum NpcType
    {
    
    }
    

    public class NpcFacadeModel
    {
        public string Id;
        public string Name;
        public NpcType Type;
        public NpcAiModel Ai;
        public NpcMoveModel Move;
    }

 
}
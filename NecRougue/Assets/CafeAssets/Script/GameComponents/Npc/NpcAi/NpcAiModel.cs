using System.Collections.Generic;
using UnityEngine;

namespace CafeAssets.Script.GameComponents.Npc.NpcAi
{

  
    
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
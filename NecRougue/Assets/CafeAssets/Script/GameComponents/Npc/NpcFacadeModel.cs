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
    
    public enum NpcParameters
    {
        SitDown,
    }
    public class NpcModel
    {
        public GameObject GameObject;
    }
    public class NpcFacadeModel
    {
        public string Name;
        public NpcType Type;
        public NpcAiModel Ai;
        public NpcMoveModel Move;
    }

    public class NpcParamModel
    {
        public Dictionary<NpcParameterStyle, int> Param;
        public NpcParamModel()
        {
            Param = new Dictionary<NpcParameterStyle, int>();
            foreach (NpcParameterStyle key in Enum.GetValues(typeof(NpcParameterStyle)))
            {
                Param.Add(key,0);
            }
        }

        public void Set(NpcParameterStyle key, int num)
        {
            Param[key] = num;
        }

        public int Get(NpcParameterStyle key)
        {
            return Param[key];
        }

        public int GetSum()
        {
            return Param.Sum(keyValuePair => keyValuePair.Value);
        }
    }
}
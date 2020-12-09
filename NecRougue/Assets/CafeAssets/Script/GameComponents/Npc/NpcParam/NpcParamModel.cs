using System;
using System.Collections.Generic;
using System.Linq;

namespace CafeAssets.Script.GameComponents.Npc.NpcParam
{
    public enum NpcParameterStyle
    {
        Fixable,
        Fluid
    }
    public enum NpcParameterOperations
    {
        Add = 1,
        Times = 2,
        Division = 3,
    }

    public class NpcDynamicParamModel
    {
        public Dictionary<NpcParameterStyle, int> Param;
        public NpcDynamicParamModel()
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
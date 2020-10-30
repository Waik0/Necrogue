using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public enum NpcType
{
    
}

public class NpcModel
{
    
}
public class NpcParamModel
{
    public Dictionary<ParameterStyle, int> Param;
    public NpcParamModel()
    {
        Param = new Dictionary<ParameterStyle, int>();
        foreach (ParameterStyle key in Enum.GetValues(typeof(ParameterStyle)))
        {
            Param.Add(key,0);
        }
    }

    public void Set(ParameterStyle key, int num)
    {
        Param[key] = num;
    }

    public int Get(ParameterStyle key)
    {
        return Param[key];
    }

    public int GetSum()
    {
        return Param.Sum(keyValuePair => keyValuePair.Value);
    }
}
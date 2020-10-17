using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public enum ParameterStyle
{
    Fixable,
    Fluid
    
}

public class GameParamModel
{
    public Dictionary<ParameterStyle, int> Param;
    public GameParamModel()
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
public class GameParameterPresenter : IDisposable
{
    private Dictionary<string, GameParamModel> IntParams;//固定資産
    public void Reset()
    {
        IntParams.Clear();
    }

    void AddModel(string key)
    {
        if (!IntParams.ContainsKey(key))
        {
            IntParams.Add(key,new GameParamModel());
        }
    }
    public int Get(string key)
    {
        AddModel(key);
        return IntParams[key].GetSum();
    }

    public int Set(ParameterStyle style,string key, int num)
    {
        AddModel(key);
        IntParams[key].Set(style,num);
        return IntParams[key].GetSum();
    }

    public int Operation(ParameterStyle style,string key, GameParameterOperations operation, int num)
    {
        AddModel(key);
        switch (operation)
        {
            case GameParameterOperations.Add:
                IntParams[key].Set(style, IntParams[key].Get(style) + num);
                break;
            case GameParameterOperations.Times:
                IntParams[key].Set(style, IntParams[key].Get(style) * num);
                break;
            case GameParameterOperations.Division:
                IntParams[key].Set(style, IntParams[key].Get(style) / num);
                break;
        }
        return IntParams[key].GetSum();
    }

    public void Dispose()
    {
        Debug.Log("[GameParam]Dispose");
    }
}

using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameParameterPresenter : IDisposable
{
    private Dictionary<string, int> IntParams;
    public void Reset()
    {
        IntParams.Clear();
    }

    public int Get(string key)
    {
        if (!IntParams.ContainsKey(key))
        {
            IntParams.Add(key,0);
        }
        return IntParams[key];
    }

    public int Set(string key, int num)
    {
        if (!IntParams.ContainsKey(key))
        {
            IntParams.Add(key,0);
        }

        IntParams[key] = num;
        return IntParams[key];
    }

    public int Operation(string key, GameParameterOperations operation, int num)
    {
        if (!IntParams.ContainsKey(key))
        {
            IntParams.Add(key,0);
        }

        switch (operation)
        {
            case GameParameterOperations.Add:
                IntParams[key] += num;
                break;
            case GameParameterOperations.Times:
                IntParams[key] *= num;
                break;
            case GameParameterOperations.Division:
                IntParams[key] /= num;
                break;
        }
        return IntParams[key];
    }

    public void Dispose()
    {
        Debug.Log("[GameParam]Dispose");
    }
}

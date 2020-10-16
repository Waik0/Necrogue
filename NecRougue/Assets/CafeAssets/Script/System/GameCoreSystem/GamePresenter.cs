using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GamePresenter
{
    private GameParameterPresenter _parameter;

    public GamePresenter()
    {
        _parameter = new GameParameterPresenter();
    }
    public void LoadData(string path)
    {
        
    }

    public void ResetParams()
    {
        _parameter.Reset();
    }

    public void ResetMap()
    {
        
    }
}

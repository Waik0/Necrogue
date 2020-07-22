using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class IntroPresenter
{
    
    private string _dispString;
    private IntroDataUseCase _introDataUseCase;
    private int _current;
    public void Inject(IntroDataUseCase introDataUseCase)
    {
        _introDataUseCase = introDataUseCase;
    }
    
    public bool MessageNext()
    {
        
        if (_introDataUseCase.Data.Message.Length <= _current)
        {
            return false;
        }
        _dispString += _introDataUseCase.Data.Message[_current];
        _current++;
        return true;
    }

    public void ResetPresenter()
    {
        _current = 0;
        _dispString = "";
    }
    public void MessageAll()
    {
        _dispString = _introDataUseCase.Data.Message;
    }
    
    #if DEBUG

    public void DebugUI()
    {
        GUILayout.Label(_dispString);
    }
    #endif
}

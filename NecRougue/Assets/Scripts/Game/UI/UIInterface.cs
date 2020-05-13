using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public interface IModalUI
{
    void ResetUI();
    bool UpdateUI();//true is end 
    
}

public interface IModalUI<T>
{
    void ResetUI();
    T UpdateUI();
}


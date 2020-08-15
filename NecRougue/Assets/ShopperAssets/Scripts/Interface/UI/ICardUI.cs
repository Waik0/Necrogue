using System;
using System.Collections;
using System.Collections.Generic;
using ShopperAssets.Scripts.Game;
using UnityEngine;
/// <summary>
/// カード表示
/// </summary>
public interface ICardUI
{
    void SetCard(CardModel card);
    
}

public interface ISelectableUI
{
    bool IsSelected { get; }
    string Unique { get; set; }
    string GlobalSelectedUnique { get; }
    Action<string> OnSelected { get; set; }
    Action<string> OnExecuted { get; set; }
}

public abstract class ICardIconUI : MonoBehaviour,ICardUI, ISelectableUI
{
    protected static string _selectedUnique = "";
    public abstract void SetCard(CardModel card);
    public bool IsSelected => GlobalSelectedUnique == Unique;
    public string GlobalSelectedUnique => _selectedUnique;
    public string Unique { get; set; }
    public Action<string> OnSelected { get; set; }
    public Action<string> OnExecuted { get; set; }
}
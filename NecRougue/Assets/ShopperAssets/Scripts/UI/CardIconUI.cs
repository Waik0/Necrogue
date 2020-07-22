using System;
using System.Collections;
using System.Collections.Generic;
using ShopperAssets.Scripts.Game;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

public class CardIconUI : MonoBehaviour
{
    private static string _selectedGuid;
    public static string SelectedGuid => _selectedGuid;
    
    [SerializeField] private Image _icon;
    [SerializeField] private Button _button;
    public void SetCard(CardModel card)
    {
        _icon.sprite = null;
    }
    
    public string CardGUID;
    public Action<string> OnClickOnce;
    public Action<string> OnClickTwice;
    
    public void Awake()
    {
        _button.onClick.AddListener(() =>
        {
            if (_selectedGuid != CardGUID)
            {
                _selectedGuid = CardGUID;
                OnClickOnce?.Invoke(CardGUID);
            }
            else
            {
                OnClickTwice?.Invoke(CardGUID);
            }
        });
    }
}

using System;
using System.Collections;
using System.Collections.Generic;
using ShopperAssets.Scripts.Game;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

public class CardIconUIDebug : ICardIconUI
{
    [SerializeField] private Image _icon;
    [SerializeField] private Button _button;
    [SerializeField] private Text _debug;
    public override void SetCard(CardModel card)
    {
        _icon.sprite = null;
        _debug.text = card.Name;
    }
    
    public void Awake()
    {
        _button.onClick.AddListener(() =>
        {
            if (!IsSelected)
            {
                _selectedUnique = Unique;
                OnSelected?.Invoke(Unique);
            }
            else
            {
                OnExecuted?.Invoke(Unique);
                _selectedUnique = "";
            }
        });
    }

  
}

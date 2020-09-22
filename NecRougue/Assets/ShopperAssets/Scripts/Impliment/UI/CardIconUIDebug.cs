using System;
using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using DG.Tweening.Core;
using ShopperAssets.Scripts.Game;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

public class CardIconUIDebug : ICardIconUI
{
    [SerializeField] private Image _icon;
    [SerializeField] private Button _button;
    [SerializeField] private Text _debug;
    [SerializeField] private Text _price;


    public override void SetCard(CardModel card)
    {
        _icon.sprite = null;
        _debug.text = card.Name;
        _price.text = card.Price.ToString();
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

using System;
using System.Collections;
using System.Collections.Generic;
using ShopperAssets.Scripts.Game;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

public class CardIconUI : ICardIconUI
{
    [SerializeField] private Image _icon;
    [SerializeField] private Button _button;
    [SerializeField] private Text _text = default;
    [SerializeField] private Text _title = default;
    public override void SetCard(CardModel card)
    {
        _icon.sprite = null;
        if (_title != null) _title.text = card.Name;
        if (_text != null) _text.text = card.Description;
    }

    public void ResetCard()
    {
        _icon.sprite = null;
        if (_title != null) _title.text = "";
        if (_text != null) _text.text = "";
    } 
    public void Awake()
    {
        if (_button != null)
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
                }
            });
        }
    }
}

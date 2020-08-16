using System;
using System.Collections;
using System.Collections.Generic;
using ShopperAssets.Scripts.Game;
using UnityEngine;
using UnityEngine.UI;

public class DeckUI : MonoBehaviour
{
    //prefab
    [SerializeField] private ICardIconUI _cardIconUiPrefab; 
    //root
    [SerializeField] private GridLayoutGroup _hand;

    [SerializeField] private Text _deckNum;
    [SerializeField] private Text _trashNum;

    public void ResetUI()
    {
        
        foreach (Transform o in _hand.transform)
        {
            Destroy(o.gameObject);
        }

        _trashNum.text = "";
        _deckNum.text = "";
    }
    public void SetHandAll(List<CardModel> cards,Action<string> onClick,Action<string> onUse)
    {
        foreach (Transform o in _hand.transform)
        {
            Destroy(o.gameObject);
        }

        foreach (var cardModel in cards)
        {
            var cardicon = Instantiate(_cardIconUiPrefab, _hand.transform);
            cardicon.SetCard(cardModel);
            cardicon.Unique = cardModel.GUID;
            cardicon.OnSelected = onClick;
            cardicon.OnExecuted = onUse;
        }
    }

    public void AddHand(CardModel cards, Action<string> onClick)
    {
        
    }

    public void SetTrash(int num)
    {
        _trashNum.text = $"trash:{num}";
    }

    public void SetDeck(int num)
    {
        _deckNum.text = $"deck:{num}";
    }

}

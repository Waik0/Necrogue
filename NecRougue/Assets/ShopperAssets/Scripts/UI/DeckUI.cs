using System;
using System.Collections;
using System.Collections.Generic;
using ShopperAssets.Scripts.Game;
using UnityEngine;
using UnityEngine.UI;

public class DeckUI : MonoBehaviour
{
    //prefab
    [SerializeField] private CardIconUI _cardIconUiPrefab; 
    //root
    [SerializeField] private GridLayoutGroup _hand;
    [SerializeField] private GridLayoutGroup _shop;
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
            cardicon.CardGUID = cardicon.CardGUID;
            cardicon.OnClickOnce = onClick;
            cardicon.OnClickTwice = onUse;
        }
    }

    public void AddHand(CardModel cards, Action<string> onClick)
    {
        
    }

    public void SetShopAll(List<CardModel> cards,Action<string> onClick,Action<string> onBuy)
    {
        foreach (Transform o in _shop.transform)
        {
            Destroy(o.gameObject);
        }

        foreach (var cardModel in cards)
        {
            var cardicon = Instantiate(_cardIconUiPrefab, _hand.transform);
            cardicon.SetCard(cardModel);
            cardicon.CardGUID = cardicon.CardGUID;
            cardicon.OnClickOnce = onClick;
            cardicon.OnClickTwice = onBuy;
        }
    }

    public void AddShop(CardModel cards,Action<string> onBuy)
    {
        
    }
    public void SetDeck(int num)
    {
        
    }

}

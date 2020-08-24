using System;
using System.Collections;
using System.Collections.Generic;
using ShopperAssets.Scripts.Game;
using UnityEngine;
using UnityEngine.UI;

public class ShopUI : MonoBehaviour
{
    [SerializeField] private ICardIconUI _cardIconUiPrefab;
    [SerializeField] private Text _coin;
    [SerializeField] private GridLayoutGroup _shop;
    [SerializeField] private Button _shopLevelUpButton;

    public Action<string> OnClick;
    public Action<string> OnBuy;

    public void SetAction(Action<string> onClick, Action<string> onBuy)
    {
        OnClick = onClick;
        OnBuy = onBuy;
    }
    public void ResetUI()
    {
        foreach (Transform o in _shop.transform)
        {
            Destroy(o.gameObject);
        }

        _coin.text = "";
    }

    public void AddShop(CardModel cards,Action<string> onBuy)
    {
        
    }

    public void SetCoin(int coin)
    {
        _coin.text = $"x {coin}";
    }
    public void SetShopAll(List<CardModel> cards)
    {
        foreach (Transform o in _shop.transform)
        {
            Destroy(o.gameObject);
        }

        foreach (var cardModel in cards)
        {
            var cardicon = Instantiate(_cardIconUiPrefab, _shop.transform);
            cardicon.SetCard(cardModel);
            cardicon.Unique = cardModel.GUID;
            cardicon.OnSelected = OnClick;
            cardicon.OnExecuted = OnBuy;
        }
    }
    // Update is called once per frame

}

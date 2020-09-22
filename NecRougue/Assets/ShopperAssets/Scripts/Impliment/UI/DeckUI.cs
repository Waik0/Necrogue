using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using ShopperAssets.Scripts.Game;
using ShopperAssets.Scripts.Interface.Game;
using UnityEngine;
using UnityEngine.UI;

public class DeckUI : MonoBehaviour
{
    //prefab
    [SerializeField] private CardIcon _cardIconUiPrefab;

    [SerializeField] private GameObject _blank;
    //root
    [SerializeField] private GridLayoutGroup _hand;
    [SerializeField] private GridLayoutGroup _action;
    [SerializeField] private Text _deckNum;
    [SerializeField] private Text _trashNum;
    [SerializeField] private RectTransform _handIconRoot;
    [SerializeField] private RectTransform _trashRoot;
    [SerializeField] private RectTransform _deckRoot;
    //hide
    [SerializeField] private RectTransform _trashHide;
    [SerializeField] private RectTransform _deckHide;
    public Action<string> OnClick;
    public Action<string> OnUse;
    private Dictionary<string,CardIcon> Hands = new Dictionary<string, CardIcon>();
    public void SetAction(Action<string> onClick, Action<string> onBuy)
    {
        OnClick = onClick;
        OnUse = onBuy;
    }
    public void ResetUI()
    {
        
        foreach (Transform o in _hand.transform)
        {
            Destroy(o.gameObject);
        }
        foreach (Transform o in _action.transform)
        {
            Destroy(o.gameObject);
        }
        foreach (var keyValuePair in Hands)
        {
            Destroy(keyValuePair.Value.gameObject);
        }
        Hands.Clear();
        _trashNum.text = "";
        _deckNum.text = "";
    }

    public void SetCardsAll(IPlayerUsecase usecase)
    {
        var del = Hands.Keys.ToList();
        SetHandAll(usecase.Hand,del);
        SetActionAll(usecase.ActionArea,del);
        SetDeckAll(usecase.Deck,del);
        SetTrashAll(usecase.Trash,del);
        foreach (var s in del)
        {
            Hands.TryGetValue(s,out var val);
            if (val != null)
            {
                //trash
                //deck
                //remove
                Debug.Log($"remove {s}");
                Destroy(val.gameObject);
                Hands.Remove(s);
            }
        }
    }
    void SetHandAll(List<CardModel> cards,List<string> del)
    {
       
        foreach (Transform o in _hand.transform)
        {
            Destroy(o.gameObject);
        }

        foreach (var cardModel in cards)
        {
            var aim = Instantiate(_blank,_hand.transform);
            aim.name = $"Aim_{cardModel.Name}";
            SetCardIcon(cardModel, aim,OnClick,OnUse);
            del.Remove(cardModel.GUID);
        }
       
    }
    void SetActionAll(List<CardModel> cards,List<string> del)
    {
        foreach (Transform o in _action.transform)
        {
            Destroy(o.gameObject);
        }

        foreach (var cardModel in cards)
        {
            var aim = Instantiate(_blank,_action.transform);
            aim.name = $"Aim_{cardModel.Name}";
            SetCardIcon(cardModel, aim,null,null);
            del.Remove(cardModel.GUID);
        }
    }
    void SetDeckAll(List<CardModel> cards,List<string> del)
    {
        foreach (var cardModel in cards)
        {
            SetCardIcon(cardModel, _deckRoot.gameObject,null,null);
            del.Remove(cardModel.GUID);
        }
    }
    void SetTrashAll(List<CardModel> cards,List<string> del)
    {
        foreach (var cardModel in cards)
        {
            SetCardIcon(cardModel, _trashRoot.gameObject,null,null);
            del.Remove(cardModel.GUID);
        }
    }
    void SetCardIcon(CardModel cardModel,GameObject aim,Action<string> onClick,Action<string> onUse)
    {
        Hands.TryGetValue(cardModel.GUID,out var val);
        if (val == null)
        {
            var cardicon = Instantiate(_cardIconUiPrefab, _handIconRoot);
            cardicon.SetCard(cardModel);
            cardicon.Unique = cardModel.GUID;
            Hands.Add(cardModel.GUID,cardicon);
            val = cardicon;
        }
        val.OnSelected = onClick;
        val.OnExecuted = onUse;
        val.SetAim(aim);
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

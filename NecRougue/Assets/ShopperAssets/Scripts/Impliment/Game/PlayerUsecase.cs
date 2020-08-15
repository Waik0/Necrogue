using System.Collections;
using System.Collections.Generic;
using ShopperAssets.Scripts.Game;
using ShopperAssets.Scripts.Interface.Game;
using ShopperAssets.Scripts.Master;
using UnityEngine;

public class PlayerUsecase : IPlayerUsecase
{
    public List<CardModel> Deck { get; private set; }
    public List<CardModel> Hand { get; private set; }

    public List<CardModel> Trash { get; private set; }

    public List<CardModel> Removed { get; private set; }
    public CharacterModel PlayerCharacter { get; private set; }
    public int Coin { get; private set; }
    public int HandMax { get; private set; }
    public void Reset()
    {
        Deck = new List<CardModel>();
        Hand = new List<CardModel>();
        Trash = new List<CardModel>();
        Coin = 0;
        HandMax = 2;
        
        PlayerCharacter = new CharacterModel()
        {
            Attack = 0,
            Defence =  0,
            Hp = 5,
        };
        SetFirstDeck();
    }

    private void SetFirstDeck()
    {
        var coin = MasterdataManager.Get<ShMstCardRecord>(101);
        var hot = MasterdataManager.Get<ShMstCardRecord>(102);
        Deck.Add(new CardModel().Generate(coin));
        Deck.Add(new CardModel().Generate(coin));
        Deck.Add(new CardModel().Generate(coin));
        Deck.Add(new CardModel().Generate(hot));
        Deck.Add(new CardModel().Generate(hot));
        Deck.Add(new CardModel().Generate(coin));
    }

    public bool TrashToDeckAll()
    {
        
        Deck.AddRange(Trash);
        Trash.RemoveAll(_ => true);
        Deck.Shuffle();
        return Deck.Count > 0;
    }

    public void AddHand(CardModel card)
    {
        Hand.Add(card);
    }

    public void AddCoin(int c)
    {
        Coin++;    
    }

    public void PayCoin(int c)
    {
        Coin -= c;
    }

    public CardModel GetHand(string guid)
    {
        return Hand.Find(_ => _.GUID == guid);
    }

    public CardModel RemoveHand(string guid)
    {
        var target = Hand.Find(_ => _.GUID == guid);
        if (target != null)
        {
            Hand.Remove(target);
            Removed.Add(target);
        }
        return target;
    }

    public CardModel DropHand(string guid)
    {
        var target = Hand.Find(_ => _.GUID == guid);
        if (target != null)
        {
            Hand.Remove(target);
            Trash.Add(target);
        }
        return target;
    }

    public CardModel Draw()
    {
        if (Deck.Count > 0)
        {
            var pop = Deck[0];
            Deck.RemoveAt(0); 
            Hand.Add(pop);
            //引けた
            return pop;
        }
        //引けなかった
        return null;
    }

    public void Damage(int attack)
    {
        PlayerCharacter.Hp -= Mathf.Max(0,attack - PlayerCharacter.Defence);
    }
}

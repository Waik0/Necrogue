using System.Collections;
using System.Collections.Generic;
using ShopperAssets.Scripts.Game;
using ShopperAssets.Scripts.Interface.Game;
using ShopperAssets.Scripts.Master;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.SocialPlatforms;

public class PlayerUsecase : IPlayerUsecase
{
    public List<CardModel> Deck { get; private set; }
    public List<CardModel> Hand { get; private set; }
    public List<CardModel> ActionArea { get; private set; }
    public List<CardModel> Trash { get; private set; }

    public List<CardModel> Removed { get; private set; }
    public PlayerModel PlayerCharacter { get; private set; }
    public UnityEvent OnDamaged { get; } = new UnityEvent();
    public int Coin { get; private set; }
    public int HandMax { get; private set; }
    public int RangeByTurn { get; private set; }
    public int AttackByTurn { get; private set; }
    public void Reset()
    {
        Deck = new List<CardModel>();
        Hand = new List<CardModel>();
        Trash = new List<CardModel>();
        Removed = new List<CardModel>();
        ActionArea = new List<CardModel>();
        Coin = 0;
        HandMax = 4;
        RangeByTurn = 0;
        AttackByTurn = 0;
        PlayerCharacter = new PlayerModel()
        {
            Chara = new CharacterModel()
            {
                Attack = 0,
                Defence = 0,
                Hp = 5,
            }
        };
        SetFirstDeck();
    }

    private void SetFirstDeck()
    {
        var coin = MasterdataManager.Get<ShMstCardRecord>(201);
        var hot = MasterdataManager.Get<ShMstCardRecord>(101);
        
        Deck.Add(new CardModel().Generate(coin));
        Deck.Add(new CardModel().Generate(coin));
        Deck.Add(new CardModel().Generate(coin));
        Deck.Add(new CardModel().Generate(coin));
        Deck.Add(new CardModel().Generate(hot));
        Deck.Add(new CardModel().Generate(hot));
        Deck.Add(new CardModel().Generate(hot));
        Deck.Add(new CardModel().Generate(hot));
        Deck = Deck.Shuffle();
    
    }

    public bool TrashToDeckAll()
    {
        
        Deck.AddRange(Trash);
        Trash.RemoveAll(_ => true);
        Deck = Deck.Shuffle();
        return Deck.Count > 0;
    }

    public void HandToTrashAll()
    {
        Trash.AddRange(Hand);
        Hand.RemoveAll(_ => true);
    }

    public void ActionToTrashAll()
    {
        Trash.AddRange(ActionArea);
        ActionArea.RemoveAll(_ => true);;
    }
    
    public void AddHand(CardModel card)
    {
        Hand.Add(card);
    }

    public void AddCoin(int c)
    {
        Debug.Log("AddCoin");
        Coin += c;    
    }

    public void PayCoin(int c)
    {
        Coin -= c;
    }

    public void AddWall(int num)
    {
        PlayerCharacter.Chara.Shield += num;
    }

    public void AddBarrier()
    {
        PlayerCharacter.Chara.Barrier = true;
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
            Removed.Add(target);
            Hand.Remove(target);
            
        }
        return target;
    }

    public CardModel RemoveAction(string guid)
    {
        var target = ActionArea.Find(_ => _.GUID == guid);
        if (target != null)
        {
            Removed.Add(target);
            ActionArea.Remove(target);
           
        }
        return target;
    }

    public CardModel HandToAction(string guid)
    {
        var target = Hand.Find(_ => _.GUID == guid);
        if (target != null)
        {
            Hand.Remove(target);
            ActionArea.Add(target);
        }
        return target;
    }

    public CardModel ActionToTrash(string guid)
    {
        var target = ActionArea.Find(_ => _.GUID == guid);
        if (target != null)
        {
            ActionArea.Remove(target);
            Trash.Add(target);
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

    public CardModel DropHandRandom()
    {
        if (Hand.Count > 0)
        {
            var target = Hand[Random.Range(0, Hand.Count)];
            if (target != null)
            {
                Hand.Remove(target);
                Trash.Add(target);
                return target;
            }
        }

        return null;

    }

    public CardModel ReverseHand(string guid)
    {
        var target = ActionArea.Find(_ => _.GUID == guid);
        if (target != null)
        {
            ActionArea.Remove(target);
            Hand.Add(target);
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
        if (PlayerCharacter.Chara.Barrier)
        {
            PlayerCharacter.Chara.Barrier = false;
            return;
        }
        var dmg = Mathf.Max(0, attack - PlayerCharacter.Chara.Defence);
        if (PlayerCharacter.Chara.Shield > 0)
        {
            var shieldNum = PlayerCharacter.Chara.Shield;
            var remainDmg = Mathf.Max(0,dmg - shieldNum);
            var remainSld = Mathf.Max(0, shieldNum - dmg);
            PlayerCharacter.Chara.Shield = remainSld;
            dmg = remainDmg;
        }
        PlayerCharacter.Chara.Hp -= dmg;
        OnDamaged?.Invoke();
    }

    public void Heal(int add)
    {
        PlayerCharacter.Chara.Hp += add;
    }

    public void AddRange(int range)
    {
        RangeByTurn += range;
    }

    public void ResetRange()
    {
        RangeByTurn = 0;
    }

    public void AddAttackByTurn(int atk)
    {
        AttackByTurn += atk;
    }

    public void ResetAttackByTurn()
    {
        AttackByTurn = 0;
    }

    public void AddAttackByAttack(int atk)
    {
        throw new System.NotImplementedException();
    }
}

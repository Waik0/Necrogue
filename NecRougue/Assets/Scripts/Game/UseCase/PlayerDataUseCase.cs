using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//----------------------------------------------------------------------------------------------------------------------
//Common
//----------------------------------------------------------------------------------------------------------------------
//GameSequenceで扱うプレイヤー情報
public class PlayerDataUseCase : IEntityUseCase<PlayerData>
{
    public PlayerData _playerData;
    public PlayerData Data
    {
        get => _playerData;
    }

    public PlayerDataUseCase()
    {
        ResetData();
    }
    public bool Pay(int price)
    {
        var canpay = _playerData.Gold >= price;
        if (canpay)
        {
            _playerData.Gold -= price;
        }
        return canpay;
    }
    public void AddGold(int num)
    {
        _playerData.Gold += num;
    }
    //----------------------------------------------------------------------------------------------------------------------
    //デッキ操作
    //----------------------------------------------------------------------------------------------------------------------

    public void MakePlayerDataFromMaster(int id)
    {
        var mst = MasterdataManager.Get<MstCharacterRecord>(id);
        _playerData = new PlayerData().Generate(mst);
    } 
    public void ResetData()
    {
        _playerData = new PlayerData();
        _playerData.Deck = new List<CardData>();
        _playerData.Stock = new List<CardData>();
    }
    //主にバトルから戻った時にデッキを反映するのに使う
    public void SetDeck(List<CardData> cards)
    {
        _playerData.Deck = cards;
    }
    public void SetStock(List<CardData> cards)
    {
        _playerData.Stock = cards;
    }
    //----------------------------------------------------------------------------------------------------------------------
    //ストック操作
    //----------------------------------------------------------------------------------------------------------------------
    public void AddStock(int monsterId)
    {
        var mst = MasterdataManager.Get<MstMonsterRecord>(monsterId);
        AddStock(new CardData().Generate(mst));
    }
    public void AddStock(CardData card)
    {
        _playerData.Stock.Add(card);
    }
    public CardData PopAndRemoveStock(int order)
    {
        var pop = _playerData.Stock[order];
        _playerData.Stock.RemoveAt(order);
        return pop;
    }
}

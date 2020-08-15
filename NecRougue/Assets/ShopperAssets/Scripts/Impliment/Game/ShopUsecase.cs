using System.Collections;
using System.Collections.Generic;
using ShopperAssets.Scripts.Game;
using ShopperAssets.Scripts.Master;
using UnityEngine;

public class ShopUsecase : IShopUsecase
{
    private int GoodsNumPerRank = 20;
    private int ShopLevelMax = 4;
    public int Level { get; private set; }
    public int MaxGoodsNum { get; private set; }
    public List<CardModel> Goods { get; private set; }

    public void Reset()
    {
        Level = 0;
        MaxGoodsNum = 2;
        Goods = new List<CardModel>();
    }

    public void SupplyGoods()
    {
        var cards = MasterdataManager.Records<ShMstCardRecord>();
        var ind = Random.Range(0, cards.Length);
        while (Goods.Count < MaxGoodsNum)
        {
            Goods.Add(new CardModel().Generate(cards[ind]));
            Debug.Log("GoodsAdd");
        }
    }

    public void UpgradeGoodsLevel()
    {
        if (Level < ShopLevelMax) Level++;
    }

    public void UpgradeGoodsNum()
    {
        MaxGoodsNum++;
    }

    public int GetPrice(string guid)
    {
        var target = Goods.Find(_ => _.GUID == guid);
        if (target != null)
        {
            return target.Price;
        }
        return -1;
    }
    public CardModel Buy(string guid)
    {
        var target = Goods.Find(_ => _.GUID == guid);
        if (target != null)
        {
            Goods.Remove(target);
            return target;
        }

        return null;
    }
    
}

using System.Collections;
using System.Collections.Generic;
using System.Linq;
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
        MaxGoodsNum = 3;
        Goods = new List<CardModel>();
       
    }

    public void SupplyGoods()
    {
        var cards = MasterdataManager.Records<ShMstCardRecord>();
       
        while (Goods.Count < MaxGoodsNum)
        {
            var candidates = cards.ToList().ConvertAll(_=>_.id).ToList();
            //candidates.ToList().RemoveAll(id => Goods.Any(goods => goods.Id == id));
            Debug.Log($"Candidate:{candidates.Count}");
            var ind = Random.Range(0, candidates.Count);
            var card = MasterdataManager.Get<ShMstCardRecord>(candidates[ind]);
            Goods.Add(new CardModel().Generate(card));
            Debug.Log("GoodsAdd");
        }
    }

    public int GetUpgradeGoodsLevelPrice()
    {
        return Level * 10;
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

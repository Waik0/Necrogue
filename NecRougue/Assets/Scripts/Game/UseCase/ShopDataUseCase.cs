using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class ShopDataUseCase : IEntityUseCase<ShopData>
{
    private ShopData _shopData;
    public ShopData Data => _shopData;

    public void ResetData()
    {
        _shopData = new ShopData();
        _shopData.Cards = new List<int>(); 
    }
    public List<int> Cards() => _shopData.Cards;
    public void LotteryMonsters(int shopLevel)
    {
        ResetData();
        var count = shopLevel + 2;
        for(var i = 0;i < count; i++)
        {
            var records = MasterdataManager.Records<MstMonsterRecord>();
            var targets = records.Where(_ => _.rarity <= shopLevel && _.rarity >= shopLevel - 2).ToList();
            if(targets.Count <= 0)
            {
                continue;
            }
            var r = Random.Range(0, targets.Count);
            _shopData.Cards.Add(targets[r].Id);
        }
    }
    public void Remove(int order)
    {
        _shopData.Cards.RemoveAt(order);
    }
    public BattleCard GetBattleCardData(int order)
    {
        var mst = MasterdataManager.Get<MstMonsterRecord>(_shopData.Cards[order]);
        return new BattleCard().Generate(mst);
    }
}

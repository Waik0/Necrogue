using System.Collections;
using System.Collections.Generic;
using ShopperAssets.Scripts.Game;
using UnityEngine;


public interface IShopUsecase
{
    int Level { get; }
    int MaxGoodsNum { get; }
    List<CardModel> Goods { get; }
    
    void Reset();
    void SupplyGoods(); //商品補給
    void UpgradeGoodsLevel(); //商品ランクアップ
    void UpgradeGoodsNum(); //陳列数アップ
    CardModel Buy(string guid);//購入
    int GetPrice(string guid);

}
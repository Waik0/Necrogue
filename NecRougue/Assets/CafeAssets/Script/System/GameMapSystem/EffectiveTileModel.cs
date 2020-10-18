using UnityEngine;

namespace CafeAssets.Script.System.GameMapSystem
{
    public enum TileEffectType
    {
        Sit,//座る
        Table,//テーブル(飲食物等設置可能)
        Order,//注文
        
        
        
    }
    //
    //キャラクターが乗るか隣接すると効果が発揮されるタイル
    //IsWall = trueなら隣接 falseなら乗る
    //テーブル、レジ、コーヒーメーカーなど想定
    public class EffectiveTileModel : TileModel
    {
        [Header("効果種類")] 
        public TileEffectType EffectType;
        [Header("作業時間")]
        //注文や飲食物作成にかかる時間
        //todo キャラのパラメーターで短縮かのうにする
        public float ActionTime;
        
        
        
    }
}

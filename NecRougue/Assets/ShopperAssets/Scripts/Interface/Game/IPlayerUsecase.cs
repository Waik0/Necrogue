using System.Collections.Generic;
using ShopperAssets.Scripts.Game;
using UnityEngine.Events;

namespace ShopperAssets.Scripts.Interface.Game
{
    public interface IPlayerUsecase
    {
        List<CardModel> Deck { get; }
        List<CardModel> Hand { get; }
        List<CardModel> Trash { get; }
        List<CardModel> Removed { get; }
        PlayerModel PlayerCharacter { get; }
        UnityEvent OnDamaged { get; }
        int Coin { get; }
        int HandMax { get; }
        void Reset();
        bool TrashToDeckAll();
        void HandToTrashAll();
        void AddHand(CardModel card);
        void AddCoin(int c);
        void PayCoin(int c);
        void AddWall(int num);
        CardModel GetHand(string guid);
        CardModel RemoveHand(string guid);
        CardModel DropHand(string guid);//使うなどで住手札に行く
        
        CardModel ReverseHand(string guid);//ゴミから拾う
        CardModel Draw();
        
        void Damage(int attack);
        
    }
}

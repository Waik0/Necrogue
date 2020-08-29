using System.Collections.Generic;
using ShopperAssets.Scripts.Game;
using UnityEngine.Events;

namespace ShopperAssets.Scripts.Interface.Game
{
    public interface IPlayerUsecase
    {
        List<CardModel> Deck { get; }
        List<CardModel> Hand { get; }
        List<CardModel> Action { get; }
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
        void AddBarrier();
        CardModel GetHand(string guid);
        CardModel RemoveHand(string guid);
        CardModel DropHand(string guid);//使うなどで住手札に行く
        CardModel DropHandRandom();
        CardModel ReverseHand(string guid);//ゴミから拾う
        CardModel Draw();
        
        void Damage(int attack);
        void Heal(int add);
        void AddRange(int range);
        void ResetRange();
        void AddAttackByTurn(int atk);
        void ResetAttackByTurn();
        void AddAttackByAttack(int atk);
    }
}

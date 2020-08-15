using System.Collections.Generic;
using ShopperAssets.Scripts.Game;

namespace ShopperAssets.Scripts.Interface.Game
{
    public interface IPlayerUsecase
    {
        List<CardModel> Deck { get; }
        List<CardModel> Hand { get; }
        List<CardModel> Trash { get; }
        List<CardModel> Removed { get; }
        CharacterModel PlayerCharacter { get; }
        int Coin { get; }
        int HandMax { get; }
        void Reset();
        bool TrashToDeckAll();
        
        void AddHand(CardModel card);
        void AddCoin(int c);
        void PayCoin(int c);
        CardModel GetHand(string guid);
        CardModel RemoveHand(string guid);
        CardModel DropHand(string guid);//使うなどで住手札に行く
        CardModel Draw();
        
        void Damage(int attack);
        
    }
}

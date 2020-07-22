
using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using Random = UnityEngine.Random;

namespace ShopperAssets.Scripts.Game
{
    public class GameUseCase
    {
        private PlayerModel _playerModel = new PlayerModel();
        private ShopModel _shopModel = new ShopModel();
        private EnemyDeckModel _enemyDeckModel = new EnemyDeckModel();
        private AbilityResolver _ability = new AbilityResolver();
        public List<CardModel> PlayerHand => _playerModel.Hand;
        public List<CardModel> ShopGoods => _shopModel.Goods;
        public int PlayerHandMax => _playerModel.HandMax;
        public int PlayerHandCount => _playerModel.Hand.Count;

        public void Reset()
        {
            _playerModel.Create();
            _enemyDeckModel.Create();
            _shopModel.Create();
        }

        #region 敵操作

        public void EnemyMoveForward()
        {
            if (_enemyDeckModel.Field.Count > 0)
            {
                if (_enemyDeckModel.Field[0] == null || _enemyDeckModel.Field[0].Hp < 0)
                {
                    _enemyDeckModel.Field.RemoveAt(0);
                }
            }
            if (_enemyDeckModel.Field.Count < _enemyDeckModel.EnemyCount && _enemyDeckModel.Deck.Count > 0)
            {
                var pop = _enemyDeckModel.Deck[0];
                _enemyDeckModel.Deck.Remove(pop);
                _enemyDeckModel.Field.Add(pop);
                
            }
        }


        #endregion
        #region ショップ操作
        /// <summary>
        /// ラインナップ更新
        /// </summary>
        public void UpdateShop()
        {
            _shopModel.UpdateGoods();
        }
        /// <summary>
        /// ショップ強さアップ
        /// </summary>
        public void UpgradeShopLineup()
        {
            if (_shopModel.Level < 4) _shopModel.Level++;
        }

        /// <summary>
        /// ショップ表示数アップ
        /// </summary>
        public void UpgradeShopView()
        {
            _shopModel.GoodsNum++;
        }
        public void BuyRandom()
        {
            var id = _shopModel.Goods[Random.Range(0, _shopModel.Goods.Count - 1)].GUID;
            Buy(id);
        }
        public void Buy(string guid)
        {
            var target = _shopModel.Goods.Find(_ => _.GUID == guid);
            if (target != null)
            {
                _shopModel.Goods.Remove(target);
                _playerModel.Deck.Add(target);
                UseAbility(AbilityResolver.AbilityTiming.Get,target );
                return;
            }
        }
        

        #endregion


        public void UseHand(string guid)
        {
            var target = _playerModel.Hand.Find(_ => _.GUID == guid);
            UseAbility(AbilityResolver.AbilityTiming.Use, target);
            //残ってたらすて札に
            var remain = _playerModel.Hand.Find(_ => _.GUID == guid);
            if (remain != null)
            {
                _playerModel.Hand.Remove(remain);
                _playerModel.Trash.Add(remain);
            }
        }

        #region 捨札操作

        
        /// <summary>
        /// 山札が無くなった時に捨札をデッキに戻すやつ
        /// </summary>
        public bool TrashToDeckAll()
        {
            _playerModel.Deck.AddRange(_playerModel.Trash);
            _playerModel.Trash.RemoveAll(_ => true);
            _playerModel.Deck.Shuffle();
            //デッキが無くなったらfalse
            return _playerModel.Deck.Count > 0;
        }

        #endregion

        #region 手札操作

        /// <summary>
        /// 除外(GUID指定)
        /// </summary>
        /// <param name="handIndex"></param>
        public void RemoveHand(string guid)
        {
            var remain = _playerModel.Hand.Find(_ => _.GUID == guid);
            if (remain != null)
            {
                _playerModel.Hand.Remove(remain);
                _playerModel.Removed.Add(remain);
            }
        }

        /// <summary>
        /// 除外
        /// </summary>
        /// <param name="handIndex"></param>
        public void RemoveHand(int handIndex)
        {
            if (handIndex >= 0 && handIndex < _playerModel.Hand.Count)
            {
                var pop = _playerModel.Hand[handIndex];
                _playerModel.Hand.RemoveAt(handIndex);
                _playerModel.Removed.Add(pop);
            }
        } 
        /// <summary>
        ///手札を引く
        /// </summary>
        public bool Draw()
        {
            if (_playerModel.Deck.Count > 0)
            {
                var pop = _playerModel.Deck[0];
                _playerModel.Deck.RemoveAt(0);
                _playerModel.Hand.Add(pop);
                UseAbility(AbilityResolver.AbilityTiming.Get, pop);
                //引けた
                return true;
            }
            //引けなかった
            return false;
        }
        /// <summary>
        /// 手札をランダムに捨てさせる
        /// </summary>
        public bool DropHand()
        {
            if (_playerModel.Hand.Count > 0)
            {
                var ind = UnityEngine.Random.Range(0, _playerModel.Hand.Count);
                var pop = _playerModel.Hand[ind].GUID;
                return DropHand(pop);
            }

            return false;

        }
        /// <summary>
        /// 手札を捨てさせる
        /// </summary>
        /// <param name="guid"></param>
        /// <returns></returns>
        public bool DropHand(string guid)
        {
            var target = _playerModel.Hand.Find(_ => _.GUID == guid);
            if (target != null)
            {
                _playerModel.Hand.Remove(target);
                _playerModel.Trash.Add(target);
                UseAbility(AbilityResolver.AbilityTiming.Trash, target);
                return true;
            }
            return false;
        }
        #endregion

        #region 手札、敵効果発動

        private void UseAbility(AbilityResolver.AbilityTiming nowTiming, CardModel card)
        {
            foreach (var abilityModel in card.Abilities)
            {
                if (abilityModel.Timing == nowTiming)
                {
                    _ability.Commands.TryGetValue(abilityModel.Command, out var command);
                    command?.Invoke(card.GUID,abilityModel.AbilityParam1,abilityModel.AbilityParam2);
                }
            }
        }

        #endregion

        #region 攻撃

        public void AttackToEnemy(int range,int damage)
        {
            if (_enemyDeckModel.Field[range] != null)
            {
                _enemyDeckModel.Field[range].Hp -= damage;
            }
        }

        public void AttackToPlayer(int damage)
        {
            _playerModel.playerCharacter.Hp -= damage;
        }

        #endregion
    }
}

public static class ListExtensions
{
    public static void Shuffle<T>(this List<T> list)
    {
        list = list.OrderBy(i => Guid.NewGuid()).ToList();
    }
}
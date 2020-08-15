using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using ShopperAssets.Scripts.Interface.Game;
using Toast;
using UnityEngine;
using Zenject;

namespace ShopperAssets.Scripts.Game
{
    public class GamePresenter
    {
        [Inject] private IEnemyUsecase _enemyUseCase;
        [Inject] private IPlayerUsecase _playerUseCase;
        [Inject] private IShopUsecase _shopUsecase;
        [Inject] private AbilityResolver _ability;
        private GameView _gameView;
        public bool IsEndTurn;
        public GamePresenter(GameView gameView)
        {
            _gameView = gameView;
            Init();
        }

        void Init()
        {
            DebugLog.Function(this);
            _gameView.EndTurnButton.onClick.AddListener(() => IsEndTurn = true);
        }

        CardModel FindCard(string guid)
        {
            var c = _playerUseCase.Hand.Find(_ => _.GUID == guid);
            if (c == null) c = _shopUsecase.Goods.Find(_ => _.GUID == guid);
            if (c == null) c = _playerUseCase.Deck.Find(_ => _.GUID == guid);
            if (c == null) c = _playerUseCase.Trash.Find(_ => _.GUID == guid);
            if (c == null) c = _playerUseCase.Removed.Find(_ => _.GUID == guid);
            return c;
        }
        void OnClickCard(string guid)
        {
            //詳細を出す
            Debug.Log("Selected:"+guid);
            var c = FindCard(guid);
            _gameView.CardIconUI.ResetCard();
            if (c != null)
            {
                _gameView.CardIconUI.SetCard(c);
            }
        }
        /// <summary>
        /// 手札の効果発動
        /// </summary>
        /// <param name="guid"></param>
        public void UseCard(string guid)
        {
            Debug.Log("Use:"+ guid);
            var target = _playerUseCase.GetHand(guid);
            if (target != null)
            {
                _ability.UseAbility(AbilityResolver.AbilityTiming.Use, target);
                //手札に残ってたらすてる
                var pop = _playerUseCase.DropHand(guid);
                if (pop != null)
                {
                    _ability.UseAbility(AbilityResolver.AbilityTiming.Trash, pop);
                }
                
            }
            
           
            //UI更新
            UpdateUI();
        }

     

        public void BuyCard(string guid)
        {
            if (_shopUsecase.GetPrice(guid) > _playerUseCase.Coin)
            {
                Debug.Log("CantBuy");
                return;
            }

            _playerUseCase.PayCoin( _shopUsecase.GetPrice(guid) );
            Debug.Log("Buy:" + guid);
            var goods = _shopUsecase.Buy(guid);
            if (goods != null)
            {
                _playerUseCase.AddHand(goods);
                _ability.UseAbility(AbilityResolver.AbilityTiming.Get, goods);
            }
            //UI更新
            UpdateUI();
        }
     


        public void UpdateShop()
        {
            _shopUsecase.SupplyGoods();
            //UI更新
            UpdateUI();
        }
        public void Reset()
        {
            _playerUseCase.Reset();
            _enemyUseCase.Reset();
            _shopUsecase.Reset();
        }

        private void UpdateUI()
        {
            _gameView.CardIconUI.ResetCard();
            _gameView.ShopUI.SetShopAll(_shopUsecase.Goods,OnClickCard,BuyCard);
            _gameView.ShopUI.SetCoin(_playerUseCase.Coin);
            _gameView.DeckUI.SetHandAll(_playerUseCase.Hand,OnClickCard,UseCard);
            _gameView.DeckUI.SetDeck(_playerUseCase.Deck.Count);
            _gameView.DeckUI.SetTrash(_playerUseCase.Trash.Count);
            _gameView.PlayerUI.SetStatus(_playerUseCase.PlayerCharacter);
        }
        public void UpdateEnemy()
        {
            //倒されてたら敵前進
            _enemyUseCase.MoveForward();
            
        }
        /// <summary>
        /// 手札最大まで引く
        /// </summary>
        public void DrawMax()
        {
            while(_playerUseCase.Hand.Count < _playerUseCase.HandMax)
                if (_playerUseCase.Draw() == null)
                    if (!_playerUseCase.TrashToDeckAll())
                        break;
        }
        
        
  
        
    }
}

public static class ListExtensions
{
    public static void Shuffle<T>(this List<T> list)
    {
        list = list.OrderBy(i => Guid.NewGuid()).ToList();
    }
}
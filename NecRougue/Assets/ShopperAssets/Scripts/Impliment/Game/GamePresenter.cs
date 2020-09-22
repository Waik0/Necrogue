using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using ShopperAssets.Scripts.Interface.Game;
using Toast;
using UnityEngine;
using UnityEngine.Events;
using Zenject;

namespace ShopperAssets.Scripts.Game
{
    public class CardEvent : UnityEvent<string>{}
    public class GamePresenter
    { 
        private IEnemyUsecase _enemyUseCase;
        private IPlayerUsecase _playerUseCase; 
        private IShopUsecase _shopUsecase;
        private GameInputController _inputController;
        private GameView _gameView;
        public UnityEvent OnGameClear = new UnityEvent();
        public UnityEvent OnGameOver = new UnityEvent();
        public int EnemyCount => _enemyUseCase.EnemyCount;
        public GamePresenter(
            GameView gameView,
            IShopUsecase shopUsecase,
            IEnemyUsecase enemyUsecase,
            IPlayerUsecase playerUsecase,
            GameInputController inputController)
        {
            _gameView = gameView;
            _enemyUseCase = enemyUsecase;
            _playerUseCase = playerUsecase;
            _shopUsecase = shopUsecase;
            _inputController = inputController;
            Init();
        }
        void Init()
        {
            DebugLog.Function(this);
            
            _enemyUseCase.OnDamaged.AddListener(CheckEnemyDead);
            _playerUseCase.OnDamaged.AddListener( CheckPlayerDead);
         
        }

        void CheckEnemyDead()
        {
            //敵死亡チェック
            _enemyUseCase.CheckDead();
            _gameView.EnemyUI.SetEnemies(_enemyUseCase.Field);
        }

        void CheckPlayerDead()
        {
            //死亡チェック
            if (_playerUseCase.PlayerCharacter.Chara.Hp <= 0) 
            {//復活不可能であれば
                OnGameOver.Invoke();
            }
        }

        public int HandCount()
        {
            return _playerUseCase.Hand.Count;
        }

        public CardModel UseHand(string guid)
        {
            return _playerUseCase.HandToAction(guid);
        }
        public CardModel PopHand(string guid)
        {
            return _playerUseCase.DropHand(guid);
        }

        public void ReverseToHand(string guid)
        {
            _playerUseCase.ReverseHand(guid);
        }

        public List<CardModel> AllHand() => _playerUseCase.Hand;
        public CardModel FindHand(string guid)
        {
            var c = _playerUseCase.Hand.Find(_ => _.GUID == guid);
            return c;
        }
        public CardModel FindTrash(string guid)
        {
            var c = _playerUseCase.Trash.Find(_ => _.GUID == guid);
            return c;
        }
        CardModel FindCard(string guid)
        {
            var c = _playerUseCase.Hand.Find(_ => _.GUID == guid);
            if (c == null) c = _shopUsecase.Goods.Find(_ => _.GUID == guid);
            if (c == null) c = _shopUsecase.ShopLevelUpGoods.Find(_ => _.GUID == guid);
            if (c == null) c = _playerUseCase.Deck.Find(_ => _.GUID == guid);
            if (c == null) c = _playerUseCase.Trash.Find(_ => _.GUID == guid);
            if (c == null) c = _playerUseCase.Removed.Find(_ => _.GUID == guid);
            return c;
        }
        public void SelectCard(string guid)
        {
            Debug.Log("Selected:"+guid);
        }

        public void DeselectCard(string guid)
        {
            
        }
        public void DeselectCardAll()
        {
            
        }
        /// <summary>
        /// 詳細を表示
        /// </summary>
        /// <param name="guid"></param>
        public void ViewCardInfo(string guid)
        {
            var c = FindCard(guid);
            _gameView.CardIconUI.ResetCard();
            if (c != null)
            {
                _gameView.CardIconUI.SetCard(c);
            }
            
        }


        public void ShopLevelUp()
        {
            if (_shopUsecase.GetUpgradeGoodsLevelPrice() > _playerUseCase.Coin)
            {
                Debug.Log("CantLevelUp");
                return;
            }

            _shopUsecase.UpgradeGoodsLevel();
        }

        public bool BuyCard(string guid)
        {
            if (_shopUsecase.GetPrice(guid) > _playerUseCase.Coin)
            {
                Debug.Log("CantBuy");
                return false;
            }

            _playerUseCase.PayCoin( _shopUsecase.GetPrice(guid) );
            Debug.Log("Buy:" + guid);
            var goods = _shopUsecase.Buy(guid);
            if (goods != null)
            {
                _playerUseCase.AddHand(goods);
            }

            return true;
        }

        public int EnemyTurn(int index)
        {
            if (_enemyUseCase.Field.Count <= index || _enemyUseCase.Field[index] == null)
                return -1;
            var abilityIndex = _enemyUseCase.EnemyTurn(index);
            return abilityIndex;
        }
        public EnemyModel FieldEnemy(int index)
        {
            if (_enemyUseCase.Field.Count <= index || _enemyUseCase.Field[index] == null)
                return null;
            return _enemyUseCase.Field[index];
        }
        public void Reset()
        {
            _playerUseCase.Reset();
            _enemyUseCase.Reset();
            _shopUsecase.Reset();
        }
        public void UpdateShop()
        {
            _shopUsecase.SupplyGoods();
        }

        public void UpdateShopLevelUpGoods()
        {
            _shopUsecase.SupplyShopLevelUpCard();
        }
        //
        public void UpdateUI()
        {
            _gameView.CardIconUI.ResetCard();
            _gameView.ShopUI.SetShopAll(_shopUsecase.Goods);
            _gameView.ShopUI.SetShopLevelUpGoodsAll(_shopUsecase.ShopLevelUpGoods);
            _gameView.ShopUI.SetCoin(_playerUseCase.Coin);
            _gameView.DeckUI.SetCardsAll(_playerUseCase);
            _gameView.DeckUI.SetDeck(_playerUseCase.Deck.Count);
            _gameView.DeckUI.SetTrash(_playerUseCase.Trash.Count);
            _gameView.PlayerUI.SetStatus(_playerUseCase.PlayerCharacter.Chara);
        }
        public void UpdateEnemy()
        {
            //倒されてたら敵前進
            _enemyUseCase.MoveForward();
            _gameView.EnemyUI.SetEnemies(_enemyUseCase.Field);
            
        }

        public bool TrashAllAction()
        {
            var haveAction = _playerUseCase.ActionArea.Count > 0;
            _playerUseCase.ActionToTrashAll();
            return haveAction;
        }
        /// <summary>
        /// 手札全捨て
        /// </summary>
        public bool TrashAllHand()
        {
            var haveHand = _playerUseCase.Hand.Count > 0;
            _playerUseCase.HandToTrashAll();
            return haveHand;
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

        public bool IsHandDrawMax()
        {
            return _playerUseCase.Hand.Count >= _playerUseCase.HandMax;
        }
        public CardModel Draw()
        {
            return _playerUseCase.Draw();
        }
        public bool TrashToDeckAll()
        {
            return _playerUseCase.TrashToDeckAll();
        }
        
        
        
    }
}

public static class ListExtensions
{
    public static List<T> Shuffle<T>(this List<T> list)
    {
        Debug.Log(Guid.NewGuid());
        return list.OrderBy(i => Guid.NewGuid()).ToList();
    }
}
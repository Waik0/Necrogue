using System.Collections;
using System.Collections.Generic;
using Toast;
using UnityEngine;
using Zenject;

namespace ShopperAssets.Scripts.Game
{
    public class GamePresenter
    {
        [Inject] private GameUseCase _gameUseCase;
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

        void OnClickCard(string guid)
        {
            //詳細を出す
            Debug.Log("Selected:"+guid);
        }
        /// <summary>
        /// 手札の効果発動
        /// </summary>
        /// <param name="guid"></param>
        void OnUseCard(string guid)
        {
            Debug.Log("Use:"+ guid);
            _gameUseCase.UseHand(guid);
            //UI更新
            _gameView.DeckUI.SetShopAll(_gameUseCase.ShopGoods,OnClickCard,OnBuyCard);
            _gameView.DeckUI.SetHandAll(_gameUseCase.PlayerHand,OnClickCard,OnUseCard);
        }

        void OnBuyCard(string guid)
        {
            Debug.Log("Buy:" + guid);
            _gameUseCase.Buy(guid);
            //UI更新
            _gameView.DeckUI.SetShopAll(_gameUseCase.ShopGoods,OnClickCard,OnBuyCard);
            _gameView.DeckUI.SetHandAll(_gameUseCase.PlayerHand,OnClickCard,OnUseCard);
        }
        public void Reset()
        {
            _gameUseCase.Reset();
        }


        public void UpdateShop()
        {
            _gameUseCase.UpdateShop();
            //UI更新
            _gameView.DeckUI.SetShopAll(_gameUseCase.ShopGoods,OnClickCard,OnBuyCard);
            _gameView.DeckUI.SetHandAll(_gameUseCase.PlayerHand,OnClickCard,OnUseCard);
        }

        public void UpdateEnemy()
        {
            //倒されてたら敵前進
            _gameUseCase.EnemyMoveForward();
            
        }
        /// <summary>
        /// 手札最大まで引く
        /// </summary>
        public void DrawMax()
        {
            while(_gameUseCase.PlayerHandCount < _gameUseCase.PlayerHandMax)
                if (!_gameUseCase.Draw())
                    if (!_gameUseCase.TrashToDeckAll())
                        break;
        }
        

    }
}
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using Zenject;

namespace ShopperAssets.Scripts.Game
{
    public class GameSequence : SequenceBase<GameSequence.State,GameSequenceReturnModel>
    {
        //injects
        private GamePresenter _gamePresenter;
        private GameView _gameView;
        private GameInputPresenter _gameInputPresenter;
        private AbilityPresenter _abilityPresenter;
        //private AwaitAbilityInput _awaitAbilityInput;
        private ITextDialogPresenter _textDialogPresenter;
        
        //private
        private bool _endSelect;
        private bool _cancelSelect;
        public enum State
        {
           
            Init,
            Menu,
            UseCard,
            BuyCard,
            TurnEnd,
            EnemyTurn,
            GameOver,

        }


        public GameSequence(
            GamePresenter gamePresenter,
            GameView gameView,
            AbilityPresenter abilityPresenter,
            GameInputPresenter gameInputController,
            ITextDialogPresenter textDialogPresenter)
        {
            _gameView = gameView;
            _abilityPresenter = abilityPresenter;
            _gameInputPresenter = gameInputController;
            _gamePresenter = gamePresenter;
            _textDialogPresenter = textDialogPresenter;

        }
        
        /// <summary>
         /// ゲーム開始時
         /// </summary>
         /// <returns></returns>
        IEnumerator Init()
        {
            DebugLog.Function(this);
            yield return null;
            _gamePresenter.Reset();
            _gameView.ResetUI();
            _gamePresenter.DrawMax();
            
            NextState(State.Menu);
        }
        /// <summary>
        /// 操作受付
        /// </summary>
        /// <returns></returns>
        IEnumerator Menu()
        {
            DebugLog.Function(this);
            _gamePresenter.UpdateShop();
            _gamePresenter.UpdateUI();
            _gameInputPresenter.ChangeCommand(InputCommand.Normal);
            while (true)
            {
                switch (_gameInputPresenter.MenuInputState)
                {
                    case MenuInputState.UseCard:
                        NextState(State.UseCard);
                        break;
                    case MenuInputState.BuyCard:
                        NextState(State.BuyCard);
                        break;
                    case MenuInputState.EndTurn:
                        NextState(State.TurnEnd);
                        break;
                }

                yield return null;
            }
        }

        IEnumerator UseCard()
        {
            _gameInputPresenter.ChangeCommand(InputCommand.None);
            //todo 共通化---
            _cancelSelect = false;
            _endSelect = false;
            //---
            DebugLog.Function(this);
            var cardGUID = _gameInputPresenter.UsingCard;
            var card = _gamePresenter.PopHand(cardGUID);
            if (card != null)
            {
                yield return _abilityPresenter.ResolveAbility(AbilityUseCase.AbilityTiming.Use, card,true, () =>
                {
                    _gamePresenter.ReverseToHand(card.GUID);
                    Debug.Log($"キャンセルされたので戻す");
                });
               
            }
            var cardTrash = _gamePresenter.FindTrash(cardGUID);
            if (cardTrash != null)
            { 
                yield return _abilityPresenter.ResolveAbility(AbilityUseCase.AbilityTiming.Trash, cardTrash,true,null);
            }
           
            //todo 共通化---
            _cancelSelect = false;
            _endSelect = false;
            //---
            
            _gamePresenter.UpdateUI();
            //ゲームオーバー判定のためここで一時停止
            yield return null;
           
          
            NextState(State.Menu);
        }

        IEnumerator BuyCard()
        {
            DebugLog.Function(this);
            _gameInputPresenter.ChangeCommand(InputCommand.None);
            var buyCard = _gameInputPresenter.BuyingCard;
            var buy = _gamePresenter.BuyCard(buyCard);
            if (buy)
            {
                var card = _gamePresenter.FindHand(buyCard);
                yield return _abilityPresenter.ResolveAbility(AbilityUseCase.AbilityTiming.Get,card ,false,null);
            }
            _gamePresenter.UpdateUI();
            //ゲームオーバー判定のためここで一時停止
            yield return null;
            NextState(State.Menu);
            yield return null;
        }
        IEnumerator TurnEnd()
        {
            DebugLog.Function(this);
            _gameInputPresenter.ChangeCommand(InputCommand.None);
            //全部捨てる
            _gamePresenter.TrashAllHand();
            //最大まで引く
            _gamePresenter.DrawMax();
            NextState(State.EnemyTurn);
            yield return null;
        }
        IEnumerator EnemyTurn()
        {
            DebugLog.Function(this);
            _gameInputPresenter.ChangeCommand(InputCommand.None);
            for (int i = 0; i < _gamePresenter.EnemyCount; i++)
            {
                var enemy = _gamePresenter.FieldEnemy(i);
                if (enemy == null)
                {
                    continue;
                }
                var index = _gamePresenter.EnemyTurn(i);
                yield return _abilityPresenter.ResolveAbilityEnemy(AbilityUseCase.AbilityTiming.EnemyTurn, enemy, index);
            }
            //ゲームオーバー判定のためここで一時停止
            yield return null;
            //敵を前進
            _gamePresenter.UpdateEnemy();
            NextState(State.Menu);
            yield return null;
        }

        IEnumerator GameOver()
        {
            DebugLog.Function(this);
            _gameInputPresenter.ChangeCommand(InputCommand.GameOver);
            yield return null;
            while ( !(Input.GetMouseButtonDown(0)))
            {
                yield return null;
            }
            NextState(State.Init);
        }
  
        
        
    }

}

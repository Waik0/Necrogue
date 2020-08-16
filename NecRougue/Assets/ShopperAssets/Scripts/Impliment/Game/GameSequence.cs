using System.Collections;
using UnityEngine;
using Zenject;

namespace ShopperAssets.Scripts.Game
{
    public class GameSequence : SequenceBase<GameSequence.State,GameSequenceReturnModel>
    {
        private GamePresenter _gamePresenter;
       
        public enum State
        {
            Init,
            Menu,
            TurnEnd,
            EnemyTurn,
            GameOver,

        }

        public GameSequence(GamePresenter gamePresenter)
        {
            _gamePresenter = gamePresenter;
            _gamePresenter.OnGameOver.AddListener(() => NextState(State.GameOver) );
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
            while (!_gamePresenter.IsEndTurn)
            {
                yield return null;
            }
            NextState(State.TurnEnd);
            yield return null;
        }
        IEnumerator TurnEnd()
        {
            DebugLog.Function(this);
            //全部捨てる
            _gamePresenter.TrashAllHand();
            //最大まで引く
            _gamePresenter.DrawMax();
            _gamePresenter.IsEndTurn = false;
            NextState(State.EnemyTurn);
            yield return null;
        }
        IEnumerator EnemyTurn()
        {
            DebugLog.Function(this);
            for (int i = 0; i < _gamePresenter.EnemyCount; i++)
            {
                _gamePresenter.EnemyTurn(i);
                //演出
                yield return null;
            }
            //敵を前進
            _gamePresenter.UpdateEnemy();
            NextState(State.Menu);
            yield return null;
        }

        IEnumerator GameOver()
        {
            DebugLog.Function(this);
            yield return null;
            while ( !(Input.GetMouseButtonDown(0)))
            {
                yield return null;
            }
            NextState(State.Init);
        }
    }

}

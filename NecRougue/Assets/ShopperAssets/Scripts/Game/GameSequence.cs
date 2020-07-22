using System.Collections;
using Zenject;

namespace ShopperAssets.Scripts.Game
{
    public class GameSequence : SequenceBase<GameSequence.State,GameSequenceReturnModel>
    {
        [Inject] private GamePresenter _gamePresenter;
       
        public enum State
        {
            Init,
            Menu,
            TurnEnd,
            EnemyTurn

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
            //最大まで引く
            _gamePresenter.DrawMax();
            //敵を前進
            _gamePresenter.UpdateEnemy();
            _gamePresenter.IsEndTurn = false;
            NextState(State.EnemyTurn);
            yield return null;
        }
        IEnumerator EnemyTurn()
        {
            DebugLog.Function(this);
           
            NextState(State.Menu);
            yield return null;
        }
    }

}

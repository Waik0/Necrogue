using System;
using System.Collections;
using ShopperAssets.Scripts.Game;
using UnityEngine;
using Zenject;

namespace ShopperAssets.Scripts.Sequence
{
    public class AllSequence : InitialSequenceBase<AllSequence.State>
    {
        public enum State
        {
            Init,
            Advertize,
            Menu,
            Game,
            Ending,
        }
        
        [Inject] private GameSequence _gameSequence;
        
        IEnumerator Init()
        {
            DontDestroyOnLoad(gameObject);
            DebugLog.Function(this);
            NextState(State.Advertize);
            yield return null;
        }

        IEnumerator Advertize()
        {
            DebugLog.Function(this);
            NextState(State.Menu);
            yield return null;
        }
        
        IEnumerator Menu()
        {
            DebugLog.Function(this);
            NextState(State.Game);
            yield return null;
        }
        /// <summary>
        /// State.Game
        /// </summary>
        /// <returns></returns>
        IEnumerator Game()
        {
            DebugLog.Function(this);
            yield return _gameSequence.UpdateCoroutine(); 
            switch (_gameSequence.GetReturn().State)
            {
                case GameSequenceReturnModel.EndState.Error:
                    NextState(State.Init);
                    break;
                case GameSequenceReturnModel.EndState.Over:
                    NextState(State.Advertize);
                    break;
                case GameSequenceReturnModel.EndState.Clear:
                    NextState(State.Ending);
                    break;
            }
        }
        /// <summary>
        /// State.Ending
        /// </summary>
        /// <returns></returns>
        IEnumerator Ending()
        {
            yield return null;
        }
    }
}

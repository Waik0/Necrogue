using System.Collections;
using Toast;
using UnityEngine;
using UnityEngine.SceneManagement;
using Zenject;

namespace CafeAssets.Script.Sequence
{
    public class RootSequence : ISequence
    {
        public enum State
        {
            Init,
            Game,
            End,
        }

        private Statemachine<State> _statemachine;
        private GameSequence _gameSequence;
        public RootSequence(
            GameSequence gameSequence
            )
        {
            _gameSequence = gameSequence;
            _statemachine = new Statemachine<State>();
            _statemachine.Init(this);
        }
        IEnumerator Init()
        {
            Debug.Log("[Root]Initialize");
            Application.targetFrameRate = 60;
            _statemachine.Next(State.Game);
            yield return null;
        }
        IEnumerator Game()
        {
            Debug.Log("[Root]Game");
            SceneManager.LoadScene("Game");
            while (_gameSequence.UpdateState())
            {
                yield return null;
            }
            _statemachine.Next(State.End);
           
        }
        public bool UpdateState()
        {
            _statemachine.Update();
            return _statemachine.Current != State.End;
        }
    }
}

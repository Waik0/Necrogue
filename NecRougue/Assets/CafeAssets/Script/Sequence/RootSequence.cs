using System.Collections;
using Toast;
using UnityEngine;
using UnityEngine.Assertions;
using UnityEngine.SceneManagement;
using Zenject;

namespace CafeAssets.Script.Sequence
{
    public class RootSequence : MonoBehaviour,ISequence
    {
        public enum State
        {
            Init,
            Game,
            End,
        }

        private Statemachine<State> _statemachine;
        void Awake()
        {
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
            yield return LoadScene("Game");
            var gameSequence = GameObject.FindObjectOfType<GameSequence>();
            Assert.IsNotNull(gameSequence);
            while (gameSequence.UpdateState())
            {
                yield return null;
            }
            _statemachine.Next(State.End);
           
        }

        IEnumerator LoadScene(string name)
        {
            Debug.Log("[Root]" + name);
            var asyncOperation = SceneManager.LoadSceneAsync(name);
            while (!asyncOperation.isDone)
            {
                yield return null;
            }
        }
        public bool UpdateState()
        {
            _statemachine.Update();
            return _statemachine.Current != State.End;
        }
    }
}

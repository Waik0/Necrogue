using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Zenject;

namespace CafeAssets.Script.System.GameCoreSystem
{
    public interface IGameResettable
    {
        //resetだとmonobehaviourのと被る
        void ResetOnGame();
    }

    public interface IGameResettableManager
    {
        void ResetOnGame();
    }
    public class GameResettableManager : IGameResettableManager,IManager<IGameResettable>
    {
        private List<IGameResettable> _gameResettables = new List<IGameResettable>();
        public GameResettableManager(
            [InjectOptional] List<IGameResettable> gameResettables
        )
        {
            Debug.Log(gameResettables.Count + "Resettables");
            _gameResettables = gameResettables;
        }

        public void ResetOnGame()
        {
            foreach (var gameResettable in _gameResettables)
            {
                gameResettable?.ResetOnGame();
            }
        }

        public void Add(IGameResettable element)
        {
            Debug.Log("AddResettable");
            _gameResettables.Add(element);
        }

        public void RemoveNull()
        {
            _gameResettables?.RemoveAll(_ => _ == null);
        }
    }
}
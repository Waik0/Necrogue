using System.Collections;
using System.Collections.Generic;
using CafeAssets.Script.Interface.Layer_01.Manager;
using UnityEngine;
using Zenject;

namespace CafeAssets.Script.System.GameCoreSystem
{
    public class GameResettableManager : IGameResettableManager
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

        public void Dispose()
        {
            _gameResettables?.Clear();
        }
    }
}
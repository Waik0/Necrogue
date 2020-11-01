using System.Collections;
using System.Collections.Generic;
using CafeAssets.Script.Interface.Layer_01.Manager;
using CafeAssets.Script.Model;
using UnityEngine;

namespace CafeAssets.Script.System.GameTimeSystem
{




    public class GameTimeManager : IGameTimeManager
    {
        private GameTimeModel _timeModel;
        private List<IGameTickable> _tickables;

        public GameTimeManager(
            List<IGameTickable> tickables
        )
        {
            _timeModel = new GameTimeModel(0);
            _tickables = tickables;
        }

        public void Add(IGameTickable tickable)
        {
            _tickables.Add(tickable);
        }

        public void RemoveNull()
        {
            _tickables.RemoveAll(_ => _ == null);
        }

        public void Tick()
        {
            _timeModel.TotalMinutes += 1;
            foreach (var gameTickable in _tickables)
            {
                gameTickable?.TickOnGame(this);
            }

            RemoveNull();
        }

        public GameTimeModel GetNow()
        {
            return _timeModel;
        }

        public void Set(GameTimeModel model)
        {
            _timeModel = model;
        }

        public void Dispose()
        {
            Debug.Log("[GameTimeManager]Dispose");
            _tickables?.Clear();
        }
    }
}
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace CafeAssets.Script.System.GameTimeSystem
{
    public interface IGameTickable
    {
        void TickOnGame(IGameTimeManager gameTimeManager);
    }

    public interface IGameTimeManager
    {
        //method
        void Tick();
        GameTimeModel GetNow();
        void Set(GameTimeModel model);
    }


    public class GameTimeManager : IGameTimeManager, IManager<IGameTickable>
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
    }
}
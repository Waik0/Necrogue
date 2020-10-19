using UnityEngine;
using UnityEngine.Events;

namespace CafeAssets.Script.System.GameTimeSystem
{
    public interface IGameTimeUseCase
    {
        //event
        UnityEvent OnTick { get; }
        //method
        void Tick();
        GameTimeModel GetNow();
        void Set(GameTimeModel model);
    }
    public class GameTimeUseCase : IGameTimeUseCase
    {
        private GameTimeModel _timeModel;

        public UnityEvent OnTick { get; } = new UnityEvent();
        public GameTimeUseCase()
        {
            _timeModel = new GameTimeModel(0);
        }

        public void Tick()
        {
            _timeModel.TotalMinutes += 1;
            OnTick.Invoke();
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

    public class GameTimeModel
    {
        public GameTimeModel(long first)
        {
            TotalMinutes = first;
        }
        public long TotalMinutes;//最小単位は分
        public long Minutes => TotalMinutes % 60;
        public long Hour => (TotalMinutes / 60) % 24;
        public long Day => (TotalMinutes / 1440) % 7 + 1;
        public long Week => (TotalMinutes / 10080) % 4 + 1;
        public long Month => (TotalMinutes / 40320) % 12 + 1;
        public long Year => (TotalMinutes / 483840) + 1;
    }
}

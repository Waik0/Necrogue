using System;
using CafeAssets.Script.GameComponents.Npc;
using UniRx;

namespace CafeAssets.Script.GameComponents.NpcCollection
{

    /// <summary>
    /// Npcをプールから生成する
    /// todo 店員スポナーと客スポナーを派生させる させないかも...？
    /// </summary>
    public interface INpcSpawner
    {
        IObservable<INpcCollection> OnSpawn { get; }
        void Spawn(NpcFacadeModel model);
    }
    public class NpcSpawner : INpcSpawner
    {
        private NpcCollectionFactory _factory;

        private Subject<INpcCollection> _onSpawn;
        public IObservable<INpcCollection> OnSpawn
        {
            get
            {
                if (_onSpawn == null)
                {
                    _onSpawn = new Subject<INpcCollection>();
                }
                return _onSpawn;
            }
        }

        //private List<INpcFacade> _npcFacades = new List<INpcFacade>();
        public NpcSpawner(
            NpcCollectionFactory factory
            )
        {
            _factory = factory;
        }

 

        public void Spawn(NpcFacadeModel model)
        {
            var spawned = _factory.Create(model);
            _onSpawn.OnNext(spawned);
        }

        public void ResetOnGame()
        {
            //todo 全削除
        }

        // //todo 分離 スポナーの役割ではない
        // public void TickOnGame(IGameTimeManager gameTimeManager)
        // {
        //     foreach (var r in _registry.Entity)
        //     {
        //         r.Tick();
        //     }
        // }
    }
}
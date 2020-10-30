using System.Collections;
using System.Collections.Generic;
using CafeAssets.Script.System.GameCoreSystem;
using CafeAssets.Script.System.GameTimeSystem;
using UnityEngine;
using NotImplementedException = System.NotImplementedException;

namespace CafeAssets.Script.System.GameNpcSystem
{
    /// <summary>
    /// todo 店員スポナーと客スポナーを派生させる させないかも...？
    /// </summary>
    public interface INpcSpawner
    {
        void Spawn(NpcFacadeModel model);
    }
    //todo poolする場合Registryも実装しなくてはならん
    public class NpcSpawner : INpcSpawner,IGameResettable,IGameTickable
    {
        private NpcFacade.Factory _factory;
        private NpcRegistry _registry;
        private List<INpcFacade> _npcFacades = new List<INpcFacade>();
        public NpcSpawner(
            NpcFacade.Factory factory,
            NpcRegistry registry
            )
        {
            _factory = factory;
            _registry = registry;
        }
        public void Spawn(NpcFacadeModel model)
        {
            _factory.Create(model);
        }

        public void ResetOnGame()
        {
            //todo 全削除
        }

        public void TickOnGame(IGameTimeManager gameTimeManager)
        {
            foreach (var r in _registry.Entity)
            {
                r.Tick(gameTimeManager);
            }
        }
    }
}
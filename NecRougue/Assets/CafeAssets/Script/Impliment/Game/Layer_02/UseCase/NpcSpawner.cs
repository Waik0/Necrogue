using System.Collections;
using System.Collections.Generic;
using CafeAssets.Script.Impliment.Game.Layer_01.Factory;
using CafeAssets.Script.Interface.Layer_01.Manager;
using CafeAssets.Script.Interface.Registry;
using CafeAssets.Script.Interface.UseCase;
using CafeAssets.Script.Model;
using CafeAssets.Script.System.GameCoreSystem;
using CafeAssets.Script.System.GameTimeSystem;
using UnityEngine;
using NotImplementedException = System.NotImplementedException;

namespace CafeAssets.Script.System.GameNpcSystem
{

    public class NpcSpawner : INpcSpawner,IGameResettable,IGameTickable
    {
        private NpcCollectionFactory _factory;
        private INpcRegistry _registry;
        //private List<INpcFacade> _npcFacades = new List<INpcFacade>();
        public NpcSpawner(
            NpcCollectionFactory factory,
            INpcRegistry registry
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

        //todo 分離 スポナーの役割ではない
        public void TickOnGame(IGameTimeManager gameTimeManager)
        {
            foreach (var r in _registry.Entity)
            {
                r.Tick();
            }
        }
    }
}
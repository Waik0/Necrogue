using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using CafeAssets.Script.Interface.Facade;
using CafeAssets.Script.Interface.Layer_01.Manager;
using CafeAssets.Script.Interface.Layer_02.UseCase;
using CafeAssets.Script.Interface.Registry;
using CafeAssets.Script.Model;
using CafeAssets.Script.System.GameTimeSystem;
using UnityEngine;
using Zenject;
using NotImplementedException = System.NotImplementedException;

namespace CafeAssets.Script.System.GameNpcSystem
{
    /// <summary>
    /// ファサード
    /// NPCインスタンスへの窓口の役割
    /// </summary>
    public class NpcFacade : MonoBehaviour, INpcFacade, INpcCollection, IPoolable<NpcFacadeModel, IMemoryPool>
    {
        public NpcFacadeModel FacadeModel { get; private set; }

        private INpcAiUseCase _aiUseCase;
        private INpcParamUseCase _paramUseCase;
        private INpcRegistry _registry;
        private INpcMoveUseCase _moveUseCase;
        private INpcSpawnManager _npcSpawnManager;

        [Inject]
        void Inject(
            INpcAiUseCase aiUseCase,
            INpcMoveUseCase moveUseCase,
            INpcParamUseCase paramUseCase,
            INpcRegistry registry,
            INpcSpawnManager spawnManager
        )
        {
            _aiUseCase = aiUseCase;
            _moveUseCase = moveUseCase;
            _paramUseCase = paramUseCase;
            _registry = registry;
            _npcSpawnManager = spawnManager;
        }

        public void OnDespawned()
        {
            _registry.Remove(this);
        }

        public void OnSpawned(NpcFacadeModel p1, IMemoryPool p2)
        {
            Debug.Log("Spawned " + gameObject.name);
            p1.Move.Self = gameObject;
            _aiUseCase.Reset(p1.Ai);
            _moveUseCase.Reset(p1.Move);
            _paramUseCase.Reset();
            _registry.Add(this);
            _npcSpawnManager.OnSpawn(new NpcModel()
            {
                GameObject = gameObject
            });
            FacadeModel = p1;
        }


        /// <summary>
        /// Tickイベント
        /// AI回したり
        /// </summary>
        /// <param name="gameTimeManager"></param>

        public void Tick()
        {
            _aiUseCase.UpdateAction();
        }

        public NpcActionPattern CurrentAction() => _aiUseCase.Current;
        public string[] GetParamKeys()
        {
            return _paramUseCase.GetKeys();
        }

        public int GetParam(string key)
        {
            return _paramUseCase.Get(key);
        }
    }
}
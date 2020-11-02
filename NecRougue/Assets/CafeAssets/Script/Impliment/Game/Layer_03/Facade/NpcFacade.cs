using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using CafeAssets.Script.Interface.Facade;
using CafeAssets.Script.Interface.Registry;
using CafeAssets.Script.Interface.UseCase;
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
        private Dictionary<NpcActionPattern, INpcActionUseCase> _actionUseCases;
        

        [Inject]
        void Inject(
            INpcAiUseCase aiUseCase,
            INpcMoveUseCase moveUseCase,
            INpcParamUseCase paramUseCase,
            INpcRegistry registry,
            List<INpcActionUseCase> actions
        )
        {
            _aiUseCase = aiUseCase;
            _moveUseCase = moveUseCase;
            _paramUseCase = paramUseCase;
            _registry = registry;
            _actionUseCases = actions.ToDictionary(a => a.TargetPattern);
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
            FacadeModel = p1;
        }

        /// <summary>
        /// 行動する または 次の行動を考える
        /// </summary>
        void UpdateAction()
        {
            if (!_actionUseCases.ContainsKey(_aiUseCase.Current)) return;
            var currentAction = _actionUseCases[_aiUseCase.Current];
            switch (currentAction.CurrentStatus)
            {
                case NpcActionStatus.Sleep:
                    currentAction.StartAction(_aiUseCase.CurrentParam);
                    break;
                case NpcActionStatus.Doing:
                    currentAction.Tick();
                    break;
                case NpcActionStatus.Complete:
                    currentAction.EndAction();
                    _aiUseCase.Think();
                    break;
            }
        }


        /// <summary>
        /// Tickイベント
        /// AI回したり
        /// </summary>
        /// <param name="gameTimeManager"></param>

        public void Tick()
        {
            UpdateAction();
        }
    }
}
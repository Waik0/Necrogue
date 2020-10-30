using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using CafeAssets.Script.System.GameTimeSystem;
using UnityEngine;
using Zenject;
using NotImplementedException = System.NotImplementedException;

namespace CafeAssets.Script.System.GameNpcSystem
{
    public interface INpcFacade
    {
        NpcFacadeModel FacadeModel { get; }
        void Tick(IGameTimeManager gameTimeManager);
    }
    /// <summary>
    /// ファサード
    /// NPCインスタンスへの窓口の役割
    /// </summary>
    public class NpcFacade : MonoBehaviour,INpcFacade, IPoolable<NpcFacadeModel, IMemoryPool> 
    {
        public class Factory : PlaceholderFactory<NpcFacadeModel,NpcFacade>{}
        
        public NpcFacadeModel FacadeModel { get; private set; }
        
        private INpcAiUseCase _aiUseCase;
        private INpcParamUseCase _paramUseCase;
        private Dictionary<NpcActionPattern, INpcActionUseCase> _actionUseCases;
        private NpcRegistry _registry;
        [Inject]
        void Inject(
            INpcAiUseCase aiUseCase,
            INpcParamUseCase paramUseCase,
            NpcRegistry registry,
            List<INpcActionUseCase> actions
        )
        {
            _aiUseCase = aiUseCase;
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
            _aiUseCase.Reset(p1.Ai);
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
            currentAction.Tick();
            switch (currentAction.CurrentStatus)
            {
                case NpcActionStatus.Start:
                    currentAction.StartAction(_aiUseCase.CurrentParam);
                    break;
                case NpcActionStatus.Doing:
                    break;
                case NpcActionStatus.Complete:
                    _aiUseCase.Think();
                    break;
            }
        }


        /// <summary>
        /// Tickイベント
        /// AI回したり
        /// </summary>
        /// <param name="gameTimeManager"></param>
        public void Tick(IGameTimeManager gameTimeManager)
        {
            UpdateAction();
        }
    }

    public class NpcFacadeModel
    {
        public string Name;
        public NpcType Type;
        public NpcAiModel Ai;
    }
}
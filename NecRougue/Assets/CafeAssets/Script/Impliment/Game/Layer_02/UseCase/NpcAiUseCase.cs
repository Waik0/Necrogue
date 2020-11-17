using System.Collections.Generic;
using System.Linq;
using CafeAssets.Script.Interface.Layer_01.Manager;
using CafeAssets.Script.Interface.Layer_02.UseCase;
using CafeAssets.Script.Interface.Registry;
using CafeAssets.Script.Model;
using CafeAssets.Script.System.GameTimeSystem;
using UnityEngine;
using NotImplementedException = System.NotImplementedException;

namespace CafeAssets.Script.System.GameNpcSystem
{
    /// <summary>
    /// NPCの行動を制御
    /// INpcActionUseCaseとINpcActionConditionUseCaseに依存
    /// </summary>
    public class NpcAiUseCase : INpcAiUseCase
    {
        private INpcRegistry _registry;
        private Dictionary<NpcActionPattern, INpcActionUseCase> _actionUseCases;
        private Dictionary<NpcActionPattern, INpcActionConditionUseCase> _conditionUseCases;
        private NpcAiModel _model;
        public NpcAiUseCase(
            INpcRegistry registry,
            List<INpcActionUseCase> actions,
            List<INpcActionConditionUseCase> conditions
        )
        {
            _registry = registry;
            _actionUseCases = actions.ToDictionary(a => a.TargetPattern);
            _conditionUseCases = conditions.ToDictionary(c => c.TargetPattern);
        }

        public NpcActionPattern Current { get; private set; } = NpcActionPattern.Stop;
        public NpcActionModel CurrentParam { get; private set; } = new NpcActionModel();
        
        /// <summary>
        /// 優先度の高い行動を選択
        /// </summary>
        public void Think()
        {
            if (_model == null) return;
            foreach (var npcActionPattern in _model.Priority)
            {
                if (_conditionUseCases.ContainsKey(npcActionPattern))
                {
                    if (_conditionUseCases[npcActionPattern].CanAction())
                    {
                        Current = npcActionPattern;
                        return;
                    }
                }
            }
            //Current = NpcActionPattern.MoveToRandomPlace;
            //Debug.Log("RandomMove");
        }
        /// <summary>
        /// 選択された行動を実行
        /// </summary>
        public void UpdateAction()
        {
            Debug.Log(Current);
            if (_model == null) return;
            if (!_actionUseCases.ContainsKey(Current)) return;
            var currentAction = _actionUseCases[Current];
            switch (currentAction.CurrentStatus)
            {
                case NpcActionStatus.Sleep:
                    currentAction.StartAction(CurrentParam);
                    break;
                case NpcActionStatus.Doing:
                    currentAction.Tick();
                    break;
                case NpcActionStatus.Complete:
                    currentAction.EndAction(); 
                    Think();
                    break;
            }
        }

        public void Reset(NpcAiModel model)
        {
            _model = model; 
            Debug.Log(_registry.Entity.Count());
        }


        
    }

}

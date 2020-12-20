using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace CafeAssets.Script.GameComponents.Npc.NpcAi
{
    
    #region BoundsOut


    /// <summary>
    /// 行動を決定する
    /// </summary>
    public interface INpcAiUseCase
    {
        NpcActionPattern Current { get; }
        void UpdateAction();
        void Reset(NpcAiModel model);
    }
    
    #endregion
   
    #region BoundsIn

    
    /// <summary>
    /// 行動のステータス
    /// </summary>
    public enum NpcActionStatus
    {
        Sleep,
        Doing,
        Complete,
    }
    /// <summary>
    /// アクションはこれを継承して実装していく
    /// </summary>
    public interface INpcActionUseCase
    {
        /// <summary>
        /// DictionaryのキーになるのでUniqueな値にしないとエラーになる
        /// </summary>
        NpcActionPattern TargetPattern { get; }
        NpcActionStatus CurrentStatus { get; }
        void StartAction();
        void EndAction();
        void Tick();
    }
    

    public interface INpcActionConditionUseCase
    {
        NpcActionPattern TargetPattern { get; }
        bool CanAction();
    }
    
    #endregion

        

    /// <summary>
    /// NPCの行動を制御
    /// INpcActionUseCaseとINpcActionConditionUseCaseに依存
    /// </summary>
    public class NpcAiUseCase : INpcAiUseCase
    {
        public NpcActionPattern Current { get; private set; } = NpcActionPattern.Stop;
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



        /// <summary>
        /// 優先度の高い行動を選択
        /// </summary>
        public void Think()
        {
            if (_model == null) return;
            foreach (var npcActionPattern in _model.Priority)
            {
                if (!_conditionUseCases.ContainsKey(npcActionPattern)) continue;
                if (!_conditionUseCases[npcActionPattern].CanAction()) continue;
                Current = npcActionPattern;
                return;
            }
        }
        /// <summary>
        /// 選択された行動を実行
        /// </summary>
        public void UpdateAction()
        {
//            Debug.Log(Current);
            if (_model == null) return;
            if (!_actionUseCases.ContainsKey(Current)) return;
            var currentAction = _actionUseCases[Current];
            switch (currentAction.CurrentStatus)
            {
                case NpcActionStatus.Sleep:
                    currentAction.StartAction();
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

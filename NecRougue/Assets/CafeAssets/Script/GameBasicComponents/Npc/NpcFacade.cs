using CafeAssets.Script.GameComponents.Npc.NpcAi;
using CafeAssets.Script.GameComponents.Npc.NpcMove;
using CafeAssets.Script.System.GameNpcSystem;
using UnityEngine;
using Zenject;
using NotImplementedException = System.NotImplementedException;

namespace CafeAssets.Script.GameComponents.Npc
{
    #region BoundsIO

    /// <summary>
    /// Npc Component System
    /// 
    /// NPCの動作、状態、パラメータを管理するコンポーネント群
    /// 
    /// </summary>
    public interface INpcFacade
    {
        string Id { get; }
        GameObject GameObject { get; }
        /// <summary>
        /// 現在の行動状態
        /// </summary>
        /// <returns></returns>
        NpcActionPattern CurrentAction();

        INpcParamRegistry OwnParamRegistry();
    }

    #endregion
   
    /// <summary>
    /// 各機能への窓口
    /// </summary>
    sealed class NpcFacade : MonoBehaviour, INpcFacade, INpcCollection, IPoolable<NpcFacadeModel, IMemoryPool>
    {
        public NpcActionPattern CurrentAction() => _aiUseCase.Current;
  

        private INpcAiUseCase _aiUseCase;
        private INpcRegistry _registry;
        private INpcMoveUseCase _moveUseCase;
        private INpcParamRegistry _npcParamRegistry;
        public string Id { get; private set; }  
        public GameObject GameObject => gameObject;
        [Inject]
        void Inject(
            INpcAiUseCase aiUseCase,
            INpcMoveUseCase moveUseCase,
            INpcRegistry registry,
            INpcParamRegistry paramRegistry
        )
        {
            _aiUseCase = aiUseCase;
            _moveUseCase = moveUseCase;
            _npcParamRegistry = paramRegistry;
            _registry = registry;
        }

        public void OnDespawned()
        {
            _registry.Remove(this);
        }

        public void OnSpawned(NpcFacadeModel p1, IMemoryPool p2)
        {
            Debug.Log("Spawned " + gameObject.name);
            p1.Move.Self = gameObject;
            Id = p1.Id;
            _aiUseCase.Reset(p1.Ai);
            _moveUseCase.Reset(p1.Move);
            _npcParamRegistry.Reset();
            _registry.Add(this);
          
        }


        /// <summary>
        /// Tickイベント
        /// AI回したり
        /// </summary>
        /// <param name="gameTimeManager"></param>

        public void Tick()
        {
            _aiUseCase.UpdateAction();
            _npcParamRegistry.Tick();
        }
        public INpcParamRegistry OwnParamRegistry()
        {
            return _npcParamRegistry;
        }
        
    }
}
using CafeAssets.Script.GameComponents.Npc.NpcAi;
using CafeAssets.Script.GameComponents.Npc.NpcMove;
using CafeAssets.Script.System.GameNpcSystem;
using UnityEngine;
using Zenject;

namespace CafeAssets.Script.GameComponents.Npc
{
    #region BoundsIO

    /// <summary>
    /// Npc Component System
    /// ver 0.1
    /// 
    /// NPCの動作、状態、パラメータを管理するコンポーネント群
    /// 
    /// </summary>
    public interface INpcFacade
    {
        /// <summary>
        /// 現在の行動状態
        /// </summary>
        /// <returns></returns>
        NpcActionPattern CurrentAction();
        string[] GetParamKeys();
        int GetParam(string key);
    }

    #endregion
   
    /// <summary>
    /// 各機能への窓口
    /// </summary>
    sealed class NpcFacade : MonoBehaviour, INpcFacade, INpcCollection, IPoolable<NpcFacadeModel, IMemoryPool>
    {
        public NpcActionPattern CurrentAction() => _aiUseCase.Current;
        private INpcAiUseCase _aiUseCase;
        private INpcParamUseCase _paramUseCase;
        private INpcRegistry _registry;
        private INpcMoveUseCase _moveUseCase;

        [Inject]
        void Inject(
            INpcAiUseCase aiUseCase,
            INpcMoveUseCase moveUseCase,
            INpcParamUseCase paramUseCase,
            INpcRegistry registry
        )
        {
            _aiUseCase = aiUseCase;
            _moveUseCase = moveUseCase;
            _paramUseCase = paramUseCase;
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
            _aiUseCase.Reset(p1.Ai);
            _moveUseCase.Reset(p1.Move);
            _paramUseCase.Reset();
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
        }


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
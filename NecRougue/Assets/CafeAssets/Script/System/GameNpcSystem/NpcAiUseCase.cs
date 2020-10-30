using System.Linq;
using CafeAssets.Script.System.GameTimeSystem;
using UnityEngine;
using NotImplementedException = System.NotImplementedException;

namespace CafeAssets.Script.System.GameNpcSystem
{
    public enum NpcActionPattern
    {
        Stop,
        MoveToPlace,
        MoveToNpc,
        Order,
        Talk,
        Cook,
        Carry,
        Pay,
        Clean,
        Fortune,
        
        
        
    }
    /// <summary>
    /// 行動を決定する
    /// </summary>
    public interface INpcAiUseCase
    {
        NpcActionPattern Current { get; }
        NpcActionModel CurrentParam { get; }
        void Think();
        void Reset(NpcAiModel model);
    }
    public class NpcAiUseCase : INpcAiUseCase
    {
        private NpcRegistry _registry;

        public NpcAiUseCase(
            NpcRegistry registry
        )
        {
            _registry = registry;
        }

        public NpcActionPattern Current { get; private set; } = NpcActionPattern.Stop;
        public NpcActionModel CurrentParam { get; private set; } = new NpcActionModel();

        public void Think()
        {
            
        }

        public void Reset(NpcAiModel model)
        {
            Debug.Log(_registry.Entity.Count());
        }

        public void Tick(IGameTimeManager gameTimeManager)
        {
            CurrentParam.Param = "";
        }
        
    }

    public class NpcAiModel
    {
        
    }
}

using System.Linq;
using CafeAssets.Script.Interface.Layer_01.Manager;
using CafeAssets.Script.Interface.Registry;
using CafeAssets.Script.Interface.UseCase;
using CafeAssets.Script.Model;
using CafeAssets.Script.System.GameTimeSystem;
using UnityEngine;
using NotImplementedException = System.NotImplementedException;

namespace CafeAssets.Script.System.GameNpcSystem
{

    public class NpcAiUseCase : INpcAiUseCase
    {
        private INpcRegistry _registry;

        public NpcAiUseCase(
            INpcRegistry registry
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

}

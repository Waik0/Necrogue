using CafeAssets.Script.Interface.Controller;
using CafeAssets.Script.Interface.Layer_01.Manager;
using CafeAssets.Script.Interface.UseCase;
using CafeAssets.Script.Model;
using UnityEngine;
using NotImplementedException = System.NotImplementedException;

namespace CafeAssets.Script.System.GameCoreSystem
{

    //テスト
    public class DebugNpcSpawnTimingController : INpcSpawnTimingController,IGameTickable
    {
        private INpcSpawner _npcSpawner;
        private bool test = false;
        public DebugNpcSpawnTimingController(
            INpcSpawner npcSpawner
            )
        {
            _npcSpawner = npcSpawner;
        }
        public void TickOnGame(IGameTimeManager gameTimeManager)
        {

            if (!test)
            {
                test = true;
                _npcSpawner.Spawn(new NpcFacadeModel(){});
            }
        }
    }
}

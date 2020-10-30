using CafeAssets.Script.System.GameNpcSystem;
using CafeAssets.Script.System.GameTimeSystem;
using UnityEngine;
using NotImplementedException = System.NotImplementedException;

namespace CafeAssets.Script.System.GameCoreSystem
{
    /// <summary>
    /// 全体の状況を見てNpcの出現を管理するクラス
    ///todo 店員と客で分ける
    /// </summary>
    public interface INpcSpawnTimingController
    {
        
    }
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

using System.Collections.Generic;
using CafeAssets.Script.GameComponents.Npc;
using CafeAssets.Script.GameComponents.Npc.NpcAi;
using CafeAssets.Script.GameComponents.NpcCollection;
using UnityEngine;

namespace CafeAssets.Script.System.GameCoreSystem
{
    public interface INpcSpawnTimingController
    {
        void DebugSpawn();
    }
    //テスト
    public class DebugNpcSpawnTimingController : INpcSpawnTimingController
    {
        private INpcSpawner _npcSpawner;
        private bool test = false;
        public DebugNpcSpawnTimingController(
            INpcSpawner npcSpawner
            )
        {
            _npcSpawner = npcSpawner;
        }
        public void DebugSpawn()
        {

            if (!test)
            {
                test = true;
                _npcSpawner.Spawn(new NpcFacadeModel()
                {
                    Ai = new NpcAiModel()
                    {
                        Priority = new List<NpcActionPattern>()
                        {
                            //座ってるとき
                            NpcActionPattern.Order,
                            //フリーの時
                            NpcActionPattern.MoveToChair,
                            NpcActionPattern.MoveToRandomPlace,
                            NpcActionPattern.Stop,
                        }
                    },
                    Move = new NpcMoveModel()
                    {
                        Position = Vector3.zero,
                    }
                });
                _npcSpawner.Spawn(new NpcFacadeModel()
                {
                    Ai = new NpcAiModel()
                    {
                        Priority = new List<NpcActionPattern>()
                        {
                            NpcActionPattern.MoveToOrder,
                            NpcActionPattern.MoveToRandomPlace,
                            NpcActionPattern.Stop,
                        }
                    },
                    Move = new NpcMoveModel()
                    {
                        Position = Vector3.one,
                    }
                });
            }
        }
    }
}

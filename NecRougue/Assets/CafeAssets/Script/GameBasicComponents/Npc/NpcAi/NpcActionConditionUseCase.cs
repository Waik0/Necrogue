using CafeAssets.Script.GameComponents.Tilemap;
using CafeAssets.Script.GameComponents.TilemapParams;
using CafeAssets.Script.System.GameNpcSystem;
using UnityEngine;

namespace CafeAssets.Script.GameComponents.Npc.NpcAi
{
    public class NpcStopCondition : INpcActionConditionUseCase
    {
        public NpcActionPattern TargetPattern { get; private set; } = NpcActionPattern.Stop;
        public bool CanAction()
        {
            return true;
        }
    }

    public class NpcMoveToRandomPlaceCondition : INpcActionConditionUseCase
    {
        public NpcActionPattern TargetPattern { get; private set; } = NpcActionPattern.MoveToRandomPlace;
        private ITilemapUseCase _tilemapUseCase;

        private NpcParamWaitTime _npcParamWaitTime;
        //RandomMoveする確率
        private float _fallDownProbability = 0.8f;
        public NpcMoveToRandomPlaceCondition(
            ITilemapUseCase tilemapUseCase,
            NpcParamWaitTime waitTime
        )
        {
            _tilemapUseCase = tilemapUseCase;
            _npcParamWaitTime = waitTime;
        }
        public bool CanAction()
        {
            return  _tilemapUseCase.CellBounds.size.x > 0 &&
                    _tilemapUseCase.CellBounds.size.y > 0 && 
                    _npcParamWaitTime.Time <= 0 &&
                    Random.Range(0f,1f) > _fallDownProbability;
        }
    }

//緊急脱出
}
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
        //RandomMoveする確率
        private float _fallDownProbability = 0.8f;
        public NpcMoveToRandomPlaceCondition(
            ITilemapUseCase tilemapUseCase
        )
        {
            _tilemapUseCase = tilemapUseCase;
        }
        public bool CanAction()
        {
            return  _tilemapUseCase.CellBounds.size.x > 0 &&
                    _tilemapUseCase.CellBounds.size.y > 0 && 
                    Random.Range(0f,1f) > _fallDownProbability;
        }
    }
    public class NpcMoveToChairCondition : INpcActionConditionUseCase
    {
        public NpcActionPattern TargetPattern { get; private set; } = NpcActionPattern.MoveToChair;
        private ITilemapUseCase _tilemapUseCase;
        private ITilemapParamUseCase _tilemapParamUseCase;
    
        public NpcMoveToChairCondition(
            ITilemapUseCase tilemapUseCase,
            ITilemapParamUseCase tilemapParamUseCase
        )
        {
            _tilemapUseCase = tilemapUseCase;
            _tilemapParamUseCase = tilemapParamUseCase;
        }
        public bool CanAction()
        {
            return _tilemapParamUseCase.FindCanSitDownPlace().Length > 0;
        }
    }
//緊急脱出
}
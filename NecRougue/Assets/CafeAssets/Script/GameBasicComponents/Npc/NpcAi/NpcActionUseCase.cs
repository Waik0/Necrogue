using System.Collections.Generic;
using CafeAssets.Script.GameComponents.Npc;
using CafeAssets.Script.GameComponents.Npc.NpcAi;
using CafeAssets.Script.GameComponents.Npc.NpcMove;
using CafeAssets.Script.GameComponents.Npc.NpcParam;
using CafeAssets.Script.GameComponents.Tilemap;
using CafeAssets.Script.GameComponents.TilemapParams;
using CafeAssets.Script.GameComponents.TilemapPassability;
using UnityEngine;

namespace CafeAssets.Script.System.GameNpcSystem
{
    
    //停止
    public class NpcStop : INpcActionUseCase
    {
        public NpcActionPattern TargetPattern => NpcActionPattern.Stop;
        public NpcActionStatus CurrentStatus { get; private set; }

        private int count = 0;
        //停止し続けるフレーム数
        private int _stopCount = 10;
        public void StartAction()
        {
            CurrentStatus = NpcActionStatus.Doing;
            count = _stopCount;
        }

        public void EndAction()
        {
            CurrentStatus = NpcActionStatus.Sleep;
        }

        public void Tick()
        {
            count--;
            if (count == 0)
            {
                CurrentStatus = NpcActionStatus.Complete;
            }
        }
    }
    
    /// <summary>
    /// ランダムな場所に移動
    /// </summary>
    public class NpcMoveToRandomPlace : INpcActionUseCase
    {
        public NpcActionPattern TargetPattern => NpcActionPattern.MoveToRandomPlace;
        public NpcActionStatus CurrentStatus { get; private set; }
        private INpcMoveUseCase _moveUseCase;
        private INpcAstarMoveUseCase _astarMoveUseCase;
        private NpcParamSitDown _sitDown;
        private ITilemapPassabilityUseCase _tilemapPassabilityUseCase;
        private ITilemapUseCase _tilemapUseCase;
        public NpcMoveToRandomPlace(
            INpcMoveUseCase moveUseCase,
            INpcAstarMoveUseCase moveAstarUseCase,
            ITilemapPassabilityUseCase tilemapPassabilityUseCase,
            ITilemapUseCase tilemapUseCase,
            NpcParamSitDown sitDown)
        {
            _astarMoveUseCase = moveAstarUseCase;
            _moveUseCase = moveUseCase;
            _sitDown = sitDown;
            _tilemapPassabilityUseCase = tilemapPassabilityUseCase;
            _tilemapUseCase = tilemapUseCase;
            
        }
        public void StartAction()
        {
            CurrentStatus = NpcActionStatus.Doing;
            _sitDown.State = NpcParamSitDown.SitDownState.Stand;
            _astarMoveUseCase.Reset();
            var from = _moveUseCase.CurrentPos();
            var to = _tilemapPassabilityUseCase.GetRandomPassableTilePos(from,25);
            _astarMoveUseCase.Start(to);
        }

        public void EndAction()
        {
            CurrentStatus = NpcActionStatus.Sleep;
        }

        public void Tick()
        {
            if (_astarMoveUseCase.CurrentStatus == MoveState.Moving)
            {
                _astarMoveUseCase?.Tick();
            }

            if (_astarMoveUseCase.CurrentStatus == MoveState.Complete)
            {
                CurrentStatus = NpcActionStatus.Complete;
            }

        }
    }
    
   



}

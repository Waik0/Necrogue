using System.Collections;
using System.Collections.Generic;
using CafeAssets.Script.GameComponents.Npc;
using CafeAssets.Script.GameComponents.Npc.NpcAi;
using CafeAssets.Script.GameComponents.Npc.NpcMove;
using CafeAssets.Script.GameComponents.Npc.NpcParam;
using CafeAssets.Script.GameComponents.Tilemap;
using CafeAssets.Script.GameComponents.TilemapPassability;
using CafeAssets.Script.GameUniqueComponents.FindChair;
using CafeAssets.Script.System.GameNpcSystem;
using UnityEngine;

    /// <summary>
    /// 座りに行く
    /// todo 直す 新param対応
    /// </summary>
    public class NpcMoveToChair : INpcActionUseCase
    {
        public NpcActionPattern TargetPattern { get; private set; } = NpcActionPattern.MoveToChair;
        public NpcActionStatus CurrentStatus { get; private set; }
        private INpcMoveUseCase _moveUseCase;
        private ITilemapPassabilityUseCase _tilemapPassabilityUseCase;
        private ITilemapUseCase _tilemapUseCase;
        private INpcAstarMoveUseCase _npcAstarMoveUseCase;
        private IFindChair _findChair;

        private NpcParamSitDown _npcParamSit;

        private NpcParamWaitTime _npcParamWaitTime;
        //private ITilemapParamUseCase _tilemapParamUseCase;

        public NpcMoveToChair(
            INpcMoveUseCase moveUseCase,
            ITilemapPassabilityUseCase tilemapPassabilityUseCase,
            ITilemapUseCase tilemapUseCase,
            IFindChair findChair,
            INpcAstarMoveUseCase astarMoveUseCase,
            NpcParamSitDown sitDown,
            NpcParamWaitTime waitTime
            //ITilemapParamUseCase tilemapParamUseCase
            )
        {
            _moveUseCase = moveUseCase;
            _tilemapPassabilityUseCase = tilemapPassabilityUseCase;
            _tilemapUseCase = tilemapUseCase;
            _findChair = findChair;
            _npcParamSit = sitDown;
            _npcParamWaitTime = waitTime;
            _npcAstarMoveUseCase = astarMoveUseCase;
        }
        public void StartAction()
        {
            CurrentStatus = NpcActionStatus.Doing;
            _npcAstarMoveUseCase.Reset();
            Debug.Log("イスを予約");
            SetParamStart();
            BookChair();
        }
        void SetParamStart()
        {
            _npcParamSit.State = NpcParamSitDown.SitDownState.Stand;
        }

        void BookChair()
        {
            //イスを予約
            var bookChair = _findChair.RegisterAndGetSitDownPlace();
            if (bookChair == null)
            {
                Debug.Log("イスがない");
                CurrentStatus = NpcActionStatus.Complete;
                return;
            }
            var to = new Vector3Int(bookChair.Value.x,bookChair.Value.y,0);
            _npcAstarMoveUseCase.Start(to);
        }
        public void EndAction()
        {
            CurrentStatus = NpcActionStatus.Sleep;
        }
        
        void OnEndMove()
        {
            //座った
            _npcParamSit.State = NpcParamSitDown.SitDownState.Sit;
            _npcParamWaitTime.Time = 600;
        }
        public void Tick()
        {
            if (_npcAstarMoveUseCase.CurrentStatus == MoveState.Moving)
            {
                _npcAstarMoveUseCase?.Tick();
            }

            if (_npcAstarMoveUseCase.CurrentStatus == MoveState.Complete)
            {
                CurrentStatus = NpcActionStatus.Complete;
                OnEndMove();
            }
        }
    }
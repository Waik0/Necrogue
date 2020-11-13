using System.Collections.Generic;
using System.Linq;
using CafeAssets.Script.Interface.Layer_02.UseCase;
using CafeAssets.Script.Model;
using CafeAssets.Script.System.GameTimeSystem;
using UnityEngine;
using NotImplementedException = System.NotImplementedException;

namespace CafeAssets.Script.System.GameNpcSystem
{
    
    //停止
    public class NpcStop : INpcActionUseCase
    {
        public NpcActionPattern TargetPattern { get; private set; } = NpcActionPattern.Stop;
        public NpcActionStatus CurrentStatus { get; private set; }

        private int count = 0;
        public void StartAction(NpcActionModel model)
        {
            CurrentStatus = NpcActionStatus.Doing;
            count = 5;
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
    //ランダムな場所に移動
    public class NpcMoveToRandomPlace : INpcActionUseCase
    {
        public NpcActionPattern TargetPattern { get; private set; } = NpcActionPattern.MoveToRandomPlace;
        public NpcActionStatus CurrentStatus { get; private set; }
        private INpcMoveUseCase _moveUseCase;
        private INpcParamUseCase _paramUseCase;
        private ITilemapPassabilityUseCase _tilemapPassabilityUseCase;
        private ITilemapUseCase _tilemapUseCase;
        private Stack<Vector2Int> _aimList;
        private Vector2 _aim;
        public NpcMoveToRandomPlace(
            INpcMoveUseCase moveUseCase,
            INpcParamUseCase paramUseCase,
            ITilemapPassabilityUseCase tilemapPassabilityUseCase,
            ITilemapUseCase tilemapUseCase)
        {
            _moveUseCase = moveUseCase;
            _paramUseCase = paramUseCase;
            _tilemapPassabilityUseCase = tilemapPassabilityUseCase;
            _tilemapUseCase = tilemapUseCase;
            _aimList = new Stack<Vector2Int>();
        }
        public void StartAction(NpcActionModel model)
        {
            CurrentStatus = NpcActionStatus.Doing;
            var from = _moveUseCase.CurrentPos();
            var to = _tilemapPassabilityUseCase.GetRandomPassableTilePos();
            _aimList = _tilemapPassabilityUseCase.GetRoute(from, to);
        }

        public void EndAction()
        {
            CurrentStatus = NpcActionStatus.Sleep;
        }

        public void Tick()
        {
            if (_aimList == null )
            {
                CurrentStatus = NpcActionStatus.Complete;
                return;
            }
            if ((_aim - _moveUseCase.CurrentPos()).magnitude < .2f)
            {
                if (_aimList.Count == 0)
                {
                    CurrentStatus = NpcActionStatus.Complete;
                    return;
                }

                var next = _aimList.Pop();
                _aim = _tilemapUseCase.CellToWorld(new Vector3Int(next.x,next.y,0));
                //Debug.Log(_tilemapUseCase.CellToWorld(new Vector3Int(next.x,next.y,0)));
            }
           
            float speed = .05f;
            Vector2 direction = (_aim - _moveUseCase.CurrentPos()).normalized;
            //_moveUseCase.Move(direction * speed);
            _moveUseCase.Move(_aim);

        }
    }



}

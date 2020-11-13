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
        private Vector2 _to;
        private Vector2 _from;
        private float _progress;
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
           
            //todo Paramから取得 1が最速
            float speed = .05f;
            _progress += speed;
            if (_progress >= 1)
            {//移動が終了
                if (_aimList.Count == 0)
                {//移動予定がすべて終了
                    CurrentStatus = NpcActionStatus.Complete;
                    return;
                }
                var next = _aimList.Pop();
                //今の位置から
                _from = _moveUseCase.CurrentPos();
                //次の予定地まで
                _to = _tilemapUseCase.CellToWorld(new Vector3Int(next.x,next.y,0));
                //グリッド位置分の補正
                _to.y += .25f;
                //0から1
                _progress = 0;
            }
            _moveUseCase.Move(Vector2.Lerp(_from,_to,_progress));
            //_moveUseCase.Move(_aim);

        }
    }



}

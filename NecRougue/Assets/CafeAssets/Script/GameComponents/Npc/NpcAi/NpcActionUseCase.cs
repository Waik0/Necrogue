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
        public void StartAction()
        {
            CurrentStatus = NpcActionStatus.Doing;
            var from = _moveUseCase.CurrentPos();
            var to = _tilemapPassabilityUseCase.GetRandomPassableTilePos(from,25);
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
    
    /// <summary>
    /// 座りに行く
    /// todo 直す 新param対応
    /// </summary>
    public class NpcMoveToChair : INpcActionUseCase
    {
        public NpcActionPattern TargetPattern { get; private set; } = NpcActionPattern.MoveToChair;
        public NpcActionStatus CurrentStatus { get; private set; }
        private INpcMoveUseCase _moveUseCase;
        private INpcParamUseCase _paramUseCase;
        private ITilemapPassabilityUseCase _tilemapPassabilityUseCase;
        private ITilemapUseCase _tilemapUseCase;
        //private ITilemapParamUseCase _tilemapParamUseCase;
        private Stack<Vector2Int> _aimList;
        private Vector2 _to;
        private Vector2 _from;
        private float _progress;
        public NpcMoveToChair(
            INpcMoveUseCase moveUseCase,
            INpcParamUseCase paramUseCase,
            ITilemapPassabilityUseCase tilemapPassabilityUseCase,
            ITilemapUseCase tilemapUseCase
            //ITilemapParamUseCase tilemapParamUseCase
            )
        {
            _moveUseCase = moveUseCase;
            _paramUseCase = paramUseCase;
            _tilemapPassabilityUseCase = tilemapPassabilityUseCase;
            _tilemapUseCase = tilemapUseCase;
            //_tilemapParamUseCase = tilemapParamUseCase;
            _aimList = new Stack<Vector2Int>();
        }
        public void StartAction()
        {
            CurrentStatus = NpcActionStatus.Doing;
            Debug.Log("イスを予約");
            //イスを予約
            //var blankChair = _tilemapParamUseCase.RegisterAndGetSitDownPlace();
            if (true)//blankChair == null)
            {
                Debug.Log("イスがない");
                CurrentStatus = NpcActionStatus.Complete;
                return;
            }
            // var from = _tilemapUseCase.WorldToCell(_moveUseCase.CurrentPos());
            // from.z = 0;
            // var to = new Vector3Int(blankChair.Value.x,blankChair.Value.y,0);
            // _aimList = _tilemapPassabilityUseCase.GetRoute(from, to);
        }

        public void EndAction()
        {
            CurrentStatus = NpcActionStatus.Sleep;
        }

    
        void AddProgress()
        {            
            //todo Paramから取得 1が最速
            float speed = .05f;
            _progress += speed;
            
        }
        void OnNextMove()
        {
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
        void OnEndMove()
        {
            CurrentStatus = NpcActionStatus.Complete;
            //座った
            _paramUseCase.Set(NpcParameterStyle.Fluid,NpcParameters.SitDown.ToString(), 1);   
        }
        public void Tick()
        {
            if (_aimList == null )
            {
                CurrentStatus = NpcActionStatus.Complete;
                return;
            }
            AddProgress();
            if (_progress >= 1)
            {//移動が終了
                if (_aimList.Count == 0)
                {//移動予定がすべて終了
                    OnEndMove();
                    return;
                }
                OnNextMove();
            }
            //移動
            _moveUseCase.Move(Vector2.Lerp(_from,_to,_progress));
            //_moveUseCase.Move(_aim);

        }
    }



}

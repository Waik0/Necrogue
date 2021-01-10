using System.Collections;
using System.Collections.Generic;
using CafeAssets.Script.GameComponents.Npc.NpcAi;
using CafeAssets.Script.GameComponents.Npc.NpcMove;
using CafeAssets.Script.GameComponents.Tilemap;
using CafeAssets.Script.GameComponents.TilemapPassability;
using CafeAssets.Script.GameUniqueComponents.FindChair;
using CafeAssets.Script.System.GameNpcSystem;
using UnityEngine;

public enum MoveState
{
    Sleep,
    Moving,
    Complete,
    //todo 異常系
}
/// <summary>
/// Astarによる移動
/// </summary>
public interface INpcAstarMoveUseCase
{
    void Reset();
    void Start(Vector2 to);
    void Start(Vector3Int to);
    MoveState CurrentStatus { get; }
    void Tick();
}
public class NpcAstarMoveUseCase : INpcAstarMoveUseCase　
{
    public MoveState CurrentStatus { get; private set; }
    private INpcMoveUseCase _moveUseCase;
    private ITilemapPassabilityUseCase _tilemapPassabilityUseCase;
    private ITilemapUseCase _tilemapUseCase;
    //private ITilemapParamUseCase _tilemapParamUseCase;
    private Stack<Vector2Int> _aimList;
    private Vector2 _to;
    private Vector2 _from;
    private float _progress;
    public NpcAstarMoveUseCase(
        INpcMoveUseCase moveUseCase,
        ITilemapPassabilityUseCase tilemapPassabilityUseCase,
        ITilemapUseCase tilemapUseCase

        //ITilemapParamUseCase tilemapParamUseCase
    )
    {
        _moveUseCase = moveUseCase;
        _tilemapPassabilityUseCase = tilemapPassabilityUseCase;
        _tilemapUseCase = tilemapUseCase;
        //_tilemapParamUseCase = tilemapParamUseCase;
        _aimList = new Stack<Vector2Int>();
    }

    public void Reset()
    {
        CurrentStatus = MoveState.Sleep;
    }

    public void Start(Vector3Int to)
    {
        _from = _moveUseCase.CurrentPos();
        var from = _tilemapUseCase.WorldToCell(_moveUseCase.CurrentPos());
        from.z = 0;
        to.z = 0;
        _aimList = _tilemapPassabilityUseCase.GetRoute(from, to);
        CurrentStatus = MoveState.Moving;
        OnNextMove();
    }
    public void Start(Vector2 to)
    {
        _from = _moveUseCase.CurrentPos();
        var from = _moveUseCase.CurrentPos();
        _aimList = _tilemapPassabilityUseCase.GetRoute(from, to);
        CurrentStatus = MoveState.Moving;
        OnNextMove();
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
            CurrentStatus = MoveState.Complete;
        }
        public void Tick()
        {
            if (CurrentStatus != MoveState.Moving)
            {
                return;
            }
            if (_aimList == null )
            {
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

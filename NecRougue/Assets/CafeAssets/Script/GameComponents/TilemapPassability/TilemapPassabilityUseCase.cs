using System;
using System.Collections.Generic;
using System.Linq;
using CafeAssets.Script.GameComponents.Tilemap;
using UniRx;
using UnityEngine;
using Random = UnityEngine.Random;

namespace CafeAssets.Script.GameComponents.TilemapPassability
{
    
    #region BoundsOut
    /// <summary>
    /// Passability Component System
    /// 
    /// タイルの通行可否情報を生成、公開するコンポーネント群
    /// 
    /// </summary>
    public interface ITilemapPassabilityUseCase
    {
        /// <summary>
        /// ランダムな通行可能タイルの場所を返す（タイル座標の位置から）
        /// </summary>
        /// <returns></returns>
        Vector2 GetRandomPassableTilePos(Vector3Int from,int MaxSqrtDistance);
        /// <summary>
        /// ランダムな通行可能タイルの場所を返す（ワールド座標から）
        /// </summary>
        /// <returns></returns>
        Vector2 GetRandomPassableTilePos(Vector2 from,int MaxSqrtDistance);
        /// <summary>
        /// 通行可能箇所のみを通過するルートを計算
        /// </summary>
        /// <param name="from">現在地</param>
        /// <param name="to">目的地</param>
        /// <returns></returns>
        Stack<Vector2Int> GetRoute(Vector3Int from, Vector3Int to);
        Stack<Vector2Int> GetRoute(Vector2 worldFrom, Vector2 worldTo);
       
    }

    #endregion
 
    public class TilemapPassabilityUseCase : ITilemapPassabilityUseCase
    {
        private CompositeDisposable _compositeDisposable;
        private ITilemapUseCase _tilemapUseCase;
        private AstarAlgorithm _astarAlgorithm = new AstarAlgorithm();
        private AstarAlgorithm.AstarNode[,] _field;
        private Dictionary<Vector3Int, bool> _passableMap = new Dictionary<Vector3Int, bool>();
        //private ICreatePassableData[] _createPassableDatas; 
        public TilemapPassabilityUseCase(
            ITilemapUseCase tilemapUseCase
            )
        {
            _tilemapUseCase = tilemapUseCase;
            Subscribe();
        }

        void Subscribe()
        {
            _compositeDisposable = new CompositeDisposable();
            _tilemapUseCase.OnUpdateTiles.Subscribe(SetTiles).AddTo(_compositeDisposable);
        }
        public Vector2 GetRandomPassableTilePos(Vector3Int from,int MaxSqrtDistance)
        {
            if (_passableMap == null)
            {
                return Vector2.zero;
            }
            var candidate = _passableMap.Keys.Where(_ => (from - _).sqrMagnitude <= MaxSqrtDistance).ToArray();
            if (candidate.Length == 0)
            {
                return Vector2.zero;
            }
            return _tilemapUseCase.CellToWorld(candidate[Random.Range(0, candidate.Length)]);
        }

        public Vector2 GetRandomPassableTilePos(Vector2 from, int MaxSqrtDistance)
        {
            var f = _tilemapUseCase.WorldToCell(from);
            return GetRandomPassableTilePos(f, MaxSqrtDistance);
        }

        public Stack<Vector2Int> GetRoute(Vector3Int from,Vector3Int to)
        {
            //セルが存在する範囲にfrom/toが含まれるか
            if (!(_tilemapUseCase.CellBounds.Contains(from) &&
                  _tilemapUseCase.CellBounds.Contains(to)))
            {
                //含まれない場合空で返す
                return new Stack<Vector2Int>();
            }
            //Debug.Log("探索開始");
            //A-starアルゴリズムによる探索
            _astarAlgorithm.SetField(_field);
            _astarAlgorithm.Reset();
            var fromActual = from - _tilemapUseCase.CellBounds.position;
            var toActual = to - _tilemapUseCase.CellBounds.position;
            var r = _astarAlgorithm.FindPath(new Vector2Int(fromActual.x,fromActual.y), new Vector2Int(toActual.x,toActual.y),new Vector2Int(_tilemapUseCase.CellBounds.position.x,_tilemapUseCase.CellBounds.position.y));
            return r;
        }
        public Stack<Vector2Int> GetRoute(Vector2 worldFrom ,Vector2 worldTo)
        {
            var from = _tilemapUseCase.WorldToCell(worldFrom);
            var to = _tilemapUseCase.WorldToCell(worldTo);
            return GetRoute(new Vector3Int(from.x,from.y,0), new Vector3Int(to.x,to.y,0));
        }

        void SetTileInternal(Vector3Int pos)
        {
            pos = new Vector3Int(pos.x,pos.y,0);
            if (!_passableMap.ContainsKey(pos))
            {
                _passableMap.Add(pos,false);
            }
            _passableMap[pos] = _tilemapUseCase.GetPassable(pos);
        } 
        void SetTiles(Vector3Int[] pos)
        {
            DebugLog.LogClassName(this,$"タイルの通行可否情報を更新します。 {pos.Length}箇所 ");
            foreach (var vector3Int in pos)
            {
                SetTileInternal(vector3Int);
                ReCalcNode();
            }
        }


        void ReCalcNode()
        {
            PassableMapToAstarField();
        }
        /// <summary>
        /// Astar探索で使える形に変換
        /// </summary>
        void PassableMapToAstarField()
        {
            var size = _tilemapUseCase.CellBounds.size;
            var pos = _tilemapUseCase.CellBounds.position;
            _field = new AstarAlgorithm.AstarNode[size.x + 1,size.y + 1];
            foreach (var keyValuePair in _passableMap)
            {
                var index = new Vector2Int(keyValuePair.Key.x - pos.x,keyValuePair.Key.y - pos.y );
                _field[index.x, index.y] = new AstarAlgorithm.AstarNode()
                {
                    Passable = keyValuePair.Value
                };
            }
        }
    }
    
}

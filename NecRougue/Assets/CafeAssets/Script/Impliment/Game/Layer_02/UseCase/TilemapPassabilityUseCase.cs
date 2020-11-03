using System.Collections;
using System.Collections.Generic;
using CafeAssets.Script.Interface.Layer_01.Manager;
using CafeAssets.Script.Interface.Layer_02.UseCase;
using CafeAssets.Script.Model;
using UnityEngine;
using UnityEngine.Tilemaps;
using NotImplementedException = System.NotImplementedException;

namespace CafeAssets.Script.Impliment.Game.Layer_02.UseCase
{
    public class TilemapPassabilityUseCase : ITilemapPassabilityUseCase,ITilemapReceiver
    {
        private Tilemap _passableMap;
        private ITilemapUseCase _tilemapUseCase;
        private AstarAlgorithmForTilemap _astarAlgorithm = new AstarAlgorithmForTilemap();
        public TilemapPassabilityUseCase(
            ITilemapUseCase tilemapUseCase
            )
        {
            _tilemapUseCase = tilemapUseCase;
        }

        public Tilemap PassableTilemap { get => _passableMap; } 

        public Vector2Int[] GetRoute(Vector3Int from,Vector3Int to)
        {
         
            //セルが存在する範囲にfrom/toが含まれるか
            if (!(_passableMap.cellBounds.Contains(from) &&
                _passableMap.cellBounds.Contains(to)))
            {
                //含まれない場合空で返す
                return new Vector2Int[0];
            }
            //A-starアルゴリズムによる探索
            _astarAlgorithm.Reset();
            _astarAlgorithm.Search(from,to);
            var r = _astarAlgorithm.Result;
            return r;
        }
        public Vector2Int[] GetRoute(Vector2 worldFrom ,Vector2 worldTo)
        {
            var from = _tilemapUseCase.Tilemap.WorldToCell(worldFrom);
            var to = _tilemapUseCase.Tilemap.WorldToCell(worldTo);
            return GetRoute(new Vector3Int(from.x,from.y,0), new Vector3Int(to.x,to.y,0));
        }

        void RemapPassable()
        {
            //todo passable情報が別タイルマップに集約されている
            //_passableMap.GetBoundsLocal()
            //var tiles = _passableMap.GetTilesBlock(new BoundsInt(Vector3Int.zero, (Vector3Int) _tilemapUseCase.MaxSize));
            
        }
        public void OnUpdateTile(TilemapModel model)
        {
            if (_passableMap == null)
            {
                _passableMap = model.Passable;
                _astarAlgorithm.SetField(model.Passable);
            }
            RemapPassable();
        }
    }
}

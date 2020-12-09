using System.Collections.Generic;
using System.Linq;
using CafeAssets.Script.GameComponents.Tilemap;
using UnityEngine;
using Random = UnityEngine.Random;

namespace CafeAssets.Script.GameComponents.TilemapPassability
{
    
    public interface ITilemapPassabilityUseCase
    {
        //   Tilemap PassableTilemap { set; }
        Vector2 GetRandomPassableTilePos(Vector3Int from,int MaxSqrtDistance);
        Vector2 GetRandomPassableTilePos(Vector2 from,int MaxSqrtDistance);
        Stack<Vector2Int> GetRoute(Vector3Int from, Vector3Int to);
        Stack<Vector2Int> GetRoute(Vector2 worldFrom, Vector2 worldTo);
        
        void SetTile(Vector3Int pos,ITileModel model);
        
        void SetTiles(Vector3Int[] pos,ITileModel model);
        void ReCalcNode();
    }
    public class TilemapPassabilityUseCase : ITilemapPassabilityUseCase
    {
        
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
            //Init(tilemapUseCase);
        }

        // void Init (ITilemapUseCase tilemapUseCase)
        // {
        //     _createPassableDatas = new ICreatePassableData[]
        //     {
        //         new CreatePassableDataFromPlaceTileSingle(tilemapUseCase),
        //         new CreatePassableDataFromPlaceTileDraw(tilemapUseCase),
        //         new CreatePassableDataFromPlaceTileRect(tilemapUseCase) 
        //     };
        // }

        //public Tilemap PassableTilemap { private get; set; }
        /// <summary>
        /// ランダムな通行可能タイルの場所を返す
        /// </summary>
        /// <returns></returns>
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
            //Debug.Log(from);
            //Debug.Log(to);
            //Debug.Log(_tilemapUseCase.CellBounds); 
            //セルが存在する範囲にfrom/toが含まれるか
            if (!(_tilemapUseCase.CellBounds.Contains(from) &&
                  _tilemapUseCase.CellBounds.Contains(to)))
            {
                //Debug.Log("NPCがRect内にいない");
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
            //Debug.Log("探索終了-");
            // foreach (var vector2Int in r)
            // {
            //     Debug.Log(vector2Int);
            // }
            //Debug.Log("---");
            return r;
        }
        public Stack<Vector2Int> GetRoute(Vector2 worldFrom ,Vector2 worldTo)
        {
            var from = _tilemapUseCase.WorldToCell(worldFrom);
            var to = _tilemapUseCase.WorldToCell(worldTo);
            return GetRoute(new Vector3Int(from.x,from.y,0), new Vector3Int(to.x,to.y,0));
        }

        void SetTileInternal(Vector3Int pos, ITileModel model)
        {
            pos = new Vector3Int(pos.x,pos.y,0);
            if (!_passableMap.ContainsKey(pos))
            {
                _passableMap.Add(pos,false);
            }
            _passableMap[pos] = _tilemapUseCase.GetPassable(pos);
        }
        public void SetTile(Vector3Int pos, ITileModel model)
        {
            SetTileInternal(pos, model);
        }

        public void SetTiles(Vector3Int[] pos, ITileModel model)
        {
            foreach (var vector3Int in pos)
            {
                SetTileInternal(vector3Int,model);
            }
            
        }

        public void ReCalcNode()
        {
            PassableMapToAstarField();
        }

        // void AddPassableMap(TilePlaceModel model, Dictionary<Vector3Int, bool> passable)
        // {
        //     foreach (var createPassableData in _createPassableDatas)
        //     {
        //         createPassableData.Add(model,passable);
        //     }
        // }

        // void CheckPassable()
        // {
        //     var rem = new List<Vector3Int>();
        //     foreach (var key in _passableMap.Keys.ToArray())
        //     {
        //         _passableMap[key] = _tilemapUseCase.GetPassable(key);
        //         if(! _passableMap[key] ) rem.Add(key);
        //     }
        //     foreach (var vector3Int in rem)
        //     {
        //         _passableMap.Remove(vector3Int);
        //     }
        // }
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
        //タイルマップ設置と同時
        // public void OnPlaceTile(TilePlaceModel model)
        // {
        //     AddPassableMap(model, _passableMap);
        // }
        //
        // public void OnRemoveTile(TilePlaceModel model)
        // {
        //     throw new NotImplementedException();
        // }
        //タイルマップ反映後
        // public void OnUpdateTile(TilemapModel model)
        // {
        //     CheckPassable();
        //     PassableMapToAstarField();
        // }
    }

    // interface ICreatePassableData
    // {
    //     //passableに追加する
    //     void Add(TilePlaceModel model, Dictionary<Vector3Int, bool> passable);
    //     void Remove(TilePlaceModel model, Dictionary<Vector3Int, bool> passable);
    // }
    // class CreatePassableDataFromPlaceTileSingle : ICreatePassableData
    // {
    //     private ITilemapUseCase _tilemapUseCase;
    //     public CreatePassableDataFromPlaceTileSingle(
    //         ITilemapUseCase tilemapUseCase
    //         )
    //     {
    //         _tilemapUseCase = tilemapUseCase;
    //     }
    //     public void Add(TilePlaceModel model, Dictionary<Vector3Int, bool> passable)
    //     {
    //         if (model.PlaceMode != PlaceTileMode.PlaceTileSingle)
    //             return;
    //         if (model.Model.Brush.sqrMagnitude > 1)
    //         {
    //             Debug.Log(model.Model);
    //             var positions = new Vector3Int[model.Model.Brush.x * model.Model.Brush.y];
    //             var tiles = new TileBase[model.Model.Brush.x * model.Model.Brush.y];
    //             var origin = _tilemapUseCase.WorldToCell(model.StartWorldPos);
    //             int xhalf = (model.Model.Brush.x + 1 - (model.Model.Brush.x % 2)) / 2;
    //             int yhalf = (model.Model.Brush.y + 1 - (model.Model.Brush.y % 2)) / 2;
    //             for (var i = 0; i < model.Model.Brush.x; i++)
    //             {
    //                 for (var j = 0; j < model.Model.Brush.y; j++)
    //                 {
    //                     var pos = new Vector3Int(origin.x - xhalf + i,origin.y - yhalf + j,model.Model.MinLayer);
    //                     pos.z = 0;
    //                     if (!passable.ContainsKey(pos))
    //                     {
    //                         passable.Add(pos,false);
    //                     }
    //                 }
    //             }
    //            
    //         }
    //         else
    //         {
    //             var pos = _tilemapUseCase.WorldToCell(model.StartWorldPos);
    //             pos.z = 0;
    //             if (!passable.ContainsKey(pos))
    //             {
    //                 passable.Add(pos,false);
    //             }
    //         }
    //        
    //     }
    //
    //     public void Remove(TilePlaceModel model, Dictionary<Vector3Int, bool> passable)
    //     {
    //         if (model.PlaceMode != PlaceTileMode.PlaceTileSingle)
    //             return;
    //     }
    // }
    // class CreatePassableDataFromPlaceTileRect : ICreatePassableData
    // {
    //     private ITilemapUseCase _tilemapUseCase;
    //     public CreatePassableDataFromPlaceTileRect(
    //         ITilemapUseCase tilemapUseCase
    //     )
    //     {
    //         _tilemapUseCase = tilemapUseCase;
    //     }
    //     public void Add(TilePlaceModel model, Dictionary<Vector3Int, bool> passable)
    //     {
    //         if (model.PlaceMode != PlaceTileMode.PlaceTileRect)
    //             return;
    //         var start = _tilemapUseCase.WorldToCell(model.StartWorldPos);
    //         var end = _tilemapUseCase.WorldToCell(model.EndWorldPos);
    //         var sx = Mathf.Min(start.x, end.x);
    //         var ex = Mathf.Max(start.x, end.x);
    //         var sy = Mathf.Min(start.y, end.y);
    //         var ey = Mathf.Max(start.y, end.y);
    //         for (var i = sx; i < ex; i++)
    //         {
    //             for (var j = sy; j < ey; j++)
    //             {
    //                 var pos = new Vector3Int(i,j,0);
    //                 if (!passable.ContainsKey(pos))
    //                 {
    //                     passable.Add(pos,false);
    //                 }
    //             }
    //         }
    //
    //     }
    //
    //     public void Remove(TilePlaceModel model, Dictionary<Vector3Int, bool> passable)
    //     {
    //         if (model.PlaceMode != PlaceTileMode.PlaceTileRect)
    //             return;
    //     }
    // }
    // class CreatePassableDataFromPlaceTileDraw : ICreatePassableData
    // {
    //     private ITilemapUseCase _tilemapUseCase;
    //     public CreatePassableDataFromPlaceTileDraw(
    //         ITilemapUseCase tilemapUseCase
    //     )
    //     {
    //         _tilemapUseCase = tilemapUseCase;
    //     }
    //     public void Add(TilePlaceModel model, Dictionary<Vector3Int, bool> passable)
    //     {
    //         if (model.PlaceMode != PlaceTileMode.PlaceTileDraw)
    //             return;
    //         if (model.Model.Brush.sqrMagnitude > 1)
    //         {
    //             Debug.Log(model.Model);
    //             var positions = new Vector3Int[model.Model.Brush.x * model.Model.Brush.y];
    //             var tiles = new TileBase[model.Model.Brush.x * model.Model.Brush.y];
    //             var origin = _tilemapUseCase.WorldToCell(model.StartWorldPos);
    //             int xhalf = (model.Model.Brush.x + 1 - (model.Model.Brush.x % 2)) / 2;
    //             int yhalf = (model.Model.Brush.y + 1 - (model.Model.Brush.y % 2)) / 2;
    //             for (var i = 0; i < model.Model.Brush.x; i++)
    //             {
    //                 for (var j = 0; j < model.Model.Brush.y; j++)
    //                 {
    //                     var pos = new Vector3Int(origin.x - xhalf + i,origin.y - yhalf + j,model.Model.MinLayer);
    //                     pos.z = 0;
    //                     if (!passable.ContainsKey(pos))
    //                     {
    //                         passable.Add(pos,false);
    //                     }
    //                 }
    //             }
    //            
    //         }
    //         else
    //         {
    //             var pos = _tilemapUseCase.WorldToCell(model.StartWorldPos);
    //             pos.z = 0;
    //             if (!passable.ContainsKey(pos))
    //             {
    //                 passable.Add(pos,false);
    //             }
    //         }
    //     }
    //
    //     public void Remove(TilePlaceModel model, Dictionary<Vector3Int, bool> passable)
    //     {
    //         if (model.PlaceMode != PlaceTileMode.PlaceTileDraw)
    //             return;
    //     }
    // }
}

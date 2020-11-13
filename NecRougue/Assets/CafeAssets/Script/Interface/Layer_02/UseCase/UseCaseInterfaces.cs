using System.Collections.Generic;
using CafeAssets.Script.Model;
using UnityEngine;
using UnityEngine.Tilemaps;

namespace CafeAssets.Script.Interface.Layer_02.UseCase
{
    #region GameCore
    public interface IGameUseCase
    {
        //セーブデータ
        void LoadData(string path);
        void Reset();
        void Tick();

    }
    
    #endregion
    #region NpcSystem
    /// <summary>
    /// 行動を決定する
    /// </summary>
    public interface INpcAiUseCase
    {
        NpcActionPattern Current { get; }
        NpcActionModel CurrentParam { get; }
        void Think();
        void Reset(NpcAiModel model);
    }

    public interface INpcMoveUseCase
    {
        Vector2 CurrentPos();
        void Move(Vector2 pos);
        void Reset(NpcMoveModel model);
    }
    
    /// <summary>
    /// アクションはこれを継承して実装していく
    /// </summary>
    public interface INpcActionUseCase
    {
        /// <summary>
        /// DictionaryのキーになるのでUniqueな値にしないとエラーになる
        /// </summary>
        NpcActionPattern TargetPattern { get; }
        NpcActionStatus CurrentStatus { get; }
        void StartAction(NpcActionModel model);
        void EndAction();
        void Tick();
    }
    /// <summary>
    /// Npcをプールから生成する
    /// todo 店員スポナーと客スポナーを派生させる させないかも...？
    /// </summary>
    public interface INpcSpawner
    {
        void Spawn(NpcFacadeModel model);
    }
    #endregion
    /// <summary>
    /// タイルを設置する
    /// </summary>
    public interface IPlaceTileUseCase
    {
        TileModel SelectedTile { get; set; }
        PlaceTileMode PlaceTileMode { get; set; }
    }
    public interface ITilemapUseCase
    {
        Vector2Int MaxSize { get; }
        Tilemap Tilemap { set; }
        T GetTileModel<T>(Vector3Int pos) where T : TileModel;
        Vector3Int WorldToCell(Vector3 world);
        
        Vector3 CellToWorld(Vector3Int cell);
        
        BoundsInt CellBounds { get; }
        
        bool GetPassable(Vector3Int cell);

    }
    public interface ITilemapPassabilityUseCase
    {
     //   Tilemap PassableTilemap { set; }
        Vector2 GetRandomPassableTilePos();
        Stack<Vector2Int> GetRoute(Vector3Int from, Vector3Int to);
        Stack<Vector2Int> GetRoute(Vector2 worldFrom, Vector2 worldTo);
    }
    #region GameParams
    
    public interface IGameParameterUseCase
    {
        void Reset();
        string[] GetKeys();
        int Get(string key);
        int Set(ParameterStyle style, string key, int num);
        int Operation(ParameterStyle style, string key, GameParameterOperations operation, int num);
    }

    #endregion
}

﻿using CafeAssets.Script.Model;
using UnityEngine;

namespace CafeAssets.Script.Interface.UseCase
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

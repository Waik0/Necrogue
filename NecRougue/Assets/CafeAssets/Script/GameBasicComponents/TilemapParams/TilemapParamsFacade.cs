using System;
using System.Collections;
using System.Collections.Generic;
using CafeAssets.Script.GameComponents.Tilemap;
using UniRx;
using UnityEngine;

namespace CafeAssets.Script.GameComponents.TilemapParams
{
    //外部で使用するもの
    #region BoundsIO
    /// <summary>
    /// TilemapParams Component System
    /// 
    /// タイルマップのタイルごとに与えられるデータ群を管理するコンポーネント
    /// 
    /// </summary>
    public interface ITilemapParamsFacade<T>
    {
        IObservable<Unit> OnUpdateTileParams { get; }
        Dictionary<Vector3Int, List<ITileParamsModelBase<T>>> Entity { get; }
        /// <summary>
        /// タイルのパラメーターをセットする。
        /// 既にセットされている場合は上書きする。
        /// 1箇所に対して1つのパラメーター群のみ
        /// </summary>
        /// <param name="pos"></param>
        /// <param name="model"></param>
        void SetTileParam(Vector3Int pos, List<ITileParamsModelBase<T>>  model);
        void SetTileParam((Vector3Int pos, List<ITileParamsModelBase<T>> model)[] KeyValuePair);

        /// <summary>
        /// 
        /// </summary>
        /// <param name="pos"></param>
        /// <param name="key"></param>
        /// <typeparam name="T"></typeparam>
        /// <returns></returns>
        int GetTileParam(Vector2Int pos, T key);
        Dictionary<T,int> GetTileParams(Vector2Int pos);
        /// <summary>
        /// 1箇所のパラメーター群の特定のパラメーターを書き換える
        /// </summary>
        /// <param name="pos"></param>
        /// <param name="key"></param>
        /// <param name="Param"></param>
        /// <typeparam name="T"></typeparam>
        void UpdateTileParam(Vector3Int pos, T key, int Param);
 
        /// <summary>
        /// パラメーター群を削除する
        /// </summary>
        /// <param name="pos"></param>
        void RemoveTileParam(Vector3Int pos);
    }
    
    #endregion

    /// <summary>
    /// Facade実装
    /// </summary>
    public class TilemapParamsFacade<T> : ITilemapParamsFacade<T> where T : struct

    {
    private ISubject<Unit> _onUpdateTiles;

    public IObservable<Unit> OnUpdateTileParams => _onUpdateTiles ?? (_onUpdateTiles = new Subject<Unit>());

    private ITilemapParamsUseCase<T> _useCase;

    public TilemapParamsFacade(
        ITilemapParamsUseCase<T> useCase
    )
    {
        _useCase = useCase;
    }

    public Dictionary<Vector3Int, List<ITileParamsModelBase<T>>> Entity => _useCase.Entity;

    /// <summary>
    /// パラメーターをセットする。
    /// 既にセットされている場合は上書きする。
    /// </summary>
    public void SetTileParam(Vector3Int pos, List<ITileParamsModelBase<T>> model)
    {
        _useCase.SetTileParam(pos, model);
        _onUpdateTiles?.OnNext(Unit.Default);
    }

    public void SetTileParam((Vector3Int pos, List<ITileParamsModelBase<T>> model)[] KeyValuePair)
    {
        foreach (var valueTuple in KeyValuePair)
        {
            _useCase.SetTileParam(valueTuple.pos, valueTuple.model);
        }

        _onUpdateTiles?.OnNext(Unit.Default);
    }
    /// <summary>
    /// posに影響しているパラメータを取得
    /// </summary>
    /// <param name="pos"></param>
    /// <param name="key"></param>
    /// <returns></returns>

    public int GetTileParam(Vector2Int pos, T key)
    {
        return _useCase.GetTileParam(pos, key);
    }

    public Dictionary<T,int> GetTileParams(Vector2Int pos)
    {
        return _useCase.GetTileParams(pos);
    }

    public void UpdateTileParam(Vector3Int pos, T key, int Param)
    {
        _useCase.UpdateTileParam(pos, key, Param);
    }

    public void RemoveTileParam(Vector3Int pos)
    {
        _useCase.RemoveTileParam(pos);
    }
    }
}
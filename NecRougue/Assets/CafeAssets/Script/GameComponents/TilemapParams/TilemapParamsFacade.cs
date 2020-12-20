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
    /// ver 0.1
    /// 
    /// タイルマップのタイルごとに与えられるデータ群を管理するコンポーネント
    /// 
    /// </summary>
    public interface ITilemapParamsFacade
    {
        IObservable<Unit> OnUpdateTileParams { get; }
        Dictionary<Vector3Int, List<ITileParamsModelBase>> Entity { get; }
        /// <summary>
        /// タイルのパラメーターをセットする。
        /// 既にセットされている場合は上書きする。
        /// 1箇所に対して1つのパラメーター群のみ
        /// </summary>
        /// <param name="pos"></param>
        /// <param name="model"></param>
        void SetTileParam(Vector3Int pos, List<ITileParamsModelBase>  model);
        void SetTileParam((Vector3Int pos, List<ITileParamsModelBase> model)[] KeyValuePair);

        /// <summary>
        /// 
        /// </summary>
        /// <param name="pos"></param>
        /// <param name="key"></param>
        /// <typeparam name="T"></typeparam>
        /// <returns></returns>
        ITileParamsModel<T> GetTileParam<T>(Vector3Int pos, T key) where T : struct;
        /// <summary>
        /// 1箇所のパラメーター群の特定のパラメーターを書き換える
        /// </summary>
        /// <param name="pos"></param>
        /// <param name="key"></param>
        /// <param name="Param"></param>
        /// <typeparam name="T"></typeparam>
        void UpdateTileParam<T>(Vector3Int pos, T key, int Param) where T : struct;
 
        /// <summary>
        /// パラメーター群を削除する
        /// </summary>
        /// <param name="pos"></param>
        void RemoveTileParam(Vector3Int pos);
    }
 
    /// <summary>
    /// モデルに継承させるインターフェイス
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public interface ITileParamsModel<T> : ITileParamsModelBase where T : struct
    {
        T Key { get; }
    }

    #endregion
    
    /// <summary>
    /// Facade実装
    /// </summary>
    sealed class TilemapParamsFacade : ITilemapParamsFacade
    {
        private ISubject<Unit> _onUpdateTiles;

        public IObservable<Unit> OnUpdateTileParams => _onUpdateTiles ?? (_onUpdateTiles = new Subject<Unit>());
        
        private ITilemapParamsUseCase _useCase;

        public TilemapParamsFacade(
            ITilemapParamsUseCase useCase
        )
        {
            _useCase = useCase;
        }
        public Dictionary<Vector3Int, List<ITileParamsModelBase>> Entity => _useCase.Entity;
        /// <summary>
        /// パラメーターをセットする。
        /// 既にセットされている場合は上書きする。
        /// </summary>
        public void SetTileParam(Vector3Int pos,List<ITileParamsModelBase> model)
        {
            _useCase.SetTileParam(pos,model);
            _onUpdateTiles?.OnNext(Unit.Default);
        }

        public void SetTileParam((Vector3Int pos, List<ITileParamsModelBase> model)[] KeyValuePair)
        {
            foreach (var valueTuple in KeyValuePair)
            {
                _useCase.SetTileParam(valueTuple.pos,valueTuple.model);
            }

            _onUpdateTiles?.OnNext(Unit.Default);
        }

        public ITileParamsModel<T> GetTileParam<T>(Vector3Int pos, T key) where T : struct
        {
            return _useCase.GetTileParam(pos, key);
        }

        public void UpdateTileParam<T>(Vector3Int pos, T key, int Param) where T : struct
        {
            _useCase.UpdateTileParam(pos,key,Param);
        }

        public void RemoveTileParam(Vector3Int pos)
        {
            _useCase.RemoveTileParam(pos);
        }
    }
}
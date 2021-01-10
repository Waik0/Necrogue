using System.Collections.Generic;
using CafeAssets.Script.GameComponents.Tilemap;
using CafeAssets.Script.GameComponents.TilemapParams;
using UnityEngine;

namespace CafeAssets.Script.GameComponents.TilemapPlaceController
{
    /// <summary>
    /// タイル設置時にパラメータを配置するクラス
    /// </summary>
    public class PlaceTileBehaviourRectParam : IPlaceTileBehaviour
    {
        private Vector3Int _rectStartPos;
        private ITilemapUseCase _tilemapUseCase;
        private ITilemapParamsFacade<TileEffectParams> _tilemapEffectParamsFacade;
        private ITilemapParamsFacade<TileStaticParams> _tilemapStaticParamsFacade;
        public PlaceTileBehaviourRectParam(
            ITilemapUseCase tilemapUseCase, 
            ITilemapParamsFacade<TileEffectParams> tilemapEffectParamsFacade,
            ITilemapParamsFacade<TileStaticParams> tilemapStaticParamsFacade)
        {
            _tilemapUseCase = tilemapUseCase;
            _tilemapEffectParamsFacade = tilemapEffectParamsFacade;
            _tilemapStaticParamsFacade = tilemapStaticParamsFacade;
        }

        public PlaceTileMode TargetPlaceMode => PlaceTileMode.PlaceTileRect;

        public void StartPlace(Vector3 pos,ITileModel model)
        {
            var z = model.ZMin();
            //pos.z = z;
            _rectStartPos = _tilemapUseCase.WorldToCell(pos);
            _rectStartPos.z = z;
        }

        public void UpdatePlace(Vector3 pos,ITileModel model)
        {

        }

        void SetStatic(int sx, int sy,int z,int lx,int ly, ITileModel model)
        {
            var keyValuePair = new (Vector3Int pos,  List<ITileParamsModelBase<TileStaticParams>> model)[lx * ly];
            for (var i = 0; i < lx; i++)
            {
                for (var j = 0; j < ly; j++)
                {
                    
                    var p = new List<ITileParamsModelBase<TileStaticParams>>();
                    //EffectiveTileの場合パラメーター設定
                    if (model is ITileModel staticModel)
                    {
                        p.AddRange(staticModel.StaticParams);//コピーをもらってくる
                    }

                    keyValuePair[i * ly + j] = (new Vector3Int(sx + i, sy + j, z),p);
                }
            }
            _tilemapStaticParamsFacade.SetTileParam(keyValuePair);
        }
        void SetEffect(int sx, int sy,int z,int lx,int ly, ITileModel model)
        {
            var keyValuePair = new (Vector3Int pos,  List<ITileParamsModelBase<TileEffectParams>> model)[lx * ly];
            for (var i = 0; i < lx; i++)
            {
                for (var j = 0; j < ly; j++)
                {
                    
                    var p = new List<ITileParamsModelBase<TileEffectParams>>();
                    //EffectiveTileの場合パラメーター設定
                    if (model is ITileEffectiveModel effectiveModel)
                    {
                        p.AddRange(effectiveModel.EffectiveParams);//コピーをもらってくる
                    }

                    keyValuePair[i * ly + j] = (new Vector3Int(sx + i, sy + j, z),p);
                }
            }
            _tilemapEffectParamsFacade.SetTileParam(keyValuePair);
        }
        public void EndPlace(Vector3 pos, ITileModel model)
        {
            DebugLog.LogClassName(this, "PARAM配置");
            var start = _rectStartPos;
            var z = model.ZMin();
            //pos.z = z;
            var end = _tilemapUseCase.WorldToCell(pos);
            end.z = z;
            var sx = Mathf.Min(start.x, end.x);
            var ex = Mathf.Max(start.x, end.x);
            var sy = Mathf.Min(start.y, end.y);
            var ey = Mathf.Max(start.y, end.y);
            var lx = ex - sx;
            var ly = ey - sy;
            SetEffect(sx, sy, z, lx, ly, model);
            SetStatic(sx, sy, z, lx, ly, model);
        }
    }
}
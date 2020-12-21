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
        private ITilemapParamsFacade _tilemapParamsFacade;
        public PlaceTileBehaviourRectParam(
            ITilemapUseCase tilemapUseCase, 
            ITilemapParamsFacade tilemapParamsFacade)
        {
            _tilemapUseCase = tilemapUseCase;
            _tilemapParamsFacade = tilemapParamsFacade;
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
            var keyValuePair = new (Vector3Int pos,  List<ITileParamsModelBase> model)[lx * ly];
            for (var i = 0; i < lx; i++)
            {
                for (var j = 0; j < ly; j++)
                {
                    
                    var p = new List<ITileParamsModelBase>();
                    //パラメーター設定
                    if (model is ITileModel tileModel)
                    {
                        p.AddRange(tileModel.StaticParams);//コピーをもらってくる
                    }
                    //EffectiveTileの場合パラメーター設定
                    if (model is ITileEffectiveModel effectiveModel)
                    {
                        p.AddRange(effectiveModel.EffectiveParams);//コピーをもらってくる
                    }

                    keyValuePair[i * ly + j] = (new Vector3Int(sx + i, sy + j, end.z),p);
                }
            }
            _tilemapParamsFacade.SetTileParam(keyValuePair);
        }
    }
}
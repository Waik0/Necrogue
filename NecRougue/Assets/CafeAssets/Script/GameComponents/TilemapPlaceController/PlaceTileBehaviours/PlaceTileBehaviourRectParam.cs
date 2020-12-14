using System.Collections.Generic;
using CafeAssets.Script.GameComponents.Tilemap;
using CafeAssets.Script.GameComponents.TilemapParams;
using UnityEngine;

namespace CafeAssets.Script.GameComponents.TilemapPlaceController
{
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

        public void EndPlace(Vector3 pos,ITileModel model)
        {
            DebugLog.LogClassName(this, "PARAM生成");
            var param = new List<ITileParamsModelBase>();
            //パラメーター設定
            if (model is ITileModel tileModel)
            {
                param.AddRange(tileModel.StaticParams.Params);
            }
            //EffectiveTileの場合パラメーター設定
            if (model is ITileEffectiveModel effectiveModel)
            {
                param.AddRange(effectiveModel.EffectiveParams.Params);
            }
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
            var positions = new Vector3Int[lx * ly];
            for (var i = 0; i < lx; i++)
            {
                for (var j = 0; j < ly; j++)
                {
                    positions[i * ly + j] = new Vector3Int(sx + i, sy + j, end.z);
                }
            }
            _tilemapParamsFacade.SetTileParam(positions, param);
        }
    }
}
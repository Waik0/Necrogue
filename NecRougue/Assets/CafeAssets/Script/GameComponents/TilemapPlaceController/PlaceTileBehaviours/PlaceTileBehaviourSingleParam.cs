using System.Collections.Generic;
using CafeAssets.Script.GameComponents.Tilemap;
using CafeAssets.Script.GameComponents.TilemapParams;
using UnityEngine;

namespace CafeAssets.Script.GameComponents.TilemapPlaceController
{
    public class PlaceTileBehaviourSingleParam : IPlaceTileBehaviour
    {
        private ITilemapUseCase _tilemapUseCase;
        private ITilemapParamsFacade _tilemapParamsFacade;

        public PlaceTileBehaviourSingleParam(
            ITilemapUseCase tilemapUseCase,
            ITilemapParamsFacade tilemapParamsFacade)
        {
            _tilemapUseCase = tilemapUseCase;
            _tilemapParamsFacade = tilemapParamsFacade;
        }

        public PlaceTileMode TargetPlaceMode => PlaceTileMode.PlaceTileSingle;

        public void StartPlace(Vector3 pos, ITileModel model)
        {
        }

        public void UpdatePlace(Vector3 pos, ITileModel model)
        {
        }

        public void EndPlace(Vector3 pos, ITileModel model)
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
            var z = model.ZMin();
            //pos.z = z;
            var origin = _tilemapUseCase.WorldToCell(pos);
            origin.z = z;
            var brush = model.BrushSize();
            if (brush.sqrMagnitude <= 2)
            {
                _tilemapUseCase.SetTile(origin, model);
            }
            else
            {
                var positions = new Vector3Int[brush.x * brush.y];
                int xhalf = (brush.x + 1 - (brush.x % 2)) / 2;
                int yhalf = (brush.y + 1 - (brush.y % 2)) / 2;
                for (var i = 0; i < brush.x; i++)
                {
                    for (var j = 0; j < brush.y; j++)
                    {
                        positions[i * brush.y + j] = new Vector3Int(origin.x - xhalf + i, origin.y - yhalf + j,
                            origin.z);
                    }
                }
                _tilemapParamsFacade.SetTileParam(positions,param);
            }
        }
    }
}

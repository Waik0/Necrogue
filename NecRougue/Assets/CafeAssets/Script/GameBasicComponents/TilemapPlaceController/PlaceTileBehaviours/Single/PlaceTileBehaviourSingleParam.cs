using System.Collections.Generic;
using CafeAssets.Script.GameComponents.Tilemap;
using CafeAssets.Script.GameComponents.TilemapParams;
using UnityEngine;

namespace CafeAssets.Script.GameComponents.TilemapPlaceController
{
    public class PlaceTileBehaviourSingleParam : IPlaceTileBehaviour
    {
        private ITilemapUseCase _tilemapUseCase;
        private ITilemapParamsFacade<TileEffectParams> _tilemapEffectParamsFacade;
        private ITilemapParamsFacade<TileStaticParams> _tilemapStaticParamsFacade;
        public PlaceTileBehaviourSingleParam(
            ITilemapUseCase tilemapUseCase,
            ITilemapParamsFacade<TileEffectParams> tilemapEffectParamsFacade,
            ITilemapParamsFacade<TileStaticParams> tilemapStaticParamsFacade)
        {
            _tilemapUseCase = tilemapUseCase;
            _tilemapEffectParamsFacade = tilemapEffectParamsFacade;
            _tilemapStaticParamsFacade = tilemapStaticParamsFacade;
        }

        public PlaceTileMode TargetPlaceMode => PlaceTileMode.PlaceTileSingle;

        public void StartPlace(Vector3 pos, ITileModel model)
        {
        }

        public void UpdatePlace(Vector3 pos, ITileModel model)
        {
        }
        
        //todo リファクタ
        public void EndPlace(Vector3 pos, ITileModel model)
        {
            DebugLog.LogClassName(this, "PARAM配置");
            var z = model.ZMin();
            //pos.z = z;
            var origin = _tilemapUseCase.WorldToCell(pos);
            origin.z = z;
            var brush = model.BrushSize();
            if (brush.sqrMagnitude <= 2)
            {//単一のタイル
                SetEffect(origin.x, origin.y, origin.z, 1, 1, model);
                SetStatic(origin.x, origin.y, origin.z, 1, 1, model);
            }
            else
            {//ブラシタイル
                int xhalf = (brush.x + 1 - (brush.x % 2)) / 2;
                int yhalf = (brush.y + 1 - (brush.y % 2)) / 2;
                SetEffect(origin.x - xhalf, origin.y - yhalf, origin.z, brush.x, brush.y, model);
                SetStatic(origin.x - xhalf, origin.y - yhalf, origin.z, brush.x, brush.y, model);
            }
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
    }
}

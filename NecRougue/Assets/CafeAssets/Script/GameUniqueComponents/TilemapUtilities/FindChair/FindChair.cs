using System.Collections;
using System.Collections.Generic;
using CafeAssets.Script.GameComponents.TilemapAdapter.ScriptableObjects;
using CafeAssets.Script.GameComponents.TilemapParams;
using ModestTree;
using UnityEngine;

namespace CafeAssets.Script.GameUniqueComponents.FindChair
{
    #region BoundsOut
    
    public interface IFindChair
    {
        Vector3Int? RegisterAndGetSitDownPlace();
        Vector3Int[] GetCanSitDownPlace();
    }

    #endregion
    public class FindChair : IFindChair
    {
        public enum CanSitState
        {
            Full = 0,
            CanSit = 1,
        }
        private ITilemapParamsFacade _tilemapParamsFacade;
        public FindChair(
            ITilemapParamsFacade tilemapParamsFacade)
        {
            _tilemapParamsFacade = tilemapParamsFacade;
        }

        public Vector3Int? RegisterAndGetSitDownPlace()
        {
            var pos = SelectChair(GetCanSitDownPlace());
            if (pos == null) return null;
            RegisterChair(pos.Value);
            return pos;
        }

        void RegisterChair(Vector3Int position)
        {
            _tilemapParamsFacade.UpdateTileParam(position,TileEffectParams.Sit,(int) CanSitState.Full);
        }
        //todo 相席にならないようにする
        Vector3Int? SelectChair(Vector3Int[] candidate)
        {
            if (candidate.Length == 0) return null;
            return candidate[0];
        }

        public Vector3Int[] GetCanSitDownPlace()
        {
            var ret = new List<Vector3Int>();
            foreach (var keyValuePair in _tilemapParamsFacade.Entity)
            {
                foreach (var tileParamsModelBase in keyValuePair.Value)
                {
                    var param = tileParamsModelBase as ITileParamsModel<TileEffectParams>;
                    if (param == null)
                    {
                        continue;
                    }
                    if (param.Key != TileEffectParams.Sit)
                    {
                        continue;
                    }
                    if (param.Param == (int) CanSitState.Full)
                    {
                        continue;
                    }
                    ret.Add(keyValuePair.Key);
                    break;
                }
            }
            return ret.ToArray();
        }
    }
}
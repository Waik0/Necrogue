using System.Collections;
using System.Collections.Generic;
using CafeAssets.Script.GameComponents.Npc;
using CafeAssets.Script.GameComponents.Npc.NpcAi;
using CafeAssets.Script.GameComponents.Tilemap;
using CafeAssets.Script.GameComponents.TilemapParams;
using CafeAssets.Script.System.GameNpcSystem;
using UnityEngine;

public class NpcOrderCondition : INpcActionConditionUseCase
{
    public NpcActionPattern TargetPattern => NpcActionPattern.Order;
    private NpcParamWaitTime _paramWaitTime;
    private ITilemapUseCase _tilemapUseCase;
    private ITilemapParamsFacade<TileEffectParams> _tilemapParams;
    private INpcFacade _npcFacade;
    public NpcOrderCondition(
        NpcParamWaitTime waitTime,
        ITilemapParamsFacade<TileEffectParams> tilemapParams,
        ITilemapUseCase tilemapUseCase,
        INpcFacade npcFacade)
    {
        _paramWaitTime = waitTime;
        _tilemapParams = tilemapParams;
        _tilemapUseCase = tilemapUseCase;
        _npcFacade = npcFacade;
    }
    public bool CanAction()
    {
        if (_paramWaitTime.Time <= 0) return false;
        
        var pos = (Vector2) _npcFacade.GameObject.transform.position;
        var tilePos = _tilemapUseCase.WorldToCell(new Vector3(pos.x, pos.y, 0));
        //var order = _tilemapParams.GetTileParam((Vector2Int)tilePos,TileEffectParams.Order);
        return true;
    }
}

using System.Collections;
using System.Collections.Generic;
using CafeAssets.Script.GameComponents.Npc;
using CafeAssets.Script.GameComponents.Npc.NpcAi;
using CafeAssets.Script.GameComponents.Tilemap;
using CafeAssets.Script.GameComponents.TilemapParams;
using CafeAssets.Script.System.GameNpcSystem;
using UnityEngine;


//注文
//受付状態のNPCが近くにいるときのみ可能
public class NpcOrder : INpcActionUseCase
{
    public NpcActionPattern TargetPattern => NpcActionPattern.Order;
    public NpcActionStatus CurrentStatus { get; private set; }

    private ITilemapUseCase _tilemapUseCase;
    private ITilemapParamsFacade<TileEffectParams> _tilemapParams;
    private INpcFacade _npcFacade;
    private NpcParamOrder _paramOrder;
    public NpcOrder(ITilemapParamsFacade<TileEffectParams> tilemapParams,
        ITilemapUseCase tilemapUseCase,
        INpcFacade npcFacade,
        NpcParamOrder order)
    {
        _tilemapParams = tilemapParams;
        _tilemapUseCase = tilemapUseCase;
        _npcFacade = npcFacade;
        _paramOrder = order;

    }
    
    public void StartAction()
    {
        CurrentStatus = NpcActionStatus.Doing;
        _paramOrder.State = NpcParamOrder.OrderState.Calling;
    }

    public void EndAction()
    {
        //タイムアウトしたら評価下げつつ帰る
    }

    void UpdateAction()
    {
        //店員が近づいてくるまで待つ
        //CurrentStatus = NpcActionStatus.Complete;
    }

    public void Tick()
    {
        UpdateAction();
    }
}

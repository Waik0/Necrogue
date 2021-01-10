using System.Collections;
using System.Collections.Generic;
using System.Linq;
using CafeAssets.Script.GameComponents.Npc;
using CafeAssets.Script.GameComponents.Npc.NpcAi;
using CafeAssets.Script.GameComponents.Npc.NpcMove;
using CafeAssets.Script.GameComponents.Tilemap;
using CafeAssets.Script.GameComponents.TilemapParams;
using CafeAssets.Script.System.GameNpcSystem;
using UnityEngine;


public class NpcMoveToOrder : INpcActionUseCase
{
    public NpcActionPattern TargetPattern => NpcActionPattern.MoveToOrder;
    public NpcActionStatus CurrentStatus { get; private set; }

    private ITilemapUseCase _tilemapUseCase;
    private INpcFacade _npcFacade;
    private INpcFinder _npcFinder;
    private INpcAstarMoveUseCase _npcAstarMoveUseCase;
    private INpcParamRegistry _npcParamRegistry;
    private INpcParamCollector _npcParamCollector;
    private NpcParamTakeOrder _takeOrder;
    private GameObject _moveTo = null;

    public NpcMoveToOrder(
        ITilemapUseCase tilemapUseCase,
        INpcFacade npcFacade,
        INpcFinder npcFinder,
        INpcAstarMoveUseCase npcAstarMoveUseCase,
        INpcParamCollector npcParamCollector,
        INpcParamRegistry paramRegistry,
        NpcParamTakeOrder takeOrder)
    {
        _tilemapUseCase = tilemapUseCase;
        _npcFacade = npcFacade;
        _npcParamRegistry = paramRegistry;
        _npcFinder = npcFinder;
        _npcAstarMoveUseCase = npcAstarMoveUseCase;
        _takeOrder = takeOrder;
        _npcParamCollector = npcParamCollector;
    }
    
    public void StartAction()
    {
        CurrentStatus = NpcActionStatus.Doing;
        _moveTo = null;
        _takeOrder.State = NpcParamTakeOrder.TakeOrderState.Taking;
        _npcAstarMoveUseCase.Reset();
    }

    public void EndAction()
    {
        CurrentStatus = NpcActionStatus.Sleep;
    }

    public void Tick()
    {
        if (_moveTo == null)
        {
            SearchOrder();
            return;
        }
        Debug.Log(_npcAstarMoveUseCase.CurrentStatus);
        if (_npcAstarMoveUseCase.CurrentStatus == MoveState.Moving)
        {
            _npcAstarMoveUseCase.Tick();
        }

        if (_npcAstarMoveUseCase.CurrentStatus == MoveState.Complete)
        {
            CurrentStatus = NpcActionStatus.Complete;
        }

    }
    /// <summary>
    /// 客を探す
    /// </summary>
    void SearchOrder()
    {
        INpcFacade target = null;
        var list = _npcParamCollector.FindParamHolderIds<NpcParamOrder>(p => p.State == NpcParamOrder.OrderState.Calling);
        foreach (var s in list)
        {
            if (s == _npcFacade.Id)
            {
                continue;
            }

            var r = _npcFinder.Find(s);
            if (r == null)
            {
                continue;
            }
            target = r;
            break;
        }

        if (target == null)
        {
            return;
        }
        _moveTo = target.GameObject;
        _npcAstarMoveUseCase.Start(_tilemapUseCase.WorldToCell(_moveTo.transform.position));
    }

}
using System.Collections;
using System.Collections.Generic;
using CafeAssets.Script.GameComponents.Npc;
using CafeAssets.Script.GameComponents.Npc.NpcAi;
using CafeAssets.Script.GameComponents.Tilemap;
using CafeAssets.Script.GameComponents.TilemapParams;
using CafeAssets.Script.System.GameNpcSystem;
using UnityEngine;

public class NpcMoveToOrderCondition : INpcActionConditionUseCase
{
    public NpcActionPattern TargetPattern => NpcActionPattern.MoveToOrder;
    private INpcParamCollector _paramCollectorUseCase;
    public NpcMoveToOrderCondition(
        INpcParamCollector paramCollectorForNpcParamType)
    {
        _paramCollectorUseCase = paramCollectorForNpcParamType;
    }
    public bool CanAction()
    {
        //呼び出し中のNPCを探す
        var list = _paramCollectorUseCase.FindParamHolderIds<NpcParamOrder>( p => p.State == NpcParamOrder.OrderState.Calling);
        return list.Length > 0;
    }
}

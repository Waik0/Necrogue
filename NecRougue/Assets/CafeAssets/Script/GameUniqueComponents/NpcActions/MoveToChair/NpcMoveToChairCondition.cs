using System.Collections;
using System.Collections.Generic;
using CafeAssets.Script.GameComponents.Npc.NpcAi;
using CafeAssets.Script.GameUniqueComponents.FindChair;
using CafeAssets.Script.System.GameNpcSystem;
using UnityEngine;

public class NpcMoveToChairCondition : INpcActionConditionUseCase
{
    public NpcActionPattern TargetPattern { get; private set; } = NpcActionPattern.MoveToChair;
    private IFindChair _findChair;
    private NpcParamWaitTime _npcParamWait;
    public NpcMoveToChairCondition(
        IFindChair findChair ,
        NpcParamWaitTime waitTime
    )
    {
        _findChair = findChair;
        _npcParamWait = waitTime;
    }
    public bool CanAction()
    {
        return _findChair.GetCanSitDownPlace().Length > 0 &&
               _npcParamWait.Time <= 0;
    }
}
using System.Collections;
using System.Collections.Generic;
using CafeAssets.Script.GameComponents.Npc.NpcAi;
using CafeAssets.Script.GameUniqueComponents.FindChair;
using UnityEngine;

public class NpcMoveToChairCondition : INpcActionConditionUseCase
{
    public NpcActionPattern TargetPattern { get; private set; } = NpcActionPattern.MoveToChair;
    private IFindChair _findChair;

    public NpcMoveToChairCondition(
        IFindChair findChair
    )
    {
        _findChair = findChair;
    }
    public bool CanAction()
    {
        return _findChair.GetCanSitDownPlace().Length > 0;
    }
}
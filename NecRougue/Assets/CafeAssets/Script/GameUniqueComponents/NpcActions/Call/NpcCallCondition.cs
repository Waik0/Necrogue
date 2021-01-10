using System.Collections;
using System.Collections.Generic;
using CafeAssets.Script.GameComponents.Npc.NpcAi;
using CafeAssets.Script.System.GameNpcSystem;
using UnityEngine;

public class NpcCallCondition : INpcActionConditionUseCase
{
    public NpcActionPattern TargetPattern => NpcActionPattern.Call;
    private NpcParamWaitTime _waitTime;

    public NpcCallCondition(
        NpcParamWaitTime waitTime
    )
    {
        _waitTime = waitTime;
    }
    public bool CanAction()
    {
        return _waitTime.Time > 0;
    }
}

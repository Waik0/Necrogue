using System.Collections;
using System.Collections.Generic;
using ShopperAssets.Scripts.Game;
using UnityEngine;
public enum GUIDType
{
    Hand,
    Trash,
    Enemy,
}

public class AbilityCommandRequestModel
{
    
    public (string GUID, GUIDType GuidType) Self;
    public AbilityModel AbilityModel;
    public AbilityConditionResponseModel Response;
    public AbilityUseCase.AbilityTiming NowTiming;
}

public class AbilityCommandResponseModel
{
    //結果
    public List<NextResolveAbility> NextResolveAbility;
    public AbilityPerformanceParams AbilityPerformanceParams;
}
public class AbilityConditionRequestModel
{
    public AbilityUseCase.AbilityCondition Condition;
    public int ConditionParam;
    public bool Cancelable;
}
public class AbilityConditionResponseModel
{
    public AbilityUseCase.AbilityCondition Condition;
    public (string[] GUIDs, GUIDType GuidType) Targets;
    public bool Canceled;
}
/// <summary>
/// Ability中に発動確定したアビリティをSequenceレイヤに通知するためのクラス
/// </summary>
public class NextResolveAbility
{
    public (string GUID, GUIDType GuidType) Next;
    public AbilityUseCase.AbilityTiming Timing;
}

public class AbilityPerformanceParams
{
    //todo アニメーション関係のParamも用意
    public int PlayerAction;
    public List<(string,int)> EnemyAction;//プレイヤー以外のアクション

}
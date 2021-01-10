using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// public enum NpcParamType
// {
//     //共通
//     SitDown,
//     //客 
//     Order,
//     WaitTime,
//     Eating,
//     //店員
//     Cleaning,
//     TakeOrder,
//     
// }


#region 共通

/// <summary>
/// 座っているか立っているか
/// </summary>
public class NpcParamSitDown : INpcParamCollection
{
    public enum SitDownState
    {
        Stand = 0,
        Sit = 1,
    }
    public SitDownState State { get; set; }
    public void Reset()
    {
        State = SitDownState.Stand;
    }

    public void Tick()
    {
        
    }

    public string Log()
    {
        return $"Sit : {State.ToString()}";
    }
}

#endregion

#region 客

public class NpcParamOrder : INpcParamCollection
{
    public enum OrderState
    {
        None = 0,
        Calling = 1,
        Ordering = 2,
    }
    public OrderState State { get; set; }
    public void Reset()
    {
        State = OrderState.None;
    }

    public void Tick()
    {
        
    }

    public string Log()
    {
        return $"Order : {State.ToString()}";
    }
}
public class NpcParamWaitTime : INpcParamCollection
{
    public int Time { get; set; }
    public void Reset()
    {
        Time = 0;
    }

    public void Tick()
    {
        if (Time > 0)
        {
            Time--;
        }
    }

    public string Log()
    {
        return $"WaitTime : {Time}";
    }
}
#endregion

#region 店員
public class NpcParamTakeOrder : INpcParamCollection
{
    public enum TakeOrderState
    {
        None = 0,
        Taking = 1,
    }
    public TakeOrderState State { get; set; }
    public void Reset()
    {
        State = TakeOrderState.None;
    }

    public void Tick()
    {
        
    }

    public string Log()
    {
        return $"TakeOrder : {State.ToString()}";
    }
}

#endregion

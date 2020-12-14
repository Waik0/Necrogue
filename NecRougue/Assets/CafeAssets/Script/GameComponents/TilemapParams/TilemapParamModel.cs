using System;
using System.Collections.Generic;
using System.Linq;

namespace CafeAssets.Script.GameComponents.TilemapParams
{

    /// <summary>
    /// todo 以下implに移動
    /// </summary>
    public enum TilemapParameterStyle
    {
        Fixable,
        //Fluid
    }


    public enum TilemapParameterOperations
    {
        Add = 1,
        Times = 2,
        Division = 3,
    }
    public interface ITileParamsModelBase
    {
        int Param { get; set; }
        ITileParamsModelBase DeepCopy();
    }
    
    
}
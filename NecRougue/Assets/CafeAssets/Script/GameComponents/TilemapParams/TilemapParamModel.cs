using System;
using System.Collections.Generic;
using System.Linq;
public enum GameParameters
{
    //基礎
    Money,
    //研究度
    
    MenuCoffee,
    Menu
}
namespace CafeAssets.Script.GameComponents.TilemapParams
{
    
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

    /// <summary>
    /// タイルマップでは管理できない動的なデータたち
    /// </summary>
    public class TilemapPropsModel
    {
        public bool CanSitDown;
        public bool IsSitDown;
    }
    public class TilemapDynamicParamModel
    {
        public Dictionary<TilemapParameterStyle, int> Param;
        public TilemapDynamicParamModel()
        {
            Param = new Dictionary<TilemapParameterStyle, int>();
            foreach (TilemapParameterStyle key in Enum.GetValues(typeof(TilemapParameterStyle)))
            {
                Param.Add(key,0);
            }
        }

        public void Set(TilemapParameterStyle key, int num)
        {
            Param[key] = num;
        }

        public int Get(TilemapParameterStyle key)
        {
            return Param[key];
        }

        public int GetSum()
        {
            return Param.Sum(keyValuePair => keyValuePair.Value);
        }
    }
}
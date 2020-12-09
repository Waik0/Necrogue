using System.Collections.Generic;
using System.Linq;
using CafeAssets.Script.GameComponents.Npc.NpcParam;

namespace CafeAssets.Script.System.GameNpcSystem
{
    public interface INpcParamUseCase
    {
        void Reset();
        string[] GetKeys();
        int Get(string key);
        int Set(NpcParameterStyle style, string key, int num);
        int Operation(NpcParameterStyle style, string key, NpcParameterOperations operation, int num);
    }
    /// <summary>
    /// 
    /// </summary>
    public class NpcParamUseCase : INpcParamUseCase
    {
        private Dictionary<string, NpcDynamicParamModel> IntParams;
        public void Reset()
        {
            if (IntParams == null)
            {
                IntParams = new Dictionary<string, NpcDynamicParamModel>();
            }
            IntParams.Clear();
        }

        public string[] GetKeys()
        {
            if (IntParams == null)
            {
                IntParams = new Dictionary<string, NpcDynamicParamModel>();
            }
            return IntParams.Keys.ToArray();
        }

        void AddModel(string key)
        {
            if (!IntParams.ContainsKey(key))
            {
                IntParams.Add(key,new NpcDynamicParamModel());
            }
        }

     
        public int Get(string key)
        {
            AddModel(key);
            return IntParams[key].GetSum();
        }
        

        void Clear(NpcParameterStyle style)
        {
            foreach (var gameParamModel in IntParams)
            {
                gameParamModel.Value.Set(style,0);
            }
        }
        public int Set(NpcParameterStyle style,string key, int num)
        {
            AddModel(key);
            IntParams[key].Set(style,num);
            return IntParams[key].GetSum();
        }

        public int Operation(NpcParameterStyle style,string key, NpcParameterOperations operation, int num)
        {
            AddModel(key);
            switch (operation)
            {
                case NpcParameterOperations.Add:
                    IntParams[key].Set(style, IntParams[key].Get(style) + num);
                    break;
                case NpcParameterOperations.Times:
                    IntParams[key].Set(style, IntParams[key].Get(style) * num);
                    break;
                case NpcParameterOperations.Division:
                    IntParams[key].Set(style, IntParams[key].Get(style) / num);
                    break;
            }
            return IntParams[key].GetSum();
        }

    }
    
}
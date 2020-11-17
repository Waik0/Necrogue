using System.Collections;
using System.Collections.Generic;
using System.Linq;
using CafeAssets.Script.Model;
using CafeAssets.Script.System.GameMapSystem;
using CafeAssets.Script.System.GameParameterSystem;
using UnityEngine;
using UnityEngine.Tilemaps;

namespace CafeAssets.Script.System.GameNpcSystem
{
    public interface INpcParamUseCase
    {
        void Reset();
        string[] GetKeys();
        int Get(string key);
        int Set(ParameterStyle style, string key, int num);
        int Operation(ParameterStyle style, string key, GameParameterOperations operation, int num);
    }
    /// <summary>
    /// 
    /// </summary>
    public class NpcParamUseCase : INpcParamUseCase
    {
        private Dictionary<string, GameParamModel> IntParams;
        public void Reset()
        {
            if (IntParams == null)
            {
                IntParams = new Dictionary<string, GameParamModel>();
            }
            IntParams.Clear();
        }

        public string[] GetKeys()
        {
            if (IntParams == null)
            {
                IntParams = new Dictionary<string, GameParamModel>();
            }
            return IntParams.Keys.ToArray();
        }

        void AddModel(string key)
        {
            if (!IntParams.ContainsKey(key))
            {
                IntParams.Add(key,new GameParamModel());
            }
        }

     
        public int Get(string key)
        {
            AddModel(key);
            return IntParams[key].GetSum();
        }

        void Clear(ParameterStyle style)
        {
            foreach (var gameParamModel in IntParams)
            {
                gameParamModel.Value.Set(style,0);
            }
        }
        public int Set(ParameterStyle style,string key, int num)
        {
            AddModel(key);
            IntParams[key].Set(style,num);
            return IntParams[key].GetSum();
        }

        public int Operation(ParameterStyle style,string key, GameParameterOperations operation, int num)
        {
            AddModel(key);
            switch (operation)
            {
                case GameParameterOperations.Add:
                    IntParams[key].Set(style, IntParams[key].Get(style) + num);
                    break;
                case GameParameterOperations.Times:
                    IntParams[key].Set(style, IntParams[key].Get(style) * num);
                    break;
                case GameParameterOperations.Division:
                    IntParams[key].Set(style, IntParams[key].Get(style) / num);
                    break;
            }
            return IntParams[key].GetSum();
        }

    }
    
}
using System;
using System.Collections.Generic;
using System.Linq;
using CafeAssets.Script.GameComponents.Tilemap;
using CafeAssets.Script.GameComponents.TilemapParams;
using UniRx;
using UnityEngine;


namespace CafeAssets.Script.System.GameParameterSystem
{
    public interface ITilemapStaticParameterUseCase
    {
        void Reset();
        void CalcTileParam();
        string[] GetKeys();
        int Get(string key);
        int Set(TilemapParameterStyle style, string key, int num);
        int Operation(TilemapParameterStyle style, string key, TilemapParameterOperations operation, int num);
    }

    public class TilemapStaticParamsUseCase : ITilemapStaticParameterUseCase,IDisposable
    {
        private Dictionary<string, TilemapDynamicParamModel> IntParams;//固定資産
        private TileModelProvider _tileModelProvider;
        private ITilemapUseCase _tilemapUseCase;
        private CompositeDisposable _compositeDisposable = new CompositeDisposable();
        public TilemapStaticParamsUseCase(
            TileModelProvider tileModelProvider,
            ITilemapUseCase tilemapUseCase
        )
        {
            _tileModelProvider = tileModelProvider;
            _tilemapUseCase = tilemapUseCase;
            Subscribe();
        }

        void Subscribe()
        {
            _tilemapUseCase.OnUpdateTiles.Subscribe(_=>CalcTileParam()).AddTo(_compositeDisposable);
        }
        public void Reset()
        {
            if (IntParams == null)
            {
                IntParams = new Dictionary<string, TilemapDynamicParamModel>();
            }
            IntParams.Clear();
        }

        public string[] GetKeys()
        {
            if (IntParams == null)
            {
                IntParams = new Dictionary<string, TilemapDynamicParamModel>();
            }
            return IntParams.Keys.ToArray();
        }

        void AddModel(string key)
        {
            if (!IntParams.ContainsKey(key))
            {
                IntParams.Add(key,new TilemapDynamicParamModel());
            }
        }

     
        public int Get(string key)
        {
            AddModel(key);
            return IntParams[key].GetSum();
        }

        void Clear(TilemapParameterStyle style)
        {
            foreach (var gameParamModel in IntParams)
            {
                gameParamModel.Value.Set(style,0);
            }
        }
        public int Set(TilemapParameterStyle style,string key, int num)
        {
            AddModel(key);
            IntParams[key].Set(style,num);
            return IntParams[key].GetSum();
        }

        public int Operation(TilemapParameterStyle style,string key, TilemapParameterOperations operation, int num)
        {
            AddModel(key);
            switch (operation)
            {
                case TilemapParameterOperations.Add:
                    IntParams[key].Set(style, IntParams[key].Get(style) + num);
                    break;
                case TilemapParameterOperations.Times:
                    IntParams[key].Set(style, IntParams[key].Get(style) * num);
                    break;
                case TilemapParameterOperations.Division:
                    IntParams[key].Set(style, IntParams[key].Get(style) / num);
                    break;
            }
            return IntParams[key].GetSum();
        }

        public void Dispose()
        {
            Debug.Log("[GameParam]Dispose");
            _compositeDisposable.Clear();
        }
        
        //todo 重くなりそうなので色々考えたい
        public void CalcTileParam()
        {
            Clear(TilemapParameterStyle.Fixable);
            List<ProvideParameterModelSet> models = new List<ProvideParameterModelSet>();
            foreach (var aggregateAllTileModel in _tilemapUseCase.AggregateAllTileModels())
            {
                models.AddRange(aggregateAllTileModel.GetProvideParameter());
            }
            //Debug.Log(models.Count);
            foreach (var provideParameterModelSet in models.OrderBy(_ => _.Operation))
            {
                Debug.Log($"{ provideParameterModelSet.Name} { provideParameterModelSet.Operation} {provideParameterModelSet.Num}");
                Operation(TilemapParameterStyle.Fixable, provideParameterModelSet.Name, provideParameterModelSet.Operation,provideParameterModelSet.Num);
            }
           
        }
    }


}
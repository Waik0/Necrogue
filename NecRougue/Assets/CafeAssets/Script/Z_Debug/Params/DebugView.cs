using System;
using System.Collections.Generic;
using CafeAssets.Script.GameComponents.GameInput;
using CafeAssets.Script.GameComponents.Tilemap;
using CafeAssets.Script.GameComponents.TilemapParams;
using CafeAssets.Script.GameComponents.TilemapPlaceController;
using UniRx;
using UnityEngine;
using UnityEngine.UI;
using Zenject;
public class DebugView : MonoBehaviour,IDebugView
{
    [SerializeField] private Text _text;
    private ITileSelectUseCase _tileSelectUseCase;
    private IGameInputController _gameInputView;
    private IPlaceTileUseCase _mapPlaceUseCase;
    private ITilemapParamsFacade<TileEffectParams> _tilemapEffectParamsFacade;
    private ITilemapParamsFacade<TileStaticParams> _tilemapStaticParamsFacade;
    [Inject]
    void Inject(
        ITileSelectUseCase tileSelectUseCase,
        IGameInputController gameInputView,
        IPlaceTileUseCase mapPlaceUseCase,
        ITilemapParamsFacade<TileEffectParams> tilemapEffectParamsFacade,
        ITilemapParamsFacade<TileStaticParams> tilemapStaticParamsFacade,
        ITilemapUseCase tilemapUseCase)
    {
        _tileSelectUseCase = tileSelectUseCase;
        _gameInputView = gameInputView;
        _mapPlaceUseCase = mapPlaceUseCase;
        _tilemapEffectParamsFacade = tilemapEffectParamsFacade;
        _tilemapStaticParamsFacade = tilemapStaticParamsFacade;
        _tilemapEffectParamsFacade.OnUpdateTileParams.Subscribe(_ => OnUpdateTile()).AddTo(this);
    }

    void Update()
    {
        _text.text = "MAP DEBUG \n" +
                     $"{_tileSelectUseCase.SelectedType}\n" +
                     $"{_mapPlaceUseCase.SelectedTile?.GetName()}\n" +
                     $"{_mapPlaceUseCase.PlaceTileMode}\n" +
                     "\n";
        _text.text +="INPUT DEBUG \n" +
                     $"{_gameInputView.Model.State} \n" +
                     $"{_gameInputView.Model.InputMode} \n" +
                     "\n";
        _text.text += "PARAM DEBUG \n";
        if (_staticParams == null)
        {
            _staticParams = new Dictionary<TileStaticParams, int>();
        }
        foreach (var keyValuePair in _staticParams)
        {
            _text.text += $"{keyValuePair.Key,-10} = {(keyValuePair.Value),-5}";
        }
        if (_effectiveParams == null)
        {
            _effectiveParams = new Dictionary<TileEffectParams, int>();
        }
        foreach (var keyValuePair in _effectiveParams)
        {
            _text.text += $"{keyValuePair.Key,-10} = {(keyValuePair.Value),-5}";
        }
    }

    
    private Dictionary<TileStaticParams, int> _staticParams;
    private Dictionary<TileEffectParams, int> _effectiveParams;
    //Param集計情報更新
    void OnUpdateTile()
    {
        UpdateParams(_tilemapStaticParamsFacade,_staticParams);
        UpdateParams(_tilemapEffectParamsFacade,_effectiveParams);
    }

    void UpdateParams<T>(ITilemapParamsFacade<T> facade,Dictionary<T,int> dic )where T : struct
    {
        if (dic == null)
        {
            dic = new Dictionary<T, int>();
        }
        foreach (T value in Enum.GetValues(typeof(T)))
        {
            if (!dic.ContainsKey(value))
            {
                continue;
            }
            dic[value] = 0;
        }
        foreach (var keyValuePair in facade.Entity)
        {
            foreach (var tileParamsModelBase in keyValuePair.Value)
            {
                if (tileParamsModelBase is ITileParamsModelBase<T> m)
                {
                    if (!dic.ContainsKey(m.Key))
                    {
                        dic.Add(m.Key,0);
                    }

                    dic[m.Key] += m.Param;
                }
            }
        }
    }

}



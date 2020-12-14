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
    private ITilemapParamsFacade _tilemapParamsFacade;
    [Inject]
    void Inject(
        ITileSelectUseCase tileSelectUseCase,
        IGameInputController gameInputView,
        IPlaceTileUseCase mapPlaceUseCase,
        ITilemapParamsFacade tilemapParamsFacade,
        ITilemapUseCase tilemapUseCase)
    {
        _tileSelectUseCase = tileSelectUseCase;
        _gameInputView = gameInputView;
        _mapPlaceUseCase = mapPlaceUseCase;
        _tilemapParamsFacade = tilemapParamsFacade;
        _tilemapParamsFacade.OnUpdateTileParams.Subscribe(_ => OnUpdateTile()).AddTo(this);
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
        if (_params == null)
        {
            _params = new Dictionary<TileStaticParams, int>();
        }
        foreach (var keyValuePair in _params)
        {
            _text.text += $"{keyValuePair.Key,-10} = {(keyValuePair.Value),-5}";
        }
    }

    
    private Dictionary<TileStaticParams, int> _params;
    //Param集計情報更新
    void OnUpdateTile()
    {
        if (_params == null)
        {
            _params = new Dictionary<TileStaticParams, int>();
        }
        foreach (TileStaticParams value in Enum.GetValues(typeof(TileStaticParams)))
        {
            if (!_params.ContainsKey(value))
            {
               continue;
            }
            _params[value] = 0;
        }
        foreach (var keyValuePair in _tilemapParamsFacade.Entity)
        {
            foreach (var tileParamsModelBase in keyValuePair.Value)
            {
                if (tileParamsModelBase is ITileParamsModel<TileStaticParams> m)
                {
                    Debug.Log(keyValuePair.Key + " " + m.Key + " " + m.Param);
                    if (!_params.ContainsKey(m.Key))
                    {
                        _params.Add(m.Key,0);
                    }

                    _params[m.Key] += m.Param;
                }
            }
        }

       
    }

}



using System.Collections;
using System.Collections.Generic;
using CafeAssets.Script.Interface.UseCase;
using CafeAssets.Script.Interface.View;
using CafeAssets.Script.System.GameInputSystem;
using CafeAssets.Script.System.GameMapSystem;
using CafeAssets.Script.System.GameParameterSystem;
using UnityEngine;
using UnityEngine.UI;
using Zenject;

public class DebugView : MonoBehaviour,IDebugView
{
    [SerializeField] private Text _text;
    private ITilemapView _tilemapView;
    private ITileSelectView _tileSelectView;
    private IGameInputView _gameInputView;
    private IPlaceTileUseCase _mapPlaceUseCase;
    private IGameParameterUseCase _gameParameterUseCase;
    [Inject]
    void Inject(
        ITilemapView tilemapView, 
        ITileSelectView tileSelectView,
        IGameInputView gameInputView,
        IPlaceTileUseCase mapPlaceUseCase,
        IGameParameterUseCase gameParameterUseCase)
    {
        _tilemapView = tilemapView;
        _tileSelectView = tileSelectView;
        _gameInputView = gameInputView;
        _mapPlaceUseCase = mapPlaceUseCase;
        _gameParameterUseCase = gameParameterUseCase;
    }

    void Update()
    {
        _text.text = "MAP DEBUG \n" +
                     $"{_tileSelectView.SelectedType}\n" +
                     $"{_mapPlaceUseCase.SelectedTile?.Name}\n" +
                     $"{_mapPlaceUseCase.PlaceTileMode}\n" +
                     "\n";
        _text.text +="INPUT DEBUG \n" +
                     $"{_gameInputView.Model.State} \n" +
                     $"{_gameInputView.Model.InputMode} \n" +
                     "\n";
        _text.text += "PARAM DEBUG \n";
        foreach (var key in _gameParameterUseCase.GetKeys())
        {
            _text.text += $"{key,-10} = {_gameParameterUseCase.Get(key),-5}";
        }
    }

}

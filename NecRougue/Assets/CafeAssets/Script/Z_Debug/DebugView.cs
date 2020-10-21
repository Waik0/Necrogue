using System.Collections;
using System.Collections.Generic;
using CafeAssets.Script.System.GameInputSystem;
using CafeAssets.Script.System.GameMapSystem;
using UnityEngine;
using UnityEngine.UI;
using Zenject;

public class DebugView : MonoBehaviour,IDebugView
{
    [SerializeField] private Text _text;
    private IMapView _mapView;
    private ITileSelectView _tileSelectView;
    private IGameInputView _gameInputView;
    private IMapPlaceUseCase _mapPlaceUseCase;
    [Inject]
    void Inject(
        IMapView mapView, 
        ITileSelectView tileSelectView,
        IGameInputView gameInputView,
        IMapPlaceUseCase mapPlaceUseCase)
    {
        _mapView = mapView;
        _tileSelectView = tileSelectView;
        _gameInputView = gameInputView;
        _mapPlaceUseCase = mapPlaceUseCase;
    }

    void Update()
    {
        _text.text = "MAP DEBUG \n"+
            $"{_tileSelectView.SelectedType}\n" + 
            $"{_mapPlaceUseCase.SelectedTile}\n\n" +
            "INPUT DEBUG \n"+
            $"{_gameInputView.Model.State} \n"+
            $"{_gameInputView.Model.IsPlaceTileMode}";
    }

}

using CafeAssets.Script.GameComponents.GameInput;
using CafeAssets.Script.GameComponents.Tilemap;
using CafeAssets.Script.GameComponents.TilemapPlaceController;
using CafeAssets.Script.System.GameMapSystem;
using CafeAssets.Script.System.GameParameterSystem;
using UnityEngine;
using UnityEngine.UI;
using Zenject;

public class DebugView : MonoBehaviour,IDebugView
{
    [SerializeField] private Text _text;
    private ITileSelectUseCase _tileSelectUseCase;
    private IGameInputController _gameInputView;
    private IPlaceTileUseCase _mapPlaceUseCase;
    private ITilemapStaticParameterUseCase _gameParameterUseCase;
    [Inject]
    void Inject(
        ITileSelectUseCase tileSelectUseCase,
        IGameInputController gameInputView,
        IPlaceTileUseCase mapPlaceUseCase,
        ITilemapStaticParameterUseCase gameParameterUseCase)
    {
        _tileSelectUseCase = tileSelectUseCase;
        _gameInputView = gameInputView;
        _mapPlaceUseCase = mapPlaceUseCase;
        _gameParameterUseCase = gameParameterUseCase;
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
        foreach (var key in _gameParameterUseCase.GetKeys())
        {
            _text.text += $"{key,-10} = {_gameParameterUseCase.Get(key),-5}";
        }
    }

}



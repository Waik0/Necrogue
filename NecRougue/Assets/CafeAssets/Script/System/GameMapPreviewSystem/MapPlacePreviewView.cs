using System;
using System.Collections;
using System.Collections.Generic;
using CafeAssets.Script.System.GameCoreSystem;
using CafeAssets.Script.System.GameMapSystem;
using UnityEngine;
using UnityEngine.Tilemaps;
using Zenject;

public interface IMapPlacePreviewView
{
    
}
public class MapPlacePreviewView : MonoBehaviour,IMapPlacePreviewView,IGameScreenInputReceiver,IGameResettable
{
    [SerializeField] private Tilemap _tilemap;
    [SerializeField] private TilemapRenderer _tilemapRenderer;
    [SerializeField] private TileModel _allowTile;

    private IMapPlaceUseCase _placeUseCase;
    [Inject]
    public void Inject(
        IMapPlaceUseCase placeUseCase
    )
    {
        _placeUseCase = placeUseCase;
    }

    public void ResetOnGame()
    {
        Debug.Log("[MapPreviewView]Reset");
        _tilemap.ClearAllTiles();
    }

    void SetPreview(GameInputModel model)
    {
        _tilemap.ClearAllTiles();
        var placeModel = model.ToPlaceModel(_placeUseCase.PlaceTileMode);
        placeModel.Model = _allowTile;
        _tilemap.SetTileModel(placeModel);
    }

    public void GameInput(GameInputModel model)
    {
        if(model.InputMode != InputMode.PlaceTile)
            return;
        switch (model.State)
        {
            case GameInputState.PointerDown:
            case GameInputState.Drug:
                SetPreview(model);
                break;
            case GameInputState.PointerUp:
                _tilemap.ClearAllTiles();
                break;
            default:
                throw new ArgumentOutOfRangeException();
        }
    }
}

using CafeAssets.Script.Interface.Layer_01.Manager;
using CafeAssets.Script.Interface.UseCase;
using CafeAssets.Script.Interface.View;
using CafeAssets.Script.Model;
using CafeAssets.Script.System.GameCameraSystem;
using CafeAssets.Script.System.GameMapSystem;
using UnityEngine;
using UnityEngine.EventSystems;
using Zenject;

namespace CafeAssets.Script.System.GameInputSystem
{
    public class GameInputView : MonoBehaviour,IGameInputView
    {
        private ITilemapView _tilemapView;
        private ICameraView _cameraView;
        private IGameInputManager _inputManager;
        private ITileSelectView _tileSelectView;
        private IPlaceTileUseCase _mapPlaceUseCase;
        private GameInputModel _model;
        private Vector3 _before;
        public GameInputModel Model => _model;
        [Inject]
        void Inject(
            ITilemapView tilemapView,
            ICameraView cameraView,
            IGameInputManager inputManager,
            IPlaceTileUseCase mapPlaceUseCase,
            ITileSelectView tileSelectView)
        {
            _tilemapView = tilemapView;
            _cameraView = cameraView;
            _inputManager = inputManager;
            _tileSelectView = tileSelectView;
            _mapPlaceUseCase = mapPlaceUseCase;
            _model = new GameInputModel();
        }

        void SendInput(BaseEventData e,GameInputState state)
        {
          
            var pe = (PointerEventData) e;
            var pos = _cameraView.ScreenToWorldPoint(pe.position);
            //前回がUpだったか、押されたタイミング
            if (_model.State == GameInputState.PointerUp || 
                state == GameInputState.PointerDown)
            {
                Debug.Log("Reset");
                _before = pe.position;
                _model.DownPos = pe.position;
                _model.WorldDownPos = pos;
            }

            if (state == GameInputState.PointerDown)
            {
                //downの時のみ判定
                var mode = InputMode.MoveCamera;
                if (_mapPlaceUseCase.SelectedTile != null)
                {
                    mode = InputMode.PlaceTile;
                }
                _model.InputMode = mode;
            }
            
            _model.Delta = pe.delta;
            _model.WorldDelta = pos - _cameraView.ScreenToWorldPoint(_before);
            _model.CurrentPos = pe.position;
            _model.WorldCurrentPos = pos;
            _model.State = state;
            
            _inputManager.InputOnGame(_model);
            _before = pe.position;
        }
        public void OnPointerDown(BaseEventData e)
        {
            //check tile state
            
            SendInput(e,GameInputState.PointerDown);
        }

        public void OnPointerUp(BaseEventData e)
        {
            SendInput(e,GameInputState.PointerUp);
        }

        public void OnDrag(BaseEventData e)
        {
            SendInput(e,GameInputState.Drug);
        }

       
    }
}

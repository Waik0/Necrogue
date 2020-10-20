using CafeAssets.Script.System.GameCameraSystem;
using CafeAssets.Script.System.GameMapSystem;
using UnityEngine;
using UnityEngine.EventSystems;
using Zenject;

namespace CafeAssets.Script.System.GameInputSystem
{
    public interface IGameInputView
    {
        
    }
    public class GameInputView : MonoBehaviour,IGameInputView
    {
        private bool _placeTileMode = false;
        private IMapView _mapView;
        private ICameraView _cameraView;
        private IGameInputManager _inputManager;
        private GameInputModel _model;
        [Inject]
        void Inject(
            IMapView mapView,
            ICameraView cameraView,
            IGameInputManager inputManager)
        {
            _mapView = mapView;
            _cameraView = cameraView;
            _inputManager = inputManager;
            _model = new GameInputModel();
        }

        void SendInput(BaseEventData e,GameInputState state)
        {
            var pe = (PointerEventData) e;
            _model.Delta = pe.delta;
            _model.Pos = pe.position;
            _model.State = state;
            _model.IsPlaceTileMode = _placeTileMode;
            _inputManager.InputOnGame(_model);
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

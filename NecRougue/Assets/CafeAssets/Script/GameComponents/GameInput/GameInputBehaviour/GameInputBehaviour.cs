using CafeAssets.Script.GameComponents.TilemapPlaceController;
using CafeAssets.Script.GameComponents.TilemapPlacePreview;
using CafeAssets.Script.System.GameCameraSystem;
using NotImplementedException = System.NotImplementedException;

namespace CafeAssets.Script.GameComponents.GameInput
{
    public class SendCameraInput : IGameInputBehaviour
    {
        public InputMode TargetInputMode => InputMode.MoveCamera;
        private ICameraUseCase _cameraUseCase;
        public SendCameraInput(
            ICameraUseCase cameraUseCase
        )
        {
            _cameraUseCase = cameraUseCase;
        }

        public void OnStartInput(GameInputModel model)
        {
            _cameraUseCase.MoveCameraDelta(model.WorldDelta);
        }

        public void OnUpdateInput(GameInputModel model)
        {
            _cameraUseCase.MoveCameraDelta(model.WorldDelta);
        }

        public void OnEndInput(GameInputModel model)
        {
            _cameraUseCase.MoveCameraDelta(model.WorldDelta);
        }
    }
    public class SendTilemapInput : IGameInputBehaviour
    {
        public InputMode TargetInputMode => InputMode.PlaceTile;
        private IPlaceTileUseCase _placeTileUseCase;
        private ITileSelectUseCase _tileSelectUseCase;
        public SendTilemapInput(
            IPlaceTileUseCase placeTileUseCase,
            ITileSelectUseCase tileSelectUseCase
            )
        {
            _placeTileUseCase = placeTileUseCase;
            _tileSelectUseCase = tileSelectUseCase;
        }

       

        public void OnStartInput(GameInputModel model)
        {
            DebugLog.Function(this);
            _placeTileUseCase.StartPlace(model.WorldCurrentPos);
        }

        public void OnUpdateInput(GameInputModel model)
        {
            _placeTileUseCase.UpdatePlace(model.WorldCurrentPos);
        }

        public void OnEndInput(GameInputModel model)
        {
            DebugLog.Function(this);
            _placeTileUseCase.EndPlace(model.WorldCurrentPos);
            _tileSelectUseCase.ClearSelect();
        }
    }
}
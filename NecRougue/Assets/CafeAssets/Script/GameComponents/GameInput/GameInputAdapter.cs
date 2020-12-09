using System;
using System.Collections.Generic;
using CafeAssets.Script.GameComponents.TilemapPlaceController;
using CafeAssets.Script.System.GameCameraSystem;
using UnityEngine;
using UnityEngine.EventSystems;
using Zenject;

namespace CafeAssets.Script.GameComponents.GameInput
{
    public enum InputMode
    {
        MoveCamera,
        PlaceTile,
        None,
    }
    public enum GameInputState
    {
        PointerDown,
        Drug,
        PointerUp
    }

    public class GameInputModel
    {
        public Vector3 CurrentPos;
        public Vector3 WorldCurrentPos;
        public Vector3 Delta;
        public Vector3 WorldDelta;
        public Vector3 DownPos;
        public Vector3 WorldDownPos;
        public InputMode InputMode;
        public GameInputState State;
    }

    
    /// <summary>
    /// 入力コントローラー
    /// アダプタを経由して入力を取得するので比較的下位レイヤーに位置
    /// </summary>
    public interface IGameInputController
    {
        GameInputModel Model { get; }
    }

    public interface IGameInputAdapter
    {
        void OnPointerDown(BaseEventData e);
        void OnPointerUp(BaseEventData e);
        void OnDrag(BaseEventData e);
    }
    /// <summary>
    /// 入力を受け取る側
    /// </summary>
    public interface IGameInputBehaviour
    {
        InputMode TargetInputMode { get; }
        void OnStartInput(GameInputModel model);
        void OnUpdateInput(GameInputModel model);
        void OnEndInput(GameInputModel model);
    }

    /// <summary>
    /// 入力接続
    /// 分離は今いらないので、コントローラーの実装も兼ねる
    /// </summary>
    public class GameInputAdapter : MonoBehaviour,IGameInputController,IGameInputAdapter
    {

        private ICameraUseCase _cameraUseCase;
        private List<IGameInputBehaviour> _inputBehaviours;
        private IPlaceTileUseCase _mapPlaceUseCase;
        private GameInputModel _model = new GameInputModel();
        private Vector3 _before;
        public GameInputModel Model => _model;

        [Inject]
        void Inject(
            ICameraUseCase cameraUseCase,
            IPlaceTileUseCase mapPlaceUseCase,
            List<IGameInputBehaviour> inputBehaviours
        
        )
        {
            
            _cameraUseCase = cameraUseCase;
            _inputBehaviours = inputBehaviours;
            _mapPlaceUseCase = mapPlaceUseCase;
        }
        /// <summary>
        /// 入力を受け取ったときの処理
        /// </summary>
        /// <param name="sendData"></param>
        /// <exception cref="ArgumentOutOfRangeException"></exception>
        void SendInput(GameInputModel sendData)
        {
            foreach (var gameInputBehaviour in _inputBehaviours)
            {
                if(gameInputBehaviour.TargetInputMode != sendData.InputMode)
                    continue;
                switch (sendData.State)
                {
                    case GameInputState.PointerDown:
                        gameInputBehaviour.OnStartInput(sendData);
                        break;
                    case GameInputState.Drug:
                        gameInputBehaviour.OnUpdateInput(sendData);
                        break;
                    case GameInputState.PointerUp:
                        gameInputBehaviour.OnEndInput(sendData);
                        break;
                }
            }
        }
        void CalcInput(BaseEventData e,GameInputState state)
        {
          
            var pe = (PointerEventData) e;
            var pos = _cameraUseCase.ScreenToWorldPoint(pe.position);
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
            _model.WorldDelta = pos - _cameraUseCase.ScreenToWorldPoint(_before);
            _model.CurrentPos = pe.position;
            _model.WorldCurrentPos = pos;
            _model.State = state;
            
//            _inputManager.InputOnGame(_model);
            SendInput(_model);
            _before = pe.position;
        }
        
        //Adapterの実装
        public void OnPointerDown(BaseEventData e)
        {
            //check tile state
            
            CalcInput(e,GameInputState.PointerDown);
        }

        public void OnPointerUp(BaseEventData e)
        {
            CalcInput(e,GameInputState.PointerUp);
        }

        public void OnDrag(BaseEventData e)
        {
            CalcInput(e,GameInputState.Drug);
        }

   
    }
}

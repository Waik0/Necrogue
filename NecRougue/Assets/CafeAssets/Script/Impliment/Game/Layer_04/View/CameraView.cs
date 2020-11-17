using System.Collections;
using System.Collections.Generic;
using CafeAssets.Script.Interface.Layer_01.Manager;
using CafeAssets.Script.Interface.View;
using CafeAssets.Script.Model;
using UnityEngine;
using NotImplementedException = System.NotImplementedException;

namespace CafeAssets.Script.System.GameCameraSystem
{
    public class CameraView : MonoBehaviour,ICameraView,IGameScreenInputReceiver
    {
        [SerializeField] private Camera _target;
        private Vector3 _anchor;
        public void MoveCamera(Vector3 pos)
        {
            pos.z = -10;
            _target.gameObject.transform.position = pos;
        }

        public Vector3 ScreenToWorldPoint(Vector2 screenPoint)
        {
            return _target.ScreenToWorldPoint(screenPoint);
        }

        public Vector2 WorldToScreenPoint(Vector3 world)
        {
            return _target.WorldToScreenPoint(world);
        }

        public void GameInput(GameInputModel model)
        {
            if(model.InputMode == InputMode.MoveCamera) MoveCamera(_target.gameObject.transform.position - model.WorldDelta);
        }
    }
}
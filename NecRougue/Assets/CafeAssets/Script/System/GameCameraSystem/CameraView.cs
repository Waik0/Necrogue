using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using NotImplementedException = System.NotImplementedException;

namespace CafeAssets.Script.System.GameCameraSystem
{
    public interface ICameraView
    {
        void MoveCamera(Vector3 pos);
        Vector3 ScreenToWorldPoint(Vector2 screenPoint);
    }
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

        public void GameInput(GameInputModel model)
        {
            if(!model.IsPlaceTileMode) MoveCamera(_target.gameObject.transform.position - model.WorldDelta);
        }
    }
}
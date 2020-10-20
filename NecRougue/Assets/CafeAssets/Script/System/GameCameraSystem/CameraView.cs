using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using NotImplementedException = System.NotImplementedException;

namespace CafeAssets.Script.System.GameCameraSystem
{
    public interface ICameraView
    {
        void MoveCamera(Vector2 move);
    }
    public class CameraView : MonoBehaviour,ICameraView,IGameScreenInputReceiver
    {
        [SerializeField] private Camera _target;
        public void MoveCamera(Vector2 move)
        {
            _target.gameObject.transform.Translate(move);
        }

        public void GameInput(GameInputModel model)
        {
            Debug.Log(model.Delta);
        }
    }
}
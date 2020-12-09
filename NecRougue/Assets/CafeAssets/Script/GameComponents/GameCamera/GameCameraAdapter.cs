using UnityEngine;

namespace CafeAssets.Script.System.GameCameraSystem
{
    /// <summary>
    /// ゲーム内メインカメラ制御
    /// </summary>
    public interface ICameraUseCase
    {
        void MoveCameraDelta(Vector3 worldDelta);
        Vector3 ScreenToWorldPoint(Vector2 screenPoint);
        Vector2 WorldToScreenPoint(Vector3 world);
    }
    public class GameCameraAdapter : MonoBehaviour,ICameraUseCase
    {
        [SerializeField] private Camera _target;
        private Vector3 _anchor;
        void MoveCameraInternal(Vector3 pos)
        {
            pos.z = -10;
            _target.gameObject.transform.position = pos;
        }
        /// <summary>
        /// ドラッグされたときに動かす
        /// </summary>
        /// <param name="pos"></param>
        public void MoveCameraDelta(Vector3 worldDelta)
        {
            MoveCameraInternal(_target.gameObject.transform.position - worldDelta);
        }

        public Vector3 ScreenToWorldPoint(Vector2 screenPoint)
        {
            return _target.ScreenToWorldPoint(screenPoint);
        }

        public Vector2 WorldToScreenPoint(Vector3 world)
        {
            return _target.WorldToScreenPoint(world);
        }
    }
}
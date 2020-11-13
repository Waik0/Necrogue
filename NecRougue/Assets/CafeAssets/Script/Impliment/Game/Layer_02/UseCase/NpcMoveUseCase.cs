using CafeAssets.Script.Interface.Layer_02.UseCase;
using CafeAssets.Script.Model;
using UnityEngine;
using NotImplementedException = System.NotImplementedException;

namespace CafeAssets.Script.Impliment.Game.Layer_02.UseCase
{
    public class NpcMoveUseCase : INpcMoveUseCase
    {
        private NpcMoveModel _model;
        private GameObject gameObject;
        public Vector2 CurrentPos()
        {
            if (gameObject != null)
            {
                return gameObject.transform.position;
            }
            return Vector2.zero;
        }

        public void Move(Vector2 pos)
        {
            if (gameObject != null)
            {
                //_rigidbody.AddForce(delta);
                gameObject.transform.position = pos;
                //_rigidbody.transform.Translate(delta);
            }
        }

        public void Reset(NpcMoveModel model)
        {
            _model = model;
            _model.Self.transform.position = _model.Position;
            gameObject = _model.Self;
        }
    }
}

using CafeAssets.Script.Interface.Layer_02.UseCase;
using CafeAssets.Script.Model;
using UnityEngine;
using NotImplementedException = System.NotImplementedException;

namespace CafeAssets.Script.Impliment.Game.Layer_02.UseCase
{
    public class NpcMoveUseCase : INpcMoveUseCase
    {
        private NpcMoveModel _model;
        private Rigidbody2D _rigidbody;
        public Vector2 CurrentPos()
        {
            if (_rigidbody != null)
            {
                return _rigidbody.position;
            }
            return Vector2.zero;
        }

        public void Move(Vector2 delta)
        {
            if (_rigidbody != null)
            {
                //_rigidbody.AddForce(delta);
               // _rigidbody.transform.position = delta
               _rigidbody.transform.Translate(delta);
            }
        }

        public void Reset(NpcMoveModel model)
        {
            _model = model;
            _model.Self.transform.position = _model.Position;
            _rigidbody = _model.Self.GetComponent<Rigidbody2D>();
        }
    }
}

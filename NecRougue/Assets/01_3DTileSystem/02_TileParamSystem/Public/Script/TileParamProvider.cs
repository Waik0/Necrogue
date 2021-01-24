using Basic3DTileSystem.Source.Core.Script;
using TileParamSystem.Source.Core.Script;
using UnityEngine;
using Zenject;

namespace TileParamSystem.Source.Core.Script
{
    public abstract class TileParamProvider : MonoBehaviour
    {
        [SerializeField] private int _radius;
        [SerializeField] private BoxCollider _collider;
        private ITilemap3DEditFacade _tilemap3DEditFacade;
        public int Radius => _radius;
        /*
        [Inject]
        public void Inject(
            ITilemap3DEditFacade facade
        )
        {
            Debug.Log("ASDFASDF");
            _tilemap3DEditFacade = facade;
        }
        */
        void Awake()
        {
            _collider.size = new Vector3(_radius * 2, 1, _radius * 2);
            _collider.gameObject.layer = LayerMask.NameToLayer("Ignore Raycast");
            _collider.isTrigger = true;
        }

        public bool CanGetParam(Vector3 receivePos)
        {
            return true;
            var p1 = _tilemap3DEditFacade.WorldToCell(receivePos);
            var p2 = _tilemap3DEditFacade.WorldToCell(transform.position);
            return ((p2 - p1).sqrMagnitude <= _radius * _radius);
        }
    }
}

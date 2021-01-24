using System;
using System.Collections.Generic;
using Basic3DTileSystem.Source.Core.Script;
using UnityEngine;
using UnityEngine.EventSystems;
using Zenject;

namespace Basic3DTileSystem.Source.Interface.Script
{
    public class TouchData
    {
        public GameObject HitObject;
        public Vector3Int HitObjectTilemapPos;
        public Vector3Int OriginTilemapPos;
        public Vector2 ScreenPos;
        public Vector3 RayPos;
    }
    public class Tilemap3DTouchToPosition : MonoBehaviour
    {
        public Action<TouchData> OnPointerDown = null;
        public Action<TouchData> OnDrug = null;
        public Action<TouchData> OnPointerUp = null;

        [SerializeField] private RectTransform _canvas;
        private ITilemap3DEditFacade _tilemap3DEditFacade;

        private RaycastHit[] _hits = new RaycastHit[5];
        
        [Inject]
        void Initialize(
            ITilemap3DEditFacade editFacade)
        {
            _tilemap3DEditFacade = editFacade;
        }

        public void PointerDown(BaseEventData b)
        {
            var p = (PointerEventData)b;
            OnPointerDown?.Invoke( CalcTilePosition(p));
           
        }

        public void Drug(BaseEventData b)
        {
            var p = (PointerEventData)b;
            OnDrug?.Invoke( CalcTilePosition(p));
        }

        public void PointerUp(BaseEventData b)
        {
            var p = (PointerEventData)b;
            OnPointerUp?.Invoke( CalcTilePosition(p));
        }
        TouchData CalcTilePosition(PointerEventData pointerEventData)
        {
            var ret = new TouchData();
            ret.ScreenPos = pointerEventData.position;
            Ray ray = RectTransformUtility.ScreenPointToRay(
                pointerEventData.pressEventCamera,
                pointerEventData.position);
            //Debug.DrawRay(ray.origin, ray.direction * 10, Color.red, 50);
            var hit = Physics.RaycastNonAlloc(ray,_hits);
            if (hit > 0)
            {
                Array.Sort( _hits, 0, hit, new RayDistanceCompare() );
                ret.HitObject = _hits[0].transform.gameObject;
                ret.HitObjectTilemapPos = _tilemap3DEditFacade.WorldToCell(_hits[0].transform.position);
            }
            //XY平面
            ///var wp = new Plane(Vector3.back, Vector3.zero).Raycast(ray, out var pos); 
            //XZ平面
            var wp = new Plane(Vector3.up, Vector3.zero).Raycast(ray, out var pos);
            ret.RayPos = ray.GetPoint(pos);
            ret.OriginTilemapPos = _tilemap3DEditFacade.WorldToCell(ret.RayPos);
            return ret;
        }
        public class RayDistanceCompare : IComparer<RaycastHit>
        {
            public int Compare( RaycastHit x, RaycastHit y )
            {
                if( x.distance < y.distance )
                {
                    return -1;
                }
                if( x.distance > y.distance )
                {
                    return 1;
                }
                return 0;
            }
        }
    }
}

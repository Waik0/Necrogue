using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Basic3DTileSystem.Source.Core.Script
{
    public enum PlaceType
    {
        Single,
        Rect
    }
    public interface ITileModel3D
    {
        GameObject Instance { get; }
        string Name { get; }
        PlaceType PlaceType { get; }
        void DeleteInstance();
        void SetPosition(Vector3 pos);
    }
    public class TileModel3D : MonoBehaviour,ITileModel3D
    {
        [SerializeField] private string _name;
        [SerializeField] private PlaceType _placeType;
        public string Name => _name;
        public PlaceType PlaceType => _placeType;
        public GameObject Instance => gameObject;
        public void DeleteInstance()
        {
            if (Instance != null)
            {
                Destroy(gameObject);
            }
        }

        public void SetPosition(Vector3 pos)
        {
            gameObject.transform.position = pos;
        }
    }
}
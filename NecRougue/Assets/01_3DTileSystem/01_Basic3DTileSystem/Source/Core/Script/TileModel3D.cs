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
        Vector3Int Size { get; }
        void DeleteInstance();
        void SetPosition(Vector3 pos);
    }
    public interface ITileModel3DReadOnly
    {
        string Name { get; }
        PlaceType PlaceType { get; }
        Vector3Int Size { get; }
    }
    public class TileModel3D : MonoBehaviour,ITileModel3D,ITileModel3DReadOnly
    {
        [SerializeField] private string _name;
        [SerializeField] private PlaceType _placeType;
        [Header("PlaceType:Singleのみ有効")]
        [SerializeField] private Vector3Int _singleSize;
        public string Name => _name;
        public PlaceType PlaceType => _placeType;
        public Vector3Int Size => _singleSize;
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
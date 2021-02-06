using System.Collections;
using System.Collections.Generic;
using System.Drawing;
using UnityEngine;

namespace Basic3DTileSystem.Source.Core.Script
{
    public interface ITilemap3DEditFacade
    {
        void SetTileList(TileModel3DList list);
        ITileModel3DReadOnly[] GetTilePrefabList();
        ITileModel3DReadOnly GetTilePrefab(int index);
        ITileModel3D GetTileNewInstance(int index);
        ITileModel3D GetTileNewInstance(string fileName);
        void ClearAllTiles();
        void SetParent(Transform parent);
        void SetTile(Vector3Int pos, int index);
        void SetTileBrush(Vector3Int pos, int index);
        void SetTiles(Vector3Int[] pos, int index);
        // void SetTile(Vector3Int pos, string fileName);
        // void SetTiles(Vector3Int[] pos, string fileName);
        // void SetTile(Vector3Int pos, ITileModel3D model);
        ITileModel3D GetTile(Vector3Int pos);
        void RemoveTile(Vector3Int pos);
        Vector3Int WorldToCell(Vector3 world);
        Vector3 CellToWorld(Vector3Int cell);
        BoundsInt CellBounds { get; }
    }
    /// <summary>
    /// 
    /// </summary>
    public class Tilemap3DEditFacade : MonoBehaviour,ITilemap3DEditFacade
    {
        [SerializeField] private Tilemap3D _tilemap;
        [SerializeField] private TileModel3DLoader _tileModel3DLoader;

        private Transform _parent;
        public void SetTileList(TileModel3DList list)
        {
            _tileModel3DLoader.SetTileList(list);
        }
        public ITileModel3DReadOnly[] GetTilePrefabList()
        {
            return _tileModel3DLoader.GetTilePrefabList();
        }


        public ITileModel3DReadOnly GetTilePrefab(int index)
        {
            return _tileModel3DLoader.GetTilePrefab(index);
        }

        public ITileModel3D GetTileNewInstance(int index)
        {
            return _tileModel3DLoader.GetTileInstance(index);
        }

        public ITileModel3D GetTileNewInstance(string fileName)
        {
            return _tileModel3DLoader.GetTileInstance(fileName);
        }

        public void ClearAllTiles()
        {
            _tilemap.ClearAllTiles();
        }

        public void SetParent(Transform parent)
        {
            _parent = parent;
        }

        public void SetTile(Vector3Int pos, int index)
        {
            var ins = _tileModel3DLoader.GetTileInstance(index);
            if (_parent != null)
                ins.Instance.transform.parent = _parent;
            _tilemap.SetTile(pos,ins);
        }

        //複数にまたがるタイル対応
        public void SetTileBrush(Vector3Int pos, int index)
        {
            var ins = _tileModel3DLoader.GetTileInstance(index);
            if (_parent != null)
                ins.Instance.transform.parent = _parent;
            Debug.Log(ins.Instance.name);
            bool canPlace = _tilemap.SetTile(pos,ins);
            if (!canPlace)
            {
                return;
            }

            var actpos = _tilemap.CellToWorld(pos);
            var scs = _tilemap.SingleCellSize;
            actpos.x += (Mathf.Max(1,ins.Size.x) - 1) * scs.x / 2;
            actpos.y += (Mathf.Max(1,ins.Size.y) - 1) * scs.y / 2;
            actpos.z += (Mathf.Max(1,ins.Size.z) - 1) * scs.z / 2;
            _tilemap.MoveTile(pos, actpos);
            for (var x = 0; x < ins.Size.x; x++)
            {
                for (var y = 0; y < ins.Size.y; y++)
                {
                    for (var z = 0; z < ins.Size.z; z++)
                    {
                        var key = pos + new Vector3Int(x, y, z);
                        if (key == pos)
                        {
                            continue;
                        }
                      
                        _tilemap.RemoveTile(key);
                        _tilemap.SetTileInstance(key,ins);
                    }
                }
            }
        }

        public void SetTiles(Vector3Int[] pos, int index)
        {
            foreach (var vector3Int in pos)
            {
                var ins = _tileModel3DLoader.GetTileInstance(index);
                _tilemap.SetTile(vector3Int,ins);
            }
        }

        // public void SetTile(Vector3Int pos, string fileName)
        // {
        //     var ins = _tileModel3DLoader.GetTileInstance(fileName);
        //     _tilemap.SetTile(pos,ins);
        // }
        //
        // public void SetTiles(Vector3Int[] pos, string fileName)
        // {
        //     foreach (var vector3Int in pos)
        //     {
        //         var ins = _tileModel3DLoader.GetTileInstance(fileName);
        //         _tilemap.SetTile(vector3Int,ins);
        //     }
        // }
        //
        // public void SetTile(Vector3Int pos, ITileModel3D model)
        // {
        //     _tilemap.SetTile(pos, model);
        // }
        public ITileModel3D GetTile(Vector3Int pos)
        {
            return _tilemap.GetTile(pos);
        }

        public void RemoveTile(Vector3Int pos)
        {
            _tilemap.RemoveTile(pos);
        }

        public Vector3Int WorldToCell(Vector3 world)
        {
            return _tilemap.WorldToCell(world);
        }

        public Vector3 CellToWorld(Vector3Int cell)
        {
            return _tilemap.CellToWorld(cell);
        }

        public BoundsInt CellBounds => _tilemap.CellBounds;
    }
}
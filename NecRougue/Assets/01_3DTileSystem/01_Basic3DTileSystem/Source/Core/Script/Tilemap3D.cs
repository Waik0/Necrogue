using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Basic3DTileSystem.Source.Core.Script
{
    internal interface ITilemap3D
    {
        void ClearAllTiles();
        void SetTileInstance(Vector3Int pos, ITileModel3D model);
        bool SetTile(Vector3Int pos, ITileModel3D model);
        bool MoveTile(Vector3Int pos,Vector3 world);
        ITileModel3D GetTile(Vector3Int pos);
        void RemoveTile(Vector3Int pos);
        
        Vector3Int WorldToCell(Vector3 world);
        
        Vector3 CellToWorld(Vector3Int cell);
        Vector3 SingleCellSize { get; }
        BoundsInt CellBounds { get; }
    }
    /// <summary>
    /// 最大サイズあり
    /// </summary>
    public class Tilemap3D : MonoBehaviour, ITilemap3D
    {
        private const int XMax = 150;
        private const int YMax = 3;
        private const int ZMax = 150;
        private readonly Vector3 _singleCellSize = new Vector3Int(1, 1, 1);
        //3次元ジャグ配列でオブジェクトを管理
        private ITileModel3D[,,] _entity = new ITileModel3D[XMax, YMax, ZMax];

        public void ClearAllTiles()
        {
            foreach (var tileModel3D in _entity)
            {
                tileModel3D?.DeleteInstance();
            }
        }

        bool IsInRange(int index, int dim)
        {
            return _entity.GetLength(index) > dim && dim >= 0;
        }
        public void SetTileInstance(Vector3Int pos, ITileModel3D model)
        {
            if (IsInRange(0, pos.x) &&
                IsInRange(1, pos.y) &&
                IsInRange(2, pos.z))
            {
                _entity[pos.x, pos.y, pos.z] = model;
            }
            else
            {
                Debug.LogError("レンジ外");
            }
        }



        public bool SetTile(Vector3Int pos, ITileModel3D model)
        {
            if (IsInRange(0, pos.x) &&
                IsInRange(1, pos.y) &&
                IsInRange(2, pos.z))
            {
                _entity[pos.x,pos.y,pos.z]?.DeleteInstance();
                _entity[pos.x, pos.y, pos.z] = model;
                _entity[pos.x, pos.y, pos.z]?.SetPosition(CellToWorld(pos));
                return true;
            }
            else
            {
                Debug.LogError("レンジ外");
                model.DeleteInstance();
                return false;
            }
        }

        public bool MoveTile(Vector3Int pos, Vector3 world)
        {
            if (IsInRange(0, pos.x) &&
                IsInRange(1, pos.y) &&
                IsInRange(2, pos.z))
            {
                _entity[pos.x, pos.y, pos.z]?.SetPosition(world);
                return true;
            }
            else
            {
                Debug.LogError("レンジ外");
                return false;
            }
        }


        public ITileModel3D GetTile(Vector3Int pos)
        {
            if (IsInRange(0, pos.x) &&
                IsInRange(1, pos.y) &&
                IsInRange(2, pos.z))
            {
                return _entity[pos.x, pos.y, pos.z];
            }

            return null;
        }
    
        public void RemoveTile(Vector3Int pos)
        { 
            if (IsInRange(0, pos.x) &&
                IsInRange(1, pos.y) &&
                IsInRange(2, pos.z))
            {
                if(_entity[pos.x, pos.y, pos.z] != null) _entity[pos.x, pos.y, pos.z]?.DeleteInstance();
                _entity[pos.x, pos.y, pos.z] = null;
            }

        }

        public Vector3Int WorldToCell(Vector3 world)
        {
            return new Vector3Int(
                (int) (world.x / _singleCellSize.x),
                (int) (world.y / _singleCellSize.y),
                (int) (world.z / _singleCellSize.z));
        }

        public Vector3 CellToWorld(Vector3Int cell)
        {
            return new Vector3(
                cell.x * _singleCellSize.x + _singleCellSize.x / 2,
                cell.y * _singleCellSize.y + _singleCellSize.y / 2,
                cell.z * _singleCellSize.z + _singleCellSize.z / 2
            );
        }

        public Vector3 SingleCellSize => _singleCellSize;

        public BoundsInt CellBounds => new BoundsInt(0, 0, 0, XMax, YMax, ZMax);
    
        //ギズモ設定
        private Color gizmoLineColor = new Color (0.4f, 0.4f, 0.3f, 1f);  
        public int gizmoMajorLines = 5; 
        void OnDrawGizmosSelected ()
        {
            // orient to the gameobject, so you can rotate the grid independently if desired
            Gizmos.matrix = transform.localToWorldMatrix;
 
            // set colours
            Color dimColor = new Color(gizmoLineColor.r, gizmoLineColor.g, gizmoLineColor.b, 0.25f* gizmoLineColor.a); 
            Color brightColor = Color.Lerp (Color.white, gizmoLineColor, 0.75f); 
 
            // draw the horizontal lines
            for (int x = 0; x < XMax + 1; x++)
            {
                // find major lines
                Gizmos.color = (x % gizmoMajorLines == 0 ? gizmoLineColor : dimColor); 
                if (x == 0)
                    Gizmos.color = brightColor;
 
                Vector3 pos1 = new Vector3(x, 0, 0) * _singleCellSize.x;
                Vector3 pos2 = new Vector3(x, YMax * _singleCellSize.y, 0) * _singleCellSize.x;  
            
 
                Gizmos.DrawLine ((pos1), ( pos2)); 
            }
 
            // draw the vertical lines
            for (int y = 0; y < YMax+1; y++)
            {
                // find major lines
                Gizmos.color = (y % gizmoMajorLines == 0 ? gizmoLineColor : dimColor); 
                if (y == 0)
                    Gizmos.color = brightColor;
 
                Vector3 pos1 = new Vector3(0, y, 0) * _singleCellSize.y;  
                Vector3 pos2 = new Vector3(XMax * _singleCellSize.x, y, 0) * _singleCellSize.y;  
            
 
                Gizmos.DrawLine ((pos1), (pos2)); 
            }
            for (int z = 0; z < ZMax+1; z++)
            {
                // find major lines
                Gizmos.color = (z % gizmoMajorLines == 0 ? gizmoLineColor : dimColor); 
                if (z == 0)
                    Gizmos.color = brightColor;
 
                Vector3 pos1 = new Vector3(0, 0, z) * _singleCellSize.z;  
                Vector3 pos2 = new Vector3(XMax * _singleCellSize.x, 0, z) * _singleCellSize.z;  
            
 
                Gizmos.DrawLine ((pos1), (pos2)); 
            }
        }
    }
}
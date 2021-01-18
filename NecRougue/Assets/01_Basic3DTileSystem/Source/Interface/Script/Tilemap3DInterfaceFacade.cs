using System;
using System.Collections;
using System.Collections.Generic;
using Basic3DTileSystem.Source.Core.Script;
using UnityEngine;
using Zenject;

namespace Basic3DTileSystem.Source.Interface.Script
{
    public interface ITilemap3DInterfaceFacade
    {
        Action<TouchData> Debug { set; }
        void SelectTile(int index);

        bool TileSelected();
        int GetSelectedIndex();
    }
    /// <summary>
    /// 入力イベントを処理する
    /// </summary>
    public interface ITilemap3DPointerEvent
    {
        bool CanProcess();
        void PointerDown(ITilemap3DInterfaceFacade interfaceFacade,TouchData pos);
        void Drug(ITilemap3DInterfaceFacade interfaceFacade,TouchData pos);
        void PointerUp(ITilemap3DInterfaceFacade interfaceFacade,TouchData pos);
        void SelectTile(int index);
    }
    /// <summary>
    /// 入力タイプの出し分け設定
    /// </summary>
    public interface ISelectInputType
    {
        void Select(ITilemap3DInterfaceFacade interfaceFacade,TouchData pos);
    }
    public class Tilemap3DInterfaceFacade : ITilemap3DInterfaceFacade,IInitializable
    {
    
        private readonly Tilemap3DSelectTile _tilemap3DSelectTile = default;
        private readonly Tilemap3DTouchToPosition _tilemap3DTouchToPosition = default;
        private readonly List<ITilemap3DPointerEvent> _pointerEvents;
        private ISelectInputType _selectInputType;

        public Action<TouchData> Debug { get; set; } = null;
        public Tilemap3DInterfaceFacade(
            [InjectOptional]
            List<ITilemap3DPointerEvent> pe,
            Tilemap3DTouchToPosition touchToPosition,
            Tilemap3DSelectTile selectTile,
            ISelectInputType selectInputType
        )
        {
            _pointerEvents = pe;
            _tilemap3DSelectTile = selectTile;
            _tilemap3DTouchToPosition = touchToPosition;
            _selectInputType = selectInputType;
        }
        public void Initialize()
        {
            _tilemap3DTouchToPosition.OnPointerDown = PointerDown;
            _tilemap3DTouchToPosition.OnDrug = Drug;
            _tilemap3DTouchToPosition.OnPointerUp = PointerUp;
            _tilemap3DSelectTile.OnSelectTile = OnSelectTile;
        }

        public void SelectTile(int index)
        {
            _tilemap3DSelectTile.SelectTile(index);
        }

        public bool TileSelected()
        {
            return _tilemap3DSelectTile.CanPlace;
        }

        public int GetSelectedIndex()
        {
            return _tilemap3DSelectTile.GetIndex();
        }

        void OnSelectTile(int index)
        {
            foreach (var tilemap3DPointerEvent in _pointerEvents)
            {
                tilemap3DPointerEvent.SelectTile(index);
            }
        }
        void PointerDown(TouchData pos)
        {
            Debug?.Invoke(pos);
            _selectInputType?.Select(this,pos);
            foreach (var tilemap3DPointerEvent in _pointerEvents)
            {
                if (tilemap3DPointerEvent.CanProcess())
                {
                    tilemap3DPointerEvent.PointerDown(this,pos);
                }
            }
        }

        void Drug(TouchData pos)
        {
            Debug?.Invoke(pos);
            foreach (var tilemap3DPointerEvent in _pointerEvents)
            {
                if (tilemap3DPointerEvent.CanProcess())
                {
                    tilemap3DPointerEvent.Drug(this,pos);
                }
            }
        }

        void PointerUp(TouchData pos)
        {
            Debug?.Invoke(pos);
            foreach (var tilemap3DPointerEvent in _pointerEvents)
            {
                if (tilemap3DPointerEvent.CanProcess())
                {
                    tilemap3DPointerEvent.PointerUp(this,pos);
                }
            }
        }

    }

// void SelectInputType(TouchData pos)
// {
//     if (_tilemap3DSelectTile.CanPlace)
//     {//タイル設置開始
//         var tile = _tileModel3DLoader.GetTilePrefab(_tilemap3DSelectTile.GetIndex());
//         if (tile != null)
//         {
//             switch (tile.PlaceType)
//             {
//                 case PlaceType.Single:
//                     _currentInputType = InputType.PlaceTileSingle;
//                     return;
//                 case PlaceType.Rect:
//                     _currentInputType = InputType.PlaceTileRect;
//                     return;
//             }
//         }
//     } 
//     //カメラ移動
//     _currentInputType = InputType.MoveCamera;
//     
// }
}
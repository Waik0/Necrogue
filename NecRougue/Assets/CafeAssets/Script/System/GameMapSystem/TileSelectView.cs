using System;
using System.Collections;
using System.Collections.Generic;
using CafeAssets.Script.System.GameCoreSystem;
using UnityEngine;
using UnityEngine.UI;
using Zenject;

namespace CafeAssets.Script.System.GameMapSystem
{
    public interface ITileSelectView
    {
        TileType SelectedType { get; set; }
    }
    public class TileSelectView : MonoBehaviour,ITileSelectView,IGameScreenInputReceiver
    {
        [SerializeField] private Canvas _canvas;
        [SerializeField] private RectTransform _tileTypeList;
        [SerializeField] private RectTransform _tileList;
        [SerializeField] private Button _cancelButton;
        //prefab
        [SerializeField] private Button _buttonPrefab;
        private TileModelProvider _tileModelProvider;
        private ITileSelectManager _tileSelectManager;
        public TileType SelectedType { get; set; }
        [Inject]
        void Inject(
            TileModelProvider tileModelProvider,
            ITileSelectManager tileSelectManager
        )
        {
            _tileSelectManager = tileSelectManager;
            _tileModelProvider = tileModelProvider;
        }

        void CreateTypeSelectButton()
        {
            foreach (TileType value in Enum.GetValues(typeof(TileType)))
            {
                var ins = Instantiate(_buttonPrefab, _tileTypeList);
                ins.gameObject.SetActive(true);
                ins.onClick.AddListener(()=>OnSelectType(value));
            }
            
        }

        void OnSelectType(TileType type)
        {
            SelectedType = type;
            _tileSelectManager.OnSelectTile(new TileSelectModel(){
                Model = _tileModelProvider.GetFloorTileModel("FloorTile_0000")
            });//debug
        }
        private void Awake()
        {
            CreateTypeSelectButton();
        }

        public void GameInput(GameInputModel model)
        {
            if (model.State == GameInputState.PointerDown)
            {
                SelectedType = TileType.None;
                _tileSelectManager.OnSelectTile(new TileSelectModel(){Model = null});
            }
        }


        
    }
}
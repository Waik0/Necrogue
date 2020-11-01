using System;
using System.Collections;
using System.Collections.Generic;
using CafeAssets.Script.Interface.Layer_01.Manager;
using CafeAssets.Script.Model;
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
    /// <summary>
    /// タイル選択UI
    /// </summary>
    public class TileSelectView : MonoBehaviour,ITileSelectView,IGameScreenInputReceiver
    {
        [SerializeField] private Canvas _canvas;
        [SerializeField] private RectTransform _tileTypeList;
        [SerializeField] private RectTransform _tileList;
        [SerializeField] private Button _cancelButton;
        //prefab
        [SerializeField] private TileSelectButton _typeButtonPrefab;
        [SerializeField] private TileSelectButton _tileButtonPrefab;
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
        private void Awake()
        {
            CreateTypeSelectButton();
            _cancelButton.onClick.AddListener(Cancel);
        }

  
        /// <summary>
        /// タイルタイプを選択
        /// </summary>
        /// <param name="type"></param>
        void OnSelectType(TileType type)
        {
            SelectedType = type;
            CreateTileSelectButton();
            //"FloorTile_0000"//debug
        }
        /// <summary>
        /// タイルを選択
        /// </summary>
        /// <param name="name"></param>
        void OnSelectTile(string name)
        {
            _tileSelectManager.OnSelectTile(new TileSelectModel()
            {
                Model = _tileModelProvider.GetFloorTileModel(name)
            });
        }
        /// <summary>
        /// キャンセルを押された
        /// </summary>
        void Cancel()
        {
            SelectedType = TileType.None;
            foreach (Transform o in _tileList.transform)
            {
                Destroy(o.gameObject);
            }

            _tileSelectManager.OnSelectTile(new TileSelectModel()
            {
                Model = null
            });
        }
        void CreateTypeSelectButton()
        {
            foreach (Transform o in _tileTypeList.transform)
            {
                Destroy(o.gameObject);
            }

            foreach (TileType value in Enum.GetValues(typeof(TileType)))
            {
                var ins = Instantiate(_typeButtonPrefab, _tileTypeList);
                ins.Setup(value.ToString(),null,()=>OnSelectType(value));
            }
        }
        void CreateTileSelectButton()
        {
            foreach (Transform o in _tileList.transform)
            {
                Destroy(o.gameObject);
            }

            foreach (var tileModel in _tileModelProvider.GetTileModelList(SelectedType))
            {
                var ins = Instantiate(_tileButtonPrefab, _tileList);
                ins.Setup(tileModel.Name,null,()=>OnSelectTile(tileModel.name));
            }
        }
        /// <summary>
        /// フィールド入力があったとき
        /// </summary>
        /// <param name="model"></param>
        public void GameInput(GameInputModel model)
        {
            //タイプだけ選択解除
            if (model.State == GameInputState.PointerDown)
            {
                foreach (Transform o in _tileList.transform)
                {
                    Destroy(o.gameObject);
                }

                SelectedType = TileType.None;
            }
        }


        
    }
}
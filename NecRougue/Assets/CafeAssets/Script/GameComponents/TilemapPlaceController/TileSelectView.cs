using System;
using CafeAssets.Script.GameComponents.Tilemap;
using CafeAssets.Script.System.GameMapSystem;
using UnityEngine;
using UnityEngine.UI;
using Zenject;

namespace CafeAssets.Script.GameComponents.TilemapPlaceController
{
    
    public interface ITileSelectUseCase
    {
        TileType?  SelectedType { get; set; }
        void ClearSelect();
    }
    /// <summary>
    /// タイル選択UI
    /// </summary>
    public class TileSelectView : MonoBehaviour,ITileSelectUseCase
    {
        [SerializeField] private Canvas _canvas;
        [SerializeField] private RectTransform _tileTypeList;
        [SerializeField] private RectTransform _tileList;
        [SerializeField] private Button _cancelButton;
        //prefab
        [SerializeField] private TileSelectButton _typeButtonPrefab;
        [SerializeField] private TileSelectButton _tileButtonPrefab;
        private TileModelProvider _tileModelProvider;
        private IPlaceTileUseCase _placeTileUseCase;
        public TileType? SelectedType { get; set; }
        [Inject]
        void Inject(
            TileModelProvider tileModelProvider,
            IPlaceTileUseCase placeTileUseCase
        )
        {
            _placeTileUseCase = placeTileUseCase;
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
        void OnSelectTile(string name,TileType type)
        {
            DebugLog.LogClassName(this,$"タイル選択 {type} {name}");
            var model = _tileModelProvider.GetTileModel(name, type);
            _placeTileUseCase.OnSelectTile(model);
            if (model != null)
            {
                _placeTileUseCase.SetPlaceTileMode(model.GetDefaultPlaceMode());
            }
        }
        /// <summary>
        /// キャンセルを押された
        /// </summary>
        void Cancel()
        {
            SelectedType = null;
            foreach (Transform o in _tileList.transform)
            {
                Destroy(o.gameObject);
            }
            _placeTileUseCase.OnSelectTile(null);
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
        /// <summary>
        /// タイルタイプが選ばれたときにタイル一覧を表示する処理
        /// </summary>
        void CreateTileSelectButton()
        {
            foreach (Transform o in _tileList.transform)
            {
                Destroy(o.gameObject);
            }

            if (SelectedType == null) 
                return;
            foreach (var tileModel in _tileModelProvider.GetTileModelList(SelectedType.Value))
            {
                var ins = Instantiate(_tileButtonPrefab, _tileList);
                if (SelectedType == null) 
                    continue;
                ins.Setup(tileModel.Name,null,()=>OnSelectTile(tileModel.name,SelectedType.Value));
            }
        }
        /// <summary>
        /// フィールド入力があったとき
        /// </summary>
        /// <param name="model"></param>
        public void ClearSelect()
        {
            //タイプだけ選択解除
            foreach (Transform o in _tileList.transform)
            {
                Destroy(o.gameObject);
            }

            SelectedType = null;
        }


        
    }
}
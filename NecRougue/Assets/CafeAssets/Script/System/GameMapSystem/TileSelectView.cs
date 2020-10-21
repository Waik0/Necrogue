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
    
    }
    public class TileSelectView : MonoBehaviour,ITileSelectView,IGameScreenInputReceiver
    {
        [SerializeField] private Canvas _canvas;
        [SerializeField] private RectTransform _tileTypeList;
        [SerializeField] private RectTransform _tileList;
        [SerializeField] private Button _buttonPrefab;
        private TileModelProvider _tileModelProvider;
        private TileType _selected;
        [Inject]
        void Inject(
            TileModelProvider tileModelProvider
            )
        {
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
            _selected = type;
        }
        private void Awake()
        {
            CreateTypeSelectButton();
        }

        public void GameInput(GameInputModel model)
        {
            if (model.State == GameInputState.PointerDown)
            {
                _selected = TileType.None;
            }
        }
    }
}
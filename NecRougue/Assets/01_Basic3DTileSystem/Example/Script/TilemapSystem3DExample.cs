using System.Collections;
using System.Collections.Generic;
using Basic3DTileSystem.Source.Core.Script;
using Basic3DTileSystem.Source.Interface.Script;
using UnityEngine;
using Zenject;

namespace Basic3DTileSystem.Example.Script
{
    public class TilemapSystem3DExample : MonoBehaviour
    {
        public enum InputType
        {
            MoveCamera,
            PlaceTileSingle,
            PlaceTileRect,
        }

        private ITilemap3DEditFacade _editFacade;
        private ITilemap3DInterfaceFacade _interfaceFacade;
        private ISelectInputCurrentType _currentType;
        [SerializeField] private TileModel3DList _model3DList;
       
        private Coroutine _coroutine = null;
        [Inject]
        void Inject(
            ITilemap3DEditFacade editFacade,
            ISelectInputCurrentType currentType,
            ITilemap3DInterfaceFacade interfaceFacade
        )
        {
            _editFacade = editFacade;
            _currentType = currentType;
            _interfaceFacade = interfaceFacade;
        }
        void Start()
        {
            _editFacade.SetTileList(_model3DList);
            var pos = new List<Vector3Int>();
            _coroutine = StartCoroutine(Test());
            Debug.Log("CellBounds");
            Debug.Log(_editFacade.CellBounds);
        }
        IEnumerator Test()
        {
            for (int i = 0; i < 3; i++)
            {
                for (int j = 0; j < 3; j++)
                {
                    _editFacade.SetTile(new Vector3Int(i,0,j),0);
                    yield return null;
                }
            }
        }

        void OnGUI()
        {
            GUILayout.BeginVertical("box");
            GUILayout.Space(150);
            GUILayout.Label($"Current : {_currentType.CurrentType}");
            if (GUILayout.Button("PlaceSingle"))
            {
                _interfaceFacade.SelectTile(0);
            }
            if (GUILayout.Button("PlaceRect"))
            {
                _interfaceFacade.SelectTile(1);
            }
            GUILayout.EndVertical();
        }

    }
}

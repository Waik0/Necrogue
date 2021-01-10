using CafeAssets.Script.GameComponents.Npc;
using CafeAssets.Script.GameComponents.Tilemap;
using CafeAssets.Script.GameComponents.TilemapParams;
using CafeAssets.Script.System.GameCameraSystem;
using UnityEngine;
using UnityEngine.UI;
using Zenject;


public class NpcDebugUnitView : MonoBehaviour,INpcDebugUnitView
{
    [SerializeField] private RectTransform _moveRoot;
    [SerializeField] private Canvas _canvas;
    [SerializeField] private Text _text;
    private ICameraUseCase _cameraUseCase;
    private ITilemapParamsFacade<TileEffectParams> _tilemapParamsFacade;
    private ITilemapUseCase _tilemapUseCase;
    [Inject]
    void Inject(ICameraUseCase cameraUseCase,
        ITilemapUseCase tilemapUseCase,
        ITilemapParamsFacade<TileEffectParams> tilemapParamsFacade,
        INpcFinder registry)
    {
        _cameraUseCase = cameraUseCase;
        _tilemapParamsFacade = tilemapParamsFacade;
        _tilemapUseCase = tilemapUseCase;
    }

    private void Awake()
    {
        _canvas.enabled = false;
    }

    public void OnSpawned(NpcDebugModel model)
    {
        _canvas.enabled = true;
    }

    public void OnDespawned()
    {
        _canvas.enabled = false;
    }

    public void UpdateView(NpcDebugModel model)
    {
        var pos = (Vector2) model.ChaseObject.transform.position;
        var tilePos = _tilemapUseCase.WorldToCell(new Vector3(pos.x, pos.y, 0));
        _moveRoot.position = _cameraUseCase.WorldToScreenPoint(pos);
        var facade = model.ChaseObject.GetComponent<INpcFacade>();
        _text.text = "\nPOS : ";
        _text.text += tilePos;
        _text.text += "\nSTATUS : " + facade?.CurrentAction().ToString();
        
        _text.text += "\nPOS PARAMS : \n";
        var dic = _tilemapParamsFacade.GetTileParams((Vector2Int)tilePos);
        _text.text += dic.Count;
        _text.text += _tilemapParamsFacade.Entity.Count;
        foreach (var keyValuePair in dic)
        {
            var str = keyValuePair.Key + " : " + keyValuePair.Value;
            _text.text += str;
        }
        _text.text += "\nOWN PARAMS : \n";
        foreach (var npcParamCollection in facade.OwnParamRegistry().Entity)
        {
            _text.text += npcParamCollection.Log() + "\n";
        }
    }
}

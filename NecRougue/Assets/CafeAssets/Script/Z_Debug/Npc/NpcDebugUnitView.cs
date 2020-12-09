using CafeAssets.Script.GameComponents.Npc;
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

    [Inject]
    void Inject(ICameraUseCase cameraUseCase)
    {
        _cameraUseCase = cameraUseCase;
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
        _moveRoot.position = _cameraUseCase.WorldToScreenPoint(pos);
        var facade = model.ChaseObject.GetComponent<INpcFacade>();
        _text.text = "STATUS : \n" + facade?.CurrentAction().ToString();
        var keys = facade?.GetParamKeys();
        _text.text += "\nPARAMS : \n";
        foreach (var key in keys)
        {
            _text.text += key + " : " + facade.GetParam(key);
        }
    }
}

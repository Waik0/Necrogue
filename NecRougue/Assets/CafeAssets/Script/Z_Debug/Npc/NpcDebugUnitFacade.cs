using UnityEngine;
using Zenject;
public interface INpcDebugUnitView
{
    void OnSpawned(NpcDebugModel model);
    void OnDespawned();
    void UpdateView(NpcDebugModel model);
}

public interface INpcDebugUnitRegistry : IRegistry<INpcDebugUnitCollection> { }
public class NpcDebugUnitFacade : MonoBehaviour,INpcDebugFacade, INpcDebugUnitCollection, IPoolable<NpcDebugModel,IMemoryPool>
{
    private INpcDebugUnitView _unitView;
    private INpcDebugUnitRegistry _registry;
    private NpcDebugModel _model;

    [Inject]
    void Inject(
        INpcDebugUnitView view,
        INpcDebugUnitRegistry registry)
    {
        _unitView = view;
        _registry = registry;
    }
    public void OnDespawned()
    {
        _unitView.OnDespawned();
        _registry.Remove(this);
    }

    public void OnSpawned(NpcDebugModel p1, IMemoryPool p2)
    {
        _unitView.OnSpawned(p1);
        _model = p1;
        _registry.Add(this);
    }

    private void Update()
    {
        _unitView.UpdateView(_model);
    }
}

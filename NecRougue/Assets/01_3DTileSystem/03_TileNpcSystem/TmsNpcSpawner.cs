using System;
using UniRx;
using Zenject;

class TmsNpcBasePool : MonoPoolableMemoryPool<TmsNpcBase.SpawnData, IMemoryPool, TmsNpcBase> { }
public interface ITmsNpcSpawner
{
    IObservable<TmsNpcBase> OnSpawn { get; }
    void Spawn(TmsNpcBase.SpawnData model);
}

public class TmsNpcSpawner : ITmsNpcSpawner, IDisposable
{
    private TmsNpcBase.TmsNpcBaseFactory _factory;

    private readonly Subject<TmsNpcBase> _onSpawn = new Subject<TmsNpcBase>();
    public IObservable<TmsNpcBase> OnSpawn => _onSpawn;
    public TmsNpcSpawner(
        TmsNpcBase.TmsNpcBaseFactory factory
    )
    {
        _factory = factory;
    }
    public void Spawn(TmsNpcBase.SpawnData model)
    {
        var spawned = _factory.Create(model);
        _onSpawn.OnNext(spawned);
    }
    public void Dispose()
    {
        _onSpawn?.Dispose();
    }
}


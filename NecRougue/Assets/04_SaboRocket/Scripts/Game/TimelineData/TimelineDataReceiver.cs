using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UniRx;
using UnityEngine;
public interface ITimelineDataReceiver {
    void StartSubscribe(ITortecUseCaseBaseWithWebSocket peer);
    void EndSubscribe();
    Action<TimelineData> OnGetTimelineData { set; }
}
public class TimelineDataReceiver : ITimelineDataReceiver,IDisposable
{
    private CompositeDisposable _compositeDisposable;
    private List<TimelineData> _datas = new List<TimelineData>();
    public void StartSubscribe(ITortecUseCaseBaseWithWebSocket peer)
    {
        _compositeDisposable?.Dispose();
        _compositeDisposable = new CompositeDisposable();
        peer.SubscribeMessage<TimelineData>(_compositeDisposable,OnGetTimelineDataInternal);
    }

    void OnGetTimelineDataInternal(string id ,TimelineData data)
    {
        OnGetTimelineData(data);
    }
    public void EndSubscribe()
    {
        _compositeDisposable?.Dispose();
    }

    public Action<TimelineData> OnGetTimelineData { get; set; }

    public void Dispose()
    {
        _compositeDisposable?.Dispose();
    }
}

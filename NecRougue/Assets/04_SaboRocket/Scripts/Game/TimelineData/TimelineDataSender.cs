using System.Collections;
using System.Collections.Generic;
using UniRx;
using UnityEngine;
using Zenject;

public class TimelineDataSender 
{
    public void Send(TimelineData data, ITortecUseCaseBaseWithWebSocket peer)
    {
        peer.BroadcastAll(data);
    }

    public IEnumerator Send(List<TimelineData> data, ITortecUseCaseBaseWithWebSocket peer)
    {
        Debug.Log("SendTimeline");
        var count = 0;
        foreach (var timelineData in data)
        {
            peer.BroadcastAll(timelineData);
            count++;
            if (count > 10)
            {
                count = 0;
                yield return null;
            }
        }
    }
}

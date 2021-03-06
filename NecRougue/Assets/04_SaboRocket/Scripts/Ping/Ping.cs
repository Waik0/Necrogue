using System;
using System.Collections;
using System.Collections.Generic;
using UniRx;
using UnityEngine;

/// <summary>
/// コネクションがアイドル状態にならないように5秒おきくらいにPing送る
/// </summary>
public class Ping : MonoBehaviour
{
    //private CompositeDisposable _compositeDisposable;
    private Coroutine _coroutine;
    public void StartPing(ITortecUseCaseBaseWithWebSocket peer)
    {
        _coroutine = StartCoroutine(PingCoroutine(peer));
    }

    IEnumerator PingCoroutine(ITortecUseCaseBaseWithWebSocket peer)
    {
        while (true)
        {
            peer.BroadcastAll<PingData>(new PingData());
            yield return new WaitForSecondsRealtime(3);
        }
    }
}
[Serializable]
public class PingData
{
    
}
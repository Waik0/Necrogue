using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CursorDataSender : MonoBehaviour
{

    private Coroutine _coroutine;
    private List<ulong> _ids = new List<ulong>();
    //List受け取りなのでたぶん外部で値変わっても対応できる
    public void StartSendCoroutine(List<ulong> ids)
    {
        //_peer = peer;
        if (_coroutine!= null)
        {
            StopCoroutine(_coroutine);
        }
        _ids = ids;
        _coroutine = StartCoroutine(SendCursorData());
    }

    public void EndSendCoroutine()
    {
        Debug.Log("カーソル送信を停止");
        if (_coroutine!= null)
        {
            StopCoroutine(_coroutine);
        }
    }

    void BroadcastCursorData(CursorData cursorData)
    {
        SteamNetworkManager.BroadcastTo(_ids,cursorData);
    }
    IEnumerator SendCursorData()
    {
        int interval = 0;
        Vector3 cache = new Vector3(-1000, -1000);
        while (true)
        {
            if (Input.GetMouseButton(0))
            {
                var mouse = Camera.main.ScreenToWorldPoint(Input.mousePosition);
                if ((mouse - cache).sqrMagnitude > 2)
                {
                    if (interval <= 0)
                    {
                        interval = 2;
                        BroadcastCursorData(new CursorData()
                        {
                            down = true,
                            id = SteamNetworkManager.GetSelfId(),
                            worldPos = Camera.main.ScreenToWorldPoint(Input.mousePosition),
                        });
                        cache = Camera.main.ScreenToWorldPoint(Input.mousePosition);
                    }
                }
            }
            if (interval > 0)
            {
                interval--;
            }
            if (Input.GetMouseButtonUp(0))
            {
                BroadcastCursorData(new CursorData()
                {
                    down = false,
                    id = SteamNetworkManager.GetSelfId(),
                    worldPos = Camera.main.ScreenToWorldPoint(Input.mousePosition),
                });
                interval = 0;
            }
            yield return null;
        }
    }
    
}
using System.Collections;
using System.Collections.Generic;
using UniRx;
using UnityEngine;
using UnityEngine.UI;
using Zenject;

public class WebSocketErrorHandleSample : MonoBehaviour
{
    [SerializeField] private Text _text;
    
    [Inject]
    void Inject(WebSocketSample ws)
    {
        ws.OnError.Subscribe(OnError).AddTo(this);
    }
    public void OnError(string error)
    {
        _text.text = error;
        fc = 0;
    }
    //tekitou
    private int fc = 0;
    void FixedUpdate()
    {
        fc++;
        if (fc > 300)
        {
            _text.text = "";
            fc = 0;
        }
    }
}

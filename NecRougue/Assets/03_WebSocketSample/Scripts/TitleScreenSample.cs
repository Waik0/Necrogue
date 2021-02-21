using System;
using System.Collections;
using System.Collections.Generic;
using UniRx;
using UnityEngine;
using UnityEngine.UI;
using Zenject;

public class TitleScreenSample : MonoBehaviour
{
    [SerializeField] private Button _host;
    [SerializeField] private Button _join;
    [SerializeField] private InputField _inputField;
    private WebSocketErrorHandleSample _error;
    private WebSocketSample _ws;
    private bool isHost = false;
    private bool isActiveView = false;
    [Inject]
    void Inject(WebSocketSample ws,
        WebSocketErrorHandleSample error)
    {
        _ws = ws;
        _error = error;
        _host.onClick.AddListener(OnHost);
        _join.onClick.AddListener(OnJoin);
      
        
    }
    void OnJoin()
    {
        isHost = false;
        if (_inputField.text == "")
        {
            _error.OnError("部屋名を入力してください。");
            return;
        }
        _ws.ConnectJoin(_inputField.text);

    }

    void OnHost()
    {
        isHost = true;
        if (_inputField.text == "")
        {
            _error.OnError("部屋名を入力してください。");
            return;
        }
        _ws.ConnectHost(_inputField.text);
    }
    
}

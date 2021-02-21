using System;
using System.Collections;
using System.Collections.Generic;
using UniRx;
using UnityEngine;
using UnityEngine.UI;
using Zenject;

public class MatchingSample : MonoBehaviour
{
    [SerializeField] private Button _ready;
    [SerializeField] private Text _readyText;
    [SerializeField] private Text _usersText;
    [SerializeField] private InputField _name;
    private Dictionary<string, (string name, string state)> _userData = new Dictionary<string, (string name, string state)>();
    private WebSocketErrorHandleSample _error;
    private WebSocketSample _ws;
    private bool isHost = false;
    private bool isActiveView = false;
    public Dictionary<string, (string name, string state)> UserData => _userData;
    [Inject]
    void Inject(WebSocketSample ws,
        WebSocketErrorHandleSample error)
    {
        _ws = ws;
        _error = error;
        _ws.OnMessage.Subscribe(Route).AddTo(this);
        _ready.onClick.AddListener(Ready);
        _name.onEndEdit.AddListener(s=>Echo());
    }

    void Join()
    {
        _ws.Send(WebSocketSample.TestCommands.Join,"");
    }

    void Echo()
    {
        _ws.Send(WebSocketSample.TestCommands.Echo,_name.text);
    }
    void Ready()
    {
        if (_ws.IsHost)
        {
            _ws.Send(WebSocketSample.TestCommands.Start, _name.text);
        }
        else
        {
            _ws.Send(WebSocketSample.TestCommands.Ready, _name.text);
        }
    }
    void Route(WebSocketSampleResponce res)
    {
        switch (res.command)
        {
            case WebSocketSample.TestCommands.Join:
                AddUser(res);
                Echo();
                break;
            case WebSocketSample.TestCommands.Echo:
                AddUser(res);
                break;
            case WebSocketSample.TestCommands.Ready:
                OnReady(res);
                break;
            case WebSocketSample.TestCommands.sys_leave:
                OnLeave(res);
                break;
        }
    }
    void AddUser(WebSocketSampleResponce res)
    {
        if (!_userData.ContainsKey(res.msgfrom))
        {
            _userData.Add(res.msgfrom,("",""));
        }
        if (res.data != "")
        {
            _userData[res.msgfrom] = (res.data, _userData[res.msgfrom].state);
        }
        _usersText.text = "";
        foreach (var keyValuePair in _userData)
        {
            _usersText.text +="Player : " + keyValuePair.Value.name + " : " + keyValuePair.Value.state;
        }
        
    }

    private void OnLeave(WebSocketSampleResponce res)
    {
        if (!_userData.ContainsKey(res.msgfrom))
        {
            return;
        }
        _userData.Remove(res.msgfrom);
        _usersText.text = "";
        foreach (var keyValuePair in _userData)
        {
            _usersText.text += "Player : " + keyValuePair.Value.name + " : " + keyValuePair.Value.state + "\n";
        }
    }
    private void OnReady(WebSocketSampleResponce res)
    {
        if (!_userData.ContainsKey(res.msgfrom))
        {
            _userData.Add(res.msgfrom,("",""));
        }
        _userData[res.msgfrom] = ( res.data,"ready");
        _usersText.text = "";
        foreach (var keyValuePair in _userData)
        {
            _usersText.text += "Player : " + keyValuePair.Value.name + " : " + keyValuePair.Value.state + "\n";
        }
    }
    void OnEnable()
    {
        _userData?.Clear();
        _readyText.text = _ws.IsHost ? "Start" : "Ready";
        Join();
    }

    private void OnDisable()
    {
        
    }
}

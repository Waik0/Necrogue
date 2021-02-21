using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Toast;
using UniRx;
using UnityEngine;
using Zenject;

public class GameFlowSample : MonoBehaviour
{
    // //logger
    // private string l;
    //
    // private void Start()
    // {
    //     Application.logMessageReceived += LogCallback;
    // }
    //
    // private void OnDestroy()
    // {
    //     Application.logMessageReceived -= LogCallback;
    // }
    //
    // void LogCallback(string condition, string stackTrace, LogType type)
    // {
    //     l += condition + "\n"; 
    // }
    // void OnGUI()
    // {
    //     GUILayout.Label(l);
    // }
    // //---
    public enum State
    {
        Init,
        Title,
        Matching,
        Game,
    }
    [Serializable]
    public class ViewSet
    {
        public GameObject GameObject;
        public State[] TargetState;
    }
    [SerializeField] private ViewSet[] _viewSets;
    private Statemachine<State> _statemachine;
    private WebSocketErrorHandleSample _error;
    private WebSocketSample _ws;

    [Inject]
    void Inject(WebSocketSample ws,
        WebSocketErrorHandleSample error)
    {
        _ws = ws;
        _error = error;
        _ws.OnConnect.Subscribe(_ => OnConnect()).AddTo(this);
        _ws.OnClose.Subscribe(_ => OnClose()).AddTo(this);
        _ws.OnMessage.Subscribe(CheckStartGame).AddTo(this);
    }
    void OnConnect()
    {
        _statemachine.Next(State.Matching);
    }

   
    void OnClose()
    {
        if (_statemachine.Current == State.Title)
        {
            _error.OnError(_ws.IsHost ? "部屋が既に存在します":"部屋がないです");
        }
        _statemachine.Next(State.Title);
    }
    void CheckStartGame(WebSocketSampleResponce res)
    {
        if (res.command == WebSocketSample.TestCommands.Start)
        {
            _statemachine.Next(State.Game);
        }
    }
    // Start is called before the first frame update
    void Awake()
    {
        _statemachine = new Statemachine<State>();
        _statemachine.Init(this);
    }

    // Update is called once per frame
    void Update()
    {
        _statemachine?.Update();
    }

    void ViewChange()
    {
        foreach (var viewSet in _viewSets)
        {
            viewSet.GameObject.SetActive(viewSet.TargetState.Any(_=>_ == _statemachine.Current));
        }
    }


    IEnumerator Init()
    {
        ViewChange();
        _statemachine.Next(State.Title);
        yield return null;
    }

    IEnumerator Title()
    {
        ViewChange();
        yield return null;
    }
    
    IEnumerator Matching()
    {
        ViewChange();
        yield return null;
    }
    
    IEnumerator Game()
    {
        ViewChange();
        yield return null;
    }
}

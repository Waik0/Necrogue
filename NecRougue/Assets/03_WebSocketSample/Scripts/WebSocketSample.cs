using System;
using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UniRx;
using WebSocketSharp;
using WebSocketSharp.Net;

public class WebSocketSampleResponce
{
    public string msgfrom;
    public string data;
    public WebSocketSample.TestCommands command;
}
public class WebSocketSample: MonoBehaviour{
    public enum TestCommands
    {
        sys_leave,
        Error,
        //マッチング
        Join,
        Echo,
        Ready,
        Start,//hostのみ
        //ゲーム内
        Cursor,
        FirstData,//hostのみ シードとプレイヤー順
        Turn,//hostのみ 次のプレイヤーを通知
        Fall,
        //
        Chat,
    }

    private WssigClient ws;
    public readonly Subject<Unit> OnConnect = new Subject<Unit>();
    public readonly Subject<string> OnError = new Subject<string>();
    public readonly Subject<WebSocketSampleResponce> OnMessage = new Subject<WebSocketSampleResponce>();
    public readonly Subject<Unit> OnClose = new Subject<Unit>();
    private Queue<Unit> _onConnectCallStack = new Queue<Unit>();
    private Queue<string> _onErrorCallStack = new Queue<string>();
    private Queue<WebSocketSampleResponce> _onMessageCallStack = new Queue<WebSocketSampleResponce>();
    private Queue<Unit> _onCloseCallStack = new Queue<Unit>();
    public bool IsHost { get; private set; }
    public string SelfId => ws?.SelfId;
    public string Room => ws?.Room;

    void Update()
    {
        //メインスレッド化する
        while (_onConnectCallStack?.Count > 0)
        {
            OnConnect?.OnNext(_onConnectCallStack.Dequeue());
        }
        while (_onErrorCallStack?.Count > 0)
        {
            OnError?.OnNext(_onErrorCallStack.Dequeue());
        }
        while (_onMessageCallStack?.Count > 0)
        {
            OnMessage?.OnNext(_onMessageCallStack.Dequeue());
        }
        while (_onCloseCallStack?.Count > 0)
        {
            OnClose?.OnNext(_onCloseCallStack.Dequeue());
        }
    }
    public void ConnectHost(string room)
    {
        Close();
        ws = new WssigClient();
        Route();
        IsHost = true;
        ws.ConnectHost(room);
   
    }

    public void ConnectJoin(string room)
    {
        Close();
        ws = new WssigClient();
        Route();
        IsHost = false;
        ws.ConnectJoin(room);
    }

    public void Send(TestCommands command ,string data)
    {
        ws?.Send(command.ToString(),data);
    }

    public void Close()
    {
        _onConnectCallStack?.Clear();
        _onMessageCallStack?.Clear();
        _onCloseCallStack?.Clear();
        _onErrorCallStack?.Clear();
        ws?.Dispose();
    }

    void Route()
    {
        ws.OnOpen = () => _onConnectCallStack?.Enqueue(Unit.Default);
        ws.OnError = e => _onErrorCallStack?.Enqueue(e);
        ws.OnMessage = m => _onMessageCallStack?.Enqueue(ToResponce(m));
        ws.OnClose = () => _onCloseCallStack?.Enqueue(Unit.Default);
    }

    WebSocketSampleResponce ToResponce(WssigPayloadProtocol data)
    {
        var canParse = Enum.TryParse<TestCommands>(data.command, out var c) && Enum.IsDefined(typeof(TestCommands), c);
        return new WebSocketSampleResponce()
        {
            command = canParse ? c : TestCommands.Error,
            data = data.data,
            msgfrom = data.msgfrom
        };
    }
    void OnDestroy()
    {
        ws?.Dispose();
        OnConnect?.Dispose();
    }

}
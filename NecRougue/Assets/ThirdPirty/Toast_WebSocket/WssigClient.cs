using System;
using UnityEngine;
using System.Collections;
using WebSocketSharp;
using WebSocketSharp.Net;
public class WssigMessageProtocol<T>
{
    public string from;
    public string data;
    public T command;
    public string version;
    public string identifer;
}

public interface IWssigClient<T> where T : struct
{
    Action<WssigMessageProtocol<T>> OnMessage { get; set; }
    void Connect(string room);
    void Dispose();

}
public class WssigClient<T>: IWssigClient<T>,IDisposable where T : struct {

    public Action<WssigMessageProtocol<T>> OnMessage { get; set; }
    public WebSocket ws;
    private string _url = "ws://localhost:3000";
    public void Connect(string room)
    {
        ws = new WebSocket(_url);
        ws.Log.Output = (data, s) =>
        {
            Debug.Log($"{data.Caller} {data.Level} {data.Message}");
        };
        ws.OnOpen += (sender, e) =>
        {
            Debug.Log("WebSocket Open");
            
        };
        ws.OnMessage += (sender, e) =>
        {
            Debug.Log("WebSocket Message Data: " + e.Data);
        };

        ws.OnError += (sender, e) =>
        {
            Debug.Log("WebSocket Error Message: " + e.Message);
        };

        ws.OnClose += (sender, e) =>
        {
            Debug.Log("WebSocket Close");
        };

        ws.Connect();

    }

    public void Send()
    {
        
    }
    public void Dispose()
    {
        ws?.Close();
        ((IDisposable) ws)?.Dispose();
        ws = null;
        
    }
}
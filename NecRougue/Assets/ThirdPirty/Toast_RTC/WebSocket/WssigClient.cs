using System;
using UnityEngine;
using System.Collections;
using WebSocketSharp;
using WebSocketSharp.Net;
[Serializable]
public class WssigPayloadProtocol
{
    public string msgfrom;
    public string data;
    public string command;
    public string version;
    public string identifer;
}
/// <summary>
/// sys_connected,sys_leaveはあらかじめ用意されたコマンド
/// sys_leaveは実装で
/// </summary>
public interface IWssigClient
{
    Action<WssigPayloadProtocol> OnMessage { get; set; }
    void ConnectHost(string room);
    void ConnectJoin(string room);
    void Dispose();
    void Send(string command,string data);
}
public class WssigClient : IWssigClient,IDisposable{
    private enum SslProtocolsHack
    {
        Tls = 192,
        Tls11 = 768,
        Tls12 = 3072
    }
    public Action OnOpen { get; set; }
    public Action<WssigPayloadProtocol> OnMessage { get; set; }
    public Action<string> OnError { get; set; }
    public Action OnClose { get; set; }
    public bool IsOpen => ws?.IsAlive ?? false;
    public WebSocket ws;
    public string SelfId { get; private set; }
    public string Room { get; private set; }
    private string _url = "wss://wssig.herokuapp.com";
    private string _idenrifer = "sabo";
    private string _version = "0.1";
    public void ConnectHost(string room)
    {
        Debug.Log("connect to " + room);
        ws = new WebSocket(_url);
        ws.SetCookie(new Cookie("create",room));
        Room = room;
        Auth();
        Route();
        Connect();
    }
    public void ConnectJoin(string room)
    {
        Debug.Log("connect to " + room);
        ws = new WebSocket(_url);
        ws.SetCookie(new Cookie("join",room));
        Room = room;
        Auth();
        Route();
        Connect();
    }

    void Auth()
    {
        ws.SetCookie(new Cookie("username","wssig"));
        ws.SetCookie(new Cookie("password","test"));
    }
    void Route()
    {
        ws.Log.Output = (data, s) =>
        {
            Debug.Log($"{data.Caller} {data.Level} {data.Message}");
        };
        ws.OnOpen += (sender, e) =>
        {
            Debug.Log("WebSocket Open");
            //OnOpen?.Invoke();
        };
        ws.OnMessage += (sender, e) =>
        {
            try
            {
                var data = JsonUtility.FromJson<WssigPayloadProtocol>(e.Data);
                if (data.command == "sys_connected")
                {
                    Debug.Log("WebSocket Open Success");
                    SelfId = data.msgfrom;
                    OnOpen?.Invoke();
                    return;
                }
                else if (data.command == "sys_leave")
                {
                    OnMessage?.Invoke(data);
                    return;
                }
                if (data.identifer != _idenrifer) 
                    return;
                if(data.version != _version)
                    return;
                OnMessage?.Invoke(data);
            }
            catch (Exception exception)
            {
                Debug.Log(exception);
                throw;
            }
            //Debug.Log("WebSocket Message Data: " + e.Data);
        };

        ws.OnError += (sender, e) =>
        {
            OnError?.Invoke(e.Message);
            Debug.Log("WebSocket Error Message: " + e.Message);
        };

        ws.OnClose += (sender, e) =>
        {
            Debug.Log("WebSocket Close" + e.Code + e.Reason + e.WasClean);
            OnClose?.Invoke();
        };
    }

    void Connect()
    {
        var sslProtocolHack = (System.Security.Authentication.SslProtocols)(SslProtocolsHack.Tls12 | SslProtocolsHack.Tls11 | SslProtocolsHack.Tls);
        if ( _url.StartsWith("wss") && ws.SslConfiguration.EnabledSslProtocols != sslProtocolHack)
        {
            ws.SslConfiguration.EnabledSslProtocols = sslProtocolHack;
        }
        ws.Connect();
    }
    public void Send(string command,string data)
    {
        ws.Send(JsonUtility.ToJson(new WssigPayloadProtocol()
        {
            command = command.ToString(),
            data = data,
            msgfrom = "",
            identifer = _idenrifer,
            version = _version,
        }));
    }
    public void Dispose()
    {
        ws?.Close();
        ((IDisposable) ws)?.Dispose();
        ws = null;
        
    }
}
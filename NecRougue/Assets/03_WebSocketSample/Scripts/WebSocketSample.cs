using System;
using UnityEngine;
using System.Collections;
using WebSocketSharp;
using WebSocketSharp.Net;

public class WebSocketSample: MonoBehaviour{
    private enum TestCommands
    {
        Test,
    }

    private WssigClient<TestCommands> ws;
    void Start()
    {
        ws = new WssigClient<TestCommands>();
        ws.Connect("test");
    }

    public void Send()
    {
    }
    void Update()
    {
    }

    void OnDestroy()
    {
        ws.Dispose();
    }

}
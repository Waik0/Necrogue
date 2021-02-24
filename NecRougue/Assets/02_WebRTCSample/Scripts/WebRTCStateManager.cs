using System.Collections;
using System.Collections.Generic;
using Unity.WebRTC;
using UnityEngine;

public class WebRTCStateManager
{
    public static WebRTCStateManager Instance { get; } = new WebRTCStateManager();
    private bool _initialized = false;
    public WebRTCStateManager()
    {
        Debug.Log("WebRTC manager Awake");
    }
    public void Init()
    {
        if (!_initialized)
        {
            Debug.Log("Initialized");
            _initialized = true;
            WebRTC.Initialize(EncoderType.Software);
        }
        else
        {
            Debug.Log("Already Initialized");
        }
    }

    public void Dispose()
    {
        Debug.Log("Dispose");
        WebRTC.Dispose();
    }
}

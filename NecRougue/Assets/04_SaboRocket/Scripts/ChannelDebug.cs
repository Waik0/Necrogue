using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Zenject;

public class ChannelDebug : MonoBehaviour
{
    [SerializeField]private Text _text;
    private ITortecHostUseCase _host;
    [Inject]
    void Inject(ITortecHostUseCase host)
    {
        _host = host;
    }
    private void Update()
    {
        _text.text = "";
        foreach (var rtcDataChannel in _host.Channels())
        {
            _text.text += rtcDataChannel.Label + " : " + rtcDataChannel.ReadyState + "\n";
        }
    }
}

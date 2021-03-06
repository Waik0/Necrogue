using System;
using System.Collections;
using System.Collections.Generic;
using Toast.RealTimeCommunication;
using UnityEngine;
using UnityEngine.UI;
using Zenject;

public class ChannelDebug : MonoBehaviour
{
    [SerializeField]private Text _text;
    private ITortecHostUseCaseWithWebSocket _host;
    [Inject]
    void Inject(ITortecHostUseCaseWithWebSocket host)
    {
        _host = host;
    }

}

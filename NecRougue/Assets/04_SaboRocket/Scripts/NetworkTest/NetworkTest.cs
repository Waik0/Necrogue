using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NetworkTest : MonoBehaviour
{
    [SerializeField] private NetworkHostInfoView _hostPrefab;
    [SerializeField] private Transform _hostParent;
    [SerializeField] private NetworkClientInfoView _clientPrefab;
    [SerializeField] private Transform _clientParent;
    public void CreateHost()
    {
        var ins = Instantiate(_hostPrefab, _hostParent);
        ins.gameObject.SetActive(true);
    }

    public void CreateClient()
    {
        var ins = Instantiate(_clientPrefab, _clientParent);
        ins.gameObject.SetActive(true);
    }
}

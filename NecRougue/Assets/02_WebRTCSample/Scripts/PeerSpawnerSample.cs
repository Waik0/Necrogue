using System;
using System.Collections;
using System.Collections.Generic;
using Toast.RealTimeCommunication;
using UnityEngine;

public class PeerSpawnerSample : MonoBehaviour
{
    [SerializeField] private GameObject _peer;
    [SerializeField] private GameObject _peer2;
    [SerializeField] private Transform _parent;
    void Awake()
    {
        _peer.gameObject.SetActive(false);
        _peer2.gameObject.SetActive(false);
    }

    private void Update()
    {
        if(Input.GetMouseButtonDown(0))
            Spawn();
        if(Input.GetMouseButtonDown(1))
            Spawn2();
    }
    
    void Spawn()
    {
        var go = Instantiate(_peer,_parent);
        go.SetActive(true);
        go.GetComponent<TORTECHost>().CreateRoom("aaa");
    }
    void Spawn2()
    {
        var go = Instantiate(_peer2,_parent);
        go.SetActive(true);
        go.GetComponent<TORTECClient>().JoinRoom("aaa");
    }
}

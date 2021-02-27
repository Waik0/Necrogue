using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NetworkHostUseCase : MonoBehaviour
{
    [SerializeField]
    private TortecHostUseCase _tortecHost;
    public void CreateRoom()
    {
        _tortecHost.CreateRoom();
    }
}

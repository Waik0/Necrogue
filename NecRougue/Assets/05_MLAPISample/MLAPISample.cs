using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MLAPISample : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        MLAPI.NetworkingManager.Singleton.StartServer();
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    void OnGUI()
    {
        if (GUILayout.Button("Server"))
        {
            MLAPI.NetworkingManager.Singleton.StartHost();
        }
    }
}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraZoom : MonoBehaviour
{
    [SerializeField] private Camera _cam;

    private void Update()
    {
        var scroll = Input.mouseScrollDelta.y;
        _cam.orthographicSize += scroll;
        

    }
    
}

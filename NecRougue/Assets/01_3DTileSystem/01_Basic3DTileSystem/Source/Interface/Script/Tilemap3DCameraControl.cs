using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface ITilemap3DCameraControl
{
    void MoveAbsolute(Vector3 pos);
    Vector3 Position();
    Vector3 ScreenToWorld(Vector2 pos);
}
public class Tilemap3DCameraControl : MonoBehaviour,ITilemap3DCameraControl
{
    [SerializeField] private Camera _camera;

    public void MoveRelative(Vector3 relative)
    {
        _camera.transform.Translate(relative);
    }

    public Vector3 Position()
    {
        return _camera.transform.position;
    }

    public Vector3 ScreenToWorld(Vector2 pos)
    {
        return _camera.ScreenToWorldPoint(pos);
    }

    public void MoveAbsolute(Vector3 pos)
    {
        _camera.transform.position = pos;
    }
}

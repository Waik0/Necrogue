using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface ITileModel3D
{
    GameObject Instance { get; }
    string Name { get; }
    void DeleteInstance();
    void SetPosition(Vector3 pos);
}
public class TileModel3D : MonoBehaviour,ITileModel3D
{
    [SerializeField] private string _name;
    public string Name => _name;
    public GameObject Instance => gameObject;
    public void DeleteInstance()
    {
        if (Instance != null)
        {
            Destroy(gameObject);
        }
    }

    public void SetPosition(Vector3 pos)
    {
        gameObject.transform.position = pos;
    }
}
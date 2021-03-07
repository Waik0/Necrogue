using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PhysicsPiece : MonoBehaviour
{
    private static int _id = 0;
    public static void InitializeSystem()
    {
        _id = 0;
    }
    //nonstatic
    [SerializeField] private Rigidbody2D _rigidbody2D;
    public int PieceId { get; private set; }
    public int Id { get; private set; }
    public int Angle => (int)gameObject.transform.eulerAngles.z;
    public Vector2Int Pos => new Vector2Int((int)gameObject.transform.position.x,(int)gameObject.transform.position.y);
    public void Place(Vector2 pos,int angle,int pieceId)
    {
        Id = _id;
        _id++;
        PieceId = pieceId;
        transform.position = pos;
        transform.eulerAngles = new Vector3(0, 0, angle);
        _rigidbody2D.velocity = new Vector2(0, 0);
        _rigidbody2D.angularVelocity = 0;
    }
}

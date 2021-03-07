using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Piece : MonoBehaviour
{
    public Sprite Sprite => GetComponent<SpriteRenderer>().sprite;
    public void SetVertex(Vector2Int pos, int angle)
    {
        transform.eulerAngles = new Vector3(0, 0, angle);
        transform.position = new Vector3(pos.x, pos.y, 0);
    }
}

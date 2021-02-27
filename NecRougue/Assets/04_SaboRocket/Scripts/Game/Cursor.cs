using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Cursor : MonoBehaviour
{
    [SerializeField] private TextMesh _textMesh;
    [SerializeField] private SpriteRenderer _spriteRenderer;
    public void SetPos(Vector2 worldPos)
    {
        transform.position = worldPos;
    }

    public void SetName(string name)
    {
        _textMesh.text = name;
    }

}

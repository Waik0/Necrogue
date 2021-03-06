using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Cursor : MonoBehaviour
{
    [SerializeField] private GameObject _cursorRoot;
    [SerializeField] private TextMesh _textMesh;
    [SerializeField] private SpriteRenderer _spriteRenderer;
    private Vector2 _aim;
    private int _renewPos = 0;
    private bool _isDown = false;
    public void SetPos(CursorData cursorData)
    {
        if (_isDown == false)
        {
            transform.position = cursorData.worldPos;
        }
        _aim = cursorData.worldPos;
        _renewPos = 10;
        _isDown = cursorData.down;
        _spriteRenderer.color = Color.white;
    }

    private void LateUpdate()
    {
        transform.position = Vector3.Lerp(transform.position,_aim,.1f);
        if (_renewPos > 0 && !_isDown)
        {
            _renewPos --;
        }
        else if(_renewPos <= 0)
        {
            if (_spriteRenderer.color.a > 0)
            {
                var c = _spriteRenderer.color;
                c.a *= 0.9f;
                _spriteRenderer.color = c;
            }
        }
    }

    public void SetName(string name)
    {
        _textMesh.text = name;
    }

}

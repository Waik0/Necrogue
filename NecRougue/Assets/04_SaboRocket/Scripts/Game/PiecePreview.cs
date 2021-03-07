using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Zenject;

public class PiecePreview : MonoBehaviour
{
    [SerializeField] private SpriteRenderer _sprite;
    private PieceDatas _pieceDatas;
    [Inject]
    void Inject(PieceDatas pieceDatas)
    {
        _pieceDatas = pieceDatas;
    }
    public void SetActive(bool active)
    {
        if (!active)
        {
            _sprite.color = Color.clear;
        }
        else
        {
            _sprite.color = Color.white;
        }
    }
    public void SetPreview(int id)
    {
        var data = _pieceDatas.GetPiece(id);
        _sprite.sprite = data.ViewPrefab.Sprite;
    }

    public void SetPos(Vector2 pos)
    {
        transform.position = pos;
    }
    public void Rotate(int ang)
    {
        transform.eulerAngles = new Vector3(0, 0, ang);
    }
}

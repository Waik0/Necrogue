using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PiecePreview : MonoBehaviour
{
    [SerializeField] private SpriteRenderer _sprite;
    [SerializeField] private Sprite[] _candidates;

    public int CandidateNum => _candidates.Length;
    public void SetSprite(int index)
    {
        _sprite.sprite = _candidates[index];
    }

    public void Rotate(int ang)
    {
        transform.eulerAngles = new Vector3(0, 0, ang);
    }
}

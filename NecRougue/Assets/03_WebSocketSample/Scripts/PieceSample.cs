using System.Collections;
using System.Collections.Generic;
using UnityEditor.U2D.Path;
using UnityEngine;
[RequireComponent(typeof(Rigidbody2D))]
[RequireComponent(typeof(SpriteRenderer))]
public class PieceSample : MonoBehaviour
{
    [SerializeField]
    private Rigidbody2D _rigidbody2D;
    [SerializeField]
    private SpriteRenderer _sprite;
    public void Init(Sprite sprite)
    {
        if (GetComponent<PolygonCollider2D>())
        {
            Destroy(GetComponent<PolygonCollider2D>());
        }
        _sprite.sprite = sprite;
        _rigidbody2D.simulated = false;
       
    }

    public void SetAng(float ang)
    {
        transform.eulerAngles = new Vector3(0, 0, ang);
    }
    public void StartCalc(Vector2 pos,float ang)
    {
        gameObject.AddComponent<PolygonCollider2D>();
        transform.eulerAngles = new Vector3(0, 0, ang);
        transform.position = pos;
        _rigidbody2D.velocity = Vector2.zero;
        _rigidbody2D.angularVelocity = 0;
        _rigidbody2D.simulated = true;
        
    }

    public bool IsSleep() => _rigidbody2D.IsSleeping();
    public bool IsDead() => transform.position.y < -100;
    private Color _c = new Color(.5f, .5f, .5f);
    void FixedUpdate()
    {
        _sprite.color = _rigidbody2D.IsSleeping() ? _c : Color.white;
    }
}

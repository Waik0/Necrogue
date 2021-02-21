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

    private bool _startCalc;
    private int _time = 0;
    public void Init(Sprite sprite)
    {
        if (GetComponent<PolygonCollider2D>())
        {
            Destroy(GetComponent<PolygonCollider2D>());
        }
        _sprite.sprite = sprite;
        _rigidbody2D.simulated = false;
        _startCalc = false;
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
        _rigidbody2D.velocity = new Vector2(0,0.1f);
        _rigidbody2D.angularVelocity = 0;
        _rigidbody2D.simulated = true;
        _startCalc = true;
        _time = 0;
    }

    public bool IsSleep() => _rigidbody2D.IsSleeping();
    public bool IsDead() => transform.position.y < -100;
    private Color _c = new Color(.5f, .5f, .5f);
    void FixedUpdate()
    {
        if (_startCalc)
        {
            _time++;
            if (_time > 50)
            {
                var a = Mathf.Abs(_rigidbody2D.angularVelocity) < .5f;
                var v = _rigidbody2D.velocity.magnitude < .005f;
                if (a)
                {
                    _rigidbody2D.angularVelocity = 0;
                }
                if (v)
                {
                    _rigidbody2D.velocity = Vector2.zero;
                }

                if (a && v)
                {
                    _rigidbody2D.Sleep();
                }


            }

            var sleep = _rigidbody2D.IsSleeping();
            _sprite.color = sleep ? _c : Color.white;
            if (_rigidbody2D.simulated && sleep) _time = 100;
        }
    }
}

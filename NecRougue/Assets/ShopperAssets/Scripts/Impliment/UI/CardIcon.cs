using System;
using System.Collections;
using System.Collections.Generic;
using ShopperAssets.Scripts.Game;
using UnityEngine;
using UnityEngine.UI;

public class CardIcon : ICardIconUI
{
    private RectTransform _target;
    [SerializeField] private RectTransform _root;
    [SerializeField] private Image _icon;
    [SerializeField] private Button _button;
    [SerializeField] private Text _debug;
    private float _speed = 3.5f;
    public void SetAim(GameObject target)
    {
        _target = target.GetComponent<RectTransform>();
    }

    private void Update()
    {
        if (_target != null)
        {
            var normal = (_target.position - _root.position).normalized;
            var ext = Mathf.Min((_target.position - _root.position).magnitude, _speed);
            if (ext < _speed)
            {
                _root.position = _target.position;
            }
            else
            {
                _root.position = _root.position + normal * ext;
            }
        }
    }

    public override void SetCard(CardModel card)
    {
        _icon.sprite = null;
        _debug.text = card.Name;
       
    }

    public void Awake()
    {
        _button.onClick.AddListener(() =>
        {
            if (!IsSelected)
            {
                _selectedUnique = Unique;
                OnSelected?.Invoke(Unique);
            }
            else
            {
                OnExecuted?.Invoke(Unique);
                _selectedUnique = "";
            }
        });
    }

}
/*
    private float _time = 0.3f;
    private float _currentTime = 0;
    private Vector2 _from;
    private Vector2 _to;
    public void SetAim(GameObject target)
    {
        _target = target.GetComponent<RectTransform>();
        _from = _root.position;
        
        _currentTime = 0;
    }

    private void Update()
    {
       
        if (_target != null)
        {
            _to = _target.position;
            _currentTime += Time.deltaTime;
            var dur = Mathf.Min(_currentTime / _time,1);
            _root.position = Vector3.Lerp(_from, _to,dur);
        }
    }*/
using System;
using System.Collections;
using System.Collections.Generic;
using CafeAssets.Script.System.GameMapSystem;
using UnityEngine;
using UnityEngine.UI;

public class TileSelectButton : MonoBehaviour
{
    [SerializeField]
    private Text _text;
    [SerializeField]
    private Image _image;
    [SerializeField] 
    private Button _button;
    public void Setup(string text,Sprite image,Action onPushButton)
    {
        _text.text = text;
        _image.sprite = image;
        _button.onClick.RemoveAllListeners();
        _button.onClick.AddListener(()=>onPushButton?.Invoke());
    }
}

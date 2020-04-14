using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ButtonUI : MonoBehaviour,IButtonUI
{
    [SerializeField] private Button _button;

    private bool _isClicked = false;
    private void Awake()
    {
        _button.onClick.AddListener(()=>_isClicked = true);
    }

    public void Init()
    {
        _isClicked = false;
    }
    public bool UpdateUI()
    {
        return _isClicked;
    }
}

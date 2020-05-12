using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ButtonUI : MonoBehaviour,IModalUI
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

    public void ResetUI()
    {
        
    }

    public bool UpdateUI()
    {
        return _isClicked;
    }
}

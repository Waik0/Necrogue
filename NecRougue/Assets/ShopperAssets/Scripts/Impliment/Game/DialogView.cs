using System.Collections;
using System.Collections.Generic;
using System.Net.Mime;
using DG.Tweening;
using DG.Tweening.Core;
using UnityEngine;
using UnityEngine.UI;

public class DialogView : MonoBehaviour
{
    [SerializeField] private RectTransform _root;
    [SerializeField] private Text _text;
    [SerializeField] private Button _yes;
    [SerializeField] private Button _no;
    private Vector2 _sizeCache;
    public Button PositiveButton => _yes;
    public Button NegativeButton => _no;
    private Tweener _tweener;
    private float param = 0;
    void Awake()
    {
        _root.gameObject.SetActive(false);
    }

    public void Open()
    {
        _text.gameObject.SetActive(false);
        _root.gameObject.SetActive(true);
        _root.sizeDelta = new Vector2(0,_sizeCache.y);
        _tweener?.Kill();
        _tweener = DOTween.To(() => param,
            p => 
            {
                param = p;
                int w = (int)(_sizeCache.x * p);
                w += w % 2;//偶数にする
                w = w < 4 ? 0 : w;
                _root.sizeDelta = new Vector2(w,_sizeCache.y);
            }, 1, 0.15f);
        _tweener.onComplete = () => { _text.gameObject.SetActive(true); };
        _tweener.SetEase(Ease.Linear);
    }

    public void Close()
    {
        _root.sizeDelta = new Vector2(_sizeCache.x,_sizeCache.y);
        _text.gameObject.SetActive(false);
        _tweener?.Kill();
        _tweener = DOTween.To(() => param,
            p =>
            {
                param = p;
                int w = (int)(_sizeCache.x * p);
                w += w % 2;//偶数にする
                w = w < 4 ? 0 : w;
                _root.sizeDelta = new Vector2(w,_sizeCache.y);
            }, 0, 0.15f);
        _tweener.SetEase(Ease.Linear);
        _tweener.onComplete = () => _root.gameObject.SetActive(false);
    }
    public void SetPosition(Vector2 pos,Vector2 size)
    {
        _root.anchoredPosition = pos;
        _sizeCache = size;
    }
    public void SetText(string text)
    {
        _text.text = text;
    }
    
}

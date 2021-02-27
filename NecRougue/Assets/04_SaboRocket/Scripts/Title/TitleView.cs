using System.Collections;
using System.Collections.Generic;
using UniRx;
using UnityEngine;
using Zenject;

public class TitleView : MonoBehaviour
{
    [SerializeField] private Transform _view;
    private TitleSequence _titleSequence;
    [Inject]
    void Inject(TitleSequence titleSequence)
    {
        _titleSequence = titleSequence;
        titleSequence.OnActiveSequence.Subscribe(OnChangeActive).AddTo(this);
        _view.gameObject.SetActive(false);
    }

    void OnChangeActive(bool active)
    {
        _view.gameObject.SetActive(active);
    }

    public void ToMatchingHost()
    {
        _titleSequence.ToHost();
    }
    public void ToMatchingClient()
    {
        _titleSequence.ToClient();
    }

}

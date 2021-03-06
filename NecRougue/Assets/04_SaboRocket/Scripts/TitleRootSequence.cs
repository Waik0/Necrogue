using System;
using System.Collections;
using System.Collections.Generic;
using Toast;
using UnityEngine;
using UnityEngine.SceneManagement;
using Zenject;

public class TitleRootSequence : MonoBehaviour
{
    public enum State
    {
        Title,
    }

    private Statemachine<State> _statemachine;
    private TitleSequence _titleSequence;

    [Inject]
    void Inject(
        TitleSequence titleSequence
    )
    {
        _titleSequence = titleSequence;
        _statemachine = new Statemachine<State>();
        _statemachine.Init(this);
        Application.targetFrameRate = 60;
    }

    void Update()
    {
        _statemachine.Update();
    }

    IEnumerator Title()
    {
        _titleSequence.ResetSequence();
        while (true)
        {
            _titleSequence.UpdateSequence();
            switch (_titleSequence.CurrentState)
            {
                case TitleSequence.State.ToMatchingHost:
                    SceneManager.LoadScene("GameHost");
                    _titleSequence.StopSequence();
                    break;
                case TitleSequence.State.ToMatchingClient:
                    SceneManager.LoadScene("GameClient");
                    _titleSequence.StopSequence();
                    break;
            }
            yield return null;
        }
    }
    
}

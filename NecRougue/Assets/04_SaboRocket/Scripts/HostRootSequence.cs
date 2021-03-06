using System;
using System.Collections;
using System.Collections.Generic;
using Toast;
using UnityEngine;
using UnityEngine.SceneManagement;
using Zenject;

public class HostRootSequence : MonoBehaviour
{
    public enum State
    {
        MatchingHost,
        GameHost,
    }

    private Statemachine<State> _statemachine;
    private MatchingHostSequence _matchingHostSequence;
    private InGameHostSequence _inGameHostSequence;

    [Inject]
    void Inject(
        MatchingHostSequence matchingSequence,
        InGameHostSequence inGameHostSequence
        )
    {
        _matchingHostSequence = matchingSequence;
        _inGameHostSequence = inGameHostSequence;
        _statemachine = new Statemachine<State>();
        _statemachine.Init(this);
        _statemachine.Next(State.MatchingHost);
        Application.targetFrameRate = 60;
    }

    void Update()
    {
        _statemachine?.Update();
    }

    IEnumerator MatchingHost()
    {
        _matchingHostSequence.ResetSequence();
        while (true)
        {
            _matchingHostSequence.UpdateSequence();
            switch (_matchingHostSequence.CurrentState)
            {
                case MatchingHostSequence.State.ToGame:
                    _statemachine.Next(State.GameHost);
                    _matchingHostSequence.StopSequence();
                    break;
                case MatchingHostSequence.State.ToTitle:
                    SceneManager.LoadScene("Title");
                    _matchingHostSequence.StopSequence();
                    break;
            }
            yield return null;
        }
        
        yield return null;
    }
    IEnumerator GameHost()
    {
        _inGameHostSequence.ResetSequence();
        while (true)
        {
            _inGameHostSequence.UpdateSequence();
            switch (_inGameHostSequence.CurrentState)
            {
                case InGameHostSequence.State.ToTitle:
                    SceneManager.LoadScene("Title");
                    _inGameHostSequence.StopSequence();
                    break;
            }
            yield return null;
        }
        yield return null;
    }
}

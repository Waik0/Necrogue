using System;
using System.Collections;
using System.Collections.Generic;
using Toast;
using UnityEngine;
using Zenject;

public class RootSequence : MonoBehaviour
{
    public enum State
    {
        Title,
        MatchingHost,
        MatchingClient,
        GameHost,
        GameClient
    }

    private Statemachine<State> _statemachine;
    private TitleSequence _titleSequence;
    private MatchingHostSequence _matchingHostSequence;
    private MatchingClientSequence _matchingClientSequence;
    private InGameHostSequence _inGameHostSequence;
    private InGameClientSequence _inGameClientSequence;
    
    [Inject]
    void Inject(
        TitleSequence titleSequence,
        MatchingHostSequence matchingSequence,
        MatchingClientSequence matchingClientSequence,
        InGameHostSequence inGameHostSequence,
        InGameClientSequence inGameClientSequence
        )
    {
        _titleSequence = titleSequence;
        _matchingHostSequence = matchingSequence;
        _matchingClientSequence = matchingClientSequence;
        _inGameHostSequence = inGameHostSequence;
        _inGameClientSequence = inGameClientSequence;
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
                    _statemachine.Next(State.MatchingHost);
                    _titleSequence.StopSequence();
                    break;
                case TitleSequence.State.ToMatchingClient:
                    _statemachine.Next(State.MatchingClient);
                    _titleSequence.StopSequence();
                    break;
            }
            yield return null;
        }
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
                    _statemachine.Next(State.Title);
                    _matchingHostSequence.StopSequence();
                    break;
            }
            yield return null;
        }
        
        yield return null;
    }
    IEnumerator MatchingClient()
    {
        _matchingClientSequence.ResetSequence();
        while (true)
        {
            _matchingClientSequence.UpdateSequence();
            switch (_matchingClientSequence.CurrentState)
            {
                case MatchingClientSequence.State.ToGame:
                    _statemachine.Next(State.GameClient);
                    _matchingClientSequence.StopSequence();
                    break;
                case MatchingClientSequence.State.ToTitle:
                    _statemachine.Next(State.Title);
                    _matchingClientSequence.StopSequence();
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
                    _statemachine.Next(State.Title);
                    _inGameHostSequence.StopSequence();
                    break;
            }
            yield return null;
        }
        yield return null;
    }

    IEnumerator GameClient()
    {
        _inGameClientSequence.ResetSequence();
        while (true)
        {
            _inGameClientSequence.UpdateSequence();
            yield return null;
        }
        yield return null; 
    }
}

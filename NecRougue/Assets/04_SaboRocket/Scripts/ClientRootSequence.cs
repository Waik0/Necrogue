using System.Collections;
using System.Collections.Generic;
using Toast;
using UnityEngine;
using UnityEngine.SceneManagement;
using Zenject;

public class ClientRootSequence : MonoBehaviour
{
    public enum State
    {
        MatchingClient,
        GameClient,
    }
    private Statemachine<State> _statemachine;
    private MatchingClientSequence _matchingClientSequence;
    private InGameClientSequence _inGameClientSequence;
    [Inject]
    void Inject(
        MatchingClientSequence matchingSequence,
        InGameClientSequence inGameClientSequence
    )
    {
        _matchingClientSequence = matchingSequence;
        _inGameClientSequence = inGameClientSequence;
        _statemachine = new Statemachine<State>();
        _statemachine.Init(this);
        _statemachine.Next(State.MatchingClient);
        Application.targetFrameRate = 60;
    }
    void Update()
    {
        _statemachine?.Update();
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
                    SceneManager.LoadScene("Title");
                    _matchingClientSequence.StopSequence();
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
            switch (_inGameClientSequence.CurrentState)
            {
                case InGameClientSequence.State.ToTitle:
                    SceneManager.LoadScene("Title");
                    _inGameClientSequence.StopSequence();
                    break;
            }
            yield return null;
        }
        yield return null; 
    }
}

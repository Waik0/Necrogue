using System;
using System.Collections;
using System.Collections.Generic;
using Toast;
using UnityEngine;

public class EntireSequence : MonoBehaviour
{

    enum State
    {
        Init,
        Advertize,
        Title,
        Intro,
        Game,
    }

    enum TitleSelect
    {
        None,
        Normal,
    }
    Statemachine<State> _statemachine = new Statemachine<State>();
    IntroSequence _introSequence = new IntroSequence();
    GameSequence _gameSequence = new GameSequence();
    private TitleSelect _titleSelect = TitleSelect.None;
    void Awake()
    {
        _gameSequence.Inject();
        _introSequence.Inject();
        _statemachine.Init(this);
    }

    void Update()
    {
        _statemachine.Update();
    }

    IEnumerator Init()
    {
        Application.targetFrameRate = 60;
        _statemachine.Next(State.Advertize);
        yield return null;
    }
    IEnumerator Advertize()
    {
        _statemachine.Next(State.Title);
        yield return null;
    }
    IEnumerator Title()
    {
        while (_titleSelect == TitleSelect.None)
        {
            yield return null;
        }

        switch (_titleSelect)
        {
            case TitleSelect.None:
                break;
            case TitleSelect.Normal:
                _statemachine.Next(State.Intro);
                break;
        }
    }
    IEnumerator Intro()
    {
        _introSequence.ResetSequence();
        while (_introSequence.UpdateSequence())
        {
            yield return null;
        }
        _statemachine.Next(State.Game);
        yield return null;
    }
    IEnumerator Game()
    {
        _gameSequence.ResetSequence();
        while (_gameSequence.UpdateSequence())
        {
            yield return null;
        }
    }
    
    #if DEBUG
    void OnGUI()
    {
        GUI.skin.label.fontSize =  Screen.width / 64;
        GUI.skin.button.fontSize = Screen.width / 64;
        switch (_statemachine.Current)
        {
            case State.Init:
                break;
            case State.Advertize:
                break;
            case State.Title:
                TitleUI();
                break;
            case State.Intro:
                _introSequence.DebugUI();
                break;
            case State.Game:
                _gameSequence.DebugUI();
                break;
            default:
                throw new ArgumentOutOfRangeException();
        }
    }

    void TitleUI()
    {
        GUILayout.BeginVertical();
        if (GUILayout.Button("Normal Mode"))
        {
            _titleSelect = TitleSelect.Normal;
        }
        GUILayout.EndVertical();
    }
    #endif
    
}

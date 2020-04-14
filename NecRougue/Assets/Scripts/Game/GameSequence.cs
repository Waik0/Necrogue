using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Toast;
public class GameSequence : MonoBehaviour
{
    public enum State
    {
        Map,
        Battle,
        Event,
        Shop,
        Pause
    }

    //todo DI対応---
    [SerializeField] private MapSequence _mapSequence;
    [SerializeField] private BattleSequence _battleSequence;
    //---
    private Statemachine<State> _statemachine;
    //private
    private DataController Data = new DataController();

    public void SetData(DataController controller)
    {
        Data = controller;
    }
    void Awake()
    {
        Init();
    }

    void Init()
    {
        _statemachine = new Statemachine<State>();
        _statemachine.Init(this);
        _statemachine.Next(State.Battle);
    }

    IEnumerator Map()
    {
        yield return null;
        _mapSequence.ResetSequence();
        string result = _mapSequence.UpdateSequence();
        if (result != "")
        {
            
        }
    }

    IEnumerator Battle()
    {
        Debug.Log("[ GameSequence ] BattleStart");
        yield return null;
        _battleSequence.ResetSequence();
        string result = _battleSequence.UpdateSequence();
    }
    // Update is called once per frame
    void Update()
    {
        _statemachine.Update();
    }
#if DEBUG
    void OnGUI()
    {
        DebugUI();

    }
    void DebugUI()
    {
        GUILayout.Label("DEBUG UI");
        switch (_statemachine.Current)
        {
            case State.Map:
                break;
            case State.Battle:
                _battleSequence.DebugUI();
                break;
            case State.Event:
                break;
            case State.Shop:
                break;
            case State.Pause:
                break;
            default:
                throw new ArgumentOutOfRangeException();
        }

        
    }
#endif

}

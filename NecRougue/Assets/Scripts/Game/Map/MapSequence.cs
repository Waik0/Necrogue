using System.Collections;
using System.Collections.Generic;
using Toast;
using UnityEngine;

public class MapSequence : SequenceBehaviour
{
    [SerializeField] private IMapUI _mapUi;
    [SerializeField] private IButtonUI _settingButton;
    public enum State
    {
        Init,
        Select,
        End
    }
    private string _result = "";
    private Statemachine<State> _statemachine;

    void Awake()
    {
        _statemachine = new Statemachine<State>();
        _statemachine.Init(this);
    }
    public override void ResetSequence()
    {
        _result = "";
        _statemachine.Next(State.Init);
    }

    private IEnumerator Init()
    {
        _statemachine.Next(State.Select);
        yield return null;
    }

    private IEnumerator Select()
    {
        int idx = _mapUi.UpdateUI();
        if (idx >= 0)
        {
            _result = idx.ToString();
            _statemachine.Next(State.End);
        }
        yield return null;
    }
    public override string UpdateSequence()
    {
        _statemachine.Update();
        return _result;
    }
    
}

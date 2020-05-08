using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Toast;
using UnityEngine;

public class MapSequence : SequenceBehaviour<int>
{
    [SerializeField] private MapUI _mapUi;
    [SerializeField] private ButtonUI _settingButton;
    public enum State
    {
        Init,
        Select,
        End
    }
    private int _result = -1;
    private Statemachine<State> _statemachine;
    //todo DI対応
    private MapDataUseCase _mapDataUseCase;
    
    public void Inject(MapDataUseCase map)
    {
        _mapDataUseCase = map;
        _mapUi.Inject(_mapDataUseCase);
    }
    void Awake()
    {
        _statemachine = new Statemachine<State>();
        _statemachine.Init(this);
    }
    public override void ResetSequence()
    {
        DebugLog.Function(this,1);
        _result = -1;
        _statemachine.Next(State.Init);
    }

   
    private IEnumerator Init()
    {
        _result = -1;
        while (_mapDataUseCase == null)
        {
            yield return null;
        }
        
        _statemachine.Next(State.Select);
        yield return null;
    }

    private IEnumerator Select() 
    {
        _mapUi.ResetUI();
        while (!_mapUi.UpdateUI())
        {
           
            yield return null;
        }
        int idx = _mapUi.GetSelectedMap();
        if (idx < 0) yield break;
        _result = idx;
        _statemachine.Next(State.End);

    }

    private IEnumerator End()
    {
        yield return null;
    }
    public override int UpdateSequence()
    {
        _statemachine.Update();
        return _result;
    }
#if DEBUG 
    public void DebugUI()
    {
        GUILayout.Label("[ MAP ] STATE : "+ _statemachine.Current.ToString());
        switch (_statemachine.Current)
        {
            case State.Init:
                break;
            case State.Select:
                _mapUi.DebugUI();
                break;
            case State.End:
                break;
            default:
                throw new ArgumentOutOfRangeException();
        }

        
    }
    
#endif

    
}

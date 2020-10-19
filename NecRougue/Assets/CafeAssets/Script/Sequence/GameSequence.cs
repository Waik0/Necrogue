﻿using System.Collections;
using System.Collections.Generic;
using CafeAssets.Script.System.GameCoreSystem;
using CafeAssets.Script.System.PropsSystem;
using Toast;
using UnityEngine;
using Zenject;

public class GameSequence : MonoBehaviour,ISequence,ISequenceResult<GameSequence.ResultState>
{
    public enum State
    {
        InitParams,
        InitMap,
        LoadData,
        GameTick,
        End,
    }

    public enum ResultState
    {
        None,
        Clear,
        Over,
    }

    private ResultState _result;
    private Statemachine<State> _statemachine;
    private Props _props;
    private IGameUseCase _gameUseCase;

    [Inject]
    void Inject(
        Props props,
        IGameUseCase gameUseCase)
    {
        _props = props;
        _gameUseCase = gameUseCase;
    }
    void Awake()
    {
        _statemachine = new Statemachine<State>();
        _statemachine.Init(this);
        
    }
    public bool UpdateState()
    {
        _statemachine.Update();
        return _statemachine.Current != State.End;
    }
    IEnumerator InitParams()
    {
        Debug.Log("[Game]Initialize");
        _gameUseCase.ResetParams();
        _statemachine.Next(State.InitMap);
        yield return null;
    }
    IEnumerator InitMap()
    {
        Debug.Log("[Game]InitializeMap");
        _gameUseCase.ResetMap();
        _statemachine.Next(State.LoadData);
        yield return null;
    }
    IEnumerator LoadData()
    {
        Debug.Log("[Game]LoadData");
        Debug.Log("DataSlot : " + _props.UsingSaveSlot);
        _statemachine.Next(State.GameTick);
        yield return null;
    }
    //ゲーム内時間を進める
    IEnumerator GameTick()
    {
        while (true)
        {
            _gameUseCase.Tick();
            yield return null;
        }
        yield return null;
    }
    public ResultState Result()
    {
        return _result;
    }
}

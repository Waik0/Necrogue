using System;
using System.Collections;
using System.Collections.Generic;
using Toast;
using UnityEngine;

public class InitialSequenceBase<T> : MonoBehaviour where T : struct
{
    private Statemachine<T> _statemachine;
    // Start is called before the first frame update
    void Awake()
    {
        _statemachine = new Statemachine<T>();
        _statemachine.Init(this);
    }

    public void NextState(T next)
    {
        _statemachine.Next(next);
    }

    // Update is called once per frame
    void Update()
    {
        _statemachine.Update();
    }
}

public class SequenceBase<T,T2> where T : struct where T2 : class
{
    private Statemachine<T> _statemachine = new Statemachine<T>();
    private bool _isEnd;
    private T2 _returns;
    public SequenceBase()
    {
        Initialize();
    }

    public T CurrentState => _statemachine.Current;

    /// <summary>
    /// 終了 戻り値設定
    /// </summary>
    protected void SetReturn(T2 returns)
    {
        _isEnd = true;
        _returns = returns;
    }

    protected void NextState(T state)
    {
        _statemachine.Next(state);
    }

    public T2 GetReturn()
    {
        return _returns;
    }
    void Initialize()
    {
        DebugLog.Function(this);
        _statemachine = new Statemachine<T>();
        _statemachine.Init(this);
    }
    void Reset()
    {
        _isEnd = false;
        var values = Enum.GetValues(typeof(T));
        foreach (var value in values)
        {
            _statemachine.Next((T)value);
            return;
        }
    }
    void Update()
    {
        _statemachine.Update();
    }

    public IEnumerator UpdateCoroutine()
    {
        Reset();
        while (!_isEnd)
        {
           Update();
            yield return null;
        }
    }
}

public class SequenceBase<T> where T : struct
{
    private Statemachine<T> _statemachine = new Statemachine<T>();
    private bool _isEnd;
    public SequenceBase()
    {
        Initialize();
    }

    /// <summary>
    /// 終了 戻り値設定
    /// </summary>
    protected void SetReturn()
    {
        _isEnd = true;
    }
    void Initialize()
    {
        DebugLog.Function(this);
        _statemachine = new Statemachine<T>();
    }
    void Reset()
    {
        _isEnd = false;
        var values = Enum.GetValues(typeof(T));
        foreach (var value in values)
        {
            _statemachine.Next((T)value);
            return;
        }
    }
    void Update()
    {
        _statemachine.Update();
    }

    public IEnumerator UpdateCoroutine()
    {
        Reset();
        while (!_isEnd)
        {
            Update();
            yield return null;
        }
    }
}



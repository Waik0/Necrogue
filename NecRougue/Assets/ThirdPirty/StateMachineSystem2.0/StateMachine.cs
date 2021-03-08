using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;

public interface IState
{
    void OnEnterState();
    //1回目はEnterと同フレームで呼ばれる
    IEnumerator Update();
}

public abstract class State : IState
{
    public Coroutine Coroutine { get; set; }
    public virtual void OnEnterState() { }
    public abstract IEnumerator Update();
    public virtual void OnExitState() { }
    private Action<Type> _onExitInternal;
    public void RegisterExitAction(Action<Type> action)
    {
        _onExitInternal = action;
    }
    public void Exit<T>() where T : class
    { 
        _onExitInternal?.Invoke(typeof(T));   
    }
}
public class StateMachine
{
    private class CoroutineHolder : MonoBehaviour{}
    private static Transform _root;
    public static Transform Root
    {
        get
        {
            if (_root == null)
                _root = new GameObject("StateMachines").transform;
            return _root;
        }
    }
    private MonoBehaviour _parent;
    private MonoBehaviour Parent
    {
        get
        {
            if (_parent == null)
            {
                var go = new GameObject("StateMachine",new []{typeof(CoroutineHolder)});
                go.transform.parent = Root;
                _parent = go.GetComponent<CoroutineHolder>();
            }
            return _parent;
        }
    }

    private Dictionary<Type, State> _states = new Dictionary<Type, State>();
   
    private Type _currentState;
    public void Add<T>(T instance) where T : State,new()
    {
        if (_states.ContainsKey(typeof(T)))
        { 
            Debug.LogError("Already Registered"); 
            return;
        }
        instance.RegisterExitAction(Next);
        _states.Add(typeof(T), instance);
    }
    //todo gameObject分けて中断&再開に対応させたい
    public void Next<T>() where T : State
    {
        Next(typeof(T));
    }
    public void Stop()
    {
        if (_currentState != null)
        {
            GetState(_currentState)?.OnExitState();
        }
        Parent.StopAllCoroutines();
    }
    public void Next(Type t)
    {
        Stop();
        _currentState = t;
        Start(t);
    }
    public State GetState(Type type)
    {
        if (_states.ContainsKey(type))
        {
            return _states[type];
        }
        return null; 
    }
    public T GetState<T>() where T : State
    {
        if (_states.ContainsKey(typeof(T)))
        {
            return _states[typeof(T)] as T;
        }
        return null;
    }
    public void Dispose()
    {
        _states.Clear(); 
        GameObject.Destroy(_root);
    }
    
    void Start(Type t)
    {
        var state = GetState(t);
        if (state == null) return;
        state.OnEnterState();
        state.Coroutine = Parent.StartCoroutine(state.Update());
    }
    // void Stop(Type t)
    // {
    //     if (_parent == null) return; 
    //     var state = GetState(t);
    //     if (state == null) return;
    //     if (state.Coroutine == null) return;
    //     _parent.StopCoroutine(state.Coroutine);
    // }

}
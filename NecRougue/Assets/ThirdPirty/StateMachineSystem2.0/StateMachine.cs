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
    private class StateMachineRoot : MonoBehaviour
    {
        #if UNITY_EDITOR
        void OnGUI()
        {
            GUILayout.BeginHorizontal();
            foreach (var componentsInChild in GetComponentsInChildren<CoroutineHolder>())
            {
                GUILayout.BeginVertical();
                GUILayout.Label("StateMachine","box");
                foreach (var stateMachineState in componentsInChild.StateMachine._states)
                {
                    if (componentsInChild.StateMachine._currentState == stateMachineState.Key)
                    {
                        GUI.enabled = false;
                    }
                    if (GUILayout.Button(stateMachineState.Key.Name))
                    {
                        componentsInChild.StateMachine.Next(stateMachineState.Key);
                    }

                    GUI.enabled = true;
                }
                GUILayout.EndVertical();
            }
            GUILayout.EndHorizontal();
        }
        #endif
    }

    private class CoroutineHolder : MonoBehaviour
    {
        public StateMachine StateMachine { get; set; }
    }
    private static Transform _root;
    public static Transform Root
    {
        get
        {
            if (_root == null)
            {
                _root = new GameObject("StateMachines").transform;
                _root.gameObject.AddComponent<StateMachineRoot>();
            }

            return _root;
        }
    }
    private CoroutineHolder _parent;
    private CoroutineHolder Parent
    {
        get
        {
            if (_parent == null)
            {
                var go = new GameObject("StateMachine",new []{typeof(CoroutineHolder)});
                go.transform.parent = Root;
                _parent = go.GetComponent<CoroutineHolder>();
                _parent.StateMachine = this;
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
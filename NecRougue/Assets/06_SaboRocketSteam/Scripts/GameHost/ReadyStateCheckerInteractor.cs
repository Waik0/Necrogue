using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
/// <summary>
/// Clientが全員readyか確認する
/// </summary>
public class ReadyStateCheckerInteractor
{
    private Dictionary<string, bool> _state = new Dictionary<string, bool>();

    public bool IsReadyAll()
    {
        return _state.All(_ => _.Value);
    }
    public void Init()
    {
        _state.Clear();
    }

    public void AllFalse()
    {
        foreach (var stateKey in _state.Keys.ToArray())
        {
            _state[stateKey] = false;
        }
    }
    public void AddReady(string player, bool flag)
    {
        Debug.Log("AddReady");
        if (_state.ContainsKey(player))
        {
            Debug.Log("ChangeState");
            _state[player] = flag;
        }
    }
    public void SetPlayers(List<string> list)
    {
        foreach (var s in list)
        {
            if(!_state.ContainsKey(s)) 
                _state.Add(s,false);
        }
    }
}

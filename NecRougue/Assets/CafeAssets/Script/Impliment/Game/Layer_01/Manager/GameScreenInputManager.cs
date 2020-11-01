using System;
using System.Collections;
using System.Collections.Generic;
using CafeAssets.Script.Interface.Layer_01.Manager;
using CafeAssets.Script.Model;
using UnityEngine;
using Zenject;




/// <summary>
/// ボタン以外の入力を管理
/// </summary>
public class GameScreenInputManager : IGameInputManager
{
    private List<IGameScreenInputReceiver> _gameInputReceivables = new List<IGameScreenInputReceiver>();
    public GameScreenInputManager(
        [InjectOptional]
        List<IGameScreenInputReceiver> gameInputReceivables
        )
    {
        if(gameInputReceivables != null)
            _gameInputReceivables = gameInputReceivables;
    }
    public void Add(IGameScreenInputReceiver element)
    {
        _gameInputReceivables.Add(element);
    }

    public void RemoveNull()
    {
        _gameInputReceivables.RemoveAll(_ => _ == null);
    }

    public void Dispose()
    {
        _gameInputReceivables.Clear();
    }
    /// <summary>
    /// ボタン以外の点でインプットがあったら発火
    /// </summary>
    /// <param name="model"></param>
    public void InputOnGame(GameInputModel model)
    {
        foreach (var gameInputReceivable in _gameInputReceivables)
        {
            gameInputReceivable?.GameInput(model);
        }
    }
}


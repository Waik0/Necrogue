using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Zenject;

public interface IGameScreenInputReceiver
{
    void GameInput(GameInputModel model);
}

public interface IGameInputManager
{
    void InputOnGame(GameInputModel model);
}
/// <summary>
/// ボタン以外の入力を管理
/// </summary>
public class GameScreenInputManager : IGameInputManager,IManager<IGameScreenInputReceiver>,IDisposable
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

public enum GameInputState
{
    PointerDown,
    Drug,
    PointerUp
}
public class GameInputModel
{
    public Vector3 CurrentPos;
    public Vector3 WorldCurrentPos;
    public Vector3 Delta;
    public Vector3 WorldDelta;
    public Vector3 DownPos;
    public bool IsPlaceTileMode;
    public GameInputState State;
}
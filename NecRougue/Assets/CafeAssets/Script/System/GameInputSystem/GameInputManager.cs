using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Zenject;

public interface IGameInputReceivable
{
    void GameInput(GameInputModel model);
}

public interface IGameInputManager
{
    void InputOnGame(GameInputModel model);
}
public class GameInputManager : IGameInputManager,IManager<IGameInputReceivable>,IDisposable
{
    private List<IGameInputReceivable> _gameInputReceivables = new List<IGameInputReceivable>();
    public GameInputManager(
        [InjectOptional]
        List<IGameInputReceivable> gameInputReceivables
        )
    {
        if(gameInputReceivables != null)
            _gameInputReceivables = gameInputReceivables;
    }
    public void Add(IGameInputReceivable element)
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
    public Vector2 Pos;
    public Vector2 Delta;
    public bool IsPlaceTileMode;
    public GameInputState State;
}
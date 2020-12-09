using System;
using System.Collections;
using System.Collections.Generic;
using CafeAssets.Script.System.GameCoreSystem;
using CafeAssets.Script.System.GameTimeSystem;
using UnityEngine;
using Zenject;

/// <summary>
/// Layer : 200
/// ゲームの各コンポーネントを接続する。
/// </summary>
public interface IGameFacade
{
    void Load();
    void Save();
}
public class GameFacade : MonoBehaviour, IGameFacade
{
    private INpcRegistry _npcRegistry;
    private IGameTimeUseCase _gameTimeUseCase;
    private INpcSpawnTimingController _spawnTimingController;
    [Inject]
    void Inject(
        INpcRegistry npcRegistry,
        IGameTimeUseCase gameTimeUseCase,
        INpcSpawnTimingController spawnTimingController
        )
    {
        _npcRegistry = npcRegistry;
        _gameTimeUseCase = gameTimeUseCase;
        _spawnTimingController = spawnTimingController;
        Debug.Log(gameTimeUseCase);
    }

    private void Awake()
    {
        Application.targetFrameRate = 60;
    }

    void Update()
    {
        _gameTimeUseCase.Tick();
        _npcRegistry.Update();
        _spawnTimingController.DebugSpawn();
    }
    public void Load()
    {
        throw new System.NotImplementedException();
    }

    public void Save()
    {
        throw new System.NotImplementedException();
    }
    
}

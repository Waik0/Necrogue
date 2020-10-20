﻿using System;
using System.Collections.Generic;
using CafeAssets.Script.System.GameMapSystem;
using CafeAssets.Script.System.GameParameterSystem;
using CafeAssets.Script.System.GameTimeSystem;
using UnityEngine;
using UnityEngine.Events;
using Zenject;

namespace CafeAssets.Script.System.GameCoreSystem
{
    public interface IGameUseCase
    {
        //セーブデータ
        void LoadData(string path);
        void Reset();
        void Tick();

    }
    
    public class GameUseCase : IGameUseCase,IDisposable
    {
        private IGameTimeManager _gameTimeUseCase;
        private IGameResettableManager _gameResettable;

        public GameUseCase(
            IGameResettableManager gameResettableManager,
            IGameTimeManager gameTimeUseCase)
        {
            _gameResettable = gameResettableManager;
            _gameTimeUseCase = gameTimeUseCase;
        }
        

        /// <summary>
        /// セーブデータよみこみ
        /// </summary>
        /// <param name="path"></param>
        public void LoadData(string path)
        {
        
        }

        public void Reset()
        {
            _gameResettable?.ResetOnGame();
        }
        

        public void Dispose()
        {
        }

        public void Tick()
        {
            _gameTimeUseCase.Tick();
        }
    }
}

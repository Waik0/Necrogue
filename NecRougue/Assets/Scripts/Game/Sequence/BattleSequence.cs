using System;
using System.Collections;
using System.Collections.Generic;
using Toast;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;
using Zenject;


public enum BattleResult
{
    None,
    Win,
    Lose,
}
public class BattleSequence : SequenceBehaviour<BattleResult>
{
    public enum State
    {
        Init,//内部的初期化
        Setup,//対面する演出
        DeckPrepare,
        Summon,
        Resolve,//実際の処理は別クラスで管理
        End
    }
    //private
    private BattleResult _result = BattleResult.None;
    private Statemachine<State> _statemachine;
    //todo DI
    private BattleDataUseCase _battleDataUseCase = new BattleDataUseCase();
    private PlayerDataUseCase _playerDataUseCase;
    private EnemyDataUseCase _enemyDataUseCase;
    private BattlePresenter _battlePresenter = new BattlePresenter();
    private BattleDeckEditSequence _battleDeckEditSequence = new BattleDeckEditSequence();
    private BattleProcessSequence _battleProcess = new BattleProcessSequence();

    
    public void Inject(PlayerDataUseCase playerDataUseCase,EnemyDataUseCase enemyDataUseCase)
    {
        _playerDataUseCase = playerDataUseCase;
        _enemyDataUseCase = enemyDataUseCase;
        _battlePresenter.Inject(_battleDataUseCase);
        _battleProcess.Inject(_battleDataUseCase);
        _battleDeckEditSequence.Inject(_battleDataUseCase,_battlePresenter);
        //イベント接続
        _battleProcess.OnCommand.RemoveAllListeners();
        _battleProcess.OnCommand.AddListener(_battlePresenter.OnCommand);
    }
    
 
    //todo デッキ変換
    //todo マスター対応
    /// <summary>
    /// バトルの結果を返す
    /// </summary>
    /// <returns></returns>
    public BattlePlayerData GetResultPlayer()
    {
        if (_statemachine.Current != State.End)
        {
            return null;
        }
        return _battleDataUseCase.GetOperationPlayer();
    }
    public override BattleResult UpdateSequence()
    {
        _statemachine.Update();
        _battlePresenter.UpdateCommandProcess();
        return _result;
    }
    private void Awake()
    {
        _statemachine = new Statemachine<State>();
        _statemachine.Init(this);
    }
    public override void ResetSequence()
    {
        _statemachine.Next(State.Init);
        _battleDataUseCase.ResetData();
        _battlePresenter.Reset();
        _battleProcess.ResetSequence();
        _result = BattleResult.None;
    }
    //todo UI初期化
    IEnumerator Init()
    {
        DebugLog.Function(this,1);
        //_decide =
        //_order = 
       
        while (_playerDataUseCase == null ||
               _enemyDataUseCase == null)
        {
            yield return null;
        }
        _statemachine.Next(State.Setup);
    }

    IEnumerator Setup()
    {
        DebugLog.Function(this,1);
        _battleDataUseCase.SetPlayer(new BattlePlayerData().Generate(_playerDataUseCase.Data));
        _battleDataUseCase.SetEnemyFromMaster(_enemyDataUseCase.Data.Id);
        _battlePresenter.Reset();
        _statemachine.Next(State.DeckPrepare);
        yield return null;
    }
    //todo デッキ変換
    IEnumerator DeckPrepare()
    {

        DebugLog.Function(this,1);
        _battleDeckEditSequence.ResetSequence();
        while (_battleDeckEditSequence.UpdateSequence())
        {
            yield return null;
        }
        _statemachine.Next(State.Resolve);
    }

    IEnumerator Resolve()
    {
        _battleProcess.ResetSequence();
        DebugLog.Function(this,1);
        while (_battleProcess.UpdateSequence())
        {
            yield return null;
        }

        while (!_battlePresenter.IsEndTurn())
        {
            yield return null;
        }
        if (_battleDataUseCase.IsEndBattle())
        {
            _statemachine.Next(State.End);
        }
        else
        {
            _statemachine.Next(State.DeckPrepare);
        }
       
        //todo 
        // if (endProcess)
        // {
        //     if (_battleDataUseCase..IsEnd)
        //     {
        //         _statemachine.Next(State.End);
        //     }
        //     else
        //     {
        //         _statemachine.Next(State.DeckPrepare);
        //     }
        //
        // }
    }

    IEnumerator End()
    {
        DebugLog.Function(this,1);
        Debug.Log(_battleDataUseCase.Winner());
        switch (_battleDataUseCase.Winner())
        {
            case PlayerType.None:
                break;
            case PlayerType.Player:
                _result = BattleResult.Win;
                break;
            case PlayerType.Enemy:
                _result = BattleResult.Lose;
                break;
            default:
                throw new ArgumentOutOfRangeException();
        }
       
        yield return null;
    }
    #if DEBUG
    public void DebugUI()
    {
        GUILayout.Label("[ BATTLE ] STATE : "+ _statemachine.Current.ToString());
        switch (_statemachine.Current)
        {
            case State.Init:
                break;
            case State.Setup:
                break;
            case State.DeckPrepare:
                
                _battlePresenter.DebugUI();
                _battleDeckEditSequence.DebugUI();
                break;
            
            case State.Resolve:
                _battleProcess.DebugUI();
                _battlePresenter.DebugUI();
                break;
            case State.End:
                break;
            default:
                throw new ArgumentOutOfRangeException();
        }
        
    }
    #endif

}

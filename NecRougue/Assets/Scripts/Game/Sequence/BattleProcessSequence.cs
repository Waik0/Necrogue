using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Toast;
using UnityEngine;
using UnityEngine.Events;
using Random = UnityEngine.Random;

/// <summary>
/// バトル計算 1ターンごと
/// </summary>
public class BattleProcessSequence : Sequence<bool>
{
    //----------------------------------------------------------------------------------------------------------------------
    // Enum,イベント用クラス定義
    //----------------------------------------------------------------------------------------------------------------------
    public enum State
    {
        Init,
        TurnStart,

        //loop
        StepStart,
        ConfirmAttacker,
        ConfirmTarget,
        Attack,
        CheckAndIncrement,
        End
    }
    public class CommandEvent : UnityEvent<BattleCommand>{ }
    //----------------------------------------------------------------------------------------------------------------------
    // Event
    //----------------------------------------------------------------------------------------------------------------------

    //演出用のイベント
    public CommandEvent OnCommand = new CommandEvent();

    //----------------------------------------------------------------------------------------------------------------------
    // フィールド
    //----------------------------------------------------------------------------------------------------------------------

    
    private Statemachine<State> _statemachine;
    private BattleDataUseCase _battleDataUseCase;

    //private int _defender;
    //----------------------------------------------------------------------------------------------------------------------
    // パブリックメソッド
    //----------------------------------------------------------------------------------------------------------------------

    public BattleProcessSequence()
    {
        _statemachine = new Statemachine<State>();
        _statemachine.Init(this);
    }
    public void Inject(BattleDataUseCase battleDataUseCase)
    {
        _battleDataUseCase = battleDataUseCase;
    }
    public override void ResetSequence()
    {
        _statemachine.Next(State.Init);
    }

    public override bool UpdateSequence()
    {
        _statemachine.Update();
        return _statemachine.Current != State.End;
        throw new NotImplementedException();
    }
    //----------------------------------------------------------------------------------------------------------------------
    // シーケンス
    //----------------------------------------------------------------------------------------------------------------------

    IEnumerator Init()
    {
        DebugLog.Function(this,2);
        while (_battleDataUseCase == null)
        {
            yield return null;
        }
      
        
        _statemachine.Next(State.TurnStart);
        yield return null;
    }
    
    
    IEnumerator TurnStart()
    {
        DebugLog.Function(this,2);
        _battleDataUseCase.ChangeState(BattleState.TurnStart);
        OnCommand.Invoke(new BattleCommand().Generate(_battleDataUseCase.GetSnapShot()));

        if (_battleDataUseCase.IsFirstTurn())
        {
            ResolveAbilityAllCard(AbilityTimingType.BattleStart);
            //_battleDataUseCase.ChangeState(BattleState.TurnStart);
        }
        _battleDataUseCase.SortPlayerOrderAndConfirmFirstAttacker();
        _battleDataUseCase.ResetAttackIndex();
        ResolveAbilityAllCard(AbilityTimingType.TurnStart);
        //_battleDataUseCase.ChangeState(BattleState.TurnStart);
        _statemachine.Next(State.StepStart);
        yield return null;
    }
    IEnumerator StepStart()
    {
        DebugLog.Function(this,2);
        _statemachine.Next(State.ConfirmAttacker);
        yield return null;
    }
    //攻守を決める
    IEnumerator ConfirmAttacker()
    {
        DebugLog.Function(this,2);
        //死亡チェック
        _battleDataUseCase.IncrementOrChangeIfAttackerIsDead();
        if (_battleDataUseCase.IsAllAttackEnd())
        {
            _statemachine.Next(State.CheckAndIncrement);
            yield return null;
        }
        ResolveAbilityAllCard(AbilityTimingType.ConfirmAttacker);
        _statemachine.Next(State.ConfirmTarget);
        yield return null;
    }

    
    IEnumerator ConfirmTarget()
    {
        DebugLog.Function(this,2);
        
        //ターゲット選出
        var selection = _battleDataUseCase.ConfirmTarget();
        if (selection == false)
        {
            //選出時に全滅していたら勝負あり
            _statemachine.Next(State.CheckAndIncrement);
            yield break;
        }
        ResolveAbility(AbilityTimingType.ConfirmTargetAttack,
            _battleDataUseCase.AttackerPlayerIndex(),
            _battleDataUseCase.AttackerDeckIndex());
        ResolveAbility(AbilityTimingType.ConfirmTargetDefence,
            _battleDataUseCase.DefenderPlayerIndex(),
            _battleDataUseCase.DefenderDeckIndex());
        _statemachine.Next(State.Attack);
        yield return null;
    }
    IEnumerator Attack()
    {
        DebugLog.Function(this,2);
        _battleDataUseCase.ChangeState(BattleState.Attack);
        _battleDataUseCase.Attack(_battleDataUseCase.DefenderDeckIndex());
        OnCommand.Invoke(new BattleCommand().Generate(_battleDataUseCase.GetSnapShot()));
        ResolveAbility(AbilityTimingType.Attack,
            _battleDataUseCase.AttackerPlayerIndex(),
            _battleDataUseCase.AttackerDeckIndex());
        ResolveAbility(AbilityTimingType.Defence,
            _battleDataUseCase.DefenderPlayerIndex(),
            _battleDataUseCase.DefenderDeckIndex());
        _statemachine.Next(State.CheckAndIncrement);

        yield return null;
    }

    IEnumerator CheckAndIncrement()
    {
        DebugLog.Function(this,2);
        var end = false;

        //全滅判定
        if (_battleDataUseCase.IsDefenderAllDead())
        {
            end = true;
            _battleDataUseCase.ConfirmWinner(_battleDataUseCase.Attacker().PlayerType);
            
        }
        //攻撃者変更
        _battleDataUseCase.IncrementAndChangeAttacker();

        //お互い攻撃終了でターンエンド
        if (_battleDataUseCase.IsAllAttackEnd())
        {
            end = true;
            
        }

        if (end)
        {
            _battleDataUseCase.ChangeState(BattleState.TurnEnd);
            //_battleDataUseCase.ResetStatus();
            OnCommand.Invoke(new BattleCommand().Generate(_battleDataUseCase.GetSnapShot()));
            _statemachine.Next(State.End);
            yield return null;
        }
       
        _statemachine.Next(State.StepStart);
        yield return null;
    }

    private void ResolveAbilityAllCard(AbilityTimingType timingType)
    {
        _battleDataUseCase.ChangeState(BattleState.Ability);
        _battleDataUseCase.ResolveAbilityAll(
            timingType,
            ss =>
                OnCommand.Invoke(new BattleCommand().Generate(ss)));
    }
    private void ResolveAbility(AbilityTimingType timingType,int type,int index)
    {
        _battleDataUseCase.ChangeState(BattleState.Ability);
        _battleDataUseCase.ResolveAbilityAll(
            timingType,
            ss =>
                OnCommand.Invoke(new BattleCommand().Generate(ss)),
            type,
            index);
    }
    
  
    // public IEnumerator Calc()
    // {
    //     while (_statemachine.Current != State.End)
    //     {
    //          _statemachine.Update();
    //          yield return null;
    //     }
    // }
    #if DEBUG
    public void DebugUI()
    {
        //GUILayout.Label("Process State : "+ _statemachine.Current);
    }
    #endif

}
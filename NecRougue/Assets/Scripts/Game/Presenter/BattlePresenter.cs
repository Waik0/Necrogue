using System;
using System.Collections;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using UnityEngine;

public class BattlePresenter 
{


    //todo DI
    private BattleDataUseCase _battleDataUseCase;
    private Queue<BattleCommand> _battleCommandQueue;
    private bool _endTurn = false;
    private bool _endAnimation = false;
    private IEnumerator _processQueue;
    private BattleViewAnimator _battleViewAnimator;

    public void Inject(BattleDataUseCase battleDataUseCase)
    {
        _battleDataUseCase = battleDataUseCase;
    }

    public BattlePresenter()
    {
        Reset();
    }
    public void Reset()
    {
        _battleViewAnimator = new BattleViewAnimator();
        _battleCommandQueue = new Queue<BattleCommand>();
        _endTurn = false;
        _endAnimation = false;
        _battleCommandQueue.Clear();
    }
     public void UpdateCommandProcess()
     {
         if (_processQueue == null)
         {
             _processQueue = ProcessQueue();
         }
    
         var isContinue = _processQueue.MoveNext();
         if (!isContinue)
         {
             _processQueue = null;
         }
    }

    public IEnumerator ProcessQueue()
    {
        var end = false;



        while (!end)
        {
            while (_battleCommandQueue.Count == 0)
            {
                yield return null;
            }

            var command = _battleCommandQueue.Dequeue();
            _battleViewAnimator.SetSnapShot(command.SnapShot);
            while (!_battleViewAnimator.Update())
            {
                yield return null;
            }

            if (command.SnapShot.State == BattleState.TurnEnd)
            {
                end = true;
            }
        }

        _endAnimation = true;
    }


    public void OnCommand(BattleCommand command)
    {
        _battleCommandQueue.Enqueue(command);
    }

    public bool IsEndBattle()
    {
        return _battleDataUseCase.IsEndBattle();
    }

    private class BattleViewAnimator
    {
        private BattleData _snapShot; 
        private bool _isEnd = false;
        public void SetSnapShot(BattleData ss)
        {
            _isEnd = false;
            _snapShot = ss;
        }



        public bool Update()
        {
            return _isEnd;
        }
#if DEBUG


        void DebugCardView(BattleCard card)
        {
            switch (card.State)
            {
                case BattleCardState.None:
                    break;
                case BattleCardState.Attack:
                    GUILayout.Label("攻撃");
                    break;
                case BattleCardState.Damage:
                    GUILayout.Label("ダメージ");
                    break;
                case BattleCardState.Ability:
                    GUILayout.Label("能力発動");
                    break;
                case BattleCardState.Dead:
                    GUILayout.Label("死亡");
                    break;
                default:
                    throw new ArgumentOutOfRangeException();
            }
        }
        public void DebugUI()
        {
            if (_snapShot == null)
            {
                return;
                
            }
            GUILayout.BeginVertical();
            GUILayout.Label(_snapShot.State.ToString());
            for (var i = 0; i < _snapShot.PlayerList.Count; i++)
            {
                var pdata = _snapShot.PlayerList[i];

                GUILayout.BeginHorizontal();
                for (int j = 0; j < pdata.Deck.Count; j++)
                { 
                    GUILayout.BeginVertical("box");
                    DebugCardView(pdata.Deck[j] );
                    GUILayout.Label(pdata.Deck[j].Id.ToString());
                    GUILayout.Label("Hp : " + pdata.Deck[j].Hp.Current.ToString());
                    GUILayout.Label("At : "+  pdata.Deck[j].Attack.Current.ToString());
                    GUILayout.EndVertical();
                }
                GUILayout.EndHorizontal();
            }

            if (_isEnd == false)
            {
                if (GUILayout.Button("Next"))
                {
                    _isEnd = true;
                }
            }
            GUILayout.EndVertical();
        }
#endif
    }
#if DEBUG

    public void DebugUI()
    {
        GUILayout.Label("CommandQueue : "+_battleCommandQueue.Count.ToString());
        _battleViewAnimator.DebugUI();
    }
    
#endif
}

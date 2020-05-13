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
        //_battleCommandQueue.Clear();
        _processQueue = null;
    }
     public void UpdateCommandProcess()
     {
         if (_processQueue == null)
         {
             _processQueue = ProcessQueue();
         }
    
         var isContinue = _processQueue.MoveNext();
   }

    public IEnumerator ProcessQueue()
    {
        DebugLog.Function(this);
        var end = false;
        _endTurn = false;


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

        _endTurn = true;
        _processQueue = null;
    }

    public bool IsEndTurn()
    {
        return _endTurn;
    }
    public void OnCommand(BattleCommand command)
    {
        _battleCommandQueue.Enqueue(command);
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
        
        int time = 0;
        void DebugCardView(BattleCard card)
        {
            switch (card.State)
            {
                case BattleCardState.None:
                    GUILayout.Label(" ");
                    break;
                case BattleCardState.Attack:
                    GUILayout.Label("<color=red>攻撃</color>");
                    break;
                case BattleCardState.Damage:
                    GUILayout.Label("<color=red>ダメージ</color>");
                    break;
                case BattleCardState.Ability:
                    GUILayout.Label("<color=yellow>能力発動</color>");
                    break;
                case BattleCardState.Dead:
                    GUILayout.Label("<color=red>死亡</color>");
                    break;
                default:
                    throw new ArgumentOutOfRangeException();
            }
        }
        public void DebugUI(bool auto = false)
        {
            var width = GUILayout.Width(Screen.width / 7);
            var height = GUILayout.Height(Screen.height / 5);
            if (_snapShot == null)
            {
                return;
                
            }
            //演出自動進行処理
            if (time <= 0 && !_isEnd)
            {//初回のみ呼ばれるはず
                switch (_snapShot.State)
                {
                    case BattleState.None:
                        break;
                    case BattleState.DeckPrepare:
                        time = 5;
                        break;
                    case BattleState.TurnStart:
                        time = 5;
                        break;
                    case BattleState.Attack:
                        time = 120;
                        break;
                    case BattleState.Ability:
                        time = 250;
                        break;
                    case BattleState.TurnEnd:
                        time = 30;
                        break;
                    case BattleState.Sell:
                        time = 2;
                        break;
                    case BattleState.End:
                        time = 5;
                        break;
                }
                //time = 100;
            }
            time--;
            //デクリメントで0以下になった時のみよばれる
            if(time <= 0)
            {
                _isEnd = true;
            }
            GUILayout.BeginVertical();
           
            for (var i = _snapShot.PlayerList.Count - 1; i >= 0; i--)
            {
                var pdata = _snapShot.PlayerList[i];
                GUILayout.Label(pdata.PlayerType.ToString());
                GUILayout.BeginHorizontal(height);
                if (pdata.Deck.Count == 0)
                {
                    GUILayout.Label(" ");
                }
                for (int j = 0; j < pdata.Deck.Count; j++)
                { 
                    GUILayout.BeginVertical("box", width, height);
                    DebugCardView(pdata.Deck[j]);
                    if(pdata.Deck[j] == null)
                    {
                        continue;
                    }
                    //GUILayout.Label(pdata.Deck[j].Id.ToString());
                    if(pdata.Deck[j].Name == null)
                    {
                        continue;
                    }
                    //Debug.Log(pdata.Deck[j].Name.ToString());
                    GUILayout.Label( pdata.Deck[j].Name.ToString() );
                    GUILayout.Label($"<color=green>H: { pdata.Deck[j].Hp.Current,-3}</color> <color=red>A: { pdata.Deck[j].Attack,-3}</color>");
                    foreach (var ability in pdata.Deck[j].AbilityList)
                    {
                        GUILayout.Label($"能力 : {ability.Name}");
                    }
                    GUILayout.EndVertical();
                }
                GUILayout.EndHorizontal();
            }

           
            GUILayout.EndVertical();
        }
        public void DebugUI2()
        {
            //var width = GUILayout.Width(Screen.width / 7);
            var height = GUILayout.Height(Screen.height / 5);
           
            if (GUILayout.Button("演出スキップ", height))
            {
                _isEnd = true;
                time = 0;
            }
        }
#endif
    }
#if DEBUG

    public void DebugUI(bool auto = false)
    {
        //GUILayout.Label("CommandQueue : "+_battleCommandQueue.Count.ToString());
        _battleViewAnimator.DebugUI(auto);
    }
    public void DebugUI2()
    {
        GUILayout.Label("演出ストック : " + _battleCommandQueue.Count.ToString());
        //GUILayout.Label("CommandQueue : "+_battleCommandQueue.Count.ToString());
        _battleViewAnimator.DebugUI2();
    }

#endif
}

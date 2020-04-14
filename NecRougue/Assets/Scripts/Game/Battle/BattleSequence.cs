using System.Collections;
using System.Collections.Generic;
using Toast;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;
using Zenject;

public class BattleSequence : SequenceBehaviour
{
    public enum State
    {
        Init,//内部的初期化
        Setup,//対面する演出
        DeckPrepare,
        Resolve,//実際の処理は別クラスで管理
        End
    }
    //inject
    //private IButtonUI _decide;
    //private IDeckUI _deckOrder;
    [SerializeField]
    private ButtonUI _decide;

    [SerializeField] private DeckOrderUI _deckOrder;
    //private
    private string _result = "";
    private Statemachine<State> _statemachine;
    private BattleData _battleData;

    // [Inject]
    // private void Inject(
    //     IButtonUI decideButton,
    //     IDeckUI deckUi)
    // {
    //     _decide = decideButton;
    //     _deckOrder = deckUi;
    // }
    //todo デッキ変換
    //todo マスター対応
    public void SetPlayer(BattlePlayerData data)
    {
        if (!_battleData.PlayerList.ContainsKey(BattleData.PlayerType.Player))
            _battleData.PlayerList.Add(BattleData.PlayerType.Player,null);
        _battleData.PlayerList[BattleData.PlayerType.Player] = data;
    }
    //todo デッキ変換
    //todo マスター対応
    public void SetEnemy(BattlePlayerData data)
    { 
        if (!_battleData.PlayerList.ContainsKey(BattleData.PlayerType.Enemy))
            _battleData.PlayerList.Add(BattleData.PlayerType.Enemy,null);
        _battleData.PlayerList[BattleData.PlayerType.Enemy] = data;
    }
    public override string UpdateSequence()
    {
        _statemachine.Update();
        return _result;
    }
    private void Awake()
    {
        _statemachine = new Statemachine<State>();
        _statemachine.Init(this);
        _battleData = new BattleData();
    }
    public override void ResetSequence()
    {
        _statemachine.Next(State.Init);
        _battleData.Reset();
    }
    //todo UI初期化
    IEnumerator Init()
    {
        //_decide =
        //_order = 
        _statemachine.Next(State.Setup);
        yield return null;
    }

    IEnumerator Setup()
    {
        _statemachine.Next(State.DeckPrepare);
        yield return null;
    }
    //todo デッキ変換
    IEnumerator DeckPrepare()
    { 
        var order = _deckOrder.GetOrder();
        if (_decide.UpdateUI())
        {
            _statemachine.Next(State.Resolve);      
        }
        yield return null;
    }

    IEnumerator Resolve()
    {
        var processor = new BattleProcess(_battleData);
        //イベント接続
        if (processor.CalcStep())
        {
            if (_battleData.IsEnd)
            {
                _statemachine.Next(State.End);   
            }
            else
            {
                _statemachine.Next(State.DeckPrepare);   
            }
            
        }
        yield return null;
    }

    #if DEBUG
    public void DebugUI()
    {
        GUILayout.Label("BATTLE");
        GUILayout.Label("STATE : "+ _statemachine.Current.ToString());
    }
    #endif

}
/// <summary>
/// バトル計算 1ターンごと
/// </summary>
public class BattleProcess
{
    public enum State
    {
        Init,
        TurnStart,
        AbilityEffectAfterTurnStart,
        //loop
        ConfirmAttacker,
        AbilityEffectAfterConfirmAttacker,
        ConfirmTarget,
        AbilityEffectAfterConfirmTarget,
        Attack,
        AbilityEffectAfterAttack,
        End
    }
    public class CardEvent : UnityEvent<BattleData.PlayerType,int> { }
    //演出用のイベント
    public UnityEvent<BattleData.PlayerType,int> OnAttack = new CardEvent();

    private Statemachine<State> _statemachine;
    private BattleData _battleDataRef;
    IEnumerator Init()
    {
        
        yield return null;
    }
    
    
    IEnumerator TurnStart()
    {
        yield return null;
    }
    IEnumerator AbilityEffectAfterTurnStart()
    {
        yield return null;
    }
    
    IEnumerator ConfirmAttacker()
    {
        yield return null;
    }
    IEnumerator AbilityEffectAfterConfirmAttacker()
    {
        yield return null;
    }
    IEnumerator ConfirmTarget()
    {
        yield return null;
    }
    IEnumerator AbilityEffectAfterConfirmTarget()
    {
        yield return null;
    }
    IEnumerator Attack()
    {
        yield return null;
    }
    IEnumerator AbilityEffectAfterAttack()
    {
        yield return null;
    }
    public BattleProcess(BattleData battleData)
    {
        _statemachine = new Statemachine<State>();
        _statemachine.Init(this);
        _battleDataRef = battleData;
    }
    /// <summary>
    /// 
    /// </summary>
    /// <param name="battleData"></param>
    /// <returns>終了(決着またはターン終了)</returns>
    public bool CalcStep()
    {
        _statemachine.Update();
        return _statemachine.Current == State.End;
    }
}
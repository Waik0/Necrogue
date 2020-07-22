using System;
using System.Collections;
using System.Collections.Generic;
using Toast;
using UnityEngine;

public class BattleDebugManager : MonoBehaviour
{
    public enum State
    {
        Init,
        Setup,
        Battle,
        End
    }
    private Statemachine<State> _statemachine;
    private BattleSequence _battleSequence = new BattleSequence();
    private PlayerDataUseCase _playerDataUseCase = new PlayerDataUseCase();
    private EnemyDataUseCase _enemyDataUseCase = new EnemyDataUseCase();
    private bool _finishSetup = false;
    void Awake()
    {
        Application.targetFrameRate = 60;
        _statemachine = new Statemachine<State>();
        _statemachine.Init(this);
        _statemachine.Next(State.Init);
    }

    void Update()
    {
        _statemachine.Update();
    }
    IEnumerator Init()
    {

        DebugLog.Function(this);
        _playerDataUseCase.MakePlayerDataFromMaster(101);
        _battleSequence.Inject(_playerDataUseCase,_enemyDataUseCase);
        _statemachine.Next(State.Setup);
        yield return null;
    }

    IEnumerator Setup()
    {
        DebugLog.Function(this);
        while (!_finishSetup)
        {
            yield return null;
        }
        _statemachine.Next(State.Battle);
        yield return null;
    }
    IEnumerator Battle()
    {
        _finishSetup = false;
        DebugLog.Function(this);
        _battleSequence.ResetSequence();
        BattleResult result = BattleResult.None;
        while (result == BattleResult.None)
        {
            result = _battleSequence.UpdateSequence();
            yield return null;
        }
        switch (result)
        {
            case BattleResult.None:
                break;
            case BattleResult.Win:
                _statemachine.Next(State.Setup);
                break;
            case BattleResult.Lose:
                _statemachine.Next(State.Setup);
                break;
        }
    }
#if DEBUG
    private Vector2 debug;
    private Vector2 logscr;
    void OnGUI()
    {
        GUI.skin.label.fontSize =  Screen.width / 64;
        GUI.skin.button.fontSize = Screen.width / 64;
        //Matrix4x4 Translation = Matrix4x4.TRS(new Vector3(0,0,0),Quaternion.identity,Vector3.one);
        //Matrix4x4 Scale = Matrix4x4.Scale(new Vector3(zoom, zoom, 1.0f));

        //GUI.matrix = Translation*Scale*Translation.inverse;
        GUILayout.BeginHorizontal(GUILayout.Width(Screen.width), GUILayout.Height(Screen.height));
        //        
        //GUILayout.BeginHorizontal(GUILayout.Height(150));
        //logscr = GUILayout.BeginScrollView(logscr,"box",GUILayout.Width(Screen.width / 8));
     
        //GUILayout.Label(GameLogger._log);

        //GUILayout.EndScrollView();
        //GUILayout.EndHorizontal();
        debug = GUILayout.BeginScrollView(debug);
        DebugUI();
        GUILayout.EndScrollView();
        GUILayout.BeginVertical("box", GUILayout.Width(Screen.width / 8));
        DebugUI2();
        GUILayout.EndVertical();
        GUILayout.EndHorizontal();
    }
    void DebugUI()
    {
        switch (_statemachine.Current)
        {
            case State.Init:
                break;
            case State.Setup:
                SetupUI();        
                break;
            case State.Battle:
                _battleSequence.DebugUI();
                break;
            case State.End:
                break;
            default:
                throw new ArgumentOutOfRangeException();
        }
    }

    void DebugUI2()
    {
        switch (_statemachine.Current)
        {
            case State.Init:
                break;
            case State.Setup:
                 SetupUI2();
                break;
            case State.Battle:
                _battleSequence.DebugUI2();
                break;
            case State.End:
                break;
            default:
                throw new ArgumentOutOfRangeException();
        }
    }

    private bool showList;
    private Vector2 scr1;
    private Vector2 scr2;
    private BattlePlayerData cache = null;
    void SetupUI()
    {
        GUILayout.Label("バトルデバッグ");
        GUILayout.BeginHorizontal();
        GUILayout.BeginVertical(GUILayout.Width(Screen.width / 2));
        
        GUILayout.Label("敵ID :" + _enemyDataUseCase.Data.Id);
        GUILayout.BeginHorizontal();
        if (cache != null)
        {
            foreach (var battleCard in cache.Deck)
            {
                GUILayout.BeginVertical("box",GUILayout.Width(Screen.width / 6));
                GUILayout.Label(battleCard.Id.ToString());
                GUILayout.Label(battleCard.Name);
                GUILayout.EndVertical();
            }
        }
        GUILayout.EndHorizontal();
        GUILayout.Label("初期手札");
        GUILayout.BeginHorizontal();
        _playerDataUseCase.Data.Stock.RemoveAll(cardData =>
        {
            GUILayout.BeginVertical("box",GUILayout.Width(Screen.width / 6));
            GUILayout.Label(cardData.Id.ToString());
            GUILayout.Label(cardData.Name);
            var remove = GUILayout.Button("削除");
            GUILayout.EndVertical();
            return remove;

        });
        
        GUILayout.EndHorizontal();
        GUILayout.Label("初期デッキ");
        GUILayout.BeginHorizontal();
        
        _playerDataUseCase.Data.Deck.RemoveAll(cardData =>
        {
            GUILayout.BeginVertical("box",GUILayout.Width(Screen.width / 6));
            GUILayout.Label(cardData.Id.ToString());
            GUILayout.Label(cardData.Name);
            var remove = GUILayout.Button("削除");
            GUILayout.EndVertical();
            return remove;

        });
        GUILayout.EndHorizontal();
        GUILayout.EndVertical();
        GUILayout.BeginVertical();
        scr1 = GUILayout.BeginScrollView(scr1,GUILayout.Height(Screen.height/2));
        foreach (var mstEnemyRecord in MasterdataManager.Records<MstEnemyRecord>())
        {
            if (GUILayout.Button(mstEnemyRecord.id.ToString()))
            {
                _enemyDataUseCase.SetEnemyId(mstEnemyRecord.id);
                cache = new BattlePlayerData().Generate(mstEnemyRecord);
            }
        }
       
        GUILayout.EndScrollView();
        scr2 = GUILayout.BeginScrollView(scr2);
        foreach (var mstMonster in MasterdataManager.Records<MstMonsterRecord>())
        {
            GUILayout.BeginHorizontal();
            GUILayout.Label($"{ mstMonster.id ,-5} : {mstMonster.name}");
            
            if (GUILayout.Button("手札に追加"))
            {
                _playerDataUseCase.AddStock(mstMonster.id);
            }
            if (GUILayout.Button("デッキに追加"))
            {
                _playerDataUseCase.AddDeck(mstMonster.id);
            }
            GUILayout.EndHorizontal();
        }
       
        GUILayout.EndScrollView();

        GUILayout.EndVertical();
        GUILayout.EndHorizontal();
    }
    void SetupUI2()
    {
        var h = GUILayout.Height(Screen.height / 5);
        if (GUILayout.Button("バトル開始", h))
        {
            _finishSetup = true;
        }
    }
    #endif
}

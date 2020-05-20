using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using Toast;
using System.Text.RegularExpressions;

public class GameSequence : MonoBehaviour
{
    public enum State
    {
        Init,
        FirstStock,
        Prepare,
        Map,
        Battle,
        Item,
        Event,
        Shop,
        Pause
    }

    //todo DI対応---
    [SerializeField] private MapSequence _mapSequence;
    private BattleSequence _battleSequence = new BattleSequence();
    private ShopSequence _shopSequence = new ShopSequence();
    private PlayerDataUseCase _playerDataUseCase = new PlayerDataUseCase();
    private MapDataUseCase _mapDataUseCase = new MapDataUseCase();
    private EnemyDataUseCase _enemyDataUseCase = new EnemyDataUseCase();
  
    //---
    private Statemachine<State> _statemachine;

    void Awake()
    {
        Application.targetFrameRate = 60;
        _statemachine = new Statemachine<State>();
        _statemachine.Init(this);
        _statemachine.Next(State.Init);
    }
    //----------------------------------------------------------------------------------------------------------------------
    //sequence
    //----------------------------------------------------------------------------------------------------------------------
    //初回に呼ばれる
    IEnumerator Init()
    {

        DebugLog.Function(this);
        _playerDataUseCase.MakePlayerDataFromMaster(101);
        _mapSequence.Inject(_mapDataUseCase);
        _battleSequence.Inject(_playerDataUseCase,_enemyDataUseCase);
        _shopSequence.Inject(_playerDataUseCase,_mapDataUseCase);
        _statemachine.Next(State.FirstStock);
        yield return null;
    }
    //初期手札を選ぶ
    IEnumerator FirstStock()
    {
        DebugLog.Function(this);
        _playerDataUseCase.AddStock(101);
        _playerDataUseCase.AddStock(101);
        _playerDataUseCase.AddStock(101);
        _playerDataUseCase.AddStock(102);
        _statemachine.Next(State.Prepare);
        yield return null;
    }
/// <summary>
/// Map切り替えのたびに呼ばれる
/// </summary>
/// <returns></returns>
    IEnumerator Prepare()
    {
        DebugLog.Function(this);
        _mapDataUseCase.CreateMap(0,50);
        _statemachine.Next(State.Map);
        yield return null;
    }
    IEnumerator Map()
    {
        DebugLog.Function(this);
        _mapSequence.ResetSequence();
        int result = -1;
        while (result < 0)
        {
            yield return null;
            result = _mapSequence.UpdateSequence();
           
        }
        _statemachine.Next(BranchFromMapResult(result));
    }

    IEnumerator Battle()
    {
        
        DebugLog.Function(this);
        _battleSequence.ResetSequence();
        BattleResult result = BattleResult.None;
        while (result == BattleResult.None)
        {
            result = _battleSequence.UpdateSequence();
            yield return null;
        }
        Debug.Log(result);
        switch (result)
        {
            case BattleResult.None:
                break;
            case BattleResult.Win:
                _mapDataUseCase.IncrementDepth();
                _statemachine.Next(State.Map);
                break;
            case BattleResult.Lose:
                break;
            default:
                throw new ArgumentOutOfRangeException();
        }
    }
    IEnumerator Shop()
    {
        _shopSequence.ResetSequence();
        while (_shopSequence.UpdateSequence())
        {
            yield return null;
        }
        _mapDataUseCase.IncrementDepth();
        _statemachine.Next(State.Map);
        yield return null;
    }
    IEnumerator Item()
    {
        _mapDataUseCase.IncrementDepth();
        _statemachine.Next(State.Map);
        yield return null;
    }
    //----------------------------------------------------------------------------------------------------------------------
    //private
    //----------------------------------------------------------------------------------------------------------------------
    void MakePlayerData()
    {
        
    }
    /// <summary>
    /// マップ選択結果によるステート分岐
    /// </summary>
    /// <param name="result"></param>
    /// <returns></returns>
    /// <exception cref="ArgumentOutOfRangeException"></exception>
    private State BranchFromMapResult(int result)
    {
        _mapDataUseCase.SetMapCurrentSerialNumber(result);
       
        switch (_mapDataUseCase.GetCurrentMapNode().MapType)
        {
            case MapType.Battle_Common:
            case MapType.Battle_Elete:
            case MapType.Battle_Necro:
            case MapType.Batlle_Boss:
                
                _enemyDataUseCase.SetEnemyId(_mapDataUseCase.GetCurrentMapEnemyId());
                return State.Battle;
            case MapType.Item:
                return State.Item;
            case MapType.Shop:
                return State.Shop;
            case MapType.Event:
                break;
            default:
                throw new ArgumentOutOfRangeException();
        }
        return State.Map;
    }
    
    // Update is called once per frame
    void Update()
    {
        _statemachine.Update();
    }


#if DEBUG


    private Vector2 debug;
    public static Vector2 logscr;
    private Font font = null;
    void OnGUI()
    {
        if (font == null)
        {
            font = Font.CreateDynamicFontFromOSFont("Courier",8);
            GUI.skin.label.font = font;
        }

        //float zoom = Screen.width / 480f;
        GUI.skin.label.fontSize =  Screen.width / 64;
        GUI.skin.button.fontSize = Screen.width / 64;
        //Matrix4x4 Translation = Matrix4x4.TRS(new Vector3(0,0,0),Quaternion.identity,Vector3.one);
        //Matrix4x4 Scale = Matrix4x4.Scale(new Vector3(zoom, zoom, 1.0f));

        //GUI.matrix = Translation*Scale*Translation.inverse;
        GUILayout.BeginHorizontal(GUILayout.Width(Screen.width), GUILayout.Height(Screen.height));
        //        
        //GUILayout.BeginHorizontal(GUILayout.Height(150));
        logscr = GUILayout.BeginScrollView(logscr,"box",GUILayout.Width(Screen.width / 8));
     
        GUILayout.Label(GameLogger._log);

        GUILayout.EndScrollView();
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
        //GUILayout.Label("[ GAME ] STATE : " +_statemachine.Current.ToString());

        switch (_statemachine.Current)
        {
            case State.Map:
                _mapSequence.DebugUI();
                break;
            case State.FirstStock:
                break;
            case State.Battle:
                _battleSequence.DebugUI();
                break;
            case State.Event:
                
                break;
            case State.Shop:
                _shopSequence.DebugUI();
                break;
            case State.Pause:
                break;
            case State.Init:
                break;
            case State.Prepare:
                break;
            case State.Item:
                break;
            default:
                throw new ArgumentOutOfRangeException();
        }

        
    }
    void DebugUI2()
    {
        GUILayout.Label($"お金 : {_playerDataUseCase.Data.Gold}");
        switch (_statemachine.Current)
        {
            case State.Map:
                break;
            case State.FirstStock:
                break;
            case State.Battle:
                _battleSequence.DebugUI2();
                break;
            case State.Event:

                break;
            case State.Shop:
                _shopSequence.DebugUI2();
                break;
            case State.Pause:
                break;
            case State.Init:
                break;
            case State.Prepare:
                break;
            case State.Item:
                break;
            default:
                throw new ArgumentOutOfRangeException();
        }
    }
#else

#endif

}
public class GameLogger
{
#if DEBUG
    public static string _log = "";
#endif
    public static void GameLog(string l)
    {
#if DEBUG
        _log += l + "\n";
        if (_log.Count(c => c == '\n') + 1 > 300)
        {
            Regex.Replace(_log, "(.*?)\n", "");
        }
        GameSequence.logscr.y = Mathf.Infinity;
#endif
    }
}

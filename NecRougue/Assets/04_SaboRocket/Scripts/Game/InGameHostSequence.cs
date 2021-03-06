using System;
using System.Collections;
using System.Collections.Generic;
using JetBrains.Annotations;
using ModestTree;
using Toast;
using UniRx;
using UnityEngine;
using Zenject;

public class InGameHostSequence : IDisposable
{
    public enum State
    {
        Init,
        GameStart,
        //loop
        WaitReady,
        NextTurn,
        WaitInput,
        CalcPhysics,
        Fall,
        CleanUp,
        GameSet,
        ToTitle,
    }
    private Statemachine<State> _statemachine;
    //receiver
    private ICursorMessageReceiver _cursorMessageReceiver;
    private IGameSequenceDataReceiver _gameSequenceDataReceiver;
    private IInputReceiver _inputReceiver;
    private ITimelineDataReceiver _timelineDataReceiver;
    private HandDataReceiver _handDataReceiver;
    //sender
    private ICursorMessageSender _cursorMessageSender;
    private IGameSequenceDataSender _gameSequenceDataSender;
    private InputSender _inputSender;
    private TimelineDataSender _timelineDataSender;
    private HandDataSender _handDataSender;

    //other
    private ITortecHostUseCaseWithWebSocket _hostUseCase;
    private PlayerDataUseCase _playerDataUseCase;
    private PlayerTurnUseCase _playerTurnUseCase;
    private CursorDataUseCase _cursorDataUseCase;
    private PhysicsUseCase _physicsUseCase;
    private PhysicsPieceRegistry _pieceRegistry;
    private ReadyStateChecker _readyStateChecker;
    private PieceViewerUseCase _pieceViewerUseCase;
    private DeckUseCase _deckUseCase;
    private HandUseCase _handUseCase;

    private List<int> _myHand = new List<int>();
    //event
    private Subject<bool> _onActiveSequence = new Subject<bool>();
    private Subject<State> _onChangeState = new Subject<State>();
    #region public

    public IObservable<bool> OnActiveSequence => _onActiveSequence;
    public IObservable<State> OnChangeState => _onChangeState;
    public State CurrentState => _statemachine.Current;
    public string TurnPlayer { get; set; }
    public void ResetSequence()
    {
        _onActiveSequence?.OnNext(true);
        NextState(State.Init);
    }

    public void StopSequence()
    {
        _onActiveSequence?.OnNext(false);
    }
    public void UpdateSequence()
    {
        _statemachine.Update();
    }

    
    #endregion

    [Inject]
    void Inject(
        ITortecHostUseCaseWithWebSocket hostUseCase,
        //receiver
        IGameSequenceDataReceiver gameSequenceDataReceiver,
        ICursorMessageReceiver cursorMessageReceiver,
        IInputReceiver inputReceiver,
        ITimelineDataReceiver timelineDataReceiver,
        //sender
        ICursorMessageSender cursorMessageSender,
        IGameSequenceDataSender gameSequenceDataSender,
        InputSender inputSender,
        TimelineDataSender timelineDataSender,
        //other
        PlayerDataUseCase playerDataUseCase,
        PlayerTurnUseCase playerTurnUseCase,
        CursorDataUseCase cursorDataUseCase,
        PhysicsUseCase physicsUseCase,
        ReadyStateChecker readyStateChecker,
        PhysicsPieceRegistry physicsPieceRegistry,
        PieceViewerUseCase pieceViewerUseCase,
        DeckUseCase deckUseCase,
        HandUseCase handUseCase,
        HandDataSender handDataSender,
        HandDataReceiver handDataReceiver
    )
    {
        _statemachine = new Statemachine<State>();
        _statemachine.Init(this);
        _hostUseCase = hostUseCase;
        _playerDataUseCase = playerDataUseCase;
        _readyStateChecker = readyStateChecker;
        _gameSequenceDataReceiver = gameSequenceDataReceiver;
        _cursorMessageReceiver = cursorMessageReceiver;
        _playerTurnUseCase = playerTurnUseCase;
        _physicsUseCase = physicsUseCase;
        _cursorMessageSender = cursorMessageSender;
        _gameSequenceDataSender = gameSequenceDataSender;
        _cursorDataUseCase = cursorDataUseCase;
        _inputSender = inputSender;
        _inputReceiver = inputReceiver;
        _pieceRegistry = physicsPieceRegistry;
        _timelineDataSender = timelineDataSender;
        _timelineDataReceiver = timelineDataReceiver;
        _pieceViewerUseCase = pieceViewerUseCase;
        _handUseCase = handUseCase;
        _deckUseCase = deckUseCase;
        _handDataReceiver = handDataReceiver;
        _handDataSender = handDataSender;
        ReceiverInit();
    }

    void ReceiverInit()
    {
        //gameSequenceDataReceiverはマッチングで初期化されている
        _gameSequenceDataReceiver.OnGetNextTurn = (t, p) =>
        {
            _playerTurnUseCase.CurrentPlayer = p;
            _playerTurnUseCase.CurrentTurn = t;
            NextState(State.WaitInput);
        };
        _cursorMessageReceiver.StartSubscribe(_hostUseCase);
        _cursorMessageReceiver.OnGetCursorData = _cursorDataUseCase.SetCursorPos;
        _inputReceiver.StartSubscribe(_hostUseCase);
        _inputReceiver.OnGetInputData = (id,data) =>
        {
            Debug.Log("GetInput");
            _handUseCase.DeleteHand(id,data.pieceId);
            _handDataSender.SendHandData(new HandData(){hand = _handUseCase.GetHand(id)},_hostUseCase);
            _pieceRegistry.Spawn(data.pieceId,data.pos,data.angle);
            NextState(State.CalcPhysics);
        };
        _timelineDataReceiver.StartSubscribe(_hostUseCase);
        _timelineDataReceiver.OnGetTimelineData = data =>
        {
            _pieceViewerUseCase.SetTimelineData(data);
        };
        _hostUseCase.OnCloseOtherCallback.Subscribe(_ => NextState(State.ToTitle));
    }
    //プレイヤーの確定とチャンネルオープン
    IEnumerator Init()
    {
        Debug.Log("StaetInitialize");
        _cursorDataUseCase.Init();
        _playerTurnUseCase.Init();
        _pieceRegistry.Init();
        _pieceViewerUseCase.Init();
        //プレイヤーの確定
        _playerDataUseCase.SetPlayerList(_hostUseCase.ConnectionIds());
        Debug.Log("Complete");
        _gameSequenceDataSender.SendReadyData(_hostUseCase);
        NextState(State.GameStart);
        yield return null;
    }
    IEnumerator GameStart()
    {
        Debug.Log("GameStart");
        _deckUseCase.InitRandom();//デッキ初期化
        _handUseCase.Init();
        _handUseCase.FirstDraw(_playerDataUseCase.Players);
     
        _cursorMessageSender.StartSendCoroutine(_hostUseCase);
        NextState(State.WaitReady);
        yield return null;
    }
    IEnumerator WaitReady()
    {
        while (!_readyStateChecker.IsReadyAll())
        {
            yield return null;
        }
        _readyStateChecker.AllFalse();
        _handDataSender.SendHandDataAll(_handUseCase.Hands,_hostUseCase);
        var pid = _playerDataUseCase.GetTurnPlayer(_playerTurnUseCase.CurrentTurn);
        _gameSequenceDataSender.SendNextTurnData(_hostUseCase,_playerTurnUseCase.CurrentTurn
            ,pid);
        var hands = _handUseCase.GetHand(pid);
        if( hands.Count <= 0 )
            NextState(State.GameSet);
    }
    IEnumerator WaitInput()
    {
        Debug.Log("WaitInput"); 
        int angle = 0;
        Debug.Log($"IAM :{_hostUseCase.SelfId} NOW : { _playerTurnUseCase.CurrentPlayer }");
        while (true)
        {
            if (_playerTurnUseCase.CurrentPlayer != _hostUseCase.SelfId)
            {
                break;
            }
            angle++;
            angle = angle % 360;
            var hand = _handUseCase.GetHand(_playerTurnUseCase.CurrentPlayer);
            if (_inputSender.TrySend(hand[0], angle, _hostUseCase))
            {
                  break;
            }
            yield return null;
        }
        yield return null;
    }

    IEnumerator CalcPhysics()
    {
        Debug.Log("Calc");
        List<TimelineData> timelineDatas = null;
        //計算
        yield return _physicsUseCase.StartCalcAwait(_pieceRegistry.PhysicsPieces, data =>
        {
            timelineDatas = data;
        },0);
        Debug.Log(timelineDatas.Count);
        //送る
        yield return _timelineDataSender.Send(timelineDatas,_hostUseCase);
        _statemachine.Next(State.Fall);
        yield return null;
    }

    IEnumerator Fall()
    {
        yield return _pieceViewerUseCase.PlayAwait(null);
        yield return null;
        _statemachine.Next(State.CleanUp);
    }

    IEnumerator CleanUp()
    {
        _playerTurnUseCase.NextTurn();
        _gameSequenceDataSender.SendReadyData(_hostUseCase);
        _statemachine.Next(State.WaitReady);
        yield return null;
    }
    IEnumerator GameSet()
    {
        _cursorMessageSender.EndSendCoroutine();
        yield return null;
    } 
    public void Dispose()
    {
        _onActiveSequence?.Dispose();
    }

    private void NextState(State state)
    {
        _statemachine.Next(state);
        _onChangeState?.OnNext(state);
    }
    
}
//webRTCベースだった時のなごり
#if false
    void ChannelCheck()
    {
        var channels = new Type[]
        {
            typeof(PlayerData),
            typeof(TimelineData),
            typeof(ObjectVertexData),
            typeof(InputData),
            typeof(CursorData),
            typeof(GameSequenceData),
        };
        OpenChannel(channels);
        var open = false;
        while (!open)
        {
            open = true;
            yield return new WaitForSecondsInStatemachine(1f);
            foreach (var channel in channels)
            {
                open &= _hostUseCase.CheckAndOpenChannelAll(channel);
                yield return null;
            }
            Debug.Log("CheckOpenChannel " + open);
        }
        Debug.Log("Complete");
    }
        void OpenChannel(Type[] types)
    {
        Debug.Log("TryOpenChannel");
        foreach (var type in types)
        {
            _hostUseCase.OpenChannelAll(type);
        }
    }

#endif
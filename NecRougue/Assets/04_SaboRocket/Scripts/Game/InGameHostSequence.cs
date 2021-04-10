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
    private RollDataReceiver _rollDataReceiver;
    //sender
    //private ICursorMessageSender _cursorMessageSender;
    private IGameSequenceDataSender _gameSequenceDataSender;
    private InputSender _inputSender;
    private TimelineDataSender _timelineDataSender;
    private HandDataSender _handDataSender;
    private DeckDataSender _deckDataSender;
    private RollDataSender _rollDataSender;
    //other
    private ITortecHostUseCaseWithWebSocket _hostUseCase;
    private PlayerDataUseCase _playerDataUseCase;
    private PlayerTurnUseCase _playerTurnUseCase;
    private CursorViewPresenter _cursorViewPresenter;
    private PhysicsUseCase _physicsUseCase;
    private PhysicsPieceRegistry _pieceRegistry;
    private ReadyStateChecker _readyStateChecker;
    private PieceViewerUseCase _pieceViewerUseCase;
    private DeckUseCase _deckUseCase;
    private HandUseCase _handUseCase;
    private RollUseCase _rollUseCase;
    private ResultUseCase _resultUseCase;
    
    private PiecePreview _preview;
    //event
    private Subject<bool> _onActiveSequence = new Subject<bool>();
    private Subject<State> _onChangeState = new Subject<State>();
    #region public

    public IObservable<bool> OnActiveSequence => _onActiveSequence;
    public IObservable<State> OnChangeState => _onChangeState;
    public State CurrentState => _statemachine.Current;
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
        HandDataReceiver handDataReceiver,
        RollDataReceiver rollDataReceiver,
        //sender
        //ICursorMessageSender cursorMessageSender,
        IGameSequenceDataSender gameSequenceDataSender,
        InputSender inputSender,
        TimelineDataSender timelineDataSender,
        DeckDataSender deckDataSender,
        HandDataSender handDataSender,
        RollDataSender rollDataSender,
        //other
        PlayerDataUseCase playerDataUseCase,
        PlayerTurnUseCase playerTurnUseCase,
        CursorViewPresenter cursorViewPresenter,
        PhysicsUseCase physicsUseCase,
        ReadyStateChecker readyStateChecker,
        PhysicsPieceRegistry physicsPieceRegistry,
        PieceViewerUseCase pieceViewerUseCase,
        DeckUseCase deckUseCase,
        HandUseCase handUseCase,
        RollUseCase rollUseCase,
        ResultUseCase resultUseCase,
        PiecePreview preview
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
        //_cursorMessageSender = cursorMessageSender;
        _gameSequenceDataSender = gameSequenceDataSender;
        _cursorViewPresenter = cursorViewPresenter;
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
        _deckDataSender = deckDataSender;
        _rollDataSender = rollDataSender;
        _rollUseCase = rollUseCase;
        _rollDataReceiver = rollDataReceiver;
        _resultUseCase = resultUseCase;
        _preview = preview;
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
       // _cursorMessageReceiver.OnGetCursorData = _cursorViewPresenter.SetCursorPos;
        _inputReceiver.StartSubscribe(_hostUseCase);
        _inputReceiver.OnGetInputData = (id,data) =>
        {
            Debug.Log("GetInput");
            //手札消す
            //すぐ引く
            _handUseCase.DeleteHand(id,data.pieceId);
            _handUseCase.TryDraw(id);
            _deckDataSender.SendDeckData(new DeckData(){deck = _deckUseCase.Deck,index = _deckUseCase.Index},_hostUseCase);
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
        //プレイヤーの確定
        _playerDataUseCase.SetPlayerList(_hostUseCase.ConnectionIds());
        //_cursorViewPresenter.Init();
        _playerTurnUseCase.Init();
        _pieceRegistry.Init();
        _pieceViewerUseCase.Init();
        _deckUseCase.InitRandom();//デッキ初期化
        _handUseCase.Init();
        _pieceRegistry.Init();
        _pieceViewerUseCase.Init();
        _rollUseCase.RandomInitRoll(_playerDataUseCase.Players);
      
        Debug.Log("Complete");
        _gameSequenceDataSender.SendReadyData(_hostUseCase);
        NextState(State.GameStart);
        yield return null;
    }
    IEnumerator GameStart()
    {

        Debug.Log("GameStart");
        //初期ドロー
        _handUseCase.FirstDraw(_playerDataUseCase.Players);
        _deckDataSender.SendDeckData(new DeckData(){deck = _deckUseCase.Deck,index = _deckUseCase.Index},_hostUseCase);
        _handDataSender.SendHandDataAll(_handUseCase.Hands,_hostUseCase);
        //_cursorMessageSender.StartSendCoroutine(_hostUseCase);
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
        if (_playerTurnUseCase.CurrentPlayer != _hostUseCase.SelfId)
        {
            yield break;
        }
        _preview.SetActive(true);
        _preview.SetPreview(_handUseCase.GetHand(_playerTurnUseCase.CurrentPlayer)[0]);
        while (true)
        {
          
            angle+=5;
            angle = angle % 360;
            var hand = _handUseCase.GetHand(_playerTurnUseCase.CurrentPlayer);
            _preview.Rotate(angle);
            _preview.SetPos(Camera.main.ScreenToWorldPoint(Input.mousePosition));
            if (_inputSender.TrySend(hand[0], angle, _hostUseCase))
            {
                _preview.SetActive(false);
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
        var isEnd = _resultUseCase.CheckIsEnd();
        if (!isEnd)
        {
            _gameSequenceDataSender.SendReadyData(_hostUseCase);
            _statemachine.Next(State.WaitReady);
        }
        else
        {
            NextState(State.GameSet);
        }
        yield return null;
    }
    IEnumerator GameSet()
    {
        //_cursorMessageSender.EndSendCoroutine();
        _readyStateChecker.AllFalse();
        _gameSequenceDataSender.SendGameOverData(_resultUseCase.CalcResult(),_hostUseCase);
        yield return null;
        NextState(State.Init);
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
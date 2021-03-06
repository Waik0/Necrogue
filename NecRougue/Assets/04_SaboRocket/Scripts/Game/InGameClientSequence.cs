using System;
using System.Collections;
using System.Collections.Generic;
using Toast;
using UniRx;
using UnityEngine;
using Zenject;

public class InGameClientSequence : IDisposable
{
    public enum State
    {
        Init,
        GameStart,
        //loop
        WaitReady,
        NextTurn,
        WaitInput,
        CalcPhysics,//結果待ち
        Fall,
        CleanUp,
        GameSet,
        ToTitle,
    }
    private Statemachine<State> _statemachine;
    //sender
    private ICursorMessageSender _cursorMessageSender;
    private InputSender _inputSender;
    private IGameSequenceDataSender _gameSequenceDataSender;
    //receiver
    private IGameSequenceDataReceiver _gameSequenceDataReceiver;
    private ICursorMessageReceiver _cursorMessageReceiver;
    private IInputReceiver _inputReceiver;
    private ITimelineDataReceiver _timelineDataReceiver;
    private HandDataReceiver _handDataReceiver;
    //other
    private ITortecClientUseCaseWithWebSocket _clientUseCase;
    private PlayerDataUseCase _playerDataUseCase;
    private PlayerTurnUseCase _playerTurnUseCase;
    private CursorDataUseCase _cursorDataUseCase;
    private PieceViewerUseCase _pieceViewerUseCase;
    private DeckUseCase _deckUseCase;
    private HandUseCase _handUseCase;
    
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
        _statemachine.Next(State.Init);
        _onChangeState?.OnNext(State.Init);
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
        ITortecClientUseCaseWithWebSocket clientUseCase,
        //receiver
        IGameSequenceDataReceiver gameSequenceDataReceiver,
        ICursorMessageReceiver cursorMessageReceiver,
        IInputReceiver inputReceiver,
        ITimelineDataReceiver timelineDataReceiver,
        //sender
        IGameSequenceDataSender gameSequenceDataSender,
        ICursorMessageSender cursorMessageSender,
        InputSender inputSender,
        //other
        PlayerDataUseCase playerDataUseCase,
        PlayerTurnUseCase playerTurnUseCase,
        CursorDataUseCase cursorDataUseCase,
        PieceViewerUseCase pieceViewerUseCase
    )
    {
        _statemachine = new Statemachine<State>();
        _statemachine.Init(this);
        _clientUseCase = clientUseCase;
        _playerDataUseCase = playerDataUseCase;
        _gameSequenceDataReceiver = gameSequenceDataReceiver;
        _cursorMessageReceiver = cursorMessageReceiver;
        _playerTurnUseCase = playerTurnUseCase;
        _cursorMessageSender = cursorMessageSender;
        _cursorDataUseCase = cursorDataUseCase;
        _inputSender = inputSender;
        _inputReceiver = inputReceiver;
        _timelineDataReceiver = timelineDataReceiver;
        _pieceViewerUseCase = pieceViewerUseCase;
        _gameSequenceDataSender = gameSequenceDataSender;
        ReceiverInit();
    }

    void ReceiverInit()
    {
        _gameSequenceDataReceiver.StartSubscribe(_clientUseCase);
        _gameSequenceDataReceiver.OnGetNextTurn = (t, p) =>
        {
            _playerTurnUseCase.CurrentPlayer = p;
            _playerTurnUseCase.CurrentTurn = t;
            NextState(State.WaitInput);
        };
        _cursorMessageReceiver.StartSubscribe(_clientUseCase);
        _cursorMessageReceiver.OnGetCursorData = _cursorDataUseCase.SetCursorPos;
        _inputReceiver.StartSubscribe(_clientUseCase);
        _inputReceiver.OnGetInputData = (id,data) =>
        {
            Debug.Log("GetInput");
            NextState(State.CalcPhysics);
        };
        _timelineDataReceiver.StartSubscribe(_clientUseCase);
        _timelineDataReceiver.OnGetTimelineData = data =>
        {
            _pieceViewerUseCase.SetTimelineData(data);
        };
        _clientUseCase.OnCloseOtherCallback.Subscribe(_ => NextState(State.ToTitle));
        _handDataReceiver?.StartSubscribe(_clientUseCase);
        _handDataReceiver.OnGetHandData = data =>
        {
            _handUseCase.SetHand(data.playerId, data.hand);
        };
    }

    //プレイヤーの確定とチャンネルオープン
    IEnumerator Init()
    {
        _cursorDataUseCase.Init();
        _playerTurnUseCase.Init();
        _pieceViewerUseCase.Init();
        _gameSequenceDataSender.SendReadyData(_clientUseCase);
        Debug.Log("Complete");
        NextState(State.GameStart);
        yield return null;
    }
    IEnumerator GameStart()
    {      
        Debug.Log("GameStart");
        _cursorMessageSender.StartSendCoroutine(_clientUseCase);
        NextState(State.WaitReady);
        yield return null;
    }
    IEnumerator WaitReady()
    {
        yield return null;
    }

    IEnumerator WaitInput()
    {
        Debug.Log("WaitInput"); 
        int angle = 0;
        Debug.Log($"IAM :{_clientUseCase.SelfId} NOW : { _playerTurnUseCase.CurrentPlayer }");
        while (true)
        {
            if (_playerTurnUseCase.CurrentPlayer != _clientUseCase.SelfId)
            {
                break;
            }
            angle++;
            angle = angle % 360;
            if (_inputSender.TrySend(_handUseCase.GetHand(_playerTurnUseCase.CurrentPlayer)[0],angle, _clientUseCase))
            {
                break;
            }
            yield return null;
        }
        yield return null;
    }

    IEnumerator CalcPhysics()
    {
        _statemachine.Next(State.Fall);
        yield return null;
    }
    IEnumerator Fall()
    {
        yield return _pieceViewerUseCase.PlayAwait(null);
        yield return null;
        _gameSequenceDataSender.SendReadyData(_clientUseCase);
        _statemachine.Next(State.WaitReady);
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
        Debug.Log("NextState : "+state);
        _statemachine.Next(state);
        _onChangeState?.OnNext(state);
    }
}

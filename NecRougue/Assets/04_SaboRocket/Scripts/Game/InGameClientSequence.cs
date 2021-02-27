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
        Game,
        ToTitle,
    }
    private Statemachine<State> _statemachine;
    private InGameUseCase _inGameUseCase;
    private ITortecClientUseCase _clientUseCase;
    private Subject<bool> _onActiveSequence = new Subject<bool>();
    private GameObject _messageSubscriber = null;
    #region public

    public IObservable<bool> OnActiveSequence => _onActiveSequence;
    public State CurrentState => _statemachine.Current;
    public void ResetSequence()
    {
        _onActiveSequence?.OnNext(true);
        _statemachine.Next(State.Init);
    }

    public void StopSequence()
    {
        _onActiveSequence?.OnNext(false);
    }
    public void UpdateSequence()
    {
        _statemachine.Update();
    }
    public void ToTitle()
    {
        _statemachine.Next(State.ToTitle);
    }
    #endregion

    [Inject]
    void Inject(
        ITortecClientUseCase clientUseCase,
        InGameUseCase inGameUseCase
    )
    {
        _clientUseCase = clientUseCase;
        _statemachine = new Statemachine<State>();
        _statemachine.Init(this);
        _inGameUseCase = inGameUseCase;
    }
    //プレイヤーの確定とチャンネルオープン
    IEnumerator Init()
    {
        //プレイヤーの確定
        _inGameUseCase.SetPlayerList(_clientUseCase.ConnectionIds());
        SubscribeMessage();
        Debug.Log("Complete");
        _statemachine.Next(State.Game);
        yield return null;
    }
    IEnumerator Game()
    {

        while (true)
        {
            _clientUseCase.BroadcastAll<CursorData>(new CursorData()
            {
                id = _clientUseCase.SelfId,
                worldPos = Camera.main.ScreenToWorldPoint(Input.mousePosition),
            });
            yield return null;
        }
    }

    void SubscribeMessage()
    {
        Debug.Log("Subscribe");
        DisposeMessage();
        _messageSubscriber = new GameObject();
        _clientUseCase.SubscribeMessage<CursorData>(_messageSubscriber,OnCursorMessage);
    }

    void OnCursorMessage(string cid ,CursorData cursorData)
    {
        _inGameUseCase.SetCursorPos(cursorData);
    }
    void DisposeMessage()
    {
        if (_messageSubscriber != null)
        {
            GameObject.Destroy(_messageSubscriber);
            _messageSubscriber = null;
        }
    }
    public void Dispose()
    {
        _onActiveSequence?.Dispose();
    }
}

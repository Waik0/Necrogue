using System;
using System.Collections;
using System.Collections.Generic;
using Toast;
using UniRx;
using UnityEngine;
using Zenject;

public class InGameHostSequence : IDisposable
{
    public enum State
    {
        Init,
        Game,
        ToTitle,
    }
    private Statemachine<State> _statemachine;
    private InGameUseCase _inGameUseCase;
    private ITortecHostUseCase _hostUseCase;
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
        ITortecHostUseCase hostUseCase,
        InGameUseCase inGameUseCase
    )
    {
        _hostUseCase = hostUseCase;
        _statemachine = new Statemachine<State>();
        _statemachine.Init(this);
        _inGameUseCase = inGameUseCase;
    }
    //プレイヤーの確定とチャンネルオープン
    IEnumerator Init()
    {
        Debug.Log("StaetInitialize");
        //プレイヤーの確定
        _inGameUseCase.SetPlayerList(_hostUseCase.ConnectionIds());
        var channels = new Type[]
        {
            typeof(PlayerData),
            typeof(TimelineData),
            typeof(ObjectVertexData),
            typeof(InputData),
            typeof(CursorData),
            typeof(GameSequenceData),
        };
        SubscribeMessage();
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
        _statemachine.Next(State.Game);
    }

    IEnumerator Game()
    {

        while (true)
        {
            if (Input.GetMouseButton(0))
            {
                _hostUseCase.BroadcastAll<CursorData>(new CursorData()
                {
                    id = _hostUseCase.SelfId,
                    worldPos = Camera.main.ScreenToWorldPoint(Input.mousePosition),
                });
            }

            yield return null;
        }
    }
    void OpenChannel(Type[] types)
    {
        Debug.Log("TryOpenChannel");
        foreach (var type in types)
        {
            _hostUseCase.OpenChannelAll(type);
        }
    }

    void SubscribeMessage()
    {
        Debug.Log("Subscribe");
        DisposeMessage();
        _messageSubscriber = new GameObject();
        _hostUseCase.SubscribeMessage<CursorData>(_messageSubscriber,OnCursorMessage);
        _hostUseCase.SubscribeMessage<PlayerData>(_messageSubscriber,OnPlayerDataMessage);
    }

    void OnPlayerDataMessage(string cid, PlayerData data)
    {
        _inGameUseCase.SetPlayerData(data);
        //リレー
        _hostUseCase.BroadcastAll<CursorData>(new CursorData()
        {
            id = _hostUseCase.SelfId,
            worldPos = Camera.main.ScreenToWorldPoint(Input.mousePosition),
        });
    }
    void OnCursorMessage(string cid ,CursorData cursorData)
    {
        _inGameUseCase.SetCursorPos(cursorData);
        //リレー
        _hostUseCase.BroadcastAll(cursorData);
        
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

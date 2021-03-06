using System.Collections;
using System.Collections.Generic;
using UniRx;
using UnityEngine;
using Zenject;

public class GameStartDataReceiverClient : MonoBehaviour
{
    private MatchingClientSequence _matchingClientSequence;
    private ITortecClientUseCaseWithWebSocket _clientUseCase;
    private GameObject _disposable;
    [Inject]
    void Inject(MatchingClientSequence matchingClientSequence,
        ITortecClientUseCaseWithWebSocket clientUseCase)
    {
        _matchingClientSequence = matchingClientSequence;
        _clientUseCase = clientUseCase;
        _matchingClientSequence.OnActiveSequence.Subscribe(a =>
        {
            if (a) StartSubscribe();
            else EndSubscribe();
        }).AddTo(this);
    }

    void StartSubscribe()
    {
        EndSubscribe();
        _disposable = new GameObject();
        _clientUseCase.SubscribeMessage<GameStartData>(_disposable,OnReceiveGameStart);
    }
    private void EndSubscribe()
    {
        if (_disposable)
        {
            Destroy(_disposable);
        }
    }

    void OnReceiveGameStart(string id, GameStartData gameStartData)
    {
        _matchingClientSequence.ToGame();
    }
}

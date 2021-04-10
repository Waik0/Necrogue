using System;
using System.Collections;
using SaboRocketSteam.Scripts.GameClient.Sequence.States;
using UnityEngine;
using Zenject;


public class GameClientSequence : MonoBehaviour
{
    [Inject] private Init _init;
    [Inject] private WaitReadyAll _waitReady; 
    [Inject] private WaitInputClient _waitInput;
    [Inject] private PieceAnimation _pieceAnimation;
    [Inject] private Reload _reload;
    private StateMachine _stateMachine;
    void Awake()
    {
        _stateMachine = new StateMachine();
        _stateMachine.Add(_init);
        _stateMachine.Add(_waitReady);
        _stateMachine.Add(_waitInput);
        _stateMachine.Add(_pieceAnimation);
        _stateMachine.Add(_reload);
        _stateMachine.Next<Init>();
    }
}



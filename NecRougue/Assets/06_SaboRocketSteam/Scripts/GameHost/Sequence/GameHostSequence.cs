using System;
using System.Collections;
using SaboRocketSteam.Scripts.GameHost.States;
using UnityEngine;
using Zenject;


public class GameHostSequence : MonoBehaviour
{
    [Inject] private Init _init;
    [Inject] private WaitReady _waitReady; 
    [Inject] private WaitInput _waitInput;
    [Inject] private CalcPhysics _calcPhysics;
    private StateMachine _stateMachine;
    void Awake()
    {
        _stateMachine = new StateMachine();
        _stateMachine.Add(_init);
        _stateMachine.Add(_waitReady);
        _stateMachine.Add(_waitInput);
        _stateMachine.Add(_calcPhysics);
        _stateMachine.Next<Init>();
    }
}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameItgSequenceInitiator : MonoBehaviour
{
    private GameSequence _gameSequence = new GameSequence();
    void Awake()
    {
        Application.targetFrameRate = 60;
        _gameSequence.Inject();
        _gameSequence.ResetSequence();
    }

    void Update()
    {
        _gameSequence.UpdateSequence();
    }
    #if DEBUG
    void OnGUI()
    {
        GUI.skin.label.fontSize =  Screen.width / 64;
        GUI.skin.button.fontSize = Screen.width / 64;
        _gameSequence.DebugUI();
    }
    #endif
}

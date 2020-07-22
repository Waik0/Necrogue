using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

public class GameView : MonoBehaviour
{
    [SerializeField] private DeckUI _deckUi;
    public DeckUI DeckUI => _deckUi;
    [SerializeField] private Button _endTurnButton;
    public Button EndTurnButton => _endTurnButton;
}

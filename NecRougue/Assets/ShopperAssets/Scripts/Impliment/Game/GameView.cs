using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

public class GameView : MonoBehaviour
{
    [SerializeField] private DeckUI _deckUi;
    [SerializeField] private ShopUI _shopUi;
    [SerializeField] private PlayerUI _playerUi;
    [SerializeField] private Button _endTurnButton;
    [SerializeField] private CardIconUI _cardIconUi;
    public DeckUI DeckUI => _deckUi;
    public ShopUI ShopUI => _shopUi;
    public PlayerUI PlayerUI => _playerUi;
    public CardIconUI CardIconUI => _cardIconUi;
    public Button EndTurnButton => _endTurnButton;
}

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
    [SerializeField] private Button _endSelectButton;
    [SerializeField] private Button _cancelButton;
    [SerializeField] private CardIconUI _cardIconUi;
    [SerializeField] private EnemyUI _enemyUi;
   
    public DeckUI DeckUI => _deckUi;
    public ShopUI ShopUI => _shopUi;
    public PlayerUI PlayerUI => _playerUi;
    public CardIconUI CardIconUI => _cardIconUi;
    public Button EndTurnButton => _endTurnButton;
    public Button EndSelectButton => _endSelectButton;
    public Button CancelButton => _cancelButton;
    public EnemyUI EnemyUI => _enemyUi;

    public void Awake()
    {
        //GetComponent<Canvas>().worldCamera = Camera.main;
    }
    public void ResetUI()
    {
        ShopUI.ResetUI();
        EnemyUI.ResetUI();
        PlayerUI.ResetUI();
        DeckUI.ResetUI();
     
    }
}

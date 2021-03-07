using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Zenject;

public class DeckView : MonoBehaviour
{
    [SerializeField] private Text _deck;
    private DeckUseCase _deckUseCase;
    [Inject]
    void Inject(
        DeckUseCase deckUseCase
    )
    {
        _deckUseCase = deckUseCase;
        _deckUseCase.OnUpdateDeck = OnUpdateDeck;
    }
    void OnUpdateDeck(List<int> deck,int index)
    {
        _deck.text = "";
        foreach (var i in deck)
        {
            _deck.text += i + ",";
        }

            
    }
}

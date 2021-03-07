using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Zenject;

public class HandView : MonoBehaviour
{
    [SerializeField] private Text _cards;
    private HandUseCase _handUseCase;
    [Inject]
    void Inject(
        HandUseCase handUseCase
    )
    {
        _handUseCase = handUseCase;
        _handUseCase.OnUpdateHand = OnUpdateHand;
    }

    void OnUpdateHand(Dictionary<string,List<int>> hands)
    {
        _cards.text = "";
        foreach (var keyValuePair in hands)
        {
            _cards.text += keyValuePair.Key + " : ";
            foreach (var x1 in keyValuePair.Value)
            {
                _cards.text += x1 + ",";
            }
            _cards.text += "\n";
        }
    }
}

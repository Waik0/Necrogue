using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Zenject;

public class RollView : MonoBehaviour
{
        [SerializeField] private Text _roll;
        private RollUseCase _rollUseCase;
        [Inject]
        void Inject(
            RollUseCase rollUseCase
        )
        {
            _rollUseCase = rollUseCase;
            _rollUseCase.OnUpdateRoll = OnUpdateDeck;
        }
        void OnUpdateDeck(Dictionary<string, RollData.Roll> dic)
        {
            _roll.text = "";
            foreach (var i in dic)
            {
                _roll.text += $"{i.Key} = {i.Value}\n";
            }
        }
}

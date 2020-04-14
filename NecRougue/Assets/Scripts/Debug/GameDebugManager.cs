using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameDebugManager : MonoBehaviour
{
    [SerializeField] private GameSequence _gameSequence;

    void Start()
    {
        _gameSequence.SetData(new DataController()
        {
            PlayerData = new PlayerData()
            {
                CharaId = 1,
                Deck = new DeckData()
                {
                    CardList = new List<int>(){ 1, 2, }
                }
            }
        });
    }
}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerTurnUseCase 
{
    public int CurrentTurn { get; set; }
    public string CurrentPlayer { get; set; }

    public void Init()
    {
        CurrentTurn = 0;
        CurrentPlayer = "";
    }

    public void NextTurn()
    {
        CurrentTurn++;
    }
}

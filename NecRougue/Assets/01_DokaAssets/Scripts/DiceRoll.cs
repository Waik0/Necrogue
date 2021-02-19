using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface IDiceRoll
{
    void Roll(int diceNum, Action<DiceResult> result);
}

public class DiceResult
{
    public int DiceTotal;
    public List<int> Dice;
}
public class DiceRollHost : IDiceRoll
{
    public void Roll(int diceNum, Action<DiceResult> result)
    {
        
    }
}

public class DiceRollClient : IDiceRoll
{
    public void Roll(int diceNum, Action<DiceResult> result)
    {
        
    }
}
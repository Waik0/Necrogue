using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DataController
{
    public PlayerData PlayerData;

    public void ResetData()
    {
        PlayerData = new PlayerData();
    }
}

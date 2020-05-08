﻿using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[Serializable,MasterPath("Master/mst_character.json")]
public class MstCharacterRecord : IMasterRecord
{
    public int Id
    {
        get => id;
    }

    public int id;
    public string name;
    public string description;
    public string imagePath;
    public int MonsterId1;
    public int MonsterId2;
    public int MonsterId3;
    public int MonsterId4;
    public int MonsterId5;
    public int MonsterId6;
    public int MonsterId7;
    public int MonsterId8;

}

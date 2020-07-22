using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[Serializable,MasterPath("Master/mst_drama.json")]
public class MstDramaRecord : IMasterRecord
{
    public int Id
    {
        get => id;
    }

    public int id;
    public string picturePath;
    public int seriese;
    public string message;
}

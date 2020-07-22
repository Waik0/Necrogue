using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
//種族
[MasterPath("Master/mst_race.json"), Serializable]
public class MstRaceRecord : IMasterRecord
{ 
    public int Id => id;
    public int id;
    public string name;
    
}

using System;
using System.Collections;
using System.Collections.Generic;
using UnityEditor.iOS.Xcode;
using UnityEngine;

[MasterPath("Master/mst_mapnode.json")]
[Serializable]
public class MstMapNodeRecord : IMasterRecord
{
    public int Id
    {
        get => id;
    }

    public int id;
    public int minDepth;
    public int maxDepth;
    public int enemyId;
    public MapType type;
    public MapOption option1;
    public MapOption option2;
    public MapOption option3;
}

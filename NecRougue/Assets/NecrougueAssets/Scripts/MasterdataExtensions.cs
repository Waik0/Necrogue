using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public static class MstMapNodeRecordExtensions
{
    public static MapNode Convert(this MstMapNodeRecord record)
    {
        return new MapNode()
        {
            NodeId = record.id,
            SerialNumber = 0,
            Depth = 0,
            MapType = record.type,
            MapOption = new[]
            {
                record.option1,
                record.option2,
                record.option3
            }.Aggregate((i, o) => i | o),
            EnemyId = record.enemyId
        };
    }
} 
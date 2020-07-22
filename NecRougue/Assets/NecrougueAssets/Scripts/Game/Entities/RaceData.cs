using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public class RaceData : IEntity,
    IConvertFromMasterRecord<MstRaceRecord>,
    IGenerateFromMasterRecord<RaceData,MstRaceRecord>
{
    public int Id;
    public string Name;
    public void Convert(MstRaceRecord record)
    {
        Id = record.Id;
        Name = record.name;
    }

    public RaceData Generate(MstRaceRecord record)
    {
        Convert(record);
        return this;
    }
}

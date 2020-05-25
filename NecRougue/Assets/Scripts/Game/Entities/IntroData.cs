using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class IntroData : IEntity,
    IConvertFromMasterRecord<MstDramaRecord>,
    IGenerateFromMasterRecord<IntroData,MstDramaRecord>
{
    public int Id;
    public string PicturePath;
    public int Seriese;
    public string Message;
    public void Convert(MstDramaRecord record)
    {
        Id = record.id;
        PicturePath = record.picturePath;
        Seriese = record.seriese;
        Message = record.message;
    }

    public IntroData Generate(MstDramaRecord record)
    {
        Convert(record);
        return this;
    }
}

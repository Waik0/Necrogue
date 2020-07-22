using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class IntroDataUseCase : IEntityUseCase<IntroData>
{
    private IntroData _data;

    public IntroData Data
    {
        get => _data;
    }

    public bool SetData(int s,int index)
    {
        //todo Master
        var drama = MasterdataManager.Records<MstDramaRecord>().Where(_ => _.seriese == s).ToList();
        if (drama.Count <= index)
        {
            return false;
        }

        var sort = drama.OrderBy(_ => _.id).ToList();
        _data = new IntroData().Generate(sort[index]);
        return true;
    }
    public void ResetData()
    {
        _data = new IntroData();
    }
}

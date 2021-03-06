using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PhysicsUseCase : MonoBehaviour
{
    private Coroutine _coroutine;
    private PhysicsPieceRegistry _physicsPieceRegistry;
    private PieceDatas _pieceDatas;
    public void StartCalc(List<PhysicsPiece> pieces,Action<List<TimelineData>> response,int firstFrame)
    {
        if (_coroutine != null)
        {
            StopCoroutine(_coroutine);
        }
        _coroutine = StartCoroutine(CalcPhysics(pieces,response,firstFrame));
    }

    public IEnumerator StartCalcAwait(List<PhysicsPiece> pieces, Action<List<TimelineData>> response, int firstFrame)
    {
        List<TimelineData> timelineDatas = null;
        yield return CalcPhysics(pieces, data =>
        {
            timelineDatas = data;
        },firstFrame);
        while (timelineDatas == null)
        {
            yield return null;
        }
        response?.Invoke(timelineDatas);
    }
    IEnumerator CalcPhysics(List<PhysicsPiece> pieces,Action<List<TimelineData>> res,int firstFrame)
    {
        var maxTime = .08f;
        var start = Time.realtimeSinceStartup;
        var timeline = new List<TimelineData>();
        var count = firstFrame;
        var maxFrame = firstFrame + 200;
        Debug.Log("StartCalc");
        while (count <= maxFrame)
        {
            
            Physics2D.Simulate(.1f);
            Physics2D.Simulate(.1f);
            timeline.Add(AddTimeLine(pieces, count, maxFrame));
            if (Time.realtimeSinceStartup - start > maxTime)
            {
                start = Time.realtimeSinceStartup;
                yield return null;
            }
            count++;
        }
        res(timeline);
        yield return null;
    }

    TimelineData AddTimeLine(List<PhysicsPiece> pieces,int frame,int maxframe)
    {
        var timeLine = new TimelineData();
        timeLine.timeLineId = frame;
//        Debug.Log( $" {frame} {maxframe} {frame >= maxframe}");
        timeLine.isLast = frame >= maxframe;
        timeLine.objects = new List<ObjectVertexData>();
        foreach (var physicsPiece in pieces)
        {
            timeLine.objects.Add(new ObjectVertexData()
            {
                uniqueId = physicsPiece.Id,
                pieceId = physicsPiece.PieceId,
                angle = physicsPiece.Angle,
                pos = physicsPiece.Pos,
            });
        }

        return timeLine;
    }
}

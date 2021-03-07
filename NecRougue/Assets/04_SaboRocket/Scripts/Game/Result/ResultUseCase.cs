using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ResultUseCase : MonoBehaviour
{
    public Action<ResultData> OnResult;
    //到達チェック
    public bool CheckIsEnd()
    {
        var origin = new Vector2(-500,-700);
        var direction = new Vector2(1, 0);
        var result = new ResultData()
        {
            winRoll = RollData.Roll.Blue
        };
        for (int i = 0; i < 1700; i++)
        {
            RaycastHit2D hit = Physics2D.Raycast(origin, direction, 1000);
            var isHit = hit.collider;
            //衝突時のRayを画面に表示
            if (hit.collider){
                Debug.DrawRay(origin, hit.point - origin, Color.blue, 3, false);
            }
            //非衝突時のRayを画面に表示
            else{
                Debug.DrawRay(origin, direction * 1000, Color.green, 3, false);
                return false;
            }

            origin.y += 1;
        }
        SetResult(result);
        return true;
    }
    public ResultData CalcResult()
    {
        var origin = new Vector2(-500,-700);
        var direction = new Vector2(1, 0);
        var result = new ResultData()
        {
            winRoll = RollData.Roll.Blue
        };
        for (int i = 0; i < 1700; i++)
        {
            RaycastHit2D hit = Physics2D.Raycast(origin, direction, 1000);
            var isHit = hit.collider;
            //衝突時のRayを画面に表示
            if (hit.collider){
                Debug.DrawRay(origin, hit.point - origin, Color.blue, 3, false);
            }
            //非衝突時のRayを画面に表示
            else{
                Debug.DrawRay(origin, direction * 1000, Color.green, 3, false);
                result.winRoll = RollData.Roll.Red;
            }

            origin.y += 1;
        }
        SetResult(result);
        return result;
    }

    public void SetResult(ResultData resultData)
    {
        OnResult?.Invoke(resultData);
    }
}
[Serializable]
public class ResultData
{
    public RollData.Roll winRoll;
}
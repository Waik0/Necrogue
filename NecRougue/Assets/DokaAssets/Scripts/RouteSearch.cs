using System;
using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;

public class RouteSearch
{

    public List<int> Result = new List<int>();

    public List<IStationModel> ResultToStation(IStationModel[] map)
    {
        var list = new List<IStationModel>();
        foreach (var r in Result)
        {
            list.Add(map[r]);
        }

        return list;
    }

    /// <summary>
    /// いけますよ
    /// </summary>
    /// <param name="pos">位置</param>
    /// <param name="num">さいころの目</param>
    /// <returns></returns>

    public IEnumerator Search(Dictionary<Vector2Int,IStationModel> map, Vector2Int pos, int diceNum,Action<Vector2Int[]> result)
    {
        Result.Clear();
        
        var list = new List<(Vector2Int before, List<Vector2Int> list)>() {(Vector2Int.zero, new List<Vector2Int>() {pos})};
        var isFinished = false;
        Vector2Int[] r = null; 
        Task.Run(() =>
        {
            r = SearchAsync(map, list, diceNum);
            isFinished = true;
        });
        while (!isFinished)
        {
            yield return null;
        }

        result(r);

        yield return null;
    }

    private Vector2Int[] SearchAsync(Dictionary<Vector2Int,IStationModel> map, List<(Vector2Int before, List<Vector2Int> list)> list, int num)
    {

        while (num >= 0)
        {
            var nextList = new List<(Vector2Int before, List<Vector2Int> list)>();
            foreach (var l in list)
            {
                Debug.Log("l : " + l.before);
                foreach (var l2 in l.list)
                {
                    nextList.Add(CanReach(map, l2, num, l.before));
                }

            }

            list.Clear();
            list = nextList;
            num = num - 1;

        }

        var result = new List<Vector2Int>();
        foreach (var l in list)
        {
            result.Add(l.before);
        }
        return result.ToArray();
    }

    private (Vector2Int before, List<Vector2Int> list) CanReach(Dictionary<Vector2Int,IStationModel>  map, Vector2Int startIdx, int length, Vector2Int before)
    {
        if (length < 0)
        {
            return (startIdx, new List<Vector2Int>() {startIdx});
        }

        Debug.Log("s :" + startIdx + ", rem" + length);
        var list = new List<Vector2Int>();
        foreach (var r in map[startIdx].Relation)
        {
            if (r != before)
            {
                list.Add(r);
            }

        }

        return (startIdx, list);
    }

    public void Search(bool[,] passable, int diceNum, Action<Vector2Int[]> getList)
    {

    }
}


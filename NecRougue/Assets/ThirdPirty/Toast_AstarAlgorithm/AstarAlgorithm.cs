#define NOT_ASYNC
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.Tilemaps;

/// <summary>
/// todo アルゴリズムが完成したら、NodeDataの削減を行う。
/// 削減対象
///  通路のノード(部屋の中点以外)、
///  ノード間のつながり
///  
/// </summary>
public class AstarAlgorithm
{
    public class AstarNode
    {
        public int Step;
        public float Distance;
        public float Weight;
        public Vector2Int Prev;
        public bool IsAlreadyPassed;
        public bool Passable;
    }
    public AstarNode[,] Field { get; private set; }
    public bool IsFinish { get; private set; }

    public bool IsReady { get; private set; } = true;
    //
    public void Reset()
    {
        foreach (var astarNode in Field)
        {
            astarNode.IsAlreadyPassed = false;
        }
    }

    public void SetField(AstarNode[,] f)
    {
        Field = f;
    }

    /// 探索
    /// <summary>
    /// </summary>
    public void Search(Vector2Int start,Vector2Int goal)
    {
        if (IsReady)
        {
            IsFinish = false;
            IsReady = false;
            StartSearch(start, goal);
        }
    }

    void StartSearch(Vector2Int start, Vector2Int goal)
    {
        var candidate = new List<Vector2Int>();
#if NOT_ASYNC
        Debug.Log("Start");
        var time = DateTime.Now;
        SearchRecursive(start, goal, candidate, true);
        Debug.Log($"Finish {(DateTime.Now - time).TotalSeconds}sec");
        IsFinish = true;
        IsReady = true;
#else
        Task.Run( async () =>
        {
            
            Debug.Log("Start");
            var time = DateTime.Now;
            await SearchRecursive(start, goal, candidate,true);
            Debug.Log($"Finish {(DateTime.Now - time).TotalSeconds}sec");
            IsFinish = true;
            IsReady = true;
        });
#endif
    }
    async Task SearchRecursive(Vector2Int start, Vector2Int goal, List<Vector2Int> candidate,bool isEntry)
    {
        //for (var i = 0; i < Field.GetLength(0); i++)
        //{
        //    for (var j = 0; j < Field.GetLength(1); j++)
        //    {

        //    }
        //}
        //初回は0でも許可
        if ((candidate.Count == 0 && !isEntry) || start == goal)
        {
            return;
        }
        candidate.AddRange(SearchCandidatesSurroundings(start.x,start.y,goal));
        var next = DecideCandidates(candidate,goal);
        candidate.Remove(next);
        OpenNode(start);//通過済みに
        await SearchRecursive(next, goal, candidate,false);

        //while ()
        //{

        //}
    }

    Vector2Int DecideCandidates(List<Vector2Int> candidate,Vector2Int goal)
    {
        foreach (var vector2Int in candidate)
        {
            CalcWeight(vector2Int, goal);
            //OpenNode(new Vector2Int(x + i, y + j));
        }

        var min = candidate.OrderBy(_ => Field[_.x, _.y].Weight).First();
        return min;

    }
    List<Vector2Int> SearchCandidatesSurroundings(int x,int y ,Vector2Int goal)
    {
        var candidate = new List<Vector2Int>();
        for (var i = -1; i < 2; i++)
        {
            for (var j = -1; j < 2; j++)
            {
                if (i == 0 && j == 0)
                {
                    continue;
                }

                if (!CheckSelectable(new Vector2Int(x + i, y + j)))
                {
                    continue;
                }
                candidate.Add(new Vector2Int(x + i, y + j));
            }
        }

        return candidate;

    }

    bool CheckSelectable(Vector2Int now)
    {
        return Field[now.x, now.y].Passable && !Field[now.x, now.y].IsAlreadyPassed;
    }
    /// <summary>
    /// 
    /// </summary>
    /// <returns></returns>
    void CalcWeight(Vector2Int now,Vector2Int end)
    {
        //var Field[now.x, now.y].Distance
        Field[now.x, now.y].Distance = (end - now).magnitude;
        Field[now.x, now.y].Weight = Field[now.x, now.y].Distance;
    }

    void OpenNode(Vector2Int now)
    {
        Field[now.x, now.y].IsAlreadyPassed = true;
    }
    

}

public class AstarAlgorithmForTilemap
{

    public Tilemap Field { get; private set; }
    public bool IsFinish { get; private set; }

    public bool IsReady { get; private set; } = true;

    public Vector2Int[] Result => _result.ToArray();

    private List<Vector2Int> _result = new List<Vector2Int>();
    //
    public void Reset()
    {
        foreach (var astarNode in Field.GetTilesBlock(Field.cellBounds))
        {
            var at =  astarNode as AstarNodeTile;
            if (at == null) continue;
            at.IsAlreadyPassed = false;
        }
        _result.Clear();
    }

    public void SetField(Tilemap f)
    {
        Field = f;
    }

    /// 探索
    /// <summary>
    /// </summary>
    public void Search(Vector3Int start,Vector3Int goal)
    {
        if (IsReady)
        {
            IsFinish = false;
            IsReady = false;
            StartSearch(start, goal);
        }
    }

    void StartSearch(Vector3Int start, Vector3Int goal)
    {
        var candidate = new List<AstarNodeTile>();
        Debug.Log("Start");
        //var time = DateTime.Now;
        SearchRecursive(new Vector2Int(start.x,start.y),new Vector2Int(goal.x,goal.y), candidate, true);
        //Debug.Log($"Finish {(DateTime.Now - time).TotalSeconds}sec");
        IsFinish = true;
        IsReady = true;
    }
    async Task SearchRecursive(Vector2Int start, Vector2Int goal, List<AstarNodeTile> candidate,bool isEntry)
    {
        //for (var i = 0; i < Field.GetLength(0); i++)
        //{
        //    for (var j = 0; j < Field.GetLength(1); j++)
        //    {

        //    }
        //}
        //初回は0でも許可
        if ((candidate.Count == 0 && !isEntry) || start == goal)
        {
            return;
        }
        
        candidate.AddRange(SearchCandidatesSurroundings(start.x,start.y,goal));
        var next = DecideCandidates(candidate,goal);
        candidate.Remove(next);
        OpenNode(start);//通過済みに
        _result.Add(next.Pos);
        await SearchRecursive(next.Pos, goal, candidate,false);

        //while ()
        //{

        //}
    }

    AstarNodeTile DecideCandidates(List<AstarNodeTile> candidate,Vector2Int goal)
    {
        foreach (var at in candidate)
        {
            if(at == null) continue;
            CalcWeight(at,at.Pos, goal);
            //OpenNode(new Vector2Int(x + i, y + j));
        }

        var min = candidate.OrderBy(_ => _.Weight).First();
        return min;

    }
    List<AstarNodeTile> SearchCandidatesSurroundings(int x,int y ,Vector2Int goal)
    {
        var candidate = new List<AstarNodeTile>();
        for (var i = -1; i < 2; i++)
        {
            for (var j = -1; j < 2; j++)
            {
                if (i == 0 && j == 0)
                {
                    continue;
                }
                var at = Field.GetTile<AstarNodeTile>(new Vector3Int(x + i, y + j, 0));
                //通過したまたは通過不可能なら次
                if (!CheckSelectable(at,new Vector2Int(x + i, y + j)))
                {
                    continue;
                }
                at.Pos = new Vector2Int(x + i, y + j);
                candidate.Add(at);
            }
        }

        return candidate;

    }

    bool CheckSelectable(AstarNodeTile at,Vector2Int now)
    {
        return  at != null && at.Passable && ! at.IsAlreadyPassed;
    }
    /// <summary>
    /// 
    /// </summary>
    /// <returns></returns>
    void CalcWeight(AstarNodeTile nowTile,Vector2Int now,Vector2Int end)
    {
        //var Field[now.x, now.y].Distance
        nowTile.Distance = (end - now).magnitude;
        nowTile.Weight = nowTile.Distance;
    }

    void OpenNode(Vector2Int now)
    {
        var at = Field.GetTile<AstarNodeTile>(new Vector3Int(now.x,now.y, 0));
        if (at == null)
        {
            return;
        }
        at.IsAlreadyPassed = true;
    }
    

}
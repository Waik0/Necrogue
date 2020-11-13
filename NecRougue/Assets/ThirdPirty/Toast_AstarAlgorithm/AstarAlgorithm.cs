#define NOT_ASYNC
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.Tilemaps;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
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
        public float Distance;//推定コスト
        public float Weight;//実コスト
        public AstarNode Prev;
        public Vector2Int PrevPos;
        public Vector2Int Pos;
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
            if (astarNode == null) continue;
            astarNode.IsAlreadyPassed = false;
            astarNode.Distance = 0;
            astarNode.Weight = 0;
            astarNode.Prev = null;
            astarNode.PrevPos = Vector2Int.zero;
        }
    }

    public void SetField(AstarNode[,] f)
    {
        Field = f;
    }

    /// 探索
    /// <summary>
    /// </summary>
 
    
    List<Vector2Int> SearchCandidatesSurroundings(int x,int y )
    {
        var candidate = new List<Vector2Int>();
        
        for (var i = -1; i < 2; i++)
        {
            for (var j = -1; j < 2; j++)
            {
                if ((i == 0 && j == 0)
                    // (i == -1 && j == -1) ||
                    // (i == 1 && j == 1) ||
                    // (i == -1 && j == 1) ||
                    // (i == 1 && j == -1)
                    )
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
        Debug.Log("Can : "+candidate.Count);
        return candidate;

    }

    bool CheckSelectable(Vector2Int now)
    {

        return now.x >= 0 &&
               now.y >= 0 &&
               now.x < Field.GetLength(0) &&
               now.y < Field.GetLength(1) && 
               Field[now.x, now.y] != null &&
               Field[now.x, now.y].Passable &&
               !Field[now.x, now.y].IsAlreadyPassed;
    }
    /// <summary>
    /// 
    /// </summary>
    /// <returns></returns>
    void CalcWeight(Vector2Int now,AstarNode parent,Vector2Int parentPos,Vector2Int end)
    {
        //var Field[now.x, now.y].Distance
        Field[now.x, now.y].Distance = (end - now).magnitude;
        Field[now.x, now.y].Weight = Field[now.x, now.y].Distance;
        //Field[now.x, now.y].Step = 0;
        if (parent != null)
        {
            Field[now.x, now.y].Weight += parent.Weight;
            Field[now.x, now.y].Prev = parent;
            Field[now.x, now.y].PrevPos = parentPos;
            //Field[now.x, now.y].Step += parent.Step + 1;
        }
        
    }
    

    public Stack<Vector2Int> FindPath(Vector2Int start, Vector2Int end,Vector2Int modify)
    {

      
        List<Vector2Int> OpenList = new List<Vector2Int>();
        List<Vector2Int> ClosedList = new List<Vector2Int>();
        List<Vector2Int> adjacencies;
        Vector2Int current = start;

        // add start node to Open List
        OpenList.Add(start);

        while (OpenList.Count != 0 && !ClosedList.Exists(x => x == end))
        {
            Debug.Log(OpenList.Count);
            current = OpenList[0];
            OpenList.Remove(current);
            ClosedList.Add(current);
            adjacencies = SearchCandidatesSurroundings(current.x,current.y);


            foreach (Vector2Int n in adjacencies)
            {
                if (!ClosedList.Contains(n))
                {
                    if (!OpenList.Contains(n))
                    {
                        CalcWeight(n,Field[current.x, current.y],current,end);
                        OpenList.Add(n);
                    }
                }
            }
        }

        // construct path, if end was not closed return null
        if (!ClosedList.Exists(x => x == end))
        {
            Debug.LogError("CannnotReach");
            return null;
        }

        // if all good, return path
        if (!ClosedList.Contains(current))
        {
            Debug.LogError("CannnotReach 2");
            return null;
        }
        Vector2Int temp = ClosedList[ClosedList.IndexOf(current)];
        Stack<Vector2Int> Path = new Stack<Vector2Int>();
        do
        {
            Path.Push(temp+modify);
            temp = Field[temp.x,temp.y].PrevPos;
        } while (temp != start && Field[temp.x,temp.y].Prev != null);

        return Path;
    }
    

}

//別のやり方
#if false
namespace AStarSharp
{
    public class Node
    {
        // Change this depending on what the desired size is for each element in the grid
    
        public Node Parent;
        public Vector2Int Position;
  
        public float DistanceToTarget;
        public float Cost;
        public float Weight;
        public float F
        {
            get
            {
                if (DistanceToTarget != -1 && Cost != -1)
                    return DistanceToTarget + Cost;
                else
                    return -1;
            }
        }
        public bool Walkable;

        public Node(Vector2Int pos, bool walkable, float weight = 1)
        {
            Parent = null;
            Position = pos;
            DistanceToTarget = -1;
            Cost = 1;
            Weight = weight;
            Walkable = walkable;
        }
    }

    public class Astar
    {
        List<List<Node>> Grid;
        int GridRows
        {
            get
            {
               return Grid[0].Count;
            }
        }
        int GridCols
        {
            get
            {
                return Grid.Count;
            }
        }

        public Astar(List<List<Node>> grid)
        {
            Grid = grid;
        }

        public Stack<Node> FindPath(Vector2Int Start, Vector2Int End)
        {
            Node start = new Node(Start, true);
            Node end = new Node(End, true);

            Stack<Node> Path = new Stack<Node>();
            List<Node> OpenList = new List<Node>();
            List<Node> ClosedList = new List<Node>();
            List<Node> adjacencies;
            Node current = start;
           
            // add start node to Open List
            OpenList.Add(start);

            while(OpenList.Count != 0 && !ClosedList.Exists(x => x.Position == end.Position))
            {
                current = OpenList[0];
                OpenList.Remove(current);
                ClosedList.Add(current);
                adjacencies = GetAdjacentNodes(current);

 
                foreach(Node n in adjacencies)
                {
                    if (!ClosedList.Contains(n) && n.Walkable)
                    {
                        if (!OpenList.Contains(n))
                        {
                            n.Parent = current;
                            n.DistanceToTarget = (n.Position - end.Position).sqrMagnitude;//Math.Abs(n.Position.x - end.Position.x) + Math.Abs(n.Position.Y - end.Position.Y);
                            n.Cost = n.Weight + n.Parent.Cost;
                            OpenList.Add(n);
                            OpenList = OpenList.OrderBy(node => node.F).ToList<Node>();
                        }
                    }
                }
            }
            
            // construct path, if end was not closed return null
            if(!ClosedList.Exists(x => x.Position == end.Position))
            {
                return null;
            }

            // if all good, return path
            Node temp = ClosedList[ClosedList.IndexOf(current)];
            if (temp == null) return null;
            do
            {
                Path.Push(temp);
                temp = temp.Parent;
            } while (temp != start && temp != null) ;
            return Path;
        }
		
        private List<Node> GetAdjacentNodes(Node n)
        {
            List<Node> temp = new List<Node>();

            int row = (int)n.Position.y;
            int col = (int)n.Position.x;

            if(row + 1 < GridRows)
            {
                temp.Add(Grid[col][row + 1]);
            }
            if(row - 1 >= 0)
            {
                temp.Add(Grid[col][row - 1]);
            }
            if(col - 1 >= 0)
            {
                temp.Add(Grid[col - 1][row]);
            }
            if(col + 1 < GridCols)
            {
                temp.Add(Grid[col + 1][row]);
            }

            return temp;
        }
    }
}
#endif
/// <summary>
/// 仕組み的に不可能だった
/// </summary>
// [Obsolete("仕組み的に不可能",true)]
/*
public class AstarAlgorithmForTilemap
{

    private Tilemap Field { get; set; }
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
                //at.Pos = new Vector2Int(x + i, y + j);
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
*/
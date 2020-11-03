using System.Collections.Generic;
/*
using Toast;
using Toast.Rg;
using UnityEngine;

public class DebugAStarAlgorithm : MonoBehaviour
{
    private AstarAlgorithm AStar = new AstarAlgorithm();
    private AstarAlgorithm.AstarNode[,] _nodes;

    private Vector2Int _startCache = Vector2Int.zero;
    private bool _isStarted = false;
    private Vector2Int _endCache = Vector2Int.zero;
    // Start is called before the first frame update
    void Start()
    {
        KeepDevGeneratorHandler.Instance.StartCreate(new KeepDevGenerator.InputData()
        {
            CreateRoomData = new List<KeepDevGenerator.CreateRoomData>()
            {
               new KeepDevGenerator.CreateRoomData(){Id = 1,Size = new Vector2Int(5,5)},
               new KeepDevGenerator.CreateRoomData(){Id = 1,Size = new Vector2Int(5,5)},
               new KeepDevGenerator.CreateRoomData(){Id = 1,Size = new Vector2Int(5,5)},
            },
            MaxSize = 120,
            Padding = 2,
            Radius = 5,
            WallThick = 2,
        });
    }

    // Update is called once per frame
    void Update()
    {
        if (!KeepDevGeneratorHandler.Instance.IsFinished)
        {
            return;
        }

        if (_nodes == null)
        {
            GetData();
        }

        SetStartAndEnd();
        StartAStar();
        DrawNodes();
        DrawStartAndEnd();
    }

    void StartAStar()
    {
        if (!_isStarted)
        {
            _isStarted = true;
            AStar.SetField(_nodes);
            AStar.Search(_startCache, _endCache);
        }
    }

    void SetStartAndEnd()
    {
        if (_nodes == null)
        {
            return;
        }
        if (_startCache != Vector2.zero && _endCache != Vector2.zero)
        {
            return;

        }
        var w = _nodes.GetLength(0);
        var h = _nodes.GetLength(1);
        var list = new List<(int i,int j)>();
        for (int i = 0; i < w; i++)
        {
            for (int j = 0; j < h; j++)
            {
                if (_nodes[i, j].Passable == true)
                {
                list.Add((i,j));
                }
            }
        }

        var result = Shuffle.RandomUniqueNunbers(0, list.Count, 2);
        _startCache = new Vector2Int(list[result[0]].i, list[result[0]].j);
        _endCache = new Vector2Int(list[result[1]].i, list[result[1]].j);
    }

    void DrawStartAndEnd()
    {

    }
    void DrawNodes()
    {
        if (_nodes == null)
        {
            return;
        }
        var w = _nodes.GetLength(0);
        var h = _nodes.GetLength(1);
        var min = Vector2.zero;
        for (int i = 0; i < w; i++)
        {
            for (int j = 0; j < h; j++)
            {
                if (_nodes[i,j].Passable == true)
                {
                    if (min == Vector2.zero)
                    {
                        min = new Vector2(i,j);
                    }
                    var pos = new Vector2(i,j);
                    DebugDraw.Rect(new Rect(pos - Vector2.one,Vector2.one * 2),_nodes[i,j].IsAlreadyPassed ? "O" : "");
                }
            }
        }

        DebugDraw.Translate(min);
    }
    void GetData()
    {
        var record = Masterdata.Table<MstFloorRecord>().Get(1);
        _nodes = KeepDevGeneratorHandler.Instance.GetResult().GetResultToFloorData(record).FloorDataToAstarNodes();
    }
}
*/
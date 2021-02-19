using System;
using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.Tilemaps;

//名前nodeでもよかったけど抽象的すぎるのでより詳細なStationにした
//あとIEnumeratorでも処理が重いとき止めるができることが分かった。
public interface IStationModel
{
    Vector2Int[] Relation { get; } 
    int Id { get; }
}
public interface IMapGenerator
{
    Dictionary<Vector2Int,IStationModel> GeneratedMap { get; }
    /// <summary>
    /// 生成
    /// </summary>
    /// <param name="seed"></param>
    void Generate(int seed);
    
    IEnumerator GenerateAsync(uint seed);
}

/// <summary>
/// ゲーム開始前のマップ自動生成
/// </summary>
public class MapGenerator : MonoBehaviour,IMapGenerator
{
    /// <summary>
    /// すごろくのマス
    /// </summary>
    [Serializable]
    public class StationModel : IStationModel
    {
        public Vector2Int[] Relation { get; set; }
        public int Id { get; set; }
    }
    private  Dictionary<Vector2Int,IStationModel> _generatedMap;
    [SerializeField] private Tilemap _tilemap;
    [SerializeField] private MapModel _mapModel;
    public Dictionary<Vector2Int,IStationModel> GeneratedMap => _generatedMap;
    /// <summary>
    /// マップ生成
    /// </summary>
    /// <param name="seed"></param>
    public void Generate(int seed)
    {
        throw new NotImplementedException();
    }

    public IEnumerator GenerateAsync(uint seed)
    {
        var size = new Vector2Int(100, 100);
        //↓これコルーチンでも行ける
        var task = GenerateDataAsync(seed,size);
        while (!task.IsCompleted)
        {
            yield return null;
        }
        _generatedMap = task.Result;
        yield return GenerateTile(_generatedMap);
    }
    //非同期マップ生成
    private async Task<Dictionary<Vector2Int, IStationModel>> GenerateDataAsync(uint seed,Vector2Int size)
    {
        return await Task.Run( () => GenerateData(seed,size,100));
    }
    private Dictionary<Vector2Int,IStationModel> GenerateData(uint seed,Vector2Int size,int maxStationNum)
    {
        Debug.Log("Start Generate Map");
        Unity.Mathematics.Random random = new Unity.Mathematics.Random(seed);

        Dictionary<Vector2Int,IStationModel> generatedMap = new Dictionary<Vector2Int, IStationModel>();
        var passable = new bool[size.x,size.y];
        var ids = new int[size.x, size.y];
        //柱を立てる
        for (var i = 0; i < size.x; i++)
        {
            for (var j = 0; j < size.y; j++)
            {
                passable[i, j] = false;
                ids[i, j] = 0;
                if (i == 0 || j == 0 || i == size.x - 1 || j == size.y - 1)
                {
                    continue;
                }
                if (i % 2 == 0 || j % 2 == 0)
                {
                    passable[i, j] = true;
                    ids[i, j] = random.NextInt(0,2);
                }
            }
        }
        //StationModelに変換
        for (var i = 0; i < size.x; i++)
        {
            for (var j = 0; j < size.y; j++)
            {
                if (passable[i, j])
                {
                    var station = new StationModel()
                    {
                        Id = ids[i, j]
                    };
                    generatedMap.Add(new Vector2Int(i, j), station);
                }
            }
        }
        Debug.Log("Relation");
        //Relationを保存
        foreach (var keyValuePair in generatedMap)
        {
            var pos = keyValuePair.Key;
            var rel = new List<Vector2Int>();
            var checkList = new[]
            {
                pos + new Vector2Int(1, 0),
                pos + new Vector2Int(0, -1),
                pos + new Vector2Int(-1, 0),
                pos + new Vector2Int(0, 1),
            };
            foreach (var vector2Int in checkList)
            {
                if (generatedMap.ContainsKey(vector2Int)) rel.Add(vector2Int);
            }
            ((StationModel) generatedMap[pos]).Relation = rel.ToArray();
        }
        Debug.Log("End Generate Map");
        return generatedMap;
      
    }

    private IEnumerator GenerateTile( Dictionary<Vector2Int,IStationModel> data)
    {
        Debug.Log("Start Generate Tile");
        var maxTime = .08f;
        var startTime = Time.realtimeSinceStartup;
        foreach (var keyValuePair in data)
        {
            
            var id = keyValuePair.Value.Id;
            _tilemap.SetTile(
                new Vector3Int(keyValuePair.Key.x,keyValuePair.Key.y,0),
                _mapModel.GetTile(id));
            var timeCheck = Time.realtimeSinceStartup - startTime;
            //Debug.Log(timeCheck);
            if (timeCheck > maxTime)
            {
                //Debug.Log("NextFrame");
                yield return null;
            }
        }
        Debug.Log("End Generate Tile");
     
    }
}

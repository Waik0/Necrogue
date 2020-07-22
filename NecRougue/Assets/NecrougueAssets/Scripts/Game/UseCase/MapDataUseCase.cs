using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using Random = UnityEngine.Random;

//----------------------------------------------------------------------------------------------------------------------
//Map
//----------------------------------------------------------------------------------------------------------------------
//マップデータ処理
public class MapDataUseCase : IEntityUseCase<MapData>
{
    private MapData _mapData;
    public MapData Data { get => _mapData; }

    public MapDataUseCase()
    {
        ResetData();
    }
    public void ResetData()
    {
        _mapData = new MapData();
        _mapData.Nodes = new List<MapNode>();
    }
    public void SetMapCurrentSerialNumber(int num)
    {
        _mapData.CurrentSerialNumber = num;
    }

    public int GetCurrentMapEnemyId()
    {
        try
        {
            return _mapData.Nodes.First(_ => _.SerialNumber == _mapData.CurrentSerialNumber).EnemyId;
        }
        catch (Exception e)
        {
            Console.WriteLine(e);
            throw;
        }
    }

    public void IncrementDepth()
    {
        _mapData.Depth++;
    }
    public int CurrentDepth() => _mapData.Depth;
    public MapNode GetCurrentMapNode()
    {
        try
        {
            return _mapData.Nodes.First(m=> m.SerialNumber == _mapData.CurrentSerialNumber);
        }
        catch (Exception e)
        {
            Console.WriteLine(e);
            throw;
        }
    }
    //----------------------------------------------------------------------------------------------------------------------
    //マップ生成
    //----------------------------------------------------------------------------------------------------------------------
    public void CreateMap(int startDepth,int endDepth)
    {
        var records = MasterdataManager.Records<MstMapNodeRecord>();
        int serial = 1;//固有番号
        List<int> beforeNodes = new List<int>();
        for (int i = startDepth; i < endDepth; i++)
        {
            var nodePerDepth = Random.Range(2, 4);
            var currentNodes = new List<int>();
            var target = records.Where(_ => _.minDepth <= i && i <= _.maxDepth).ToList();
            if (target.Count <= 0)
            {
                continue;
            }
            for (int j = 0; j < nodePerDepth; j++)
            {
                
                var data = target[Random.Range(0, target.Count)].Convert();
                data.Depth = i;
                data.SerialNumber = serial;
                data.LinkNodeSerial = new List<int>();
                _mapData.Nodes.Add(data);
                //マップつなげる
                foreach (var beforeNode in beforeNodes)
                {
                    foreach (var mapNode in _mapData.Nodes.Where(n=>n.SerialNumber == beforeNode))
                    {
                        mapNode.LinkNodeSerial.Add(serial);
                    }
                }
                currentNodes.Add(serial);
                serial++;
            }
            beforeNodes = currentNodes;
        }
        //ソート
        _mapData.Nodes.OrderBy(n => n.Depth).ThenBy(n => n.SerialNumber).ToList();
    }
}
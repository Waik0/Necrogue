using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public class PlayerData
{
    public string id;
    public string Name;
}
//1フレーム当たりのオブジェクト位置情報
[Serializable]
public class TimelineData
{
    public int timeLineId;//いくつ目のデータか
    public List<ObjectVertexData> timeLine;
}

[Serializable]
public class ObjectVertexData
{
    public Vector2 pos;
    public float angle;
}

[Serializable]
public class InputData
{
    public Vector2 pos;
    public float angle;
}

[SerializeField]
public class CursorData
{
    public string id;
    public Vector2 worldPos;//端末依存回避のためworld
}
[SerializeField]
public class GameSequenceData
{
    public enum Command
    {
        Start,
    }

    public Command command;
}

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

[Serializable]
public class PlayerGameData
{
    public string id;
    public List<string> cardIds;
    public bool isSpy;
}
//1フレーム当たりのオブジェクト位置情報
[Serializable]
public class TimelineData
{
    public int timeLineId;//いくつ目のデータか
    public List<ObjectVertexData> objects;
    public bool isLast;
}

[Serializable]
public class ObjectVertexData
{
    public int uniqueId;
    public int pieceId;
    public Vector2Int pos;
    public int angle;
}

[Serializable]
public class InputData
{
    public int pieceId;
    public Vector2Int pos;
    public int angle;
}

[Serializable]
public class CursorData
{
    public bool down;
    public string id;
    public Vector2 worldPos;//端末依存回避のためworld
}
//ゲーム開始通知
[Serializable]
public class GameStartData
{
    public List<string> players;
}
[Serializable]
public class GameSequenceData
{
    public enum Command
    {
        //Start,//ゲーム開始時にホストが送る
        Ready,//クライアントがゲーム開始を受け取ったら送る
        NextTurn,//
        GameOver,
    }

    public Command command;
    public int currentTurn;
    public string currentPlayer;
    public ResultData ResultData;
}
[Serializable]
public class HandData
{
    public string playerId;
    public List<int> hand;
}
[Serializable]
public class DeckData
{
    public int index;
    public List<int> deck;
}
[Serializable]
public class RollData
{
    public enum Roll
    {
        Blue,
        Red
    }
    public string playerId;
    public Roll roll;
}

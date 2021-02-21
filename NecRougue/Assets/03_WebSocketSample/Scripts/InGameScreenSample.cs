using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;
using Zenject;
using Random = UnityEngine.Random;

[Serializable]
public class FirstData
{
    public uint seed;
    public string[] players;
    
}

[SerializeField]
public class FallDataSample
{
    public int turn;
    public Vector2Int fallPos;
    public int fallAng;
}
public class InGameScreenSample : MonoBehaviour
{
    private class Score
    {
        public string name;
        public int lose;
    }
    [SerializeField] private MatchingSample _matchingSample;
    [SerializeField] private InGameSample _inGame;
    [SerializeField] private Text _scoreText;
    [SerializeField]  private Camera _camera;
    private WebSocketErrorHandleSample _error;
    private WebSocketSample _ws;
    private FirstData _first;
    private Dictionary<string, Score> _scores = new Dictionary<string, Score>();
    private Queue<FallDataSample> _fallDatas = new Queue<FallDataSample>();
    private int turn;
    private bool _isMyTurn = false;
    [Inject]
    void Inject(WebSocketSample ws,
        WebSocketErrorHandleSample error)
    {
        _ws = ws;
        _error = error;
    }

    void Route(WebSocketSampleResponce res)
    {
        switch (res.command)
        {
            case WebSocketSample.TestCommands.Cursor:
                break;
            case WebSocketSample.TestCommands.FirstData:
                OnFirstData(res);
                break;
            case WebSocketSample.TestCommands.Turn:
                break;
            case WebSocketSample.TestCommands.Fall:
                break;
            case WebSocketSample.TestCommands.Chat:
                break;
            default:
                throw new ArgumentOutOfRangeException();
        }
    }
    //host
    void SendFirstData()
    {
        string[] users = _matchingSample.UserData.Keys.OfType<string>().ToArray();
        int r = Random.Range(int.MinValue,int.MaxValue);
        uint u = (uint)int.MaxValue + (uint)r;
        _ws.Send(WebSocketSample.TestCommands.FirstData,
            JsonUtility.ToJson(new FirstData()
        {
            seed = u,
            players = users
        }));
    
    }
    //client & host
    void SendFallData()
    {
        var ang = _inGame.CurrentAng;
        var pos = _inGame.CurrentPos;
        _ws.Send(WebSocketSample.TestCommands.Fall,JsonUtility.ToJson(new FallDataSample()
        {
            turn = turn,
            fallAng = ang,
            fallPos = pos
        }));
    }
    //実質初期化を兼ねる
    void OnFirstData(WebSocketSampleResponce res)
    {
        var f = JsonUtility.FromJson<FirstData>(res.data);
        _first = f;
        _scores = new Dictionary<string, Score>();
        foreach (var fPlayer in f.players)
        {
            if (!_scores.ContainsKey(fPlayer))
            {
                var name = "";
                if (_matchingSample.UserData.ContainsKey(fPlayer))
                {
                    name = _matchingSample.UserData[fPlayer].name;
                }
                _scores.Add(fPlayer,new Score()
                {
                    lose = 0,
                    name = name
                });
            }
        }
        _fallDatas.Clear();
        turn = 0;
        _inGame.ReStart(f.seed);
        _isMyTurn = f.players[turn % f.players.Length] == _ws.SelfId;
    }

    void OnFalldata(WebSocketSampleResponce res)
    {
        var f = JsonUtility.FromJson<FallDataSample>(res.data);
        // if (res.msgfrom == _ws.SelfId)
        // {
        //     return;
        // }
        turn++;
        _isMyTurn = _first.players[turn % _first.players.Length] == _ws.SelfId;
        _inGame.Fall(f.fallPos, f.fallAng);
        //_fallDatas.Enqueue(f);
    }
    void MyTurnUpdate()
    {
        if (Input.GetMouseButton(0))
        {
            _inGame.SetNewX((int)_camera.ScreenToWorldPoint(Input.mousePosition).x);
        }

        if (Input.GetMouseButtonUp(0))
        {
            SendFallData();
            _isMyTurn = false;
        }
    }

    void FixedUpdate()
    {
        if(_isMyTurn && _inGame.c)
    }
}

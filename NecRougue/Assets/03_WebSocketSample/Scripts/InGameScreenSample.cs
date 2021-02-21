using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UniRx;
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
    [SerializeField] private Text _ggText;
    [SerializeField] private Text _turnText;
    [SerializeField]  private Camera _camera;
    [SerializeField] private Button _retry;
    private WebSocketErrorHandleSample _error;
    private WebSocketSample _ws;
    private FirstData _first;
    private Dictionary<string, Score> _scores = new Dictionary<string, Score>();
    private Dictionary<string, bool> _state = new Dictionary<string, bool>();

    private int turn;
    private bool _isMyTurn = false;

    void OnEnable()
    {
        if (_ws.IsHost)
        {
            SendFirstData();
        }
    }
    [Inject]
    void Inject(WebSocketSample ws,
        WebSocketErrorHandleSample error)
    {
        _ws = ws;
        _error = error;
        _ws.OnMessage.Subscribe(Route).AddTo(this);
        _inGame.OnGameOver = GameOver;
        _inGame.OnDecideNewSpawnPoint = SendEndCheck;
        _retry.onClick.AddListener(SendFirstData);
    }
    void GameOver()
    {
        var name = "";
        if (_scores.ContainsKey(_first.players[turn % _first.players.Length]))
        {
            name = _scores[_first.players[turn % _first.players.Length]].name;
            _scores[_first.players[turn % _first.players.Length]].lose++;
        }
        _ggText.text = $"{name} のまけ";
        UpdateScore();
        _retry.gameObject.SetActive(_ws.IsHost);
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
            case WebSocketSample.TestCommands.Fall:
                OnFalldata(res);
                break;
            case WebSocketSample.TestCommands.EndCheck:
                OnEndCheckData(res);
                break;
        }
    }


    //host
    void SendFirstData()
    {
        _retry.gameObject.SetActive(false);
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
        _inGame.IsRotate = false;
        var ang = _inGame.CurrentAng;
        var pos = _inGame.CurrentPos;
        _ws.Send(WebSocketSample.TestCommands.Fall,JsonUtility.ToJson(new FallDataSample()
        {
            turn = turn,
            fallAng = ang,
            fallPos = pos
        }));
    }
    void SendEndCheck(int height)
    {
        _ws.Send(WebSocketSample.TestCommands.EndCheck,"");
    }
    //実質初期化を兼ねる
    void OnFirstData(WebSocketSampleResponce res)
    {
        Debug.Log(res.command);
        var f = JsonUtility.FromJson<FirstData>(res.data);
        _first = f;
        _state.Clear();
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

            if (!_state.ContainsKey(fPlayer))
            {
                _state.Add(fPlayer,false);
            }
        }
        turn = 0;
        _inGame.ReStart(f.seed);
        _isMyTurn = f.players[turn % f.players.Length] == _ws.SelfId;
        UpdateScore();
        UpdateTurn();
        _inGame.SetNext();
        _ggText.text = "";
    }
    void OnFalldata(WebSocketSampleResponce res)
    {
        var f = JsonUtility.FromJson<FallDataSample>(res.data);
        // if (res.msgfrom == _ws.SelfId)
        // {
        //     return;
        // }
        _inGame.Fall(f.fallPos, f.fallAng);
        foreach (var key in _state.Keys.ToArray())
        {
            _state[key] = false;
        }
    }

    void OnEndCheckData(WebSocketSampleResponce res)
    {
        if (_state.ContainsKey(res.msgfrom))
        {
            _state[res.msgfrom] = true;
        }

        if (_state.All(_ => _.Value))
        {
            foreach (var key in _state.Keys.ToArray())
            {
                _state[key] = false;
            }
            UpdateTurn();
            _inGame.SetNext();
        }
    }
    void UpdateTurn()
    {
        turn++;
        _isMyTurn = _first.players[turn % _first.players.Length] == _ws.SelfId;
        var name = "";
        if (_scores.ContainsKey(_first.players[turn % _first.players.Length]))
        {
            name = _scores[_first.players[turn % _first.players.Length]].name;
        }
        _turnText.text =  $"{name} のばん (ターン {turn})";
        
    }

    void UpdateScore()
    {
        _scoreText.text = "";
        foreach (var keyValuePair in _scores)
        {
            _scoreText.text += keyValuePair.Value.name + ":" + keyValuePair.Value.lose;
        }

       
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

    void Update()
    {
        if (_isMyTurn && _inGame.CurrentState == InGameSample.State.Rotate)
        {
            MyTurnUpdate();
        }
    }
}

using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Toast;
using UnityEngine;
using Random = UnityEngine.Random;

public class InGameSample : MonoBehaviour
{
    public enum State
    {
        Wait,
        Init,
        Spawn,
        Rotate,
        Fall,
        CheckHeight,
        GameOver,
    }
    [SerializeField]
    private Sprite[] _sprites;
    [SerializeField] private PieceSample _piecePrefab;
    [SerializeField] private Camera _camera;
    private Vector2Int _spawnPoint = new Vector2Int(0, 30);
    private Unity.Mathematics.Random _random;
    private int _currentAng = 0;
    private int _currentX = 0;
    private PieceSample _currentPiece;
    private List<PieceSample> _pieceList = new List<PieceSample>();
    private Statemachine<State> _statemachine;
    public int CurrentAng=>_currentAng;
    public State CurrentState => _statemachine.Current;
    public Vector2Int CurrentPos => new Vector2Int(_currentX, _spawnPoint.y);
    public Vector2Int SpawnPoint => _spawnPoint;
    public Action<int> OnDecideNewSpawnPoint;
    public Action OnGameOver;
    public bool IsRotate { get; set; }
    void Start()
    {
        _statemachine = new Statemachine<State>();
        _statemachine.Init(this);
    }
    void Spawn(float ang)
    {
        var idx = _random.NextInt(0, _sprites.Length);
        Debug.Log(idx);
        var piece = Instantiate(_piecePrefab);
        piece.transform.position = new Vector2(_spawnPoint.x,_spawnPoint.y);
        piece.Init(_sprites[idx]);
        piece.SetAng(0);
        _currentAng = 0;
        _currentPiece = piece;
    }
    public void ReStart(uint seed)
    {
        _random = new Unity.Mathematics.Random(seed);
        Physics2D.angularSleepTolerance = 100f;
        Physics2D.linearSleepTolerance = 2f;
        _camera.transform.position = new Vector3(0,0,-10);
        _spawnPoint = new Vector2Int(0, 30);
        _pieceList.RemoveAll(_ =>
        {
            Destroy(_.gameObject);
            return true;
        });
        if (_currentPiece)
        {
            Destroy(_currentPiece.gameObject);
        }
        _statemachine.Next(State.Init);
    }

    public void SetNewPosition(int up)
    {
        _spawnPoint = new Vector2Int(0, 30 + up);
        _camera.transform.position = new Vector3(0, up, -10);
    }

    public void SetNext()
    {
        _statemachine.Next(State.Spawn);
    }
    public void SetNewX(int x)
    {
        _currentX = x;
        if (_currentPiece) 
            _currentPiece.transform.position = new Vector3(_currentX, _spawnPoint.y, 0);
    }
    public bool Fall(Vector2 pos ,float ang)
    {
        if (_currentPiece)
        {
            _pieceList.Add(_currentPiece);
            _currentPiece?.StartCalc(pos, ang);
            _currentPiece = null;
            _statemachine.Next(State.Fall);
            return true;
        }

        return false;
    }
    IEnumerator Wait()
    {
        yield return null;
    }
    IEnumerator Init()
    {
       
        yield return null;
    }

    IEnumerator Spawn()
    {
        Spawn(0);
        _statemachine.Next(State.Rotate);
        yield return null;
    }

    IEnumerator Rotate()
    {
        IsRotate = true;
        while (true)
        {
            if(IsRotate) Roll();
            yield return null;
        }
    }

    IEnumerator Fall()
    {
        var endCount = 0;
        while (true)
        {
            var end = _pieceList.All(_ => _.IsSleep());
            var dead = _pieceList.Any(_ => _.IsDead());
            if (dead)
            {
                _statemachine.Next(State.GameOver);
                yield break;
            }

            if (end)
            {
                endCount++;
            }
            else
            {
                endCount = 0;
            }

            if (endCount > 10)
            {
                _statemachine.Next(State.CheckHeight);
                endCount = 0;
            }
            yield return null;
        }
    }
    IEnumerator CheckHeight()
    {
        bool isHit = true;
        int up = -50;
        var c = 0;
        while (isHit)
        {
            var origin = new Vector2(-500,up);
            var direction = new Vector2(1, 0);
            RaycastHit2D hit = Physics2D.Raycast(origin,direction, 1000);
            isHit = hit.collider;
            //衝突時のRayを画面に表示
            if (hit.collider){
                Debug.DrawRay(origin, hit.point - origin, Color.blue, 3, false);
            }
            //非衝突時のRayを画面に表示
            else{
                Debug.DrawRay(origin, direction * 1000, Color.green, 3, false);
            }
            up++;
            c++;
            if (c > 10)
            {
                c = 0;
                yield return null;
            }
        }

        up += 10;
        OnDecideNewSpawnPoint?.Invoke(up);
        //debug
        SetNewPosition(up);
    }

  

    void Roll()
    {
        _currentAng += 5;
        _currentPiece?.SetAng(_currentAng);
    }



    IEnumerator GameOver()
    {
        Debug.Log("GG");
        OnGameOver?.Invoke(); 
        yield return null;
    }
    private void FixedUpdate()
    {
        _statemachine.Update();
    }

}

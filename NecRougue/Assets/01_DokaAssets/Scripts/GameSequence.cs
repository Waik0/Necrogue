using System.Collections;
using System.Collections.Generic;
using Toast;
using UnityEngine;
using Zenject;

public class GameSequence : ITickable,IInitializable
{
    enum GameState
    {
        Init,
        SceneEvent,//全体イベント
        Initiative,//行動順確定
        PlayerTurn,
        Check,
        CleanUp,
        End,
    }

    private IMapGenerator _mapGenerator; 
    private Statemachine<GameState> _statemachine;
    [Inject]
    void Inject(IMapGenerator generator)
    {
        _mapGenerator = generator;
    }
    public void Tick()
    {
        _statemachine.Update();
    }

    public void Initialize()
    {      

        _statemachine = new Statemachine<GameState>();
        _statemachine.Init(this);
        
    }

    /// <summary>
    /// 開始時に一度だけ呼ばれる
    /// </summary>
    /// <returns></returns>
    IEnumerator Init()
    {
        Debug.Log("Work");
        //マップ設定
        var time = Time.time;
        yield return _mapGenerator.GenerateAsync(1);
        Debug.Log($"MapGenerate{Time.time - time}");
        yield return null;
    }


}

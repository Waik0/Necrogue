using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Random = UnityEngine.Random;

public class RetroGraphicsSpriteTest : MonoBehaviour
{
    [SerializeField] private RetroScreenRenderer _renderer = default;

    [SerializeField] private RetroGraphicsSprite _sprite = default;

    [SerializeField] private RetroGraphicsFont _font = default;
    // Start is called before the first frame update
    void Start()
    {
        //Application.targetFrameRate = 60;
        _renderer.SetClearBuffer(true,RetroScreenRenderer.LayerList.Sprite);
        _renderer.DrawText(0,0,100,"てすとてきすとあいうえtおかきくけこさしすせそあいうえおかきくけこさしすせそ");
    }

    private List<TestBall> tests = new List<TestBall>();
    
    // Update is called once per frame
    void Update()
    {
       
        if (1f / Time.deltaTime > 60)
        {
            tests.Add(new TestBall(_sprite, _renderer));
        }

        foreach (var testBall in tests)
        {
            testBall.Update();
        }
    }

    private void OnGUI()
    {
        GUILayout.BeginVertical("box");
        GUILayout.Label("fps : "+  1f / Time.deltaTime);
        GUILayout.Label("counts : "+  tests.Count);
        
        GUILayout.EndVertical();
    }
}

public class TestBall
{
    public RetroGraphicsSprite Sprite;
    public RetroScreenRenderer Renderer;
    
    private float time;
    private Vector2Int pos = new Vector2Int();
    private Vector2Int center;
    public TestBall(RetroGraphicsSprite s, RetroScreenRenderer r)
    {
        Sprite = s;
        Renderer = r;
        center = new Vector2Int(Random.Range(0,600),Random.Range(0,400));
    }
    public void Update()
    {
        time+=0.1f;
        pos.x = center.x; //+ (int)(Mathf.Sin(time*Mathf.PI) * 15);
        pos.y = center.y;//+ (int)(Mathf.Cos(time*Mathf.PI) * 15);
        Renderer.DrawSprite(Sprite,pos,(int)(time % 4));
    }
    
}
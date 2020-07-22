using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.Rendering;
using UnityEngine.UI;
//ベンチマーク結果
//10万命令でも行けそう

public class RetroScreenRenderer : MonoBehaviour
{
    public enum LayerList
    {
        BackGround,
        Sprite,
        UI,
    }
    private class Layer
    {    
        int width = 255;

        int height = 320;
        public Layer(int width, int height)
        {
            this.width = width;
            this.height = height;
            buffer = Enumerable.Repeat<byte>((byte)255, width * height).ToArray();
            _texture2D =new Texture2D(width,height,TextureFormat.R8,false);
            _texture2D.filterMode = FilterMode.Point;
        }
        private Texture2D _texture2D;
        private bool _clearOnce;
        private bool _clear;
        private byte[] buffer;
        public Texture2D Texture2D => _texture2D;
        public void SetClear(bool c)
        {
            _clear = c;
        }
        public void SetClearOnce(bool c)
        {
            _clearOnce = c;
        }
        public void DrawPixel(int x, int y,byte c)
        {
            if (x < 0 || x >= width || y < 0 || y >= height)
            {
                return;
            }
            buffer[y * width + x] = c;
        }
        public void DrawPixel(int index, byte c)
        {
            if (index < 0 || index >= buffer.Length)
            {
                return;
            }
            buffer[index] = c;
        }
        public void Update()
        {
            _texture2D.LoadRawTextureData(buffer);
            _texture2D.Apply();
            if (_clear || _clearOnce)
            {
                buffer = Enumerable.Repeat<byte>((byte)255, width * height).ToArray();
                _clearOnce = false;
            }
        }
    }
    // RenderTexture renderTexture;
    // Material material;

    int width = 255;

    int height = 360;
    [SerializeField] private Shader _shader; 
    [SerializeField] private Color[] _colors;
    [SerializeField] private RetroGraphicsFont[] _fontData;
    private DeployRetroGraphicsFont[] _fontBuffer;
    private Dictionary<LayerList,Layer> _layers;
    private Canvas _canvas;
    public int Width => width;
    public int Height => height;

    // void Setup()
    // {
    //     renderTexture = new RenderTexture(width, height, 24);
    //     renderTexture.useMipMap = false;
    //     renderTexture.filterMode = FilterMode.Point;
    //     var camera = GetComponent<Camera>();
    //     camera.targetTexture = renderTexture;
    //
    //     material = new Material(Shader.Find("Custom/RetroScreen"));
    //     
    //     var cameraObject = new GameObject("Camera");
    //     var camera2 = cameraObject.AddComponent<Camera>();
    //     camera2.cullingMask = 0;
    //     camera2.transform.parent = transform;
    //
    //     var commandBuffer = new CommandBuffer();
    //     commandBuffer.Blit((RenderTargetIdentifier)renderTexture, BuiltinRenderTextureType.CameraTarget,material);
    //     camera2.AddCommandBuffer(CameraEvent.AfterEverything, commandBuffer);
    // }
    void Setup()
    {
       
        var fb = new List<DeployRetroGraphicsFont>();
        for (var i = 0; i < _fontData.Length; i++)
        {
            fb.Add(_fontData[i].Deploy());
        }

        _fontBuffer = fb.ToArray();
        width = height * Screen.width / Screen.height;
        _layers = new Dictionary<LayerList, Layer>(){
            {LayerList.BackGround,new Layer(width,height)},
            {LayerList.Sprite,new Layer(width,height)},
            {LayerList.UI,new Layer(width,height)},
        };
        _canvas = new GameObject().AddComponent<Canvas>();
        _canvas.gameObject.AddComponent<GraphicRaycaster>();
        _canvas.gameObject.AddComponent<EventSystem>();
        _canvas.gameObject.AddComponent<StandaloneInputModule>();
        _canvas.pixelPerfect = true;
        _canvas.renderMode = RenderMode.ScreenSpaceOverlay;
        var c = new Color[256];
        for (var i = 0; i < c.Length; i++)
        {
            if (_colors.Length > i)
            {
                c[i] = _colors[i];
            }
            else
            {
                c[i] = _colors[0];
            }
           
        }
        c[255] = new Color(0,0,0,0);
        Vector2 lastSize = Vector2.zero;
        foreach (var keyValuePair in _layers)
        {
            var image = new GameObject().AddComponent<Image>();
            var material = new Material(_shader);//Shader.Find("Custom/RetroScreen"));
            image.material = material;
            var scale = Screen.height / (float)keyValuePair.Value.Texture2D.height;
            image.rectTransform.parent = _canvas.transform;
            image.raycastTarget = false;
            image.transform.SetAsLastSibling();
            image.rectTransform.pivot = new Vector2(.5f,.5f);
            image.rectTransform.anchoredPosition = Vector2.zero;
            image.rectTransform.sizeDelta = new Vector2(
                keyValuePair.Value.Texture2D.width,
                keyValuePair.Value.Texture2D.height) * scale;
            image.sprite = Sprite.Create(
                keyValuePair.Value.Texture2D,
                new Rect(0,0,keyValuePair.Value.Texture2D.width,keyValuePair.Value.Texture2D.height),
                new Vector2(.5f,.5f) );
            image.material.SetColorArray("_Colors",c);
            lastSize = new Vector2(
                keyValuePair.Value.Texture2D.width,
                keyValuePair.Value.Texture2D.height) * scale;
        }

        var input = new GameObject("Input",
            typeof(Image),
            typeof(RgInputPresenter)).GetComponent<Image>();
        input.color = Color.clear;
        input.rectTransform.parent = _canvas.transform;
        input.transform.SetAsLastSibling();
        input.rectTransform.pivot = new Vector2(.5f,.5f);
        input.rectTransform.anchoredPosition = Vector2.zero;
        input.rectTransform.sizeDelta = lastSize;
        //Debug.Log(_texture2D.GetRawTextureData().Length);

        //Debug.Log(buffer.Length);

    }

// Start is called before the first frame update
    void Awake()
    {
        Setup();
    }
    
    public void DrawPixel(int index, byte c,LayerList layer = LayerList.Sprite)
    {
        _layers[layer].DrawPixel(index,c);
    }
    public void DrawPixel(int x, int y,byte c,LayerList layer = LayerList.Sprite)
    {
        _layers[layer].DrawPixel(x,y,c);
    }

    public void DrawText(int sx, int sy, int width, string text)
    {
        var l = 0;
        var y2 = 0;
        for (var i = 0; i < text.Length; i++)
        {
           
            var stride = DrawText(sx + l,sy + y2,text[i]);
            l += stride.stx;
            if (l > width)
            {
                l = 0;
                y2 += stride.sty;
            }
        }

       
    }
    public (int stx, int sty) DrawText(int sx,int sy,char text)
    {
        for (var i = 0; i < _fontData.Length; i++)
        {
            var stride = _fontBuffer[i].TryGetBuffer(text, out var buffer);
            if (buffer != null)
            {
                for (var i1 = 0; i1 < buffer.Length; i1++)
                {
                    if (buffer[i1])
                    {
                        DrawPixel(sx + i1 % stride.stx, sy + i1 / stride.stx, 3,LayerList.UI);
                    }
                }
                
                return (stride);
            }
            
        }

        return (12,12);
    }
    public void DrawCircle()
    {
        
    }

    public void DrawRect()
    {
        
    }

    public void DrawRoundedRect()
    {
        
    }

    public void DrawSprite(RetroGraphicsSprite sprite,Vector2Int center,int index = 0)
    {
        var centerIndex = center.y * width + center.x;
        var sx =  (sprite.SingleSize.x * index % sprite.Stride) ;
        var sy = (sprite.SingleSize.x * index / sprite.Stride) * sprite.SingleSize.y;
        //Debug.Log($"{sx},{sy}");
        var l  =sprite.SingleSize.x * sprite.SingleSize.y;
        for (int i = 0; i < l; i++)
        {
         
            var x = i % sprite.SingleSize.x + center.x; 
            var y = i / sprite.SingleSize.x+ center.y;
            var ind = sx + (i % sprite.SingleSize.x) + (sy + (i / sprite.SingleSize.x)) * sprite.Stride;
            if (sprite.Buffer[ind] == 255)
            {
                continue;
            }
            DrawPixel(x, y, sprite.Buffer[ind]);
        }
    }

    public void SetClearBuffer(bool flag,LayerList layerList)
    {
        _layers[layerList].SetClear(flag);
    }
    public void SetClearBufferOnce(bool flag,LayerList layerList)
    {
        _layers[layerList].SetClearOnce(flag);
    }
    // Update is called once per frame
    void LateUpdate()
    {
        foreach (var keyValuePair in _layers)
        {
            keyValuePair.Value.Update();
        }

        //Graphics.Blit(null,_renderTexture,_material);
    }

    void OnGUI()
    {
        GUILayout.Label("fps : "+  1f / Time.deltaTime);
    }
}

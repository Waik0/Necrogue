using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.UI;
//ベンチマーク結果
//10万命令でも行けそう

public class RetroScreenRenderer : MonoBehaviour
{
    // RenderTexture renderTexture;
    // Material material;

    int width = 255;

    int height = 320;
    [SerializeField] private Shader _shader; 
    [SerializeField] private Color[] _colors;
    [SerializeField] private FontData m_FontData = FontData.defaultFontData;
    private Texture2D _texture2D;
    private Canvas _canvas;
    private bool _clear;
    private byte[] buffer;
    public Font font
    { 
        get
        {
            return m_FontData.font;
        }
    }
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
        width = height * Screen.width / Screen.height;
        _texture2D =new Texture2D(width,height,TextureFormat.R8,false);
        _texture2D.filterMode = FilterMode.Point;
        _canvas = new GameObject().AddComponent<Canvas>();
        _canvas.pixelPerfect = true;
        _canvas.renderMode = RenderMode.ScreenSpaceOverlay;
        var image = new GameObject().AddComponent<Image>();
        var material = new Material(_shader);//Shader.Find("Custom/RetroScreen"));
        image.material = material;
        var scale = Screen.height / (float)_texture2D.height;
        image.rectTransform.parent = _canvas.transform;
        image.rectTransform.pivot = new Vector2(.5f,.5f);
        image.rectTransform.anchoredPosition = Vector2.zero;
        //image.rectTransform.anchorMin = Vector2.zero;
        //image.rectTransform.anchorMax = Vector2.one;
        image.rectTransform.sizeDelta = new Vector2(_texture2D.width,_texture2D.height) * scale;
        image.sprite = Sprite.Create(_texture2D,new Rect(0,0,_texture2D.width,_texture2D.height),new Vector2(.5f,.5f) );
        //Debug.Log(_texture2D.GetRawTextureData().Length);
        buffer = new byte[width * height];
        //Debug.Log(buffer.Length);
        image.material.SetColorArray("_Colors",_colors);
    }

// Start is called before the first frame update
    void Awake()
    {
        Setup();
    }
    
    public void DrawPixel(int index, byte c)
    {
        if (index < 0 || index >= buffer.Length)
        {
            return;
        }
        buffer[index] = c;
    }
    public void DrawPixel(int x, int y,byte c)
    {
        if (x < 0 || x >= width || y < 0 || y >= height)
        {
            return;
        }
        buffer[y * width + x] = c;
    }

    public void DrawText(string text)
    {
        
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

    public void DrawSprite(RetroGraphicsSprite sprite,Vector2Int center)
    {
        var centerIndex = center.y * width + center.x;
        for (int i = 0; i < sprite.Buffer.Length; i++)
        {
            var x = i % sprite.Stride + center.x;
            var y = i / sprite.Stride + center.y;
            if (sprite.Buffer[i] == 255)
            {
                continue;
            }
            DrawPixel(x, y, sprite.Buffer[i]);
        }
    }

    public void SetClearBuffer(bool flag)
    {
        _clear = flag;
    }
    // Update is called once per frame
    void LateUpdate()
    {
        _texture2D.LoadRawTextureData(buffer);
        _texture2D.Apply();
        if (_clear)
        {
            buffer = new byte[width * height];
        }
        //Graphics.Blit(null,_renderTexture,_material);
    }

    void OnGUI()
    {
        GUILayout.Label("fps : "+  1f / Time.deltaTime);
    }
}

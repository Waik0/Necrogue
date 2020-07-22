using System.Collections;
using System.Collections.Generic;
using UnityEngine;
[CreateAssetMenu]
public class RetroGraphicsSprite : ScriptableObject
{
    public int Stride;
    public Vector2Int SingleSize;
    public byte[] Buffer;
    
}

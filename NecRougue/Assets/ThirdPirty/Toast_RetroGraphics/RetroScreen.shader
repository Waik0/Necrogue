﻿Shader "Custom/RetroScreen"
{
    Properties
    {
       _MainTex("Texture",2D)="white"{}
    }
    SubShader
    {
        Tags {"Queue"="Transparent" "IgnoreProjector"="True" "RenderType"="Transparent"}
        // No culling or depth
        Cull Off ZWrite Off ZTest Always
        Blend SrcAlpha OneMinusSrcAlpha 
        Pass
        {
            CGPROGRAM
            #pragma vertex vert
            #pragma fragment frag

            #include "UnityCG.cginc"

            struct appdata
            {
                float4 vertex : POSITION;
                float2 uv : TEXCOORD0;
            };

            struct v2f
            {
                float2 uv : TEXCOORD0;
                float4 vertex : SV_POSITION;
            };
            
            v2f vert (appdata v)
            {
                v2f o;
                o.vertex = UnityObjectToClipPos(v.vertex);
                o.uv = v.uv;
                return o;
            }
            sampler2D _MainTex;
            float4 _Colors[256];
            fixed4 frag (v2f i) : SV_Target
            {
                //return col;
                //return float4(0,0,0,0);
				return _Colors[(int)(tex2D(_MainTex, i.uv).r * 256)];
            }
            ENDCG
        }
    }
}

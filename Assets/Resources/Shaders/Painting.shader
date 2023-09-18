Shader "Unlit/Painting"
{
    Properties
    {
        _BrushTex ("Brush", 2D) = "white" {}
        _DrawTex ("Draw Tex", 2D) = "white" {}
        _Color ("Main Color", Color) = (1, 1, 1, 1)
    }

    SubShader
    {
        Tags {
            "Queue"="Transparent"
            "IgnoreProjector"="True"
             "RenderType"="Transparent"
            }
        Cull Off
        Lighting Off
        ZWrite Off
        ZTest Always
        Pass
        {
            CGPROGRAM

            #include "UnityCG.cginc"
            #pragma vertex vert
            #pragma fragment frag
              
            sampler2D _BrushTex;
            half4 _BrushTex_ST;
            sampler2D _DrawTex;
            half4 _DrawTex_ST;
            float4 _Color;

            struct appdata
            {
                float4 vertex : POSITION;
                float4 color : COLOR;
                float2 uv : TEXCOORD0;
            };

            struct v2f
            {
                float4 vertex : SV_POSITION;
                float4 color : COLOR;
                float2 uv : TEXCOORD0;
                float4 screenPos : TEXCOORD1;
            };

            v2f vert(appdata v)
            {
                v2f o;
                o.vertex = UnityObjectToClipPos(v.vertex);
                o.screenPos = o.vertex;
                o.uv = v.uv;
                o.color = v.color;
                return o;
            }
      
            float4 frag (v2f i) : SV_Target
            {
                float4 brush = tex2D(_BrushTex, i.uv) * _Color;
                float2 grabTexcoord = i.screenPos.xy / i.screenPos.w;
                grabTexcoord.x = (grabTexcoord.x + 1.0) * 0.5;
                grabTexcoord.y = (grabTexcoord.y + 1.0) * 0.5;

            #if UNITY_UV_STARTS_AT_TOP
                grabTexcoord.y = 1.0 - grabTexcoord.y;
            #endif
              
                float4 draw = tex2D(_DrawTex, grabTexcoord);
                float4 color = draw * (1.0f - brush.a) + brush * brush.a;
                return color;
            }
            ENDCG
        }
    }
}
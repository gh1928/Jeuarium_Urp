Shader "Custom/BrushShader"
{
    Properties
    {
        _Color ("Color", Color) = (1,1,1,1)
        _MainTex ("Canvas Texture", 2D) = "white" {}
        _DrawTex ("Drawing Texture", 2D) = "white" {}
        _NormalTex ("Normal Map", 2D) = "white" {}
        _Resolution ("Ratio of Resolution", Float) = 1.0
    }
    SubShader
    {
        Tags { "RenderType"="Opaque" }
        LOD 200
        CGPROGRAM
        #pragma surface surf Standard fullforwardshadows

        #pragma target 3.0

        sampler2D _MainTex;
        sampler2D _DrawTex;
        sampler2D _NormalTex;

        struct Input
        {
            float2 uv_MainTex;
            float2 uv_DrawTex;
            float2 uv_NormalTex;
        };

        fixed4 _Color;

        float _Resolution;

        int isTouch;

        void surf (Input IN, inout SurfaceOutputStandard o)
        {
            fixed4 c = tex2D(_MainTex, IN.uv_MainTex);
            fixed3 d = UnpackNormal(tex2D(_NormalTex, IN.uv_NormalTex * _Resolution));
            
            o.Normal = d;
            o.Albedo = c.rgb;
            o.Alpha = c.a;
        }
        ENDCG
    }
    FallBack "Diffuse"
}

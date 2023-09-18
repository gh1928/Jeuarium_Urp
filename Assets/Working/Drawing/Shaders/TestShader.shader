Shader "Custom/TestShader"
{
    Properties
    {
        _Color ("Color", Color) = (1,1,1,1)
        _MainTex ("Canvas Texture", 2D) = "white" {}
        _DrawTex ("Drawing Texture", 2D) = "white" {}
        _NormalTex ("Normal Texture", 2D) = "white" {}
        _Size ("Size of Brush", Range(0.001,0.1)) = 0.01
        _Resolution ("Ratio of Resolution", Float) = 1.0
        _Raycast_Bool ("Bool of Raycast", Range(0,1)) = 0
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

        float4 _MainTex_TexelSize;
        float4 _DrawTex_TexelSize;

        struct Input
        {
            float3 worldPos;
            float2 uv_MainTex;
            float2 uv_DrawTex;
            float2 uv_NormalTex;
        };

        fixed4 _Color;

        float _Size;
        float _Resolution;

        float2 _Raycast_UV;

        float _Raycast_Bool;

        fixed4 result;

        void surf (Input IN, inout SurfaceOutputStandard o)
        {
            result = tex2D(_MainTex, IN.uv_MainTex);
            fixed3 d = UnpackNormal(tex2D(_NormalTex, IN.uv_NormalTex * _Resolution));
            o.Normal = d;

            if (_Raycast_Bool > 0) 
            {
                float ratio = _DrawTex_TexelSize.z * _MainTex_TexelSize.x;
                if (distance(IN.uv_MainTex, _Raycast_UV) < ratio * _Size) {
                    fixed4 c_draw = _Color * tex2D(_DrawTex, (IN.uv_DrawTex - _Raycast_UV + float2(0.5 * _Size, 0.5 * _Size)) / _Size);
                    result = lerp(result, c_draw, c_draw.a);
                }
            }
            
            o.Albedo = result.rgb;
            o.Alpha = result.a;
        }
        ENDCG
    }
    FallBack "Diffuse"
}

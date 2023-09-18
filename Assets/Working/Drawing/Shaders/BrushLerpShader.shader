// Used for Test
Shader "Custom/BrushLerpShader"
{
    Properties
    {
        _MainTex ("Canvas Texture", 2D) = "white" {}
        _SubTex ("Drawing Texture", 2D) = "white" {}
    }
    SubShader
    {
        Tags { "RenderType"="Opaque" }
        LOD 200
        CGPROGRAM
        #pragma surface surf Standard fullforwardshadows

        // Use shader model 3.0 target, to get nicer looking lighting
        #pragma target 3.0

        sampler2D _MainTex;
        sampler2D _SubTex;

        struct Input
        {
            float2 uv_MainTex;
            float2 uv_SubTex;
        };

        float tiling;
        float2 offset;

        void surf (Input IN, inout SurfaceOutputStandard o)
        {
            fixed4 c = tex2D(_MainTex, IN.uv_MainTex * tiling + offset);
            fixed4 d = tex2D(_SubTex, IN.uv_SubTex);
            fixed4 outC = lerp(d, c, c.a);
            o.Albedo = outC.rgb;
            o.Alpha = outC.a;
        }
        ENDCG
    }
    FallBack "Diffuse"
}

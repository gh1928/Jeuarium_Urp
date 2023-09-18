Shader "Custom/WaterStencilTarget"
{
    Properties
    {
        _Color ("Color", Color) = (1,1,1,1)
        _MainTex ("Main Texture", 2D) = "white" {}
        _CUBE ("Cubemap", CUBE) = ""{}
        _NormalTex0 ("Normal_0", 2D) = "bump" {}
        _NormalTex1 ("Normal_1", 2D) = "bump" {}
        _Metallic ("Metallic", Range(0,1)) = 0
        _Glossiness ("Smoothness", Range(0,1)) = 0
        _Speed ("Wave Speed", Range(0.1, 2)) = 1
    }
    SubShader
    {
        Tags { "RenderType"="Transparent" "Queue"="Transparent"}
        Blend SrcAlpha OneMinusSrcAlpha
        Stencil
        {
            Ref 1
            Comp NotEqual
        }
        LOD 200

        CGPROGRAM
        #pragma surface surf Water fullforwardshadows vertex:vert alpha:blend
        #pragma target 3.0

        sampler2D _NormalTex0;
        sampler2D _NormalTex1;
        samplerCUBE _CUBE;

        struct Input
        {
            float2 uv_NormalTex0;
            float2 uv_NormalTex1;
            float3 viewDir;
            float3 worldRefl;
            INTERNAL_DATA
        };

        struct SurfaceOutputCustom
        {
            fixed3 Albedo;
            fixed3 Normal;
            fixed3 Emission;
            half Metallic;
            half Smoothness;
            half Occlusion;
            fixed Alpha;
        };

        half _Speed;
        half _Glossiness;
        half _Metallic;
        fixed4 _Color;

        void vert (inout appdata_full v) 
        {
            v.vertex.y += cos(abs(v.texcoord.x * 2 - 1) + _Time.z) * 0.05;
        }

        void surf (Input IN, inout SurfaceOutputCustom o)
        {
            fixed4 c = _Color;
            fixed3 d1 = UnpackNormal(tex2D(_NormalTex0, IN.uv_NormalTex0 + float2(_Time.y * _Speed, 0)));
            fixed3 d2 = UnpackNormal(tex2D(_NormalTex1, IN.uv_NormalTex1 + float2(_Time.y * _Speed * 0.5, 0)));
            o.Normal = normalize(d1 + d2);
            float4 r = texCUBE(_CUBE, WorldReflectionVector(IN, o.Normal));

            o.Albedo = c.rgb;
            o.Emission = r;
            o.Metallic = _Metallic;
            o.Smoothness = _Glossiness;
            o.Alpha = c.a;
        }

        half4 LightingWater (SurfaceOutputCustom s, half3 viewDir, UnityGI gi)
        {
            float rim = saturate(dot(gi.light.dir, viewDir));
            rim = Pow5(1 - rim);

            float4 final = float4(rim * _LightColor0.rgb, s.Alpha);
            return final;
        }

        void LightingWater_GI (SurfaceOutputCustom s, UnityGIInput data, inout UnityGI gi)
        {
            gi = UnityGlobalIllumination(data, 1.0, s.Normal);
        }

        ENDCG
    }
    FallBack "Transparent"
}

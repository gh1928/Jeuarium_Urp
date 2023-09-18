Shader "Custom/WaterFillingShader"
{
    Properties
    {
        _Color ("Color", Color) = (1,1,1,1)
        _Glossiness ("Smoothness", Range(0,1)) = 0.5
        _Metallic ("Metallic", Range(0,1)) = 0.0
        _SurfaceHeight ("Surface Height", Range(0.001,1)) = 0.0

        [ToggleOff] _SpecularHighlights("Specular Highlights", Float) = 1.0
    }
    SubShader
    {
        Tags { "RenderType"="Fade" "Queue" = "Transparent+1" }
        Blend SrcAlpha OneMinusSrcAlpha Cull Off
        LOD 200

        CGPROGRAM
        
        #pragma surface surf Standard fullforwardshadows alpha:fade
        #pragma target 3.0
        
        #pragma shader_feature_local _SPECULARHIGHLIGHTS_OFF

        struct Input
        {
            float3 worldPos;
        };

        float _SurfaceHeight;
        float _ObjHeight;

        float3 _BottomPos;
        float3 _TopPos;

        half _Glossiness;
        half _Metallic;
        fixed4 _Color;

        UNITY_INSTANCING_BUFFER_START(Props)

        UNITY_INSTANCING_BUFFER_END(Props)

        void surf (Input IN, inout SurfaceOutputStandard o)
        {
            _ObjHeight = _TopPos.y - _BottomPos.y;

            if ((IN.worldPos.y - _BottomPos.y) / _ObjHeight <= _SurfaceHeight) {
                fixed4 c = _Color;
                o.Albedo = c.rgb;
                o.Metallic = _Metallic;
                o.Smoothness = _Glossiness;
                if (_Color.a != 0) o.Alpha = 0.8;
                else o.Alpha = c.a;
            }
        }
        ENDCG
    }
    FallBack "Diffuse"
}

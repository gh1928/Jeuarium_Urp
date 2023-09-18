Shader "Custom/RotationShader"
{
    Properties
    {
        _MainTex ("Albedo (RGB)", 2D) = "white" {}
        _Rotate ("Rotation Angle", Range(0, 360)) = 0
    }
    SubShader
    {
        Tags { "Queue"="Transparent" "RenderType"="Transparent" }
        LOD 200

        Blend SrcAlpha OneMinusSrcAlpha
        CGPROGRAM

        #pragma surface surf Standard alpha:blend

        #pragma target 3.0

        sampler2D _MainTex;

        struct Input
        {
            float2 uv_MainTex;
        };

        float _Rotate;
        float2 rotateUV;

        float2 RotateUV(float2 uv, float degree) 
        {
            float radian = degree * (UNITY_PI / 180);
            float c = cos(radian);
            float s = sin(radian);
            float2x2 rotationMatrix = float2x2(c, -s, s, c);

            uv -= 0.5;
            uv = mul(uv, rotationMatrix);
            uv += 0.5;

            return uv;
        }

        void surf (Input IN, inout SurfaceOutputStandard o)
        {
            rotateUV = RotateUV(IN.uv_MainTex, _Rotate);
            fixed4 c = tex2D (_MainTex, rotateUV);
            o.Albedo = c.rgb;
            o.Alpha = c.a;
        }
        ENDCG
    }
    FallBack "Diffuse"
}

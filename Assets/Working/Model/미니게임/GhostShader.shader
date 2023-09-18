// Upgrade NOTE: replaced '_Object2World' with 'unity_ObjectToWorld'

Shader "Unlit/GhostShader"
{
    Properties
    {
        _MainTex ("Texture", 2D) = "white" {}
    }
    SubShader
    {
        Blend SrcAlpha OneMinusSrcAlpha

        Tags { "RenderType"="Transparent" "Queue"="Transparent"}
        LOD 100

        Pass
        {
            CGPROGRAM
            #pragma vertex vert
            #pragma fragment frag
            // make fog work
            #pragma multi_compile_fog

            #include "UnityCG.cginc"

            struct appdata
            {
                float4 vertex : POSITION;
                float2 uv : TEXCOORD0;
                float3 normal : NORMAL;
            };

            struct v2f
            {
                float2 uv : TEXCOORD0;
                UNITY_FOG_COORDS(1)
                float4 vertex : SV_POSITION;
                float rim : TEXCOORD1;
            };

            sampler2D _MainTex;
            float4 _MainTex_ST;

            float map(float x, float in_min, float in_max, float out_min, float out_max) {
                return (x - in_min) * (out_max - out_min) / (in_max - in_min) + out_min;
            }

            v2f vert (appdata v)
            {
                v2f o;
                o.vertex = UnityObjectToClipPos(v.vertex);
                o.uv = TRANSFORM_TEX(v.uv, _MainTex);
                // compute world space position of the vertex
                float3 worldPos = mul(unity_ObjectToWorld, v.vertex).xyz;
                // compute world space view direction
                float3 worldViewDir = normalize(UnityWorldSpaceViewDir(worldPos));
                // world space normal
                float3 worldNormal = UnityObjectToWorldNormal(v.normal);
                o.rim = map( (1 - abs( dot(worldViewDir, worldNormal) )), 0, 1, 0,1 );
                UNITY_TRANSFER_FOG(o,o.vertex);
                return o;
            }

            fixed4 frag (v2f i) : SV_Target
            {
                // sample the texture
                fixed4 col = tex2D(_MainTex, i.uv);
                // apply fog
                UNITY_APPLY_FOG(i.fogCoord, col);
                col.a *= i.rim;
                return col;
            }
            ENDCG
        }
    }
}

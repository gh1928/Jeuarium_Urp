// Deprecated
Shader "Unlit/DrawingBrush"
{
Properties{
    _MainTex("Particle Texture", 2D) = "white" {}
    _DrawTex("Render Texture", 2D) = "white" {}
    _Color("Color", Color) = (1.0,1.0,1.0,1.0)
    _InvFade("Soft Particles Factor", Range(0.01,3.0)) = 1.0
}

Category{
    Tags { "Queue" = "Transparent" "IgnoreProjector" = "True" "RenderType" = "Transparent" "PreviewType" = "Plane" }
   
    SubShader {
        // extra pass that renders to depth buffer only
    //Pass {
    //    ZWrite On
    //    ColorMask 0
    //}

        Pass { Blend One OneMinusSrcAlpha

    ColorMask RGB
    Cull Off Lighting Off ZWrite Off

            CGPROGRAM
            #pragma vertex vert
            #pragma fragment frag
            #pragma target 2.0
            #pragma multi_compile_particles

            #include "UnityCG.cginc"

            sampler2D _MainTex;
            sampler2D _DrawTex;
            fixed4 _TintColor;

            struct appdata_t {
                float4 vertex : POSITION;
                fixed4 color : COLOR;
                float2 texcoord : TEXCOORD0;
                UNITY_VERTEX_INPUT_INSTANCE_ID
            };

            struct v2f {
                float4 vertex : SV_POSITION;
                fixed4 color : COLOR;
                float2 texcoord : TEXCOORD0;
                #ifdef SOFTPARTICLES_ON
                float4 projPos : TEXCOORD1;
                #endif
                UNITY_VERTEX_OUTPUT_STEREO
            };

            float4 _MainTex_ST;

            v2f vert(appdata_t v)
            {
                v2f o;
                UNITY_SETUP_INSTANCE_ID(v);
                UNITY_INITIALIZE_VERTEX_OUTPUT_STEREO(o);
                o.vertex = UnityObjectToClipPos(v.vertex);
                #ifdef SOFTPARTICLES_ON
                o.projPos = ComputeScreenPos(o.vertex);
                COMPUTE_EYEDEPTH(o.projPos.z);
                #endif
                o.color = v.color;
                o.texcoord = TRANSFORM_TEX(v.texcoord,_MainTex);
                return o;
            }

            UNITY_DECLARE_DEPTH_TEXTURE(_CameraDepthTexture);
            float _InvFade;
            float4 _Color;

            float map(float x, float in_min, float in_max, float out_min, float out_max)
            {

                return clamp( (x - in_min) * (out_max - out_min) / (in_max - in_min) + out_min, out_min, out_max);
            }
    

            fixed4 frag(v2f i) : SV_Target
            {
                #ifdef SOFTPARTICLES_ON
                float sceneZ = LinearEyeDepth(SAMPLE_DEPTH_TEXTURE_PROJ(_CameraDepthTexture, UNITY_PROJ_COORD(i.projPos)));
                float partZ = i.projPos.z;
                float fade = saturate(_InvFade * (sceneZ - partZ));
                i.color.a *= fade;
                #endif

                float sampled = tex2D(_MainTex, i.texcoord).r;
                //clip(i.color.r-1);

                i.color.a = sampled;
                float4 DrawColor = tex2D(_DrawTex, i.texcoord);//
                float3 MixColor = _Color.rgb;
                if (DrawColor.a > 0.5)
                {
                    MixColor = lerp(_Color.rgb, DrawColor.rgb, 0.5);//
                }
                return float4(MixColor.r,MixColor.g,MixColor.b,1)* map( sampled, 0.2,0.6,0,1);//i.color * tex2D(_MainTex, i.texcoord) * i.color.a;
            }
            ENDCG
        }
    }
    }
}
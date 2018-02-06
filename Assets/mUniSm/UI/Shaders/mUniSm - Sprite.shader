// Unity built-in shader source. Copyright (c) 2016 Unity Technologies. MIT license (see license.txt)
// Edit by mUniSm @ mathru.net

Shader "mUniSm/Sprite"
{
    Properties
    {
        [PerRendererData] _MainTex("Sprite Texture", 2D) = "white" {}
        [PerRendererData] _MaskTex("Mask Texture", 2D) = "white" {}
        _Color("Tint", Color) = (1,1,1,1)
        [MaterialToggle] PixelSnap("Pixel snap", Float) = 0
        [HideInInspector] _RendererColor("RendererColor", Color) = (1,1,1,1)
        [HideInInspector] _Flip("Flip", Vector) = (1,1,1,1)
        [PerRendererData] _AlphaTex("External Alpha", 2D) = "white" {}
        [PerRendererData] _EnableExternalAlpha("Enable External Alpha", Float) = 0
        _TransitionTex("Transition Texture", 2D) = "white" {}
    }

    SubShader
    {
        Tags
        {
            "Queue" = "Transparent"
            "IgnoreProjector" = "True"
            "RenderType" = "Transparent"
            "PreviewType" = "Plane"
            "CanUseSpriteAtlas" = "True"
        }

        Cull Off
        Lighting Off
        ZWrite Off
        Blend One OneMinusSrcAlpha

        Pass
        {
        CGPROGRAM

            #pragma vertex SpriteVert
            #pragma fragment SpriteFrag
            #pragma target 2.0
            #pragma multi_compile_instancing
            #pragma multi_compile _ PIXELSNAP_ON
            #pragma multi_compile _ ETC1_EXTERNAL_ALPHA
            #pragma multi_compile __ UNITY_UI_MASKING
            #pragma multi_compile __ UNITY_UI_WHITE
            #pragma multi_compile __ UNITY_UI_TRANSITION
            #pragma multi_compile __ UNITY_UI_OUTLINE
            #pragma multi_compile __ UNITY_UI_BLINK

            #ifndef UNITY_SPRITES_INCLUDED

                #define UNITY_SPRITES_INCLUDED

                #include "UnityCG.cginc"

                #ifdef UNITY_INSTANCING_ENABLED
                    UNITY_INSTANCING_CBUFFER_START(PerDrawSprite)
                        fixed4 unity_SpriteRendererColorArray[UNITY_INSTANCED_ARRAY_SIZE];
                        float4 unity_SpriteFlipArray[UNITY_INSTANCED_ARRAY_SIZE];
                    UNITY_INSTANCING_CBUFFER_END

                    #define _RendererColor unity_SpriteRendererColorArray[unity_InstanceID]
                    #define _Flip unity_SpriteFlipArray[unity_InstanceID]
                #endif

                CBUFFER_START(UnityPerDrawSprite)
                    #ifndef UNITY_INSTANCING_ENABLED
                        fixed4 _RendererColor;
                        float4 _Flip;
                    #endif
                    float _EnableExternalAlpha;
                CBUFFER_END

                fixed4 _Color;

                struct appdata_t
                {
                    float4 vertex   : POSITION;
                    float4 color    : COLOR;
                    float2 texcoord : TEXCOORD0;
                    UNITY_VERTEX_INPUT_INSTANCE_ID
                };

                struct v2f
                {
                    float4 vertex   : SV_POSITION;
                    fixed4 color : COLOR;
                    float2 texcoord : TEXCOORD0;
                    UNITY_VERTEX_OUTPUT_STEREO
                };

                sampler2D _MainTex;
                sampler2D _AlphaTex;

                #ifdef UNITY_UI_MASKING
                    sampler2D _MaskTex;
                #endif

                #ifdef UNITY_UI_TRANSITION
                    sampler2D _TransitionTex;
                    fixed _TransitionAmount;
                #endif

                #if UNITY_UI_BLINK
                    fixed2 _Speed;
                #endif

                #ifdef UNITY_UI_OUTLINE
                    #define PI 3.14159
                    fixed _OutlineSmooth;
                    fixed _OutlineSpread;
                    fixed4 _OutlineColor;
                    fixed4 light(v2f IN) {
                        fixed color = 0;
                        for (int i = 0; i <= _OutlineSmooth; i++) {
                            color += tex2Dlod(_MainTex, float4(IN.texcoord + float2(sin(PI * 2 * i / _OutlineSmooth) * _OutlineSpread, cos(PI * 2 * i / _OutlineSmooth) * _OutlineSpread), 0, 0)).a;
                        }
                        if (color > 1) { color = 1; }
                        _OutlineColor.a *= color;
                        return _OutlineColor;
                    }
                #endif

                v2f SpriteVert(appdata_t IN)
                {
                    v2f OUT;

                    UNITY_SETUP_INSTANCE_ID(IN);
                    UNITY_INITIALIZE_VERTEX_OUTPUT_STEREO(OUT);

                    #ifdef UNITY_INSTANCING_ENABLED
                        IN.vertex.xy *= _Flip.xy;
                    #endif

                    OUT.vertex = UnityObjectToClipPos(IN.vertex);
                    OUT.texcoord = IN.texcoord;
                    OUT.color = IN.color * _Color * _RendererColor;

                    #ifdef PIXELSNAP_ON
                        OUT.vertex = UnityPixelSnap(OUT.vertex);
                    #endif

                    return OUT;
                }

                fixed4 SampleSpriteTexture(float2 uv)
                {
                    fixed4 color = tex2D(_MainTex, uv);

                    #if ETC1_EXTERNAL_ALPHA
                        fixed4 alpha = tex2D(_AlphaTex, uv);
                        color.a = lerp(color.a, alpha.r, _EnableExternalAlpha);
                    #endif

                    #if UNITY_UI_BLINK
                        color.a *= (1 + sin(_Time.y * _Speed.x + _Speed.y)) / 2;
                    #endif

                    #ifdef UNITY_UI_MASKING
                        color.a *= (tex2D(_MaskTex, IN.texcoord)).x;
                    #endif

                    #ifdef UNITY_UI_OUTLINE
                        if (_OutlineSpread > 0 && color.a < 0.5) color = light(IN);
                    #endif

                    clip( color.a - 0.001 );

                    #ifdef UNITY_UI_WHITE
                        return (1.0, 1.0, 1.0, color.a);
                    #endif
                    return color;
                }

                fixed4 SpriteFrag(v2f IN) : SV_Target
                {
                    fixed4 c = SampleSpriteTexture(IN.texcoord) * IN.color;
                    c.rgb *= c.a;
                    return c;
                }

            #endif

        ENDCG
        }
    }
}

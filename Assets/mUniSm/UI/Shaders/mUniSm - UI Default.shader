// Unity built-in shader source. Copyright (c) 2016 Unity Technologies. MIT license (see license.txt)
// Edit by mUniSm @ mathru.net

Shader "mUniSm/UI Default"
{
    Properties
    {
        [PerRendererData] _MainTex("Sprite Texture", 2D) = "white" {}
        [PerRendererData] _MaskTex("Mask Texture", 2D) = "white" {}
        _TransitionTex("Transition Texture", 2D) = "white" {}

        _StencilComp("Stencil Comparison", Float) = 8
        _Stencil("Stencil ID", Float) = 0
        _StencilOp("Stencil Operation", Float) = 0
        _StencilWriteMask("Stencil Write Mask", Float) = 255
        _StencilReadMask("Stencil Read Mask", Float) = 255

        _Color("Tint", Color) = (1, 1, 1, 1)
        _ColorMask("Color Mask", Float) = 15
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

        Stencil
        {
            Ref[_Stencil]
            Comp[_StencilComp]
            Pass[_StencilOp]
            ReadMask[_StencilReadMask]
            WriteMask[_StencilWriteMask]
        }

        Cull Off
        Lighting Off
        ZWrite Off
        ZTest[unity_GUIZTestMode]
        Blend SrcAlpha OneMinusSrcAlpha
        ColorMask[_ColorMask]

        Pass
        {
        Name "Default"
        CGPROGRAM

            #pragma vertex vert
            #pragma fragment frag
            #pragma target 3.0

            #include "UnityCG.cginc"

            #pragma multi_compile __ UNITY_UI_MASKING
            #pragma multi_compile __ UNITY_UI_SOFTCLIP
            #pragma multi_compile __ UNITY_UI_WHITE
            #pragma multi_compile __ UNITY_UI_TRANSITION
            #pragma multi_compile __ UNITY_UI_OUTLINE
            #pragma multi_compile __ UNITY_UI_UVANIMATION
            #pragma multi_compile __ UNITY_UI_BLINK

            #ifndef UNITY_VERTEX_INPUT_INSTANCE_ID
                #define UNITY_VERTEX_INPUT_INSTANCE_ID
                #define UNITY_VERTEX_OUTPUT_STEREO
                #define UNITY_SETUP_INSTANCE_ID(v)
                #define UNITY_INITIALIZE_VERTEX_OUTPUT_STEREO(i)
                #define UnityObjectToClipPos(v) UnityObjectToClipPos(v)
            #endif

            struct appdata_t
            {
                float4 vertex   : POSITION;
                float4 color    : COLOR;
                float2 texcoord : TEXCOORD0;
                UNITY_VERTEX_INPUT_INSTANCE_ID
            };

            struct v2f
            {
                fixed4 color : COLOR;
                float2 texcoord  : TEXCOORD0;
                UNITY_VERTEX_OUTPUT_STEREO
            };

            int _Stencil;
            fixed4 _Color;
            fixed4 _TextureSampleAdd;

            fixed _Alpha;
            fixed4 _RectClip;

            fixed4 fragColor;
            fixed4 scrPos;
            fixed4 stepVal;

            sampler2D _MainTex;

            #ifdef UNITY_UI_MASKING
                sampler2D _MaskTex;
            #endif

            #ifdef UNITY_UI_SOFTCLIP
                fixed4 smoothVal;
                int _SoftCount;
                fixed4 _SoftClipInner[4];
                fixed4 _SoftClipOuter[4];
            #endif

            #ifdef UNITY_UI_TRANSITION
                sampler2D _TransitionTex;
                fixed _TransitionAmount;
            #endif

            #ifdef UNITY_UI_UVANIMATION
                fixed2 _Speed;
            #elif UNITY_UI_BLINK
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

            v2f vert(appdata_t IN, out float4 outpos : SV_POSITION)
            {
                v2f OUT;
                UNITY_SETUP_INSTANCE_ID(IN);
                UNITY_INITIALIZE_VERTEX_OUTPUT_STEREO(OUT);
                outpos = UnityObjectToClipPos(IN.vertex);

                #ifdef UNITY_UI_UVANIMATION
                    OUT.texcoord = IN.texcoord + ( _Speed * _Time );
                #else
                    OUT.texcoord = IN.texcoord;
                #endif

                OUT.color = IN.color * _Color;
                OUT.color.a *= 1.0 - _Alpha;
                return OUT;
            }

            fixed4 frag(v2f IN, UNITY_VPOS_TYPE screenPos : VPOS) : SV_Target
            {

                fragColor = (tex2D(_MainTex, IN.texcoord) + _TextureSampleAdd) * IN.color;

                #ifdef UNITY_UI_UVANIMATION
                    fragColor.a *= fragColor.x;
                #elif UNITY_UI_BLINK
                    fragColor.a *= (1 + sin(_Time.y * _Speed.x + _Speed.y)) / 2;
                #endif

                #ifdef UNITY_UI_MASKING
                    fragColor.a *= (tex2D(_MaskTex, IN.texcoord)).x;
                #endif

                #ifdef UNITY_UI_OUTLINE
                    if (_OutlineSpread > 0 && fragColor.a < 0.5) fragColor = light( IN );
                #endif

                if (_Stencil == 0) {
                    fixed4 scrPos = fixed4(-screenPos.xy, screenPos.xy);
                    stepVal = step(scrPos, _RectClip);
                    fragColor.a *= stepVal.x * stepVal.y * stepVal.z * stepVal.w;

                    #ifdef UNITY_UI_SOFTCLIP
                    for (int i = 0; i < 4; i++) {
                        if (_SoftCount <= i) break;
                        smoothVal = smoothstep(_SoftClipInner[i], _SoftClipOuter[i], scrPos);
                        fragColor.a *= (1 - smoothVal.x) * (1 - smoothVal.z) * (1 - smoothVal.y) * (1 - smoothVal.w);
                    }
                    #endif

                }

                #ifdef UNITY_UI_TRANSITION
                    fragColor.a *= (tex2D(_TransitionTex, IN.texcoord)).x - 1 + _TransitionAmount * 2;
                #endif

                clip( fragColor.a - 0.001 );

                #ifdef UNITY_UI_WHITE
                    return (1.0, 1.0, 1.0, fragColor.a);
                #endif

                return fragColor;
            }
        ENDCG
        }
    }
}
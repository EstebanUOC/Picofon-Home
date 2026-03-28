Shader "Custom/CircleRevealTransition"
{
    Properties
    {
        _Color ("Color", Color) = (0,0,0,1)
        _Radius ("Radius", Range(0, 1.20)) = 0
        _Center ("Center", Vector) = (0.5, 0.5, 0, 0)
    }

    SubShader
    {
        Blend SrcAlpha OneMinusSrcAlpha

        Pass
        {
            HLSLPROGRAM
            #pragma vertex vert
            #pragma fragment frag

            #include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Core.hlsl"

            struct Attributes
            {
                float4 positionOS : POSITION;
                float2 uv : TEXCOORD0;
            };

            struct Varyings
            {
                float4 positionHCS : SV_POSITION;
                float2 uv : TEXCOORD0;
            };

            CBUFFER_START(UnityPerMaterial)
                half4 _Color;
                half _Radius;
                half _Feather;
                half4 _Center;
            CBUFFER_END

            Varyings vert(Attributes v)
            {
                Varyings o;
                o.positionHCS = TransformObjectToHClip(v.positionOS.xyz);
                o.uv = v.uv;
                return o;
            }

            half4 frag(Varyings i) : SV_Target
            {
                float2 p = i.uv - _Center.xy;
                p.x *= _ScreenParams.x / _ScreenParams.y;

                float dist2 = dot(p, p);
                float radius2 = _Radius * _Radius;

                half alpha = step(radius2, dist2);

                return half4(_Color.rgb, alpha);
            }
            ENDHLSL
        }
    }
}

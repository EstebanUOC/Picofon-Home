Shader "Custom/PalleteSwap"
{
    Properties
    {
        _MainTex ("Texture", 2D) = "white" {}
        _TargetColor ("Target Color", Color) = (1,1,1,1)
        _ExcludeColor ("Exclude Color", Color) = (0,0,0,0)
        _Tolerance ("Tolerance", Range(0, 0.5)) = 0.001
        _Weight ("Weight", Range(0, 1)) = 1.0
    }

    SubShader
    {
        Tags { "RenderType" = "Transparent" }
        Blend SrcAlpha OneMinusSrcAlpha

        Pass
        {
            HLSLPROGRAM
            #pragma vertex vert
            #pragma fragment frag
            #include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Core.hlsl"

            struct appdata
            {
                float4 vertex : POSITION;
                float2 uv : TEXCOORD0;
            };

            struct v2f
            {
                float2 uv : TEXCOORD0;
                float4 vertex : SV_POSITION;
            };

            sampler2D _MainTex;
            float4 _MainTex_ST;
            float4 _OriginalColor;
            float4 _TargetColor;
            float4 _ExcludeColor;
            float _Tolerance;
            float _Weight;

            v2f vert(appdata v)
            {
                v2f o;
                o.vertex = TransformObjectToHClip(v.vertex);
                o.uv = TRANSFORM_TEX(v.uv, _MainTex);
                return o;
            }

            half4 frag(v2f i) : SV_Target
            {
                half4 col = tex2D(_MainTex, i.uv);

                if (col.a == 0)
                {
                    return half4(0, 0, 0, 0);
                }

                if (col.r + col.g + col.b == 0)
                {
                    return col;
                }

                half4 tol = col - _ExcludeColor;
                if (all(abs(tol.rgb) < _Tolerance))
                {
                    return col;
                }

                float wA = _Weight;
                float wB = 1.0 - wA;
                half4 newColor = (_TargetColor * wA) + (col * wB);

                return newColor;
            }

            ENDHLSL
        }
    }
}

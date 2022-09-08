Shader "Unlit/ColorBar"
{
    SubShader
    {
        Tags { "RenderType"="Opaque" }
        LOD 100

        Pass
        {
            CGPROGRAM
            #pragma vertex vert
            #pragma fragment frag

            #pragma multi_compile_fog

            #include "UnityCG.cginc"

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

            v2f vert (appdata v)
            {
                v2f o;
                o.vertex = UnityObjectToClipPos(v.vertex);
                o.uv = v.uv;
                return o;
            }

            fixed4 frag(v2f i) : SV_Target
            {
                fixed4 col;
                float y = i.uv.y;

                if (y <= 0.25f)
                {
                    col = fixed4(0.0f, lerp(0.0f, 1.0f, (y - 0.0f) / 0.25f), 1.0f, 1.0f);
                }
                else if (y <= 0.5f)
                {
                    col = fixed4(0.0f, 1.0f, lerp(1.0f, 0.0f, (y - 0.25f) / 0.25f), 1.0f);
                }
                else if (y <= 0.75f)
                {
                    col = fixed4(lerp(0.0f, 1.0f, (y - 0.5f) / 0.25f), 1.0f, 0.0f, 1.0f);
                }
                else
                {
                    col = fixed4(1.0f, lerp(1.0f, 0.0f, (y - 0.75f) / 0.25f), 0.0f, 1.0f);
                }

                return col;
            }
            ENDCG
        }
    }
}

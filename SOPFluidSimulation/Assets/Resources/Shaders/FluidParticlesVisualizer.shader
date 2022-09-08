Shader "Unlit/FluidParticlesVisualizer"
{
    SubShader
    {
        Tags { "RenderType"="Opaque" }
        LOD 100

        Pass
        {
            Cull Off
            CGPROGRAM
            #pragma vertex FluidParticlesVisualizerVertex
            #pragma fragment FluidParticlesVisualizerFrag
            // make fog work
            #pragma multi_compile_fog
            #pragma enable_d3d11_debug_symbols

            #include "UnityCG.cginc"
            #include "Lighting.cginc"

            float4x4 GetViewToHClipMatrix()
            {
                return UNITY_MATRIX_P;
            }

            float4x4 GetWorldToViewMatrix()
            {
                return UNITY_MATRIX_V;
            }

            float4 TransformWViewToHClip(float3 positionVS)
            {
                return mul(GetViewToHClipMatrix(), float4(positionVS, 1.0));
            }

            float3 TransformWorldToView(float3 positionWS)
            {
                return mul(GetWorldToViewMatrix(), float4(positionWS, 1.0)).xyz;
            }

            struct Varyings
            {
                float4 positionCS : SV_POSITION;
                float4 col : TEXCOORD0;
                float particlesRadius : VAR_PARTICLESRADIUS_WS;
                nointerpolation float4 sphereCenterVS : VAR_POSITION_VS;
                float2 uv : VAR_SCREEN_UV;
            };

            struct Targets
            {
                float4 fluidDepth : SV_TARGET0;
            };

            uniform float _ParticlesRadius;

            StructuredBuffer<float> ParticlesPos;
            StructuredBuffer<float> ParticlesVel;

            Varyings FluidParticlesVisualizerVertex(uint vertexID : SV_VertexID)
            {
                Varyings output;
                uint particleID = vertexID / 4;
                float3 positionWS = float3(ParticlesPos[3 * particleID], ParticlesPos[3 * particleID + 1], ParticlesPos[3 * particleID + 2]);
                output.sphereCenterVS = float4(TransformWorldToView(positionWS), 1.0f);
                output.particlesRadius = _ParticlesRadius;
                switch (vertexID % 4)
                {
                case 0:
                    output.uv = float2(-1, -1);
                    break;

                case 1:
                    output.uv = float2(-1, 1);
                    break;

                case 2:
                    output.uv = float2(1, 1);
                    break;

                case 3:
                    output.uv = float2(1, -1);
                    break;
                }
                output.positionCS = TransformWViewToHClip(output.sphereCenterVS.xyz + float3(output.particlesRadius * output.uv, 0.0f));

                
                float3 Velocity = float3(ParticlesVel[3 * particleID], ParticlesVel[3 * particleID + 1], ParticlesVel[3 * particleID + 2]);
                float ClampVel = clamp(length(Velocity), 0.0f, 4.0f) / 6.0f;
                output.col = float4(ClampVel, ClampVel, 1.0f, 1.0f);
                return output;
            }

            Targets FluidParticlesVisualizerFrag(Varyings input)
            {
                Targets output;

                float3 normalVS;
                normalVS.xy = input.uv;

                float xy_PlaneProj = dot(normalVS.xy, normalVS.xy);
                if (xy_PlaneProj > 1.0f) discard;
                normalVS.z = sqrt(1.0f - xy_PlaneProj);

                float3 positionVS = input.sphereCenterVS.xyz + normalVS * input.particlesRadius;
                
                //已经处理Flicp_Y 和 Reversed-Z
                float4 positionCS = TransformWViewToHClip(positionVS);
                //output.fluidDepth = positionCS.z / positionCS.w;
                output.fluidDepth = input.col;
                return output;
            }

            ENDCG
        }
    }
}

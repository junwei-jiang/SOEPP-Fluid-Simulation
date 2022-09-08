Shader "Unlit/DrawParticleWithMoreColor"
{
    Properties
    {
        _ParticlesRadius("Particles Radius",Float) = 0.025
        _MaxVelocity("Max Velocity",Range(0.1,20)) = 10
    }

    SubShader
    {
        Tags { "RenderType"="Opaque" }
        LOD 100

        Pass
        {
            Cull Off
            CGPROGRAM
            #pragma vertex GenerateDepthPassVertex
            #pragma fragment SpriteGenerateDepthPassFrag
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

            float _ParticlesRadius;
            float _MaxVelocity;

            StructuredBuffer<float> ParticlesPos;
            StructuredBuffer<float> ParticlesVel;

            Varyings GenerateDepthPassVertex(uint vertexID : SV_VertexID)
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
                float ClampVel = clamp(length(Velocity), 0.0f, _MaxVelocity) / _MaxVelocity;
                if (ClampVel <= 0.25f)
                {
                    output.col = float4(0.0f, lerp(0.0f, 1.0f, (ClampVel - 0.0f) / 0.25f), 1.0f, 1.0f);
                }
                else if (ClampVel <= 0.5f)
                {
                    output.col = float4(0.0f, 1.0f, lerp(1.0f, 0.0f, (ClampVel - 0.25f) / 0.25f), 1.0f);
                }
                else if (ClampVel <= 0.75f)
                {
                    output.col = float4(lerp(0.0f, 1.0f, (ClampVel - 0.5f) / 0.25f), 1.0f, 0.0f, 1.0f);
                }
                else
                {
                    output.col = float4(1.0f, lerp(1.0f, 0.0f, (ClampVel - 0.75f) / 0.25f), 0.0f, 1.0f);
                }
                return output;
            }

            Targets SpriteGenerateDepthPassFrag(Varyings input)
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

        Pass
        {
            Cull Off
            CGPROGRAM
            #pragma vertex SpriteTestPassVert
            #pragma fragment SpriteTestPassFrag
            // make fog work
            #pragma multi_compile_fog
            #pragma enable_d3d11_debug_symbols

            #include "UnityCG.cginc"
            #include "Lighting.cginc"
            #include "ShaderCommon.compute"

            float4x4 GetWorldToHClipMatrix()
            {
                return UNITY_MATRIX_VP;
            }

            float4 TransformWorldToHClip(float3 positionWS)
            {
                return mul(GetWorldToHClipMatrix(), float4(positionWS, 1.0));
            }

            uniform int3 Resolution;
            uniform float3 Origin;
            uniform float3 Spacing;

            struct Targets
            {
                float3 PointColor : SV_Target;
            };

            StructuredBuffer<float> _VectorFieldDataX;
            StructuredBuffer<float> _VectorFieldDataY;
            StructuredBuffer<float> _VectorFieldDataZ;

            StructuredBuffer<float> _FluidDomain;

            struct Varyings
            {
                float4 positionCS : SV_POSITION;
            };

            Varyings SpriteTestPassVert(uint vertexID : SV_VertexID)
            {
                Varyings output;

                int LinearIndex = vertexID / 6;
                int Offset = vertexID % 6;
                int3 CurCoordIndex = transLinearIndex2Coord(LinearIndex, Resolution);
                float3 CurVector = float3(_VectorFieldDataX[LinearIndex], _VectorFieldDataY[LinearIndex], _VectorFieldDataZ[LinearIndex]);
                float3 Position = Origin + 0.5 * Spacing + CurCoordIndex * Spacing;
                float Scale = 0.2;
                float3 CurVectorNorm = Scale * normalize(CurVector);

                switch (Offset)
                {
                case 0:
                    Position = Position;
                    break;
                case 1:
                    Position = Position + CurVectorNorm;
                    break;
                case 2:
                    Position = Position + float3(0, CurVectorNorm.y, 0);
                    break;
                case 3:
                    Position = Position + CurVectorNorm;
                    break;
                case 4:
                    Position = Position + float3(CurVectorNorm.x, 0, CurVectorNorm.z);
                    break;
                case 5:
                    Position = Position + CurVectorNorm;
                    break;
                default:
                    break;
                }

                if (dot(CurVector, CurVector) < 0.1)
                {
                    output.positionCS = float4(1000, 1000, 1000, 1);
                }
                else
                {
                    output.positionCS = TransformWorldToHClip(Position);
                }
                return output;
            }

            Targets SpriteTestPassFrag(Varyings input)
            {
                Targets output;

                output.PointColor = float4(1.0, 0.0, 0.0, 1.0);
                return output;
            }

            ENDCG
        }
    }
}

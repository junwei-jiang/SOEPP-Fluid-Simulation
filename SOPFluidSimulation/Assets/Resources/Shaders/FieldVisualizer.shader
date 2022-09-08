Shader "Unlit/FieldVisualizer"
{
    Properties
    {
        RenderColorMin("RenderColorMin", Color) = (0, 0, 1)
        RenderColorMid("RenderColorMid", Color) = (0, 1, 0)
        RenderColorMax("RenderColorMax", Color) = (1, 0, 0)
        ColorVariationCoefficient("ColorVariationCoefficient", Range(0, 1)) = 0.5
        VisualizeIntervalX("VisualizeIntervalX", int) = 10
        VisualizeIntervalY("VisualizeIntervalY", int) = 10
        VisualizeIntervalZ("VisualizeIntervalZ", int) = 10
        //_EulerParticlesTestRadius("EulerParticles Test Radius", Range(0.01, 0.15)) = 0.08
    }

    SubShader
    {
        Tags { "RenderType"="Opaque" }
        LOD 100

        Pass
        {
            Cull Off
            CGPROGRAM
            #pragma vertex CCSFieldVisualizerVert
            #pragma fragment CCSFieldVisualizerFrag
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

            uniform int3 Resolution_CCSField;
            uniform float3 Origin_CCSField;
            uniform float3 Spacing_CCSField;

            uniform float MaxValue_CCSField;

            float3 RenderColorMin;
            float3 RenderColorMid;
            float3 RenderColorMax;
            float ColorVariationCoefficient;

            StructuredBuffer<float> vScalarFieldData_CCSField;

            struct VertOutput
            {
                float4 PositionCS : SV_POSITION;
                float3 CurScalarValue : NORMAL;
            };

            struct FragOutput
            {
                float3 PointColor : SV_Target;
            };

            VertOutput CCSFieldVisualizerVert(uint vertexID : SV_VertexID)
            {
                VertOutput Output;

                float CubeVertices[] = {
                    //position           //TexCoord
                    -0.5f, -0.5f, -0.5f,  0.0f, 0.0f,
                     0.5f, -0.5f, -0.5f,  1.0f, 0.0f,
                     0.5f,  0.5f, -0.5f,  1.0f, 1.0f,
                     0.5f,  0.5f, -0.5f,  1.0f, 1.0f,
                    -0.5f,  0.5f, -0.5f,  0.0f, 1.0f,
                    -0.5f, -0.5f, -0.5f,  0.0f, 0.0f,

                    -0.5f, -0.5f,  0.5f,  0.0f, 0.0f,
                     0.5f, -0.5f,  0.5f,  1.0f, 0.0f,
                     0.5f,  0.5f,  0.5f,  1.0f, 1.0f,
                     0.5f,  0.5f,  0.5f,  1.0f, 1.0f,
                    -0.5f,  0.5f,  0.5f,  0.0f, 1.0f,
                    -0.5f, -0.5f,  0.5f,  0.0f, 0.0f,

                    -0.5f,  0.5f,  0.5f,  1.0f, 0.0f,
                    -0.5f,  0.5f, -0.5f,  1.0f, 1.0f,
                    -0.5f, -0.5f, -0.5f,  0.0f, 1.0f,
                    -0.5f, -0.5f, -0.5f,  0.0f, 1.0f,
                    -0.5f, -0.5f,  0.5f,  0.0f, 0.0f,
                    -0.5f,  0.5f,  0.5f,  1.0f, 0.0f,

                     0.5f,  0.5f,  0.5f,  1.0f, 0.0f,
                     0.5f,  0.5f, -0.5f,  1.0f, 1.0f,
                     0.5f, -0.5f, -0.5f,  0.0f, 1.0f,
                     0.5f, -0.5f, -0.5f,  0.0f, 1.0f,
                     0.5f, -0.5f,  0.5f,  0.0f, 0.0f,
                     0.5f,  0.5f,  0.5f,  1.0f, 0.0f,

                    -0.5f, -0.5f, -0.5f,  0.0f, 1.0f,
                     0.5f, -0.5f, -0.5f,  1.0f, 1.0f,
                     0.5f, -0.5f,  0.5f,  1.0f, 0.0f,
                     0.5f, -0.5f,  0.5f,  1.0f, 0.0f,
                    -0.5f, -0.5f,  0.5f,  0.0f, 0.0f,
                    -0.5f, -0.5f, -0.5f,  0.0f, 1.0f,

                    -0.5f,  0.5f, -0.5f,  0.0f, 1.0f,
                     0.5f,  0.5f, -0.5f,  1.0f, 1.0f,
                     0.5f,  0.5f,  0.5f,  1.0f, 0.0f,
                     0.5f,  0.5f,  0.5f,  1.0f, 0.0f,
                    -0.5f,  0.5f,  0.5f,  0.0f, 0.0f,
                    -0.5f,  0.5f, -0.5f,  0.0f, 1.0f
                };

                int CurCellLinearIndex = vertexID / 36;
                int CurPrimitivesVertexIndex = vertexID % 36;
                int3 CurCellCoordIndex = transLinearIndex2Coord(CurCellLinearIndex, Resolution_CCSField);

                if (!isFloatValid(vScalarFieldData_CCSField[CurCellLinearIndex]))
                {
                    Output.PositionCS = float4(1000, 1000, 1000, 1);
                    Output.CurScalarValue = float3(1, 1, 1);
                    return Output;
                }

                float3 CurVectexPosition = float3(CubeVertices[CurPrimitivesVertexIndex * 5], CubeVertices[CurPrimitivesVertexIndex * 5 + 1], CubeVertices[CurPrimitivesVertexIndex * 5 + 2]);
                CurVectexPosition = float3(CurVectexPosition.x * Spacing_CCSField.x, CurVectexPosition.y * Spacing_CCSField.y, CurVectexPosition.z * Spacing_CCSField.z);
                float3 CurCellPosition = Origin_CCSField + 0.5 * Spacing_CCSField + CurCellCoordIndex * Spacing_CCSField;

                CurVectexPosition += CurCellPosition;

                //TODO: Visualize Condition
                if (vScalarFieldData_CCSField[CurCellLinearIndex] <= 1e-2f)//== 0
                { 
                    Output.PositionCS = float4(1000, 1000, 1000, 1);
                }
                else
                {
                    Output.PositionCS = TransformWorldToHClip(CurVectexPosition);
                }
                //Output.PositionCS = TransformWorldToHClip(CurVectexPosition);

                Output.CurScalarValue = float3(vScalarFieldData_CCSField[CurCellLinearIndex], 0, 0);
                return Output;
            }

            FragOutput CCSFieldVisualizerFrag(VertOutput vInput)
            {
                FragOutput Output;

                float ColorScale = abs(vInput.CurScalarValue.x) / MaxValue_CCSField;
                if (ColorScale > ColorVariationCoefficient)
                {
                    ColorScale -= ColorVariationCoefficient;
                    ColorScale /= (1 - ColorVariationCoefficient);
                    Output.PointColor = float4(RenderColorMid + ColorScale * (RenderColorMax - RenderColorMid), 1.0);
                }
                else
                {
                    ColorScale /= ColorVariationCoefficient;
                    Output.PointColor = float4(RenderColorMin + ColorScale * (RenderColorMid - RenderColorMin), 1.0);
                }

                //Output.PointColor = float4(1.0, 0.0, 0.0, 1.0);
                return Output;
            }

            ENDCG
            }

        Pass
        {
            Cull Off
            CGPROGRAM
            #pragma vertex CCVFieldVisualizerVert
            #pragma fragment CCVFieldVisualizerFrag
            // make fog work
            #pragma multi_compile_fog
            #pragma enable_d3d11_debug_symbols

            #include "UnityCG.cginc"
            #include "Lighting.cginc"
            #include "ShaderCommon.compute"

            float4x4 getRotateMatrix(float vAngle, const float3 vAxis) 
            {
                float3 Axis = normalize(vAxis);
                float Sin = sin(radians(vAngle));
                float Cos = cos(radians(vAngle));

                float4 Row0 = float4(Axis.x * Axis.x + (1.0f - Axis.x * Axis.x) * Cos, Axis.x * Axis.y * (1.0f - Cos) - Axis.z * Sin, Axis.x * Axis.z * (1.0f - Cos) + Axis.y * Sin, 0);
                float4 Row1 = float4(Axis.x * Axis.y * (1.0f - Cos) + Axis.z * Sin, Axis.y * Axis.y + (1.0f - Axis.y * Axis.y) * Cos, Axis.y * Axis.z * (1.0f - Cos) - Axis.x * Sin, 0);
                float4 Row2 = float4(Axis.x * Axis.z * (1.0f - Cos) - Axis.y * Sin, Axis.y * Axis.z * (1.0f - Cos) + Axis.x * Sin, Axis.z * Axis.z + (1.0f - Axis.z * Axis.z) * Cos, 0);
                float4 Row3 = float4(0, 0, 0, 1);

                float4x4 RotateMatrix = float4x4(Row0, Row1, Row2, Row3);

                return RotateMatrix;
            }

            float4x4 GetWorldToHClipMatrix()
            {
                return UNITY_MATRIX_VP;
            }

            float4 TransformWorldToHClip(float3 positionWS)
            {
                return mul(GetWorldToHClipMatrix(), float4(positionWS, 1.0));
            }

            uniform int3 Resolution_CCVField;
            uniform float3 Origin_CCVField;
            uniform float3 Spacing_CCVField;

            uniform float MaxLengthValue_CCVField;

            float3 RenderColorMin;
            float3 RenderColorMid;
            float3 RenderColorMax;
            float ColorVariationCoefficient;

            int VisualizeIntervalX;
            int VisualizeIntervalY;
            int VisualizeIntervalZ;

            StructuredBuffer<float> vVectorFieldDataX_CCVField;
            StructuredBuffer<float> vVectorFieldDataY_CCVField;
            StructuredBuffer<float> vVectorFieldDataZ_CCVField;

            struct VertOutput
            {
                float4 PositionCS : SV_POSITION;
                float3 CurVectorValue : NORMAL;
            };

            struct FragOutput
            {
                float3 PointColor : SV_Target;
            };

            VertOutput CCVFieldVisualizerVert(uint vertexID : SV_VertexID)
            {
                VertOutput Output;

                int CurCellLinearIndex = vertexID / 6;
                int CurPrimitivesVertexIndex = vertexID % 6;
                int3 CurCellCoordIndex = transLinearIndex2Coord(CurCellLinearIndex, Resolution_CCVField);

                //Judge whether to discard the current cell according to the density required to be visualized
                if (CurCellCoordIndex.x % VisualizeIntervalX != 0 || CurCellCoordIndex.y % VisualizeIntervalY != 0 || CurCellCoordIndex.z % VisualizeIntervalZ != 0)
                {
                    Output.PositionCS = float4(1000, 1000, 1000, 1);
                    Output.CurVectorValue = float3(1, 1, 1);
                    return Output;
                }

                float3 CurCellVectorValue = float3(vVectorFieldDataX_CCVField[CurCellLinearIndex], vVectorFieldDataY_CCVField[CurCellLinearIndex], vVectorFieldDataZ_CCVField[CurCellLinearIndex]);
                //CurCellValueValid:Continue; Other:Return
                if (!isFloatValid(CurCellVectorValue.x) || !isFloatValid(CurCellVectorValue.y) || !isFloatValid(CurCellVectorValue.z))
                {
                    Output.PositionCS = float4(1000, 1000, 1000, 1);
                    Output.CurVectorValue = float3(1, 1, 1);
                    return Output;
                }

                float3 CurCellPosition = Origin_CCVField + 0.5 * Spacing_CCVField + CurCellCoordIndex * Spacing_CCVField;
                float3 CurVectexPosition = CurCellPosition;
                float Scale = 0.5 * length(Spacing_CCVField);
                float3 CurRenderVector = Scale * normalize(CurCellVectorValue);

                float3 MidPoint = CurVectexPosition + 0.5 * CurRenderVector;

                //TODO: The ideal situation is to cross multiply the vertical vector, 
                //but this cannot deal with the case that the visual vector is the vertical vector, 
                //and the 0.001 offset here comes from this
                float3 Axis = cross(CurRenderVector, float3(0.001, 1, 0.001));

                float3 ArrowPointA = mul(getRotateMatrix(45, Axis), float4(MidPoint - CurVectexPosition - CurRenderVector, 1.0));
                float3 ArrowPointB = mul(getRotateMatrix(-45, Axis), float4(MidPoint - CurVectexPosition - CurRenderVector, 1.0));

                ArrowPointA += CurVectexPosition + CurRenderVector;
                ArrowPointB += CurVectexPosition + CurRenderVector;

                switch (CurPrimitivesVertexIndex)
                {
                case 0:
                    CurVectexPosition = CurVectexPosition;
                    break;
                case 1:
                    CurVectexPosition = CurVectexPosition + CurRenderVector;
                    break;
                case 2:
                    CurVectexPosition = ArrowPointA;
                    break;
                case 3:
                    CurVectexPosition = CurVectexPosition + CurRenderVector;
                    break;
                case 4:
                    CurVectexPosition = ArrowPointB;
                    break;
                case 5:
                    CurVectexPosition = CurVectexPosition + CurRenderVector;
                    break;
                default:
                    break;
                }

                //TODO:Visualize Condition
                if (dot(CurCellVectorValue, CurCellVectorValue) < 0.001)
                {
                    Output.PositionCS = float4(1000, 1000, 1000, 1);
                }
                else
                {
                    Output.PositionCS = TransformWorldToHClip(CurVectexPosition);
                }

                Output.CurVectorValue = CurCellVectorValue;
                return Output;
            }

            FragOutput CCVFieldVisualizerFrag(VertOutput vInput)
            {
                FragOutput Output;

                float ColorScale = length(vInput.CurVectorValue) / MaxLengthValue_CCVField;
                if (ColorScale > ColorVariationCoefficient)
                {
                    ColorScale -= ColorVariationCoefficient;
                    ColorScale /= (1 - ColorVariationCoefficient);
                    Output.PointColor = float4(RenderColorMid + ColorScale * (RenderColorMax - RenderColorMid), 1.0);
                }
                else
                {
                    ColorScale /= ColorVariationCoefficient;
                    Output.PointColor = float4(RenderColorMin + ColorScale * (RenderColorMid - RenderColorMin), 1.0);
                }

                return Output;
            }

            ENDCG
        }

        Pass
        {
            Cull Off
            CGPROGRAM
            #pragma vertex FCVFieldVisualizerVert
            #pragma fragment FCVFieldVisualizerFrag
            // make fog work
            #pragma multi_compile_fog
            #pragma enable_d3d11_debug_symbols

            #include "UnityCG.cginc"
            #include "Lighting.cginc"
            #include "ShaderCommon.compute"

            float4x4 getRotateMatrix(float vAngle, const float3 vAxis)
            {
                float3 Axis = normalize(vAxis);
                float Sin = sin(radians(vAngle));
                float Cos = cos(radians(vAngle));

                float4 Row0 = float4(Axis.x * Axis.x + (1.0f - Axis.x * Axis.x) * Cos, Axis.x * Axis.y * (1.0f - Cos) - Axis.z * Sin, Axis.x * Axis.z * (1.0f - Cos) + Axis.y * Sin, 0);
                float4 Row1 = float4(Axis.x * Axis.y * (1.0f - Cos) + Axis.z * Sin, Axis.y * Axis.y + (1.0f - Axis.y * Axis.y) * Cos, Axis.y * Axis.z * (1.0f - Cos) - Axis.x * Sin, 0);
                float4 Row2 = float4(Axis.x * Axis.z * (1.0f - Cos) - Axis.y * Sin, Axis.y * Axis.z * (1.0f - Cos) + Axis.x * Sin, Axis.z * Axis.z + (1.0f - Axis.z * Axis.z) * Cos, 0);
                float4 Row3 = float4(0, 0, 0, 1);

                float4x4 RotateMatrix = float4x4(Row0, Row1, Row2, Row3);

                return RotateMatrix;
            }

            float4x4 GetWorldToHClipMatrix()
            {
                return UNITY_MATRIX_VP;
            }

            float4 TransformWorldToHClip(float3 positionWS)
            {
                return mul(GetWorldToHClipMatrix(), float4(positionWS, 1.0));
            }

            uniform int3 Resolution_FCVField;
            uniform float3 Origin_FCVField;
            uniform float3 Spacing_FCVField;

            uniform float MaxLengthValue_FCVField;

            float3 RenderColorMin;
            float3 RenderColorMid;
            float3 RenderColorMax;
            float ColorVariationCoefficient;

            int VisualizeIntervalX;
            int VisualizeIntervalY;
            int VisualizeIntervalZ;

            StructuredBuffer<float> vVectorFieldDataX_FCVField;
            StructuredBuffer<float> vVectorFieldDataY_FCVField;
            StructuredBuffer<float> vVectorFieldDataZ_FCVField;

            struct VertOutput
            {
                float4 PositionCS : SV_POSITION;
                float3 CurVectorValue : NORMAL;
            };

            struct FragOutput
            {
                float3 PointColor : SV_Target;
            };

            VertOutput FCVFieldVisualizerVert(uint vertexID : SV_VertexID)
            {
                VertOutput Output;

                int3 ResolutionX = Resolution_FCVField + int3(1, 0, 0);
                int3 ResolutionY = Resolution_FCVField + int3(0, 1, 0);
                int3 ResolutionZ = Resolution_FCVField + int3(0, 0, 1);

                int CurCellLinearIndex = vertexID / 6;
                int CurPrimitivesVertexIndex = vertexID % 6;
                int3 CurCellCoordIndex = transLinearIndex2Coord(CurCellLinearIndex, Resolution_FCVField);

                //Judge whether to discard the current cell according to the density required to be visualized
                if (CurCellCoordIndex.x % VisualizeIntervalX != 0 || CurCellCoordIndex.y % VisualizeIntervalY != 0 || CurCellCoordIndex.z % VisualizeIntervalZ != 0)
                {
                    Output.PositionCS = float4(1000, 1000, 1000, 1);
                    Output.CurVectorValue = float3(1, 1, 1);
                    return Output;
                }

                float Left = vVectorFieldDataX_FCVField[transCoordIndex2LinearWithOffset(CurCellCoordIndex, ResolutionX, int3(0, 0, 0))];
                float Right = vVectorFieldDataX_FCVField[transCoordIndex2LinearWithOffset(CurCellCoordIndex, ResolutionX, int3(1, 0, 0))];
                float Down = vVectorFieldDataY_FCVField[transCoordIndex2LinearWithOffset(CurCellCoordIndex, ResolutionY, int3(0, 0, 0))];
                float Up = vVectorFieldDataY_FCVField[transCoordIndex2LinearWithOffset(CurCellCoordIndex, ResolutionY, int3(0, 1, 0))];
                float Back = vVectorFieldDataZ_FCVField[transCoordIndex2LinearWithOffset(CurCellCoordIndex, ResolutionZ, int3(0, 0, 0))];
                float Front = vVectorFieldDataZ_FCVField[transCoordIndex2LinearWithOffset(CurCellCoordIndex, ResolutionZ, int3(0, 0, 1))];

                float3 CurCellVectorValue = float3(0.5 * (Left + Right), 0.5 * (Down + Up), 0.5 * (Back + Front));
                //CurCellValueValid:Continue; Other:Return
                if (!isFloatValid(Left) || !isFloatValid(Right) || !isFloatValid(Down) || !isFloatValid(Up) || !isFloatValid(Back) || !isFloatValid(Front))
                {
                    Output.PositionCS = float4(1000, 1000, 1000, 1);
                    Output.CurVectorValue = float3(1, 1, 1);
                    return Output;
                }

                float3 CurVectexPosition = Origin_FCVField + 0.5 * Spacing_FCVField + CurCellCoordIndex * Spacing_FCVField;
                float Scale = 0.5 * length(Spacing_FCVField);
                float3 CurRenderVector = Scale * normalize(CurCellVectorValue);

                float3 MidPoint = CurVectexPosition + 0.5 * CurRenderVector;

                //TODO: The ideal situation is to cross multiply the vertical vector, 
                //but this cannot deal with the case that the visual vector is the vertical vector, 
                //and the 0.001 offset here comes from this
                float3 Axis = cross(CurRenderVector, float3(0.001, 1, 0.001));

                float3 ArrowPointA = mul(getRotateMatrix(45, Axis), float4(MidPoint - CurVectexPosition - CurRenderVector, 1.0));
                float3 ArrowPointB = mul(getRotateMatrix(-45, Axis), float4(MidPoint - CurVectexPosition - CurRenderVector, 1.0));

                ArrowPointA += CurVectexPosition + CurRenderVector;
                ArrowPointB += CurVectexPosition + CurRenderVector;

                switch (CurPrimitivesVertexIndex)
                {
                case 0:
                    CurVectexPosition = CurVectexPosition;
                    break;
                case 1:
                    CurVectexPosition = CurVectexPosition + CurRenderVector;
                    break;
                case 2:
                    CurVectexPosition = ArrowPointA;
                    break;
                case 3:
                    CurVectexPosition = CurVectexPosition + CurRenderVector;
                    break;
                case 4:
                    CurVectexPosition = ArrowPointB;
                    break;
                case 5:
                    CurVectexPosition = CurVectexPosition + CurRenderVector;
                    break;
                default:
                    break;
                }

                //Visualize Condition
                if (dot(CurCellVectorValue, CurCellVectorValue) < 0.001)
                {
                    Output.PositionCS = float4(1000, 1000, 1000, 1);
                }
                else
                {
                    Output.PositionCS = TransformWorldToHClip(CurVectexPosition);
                }

                Output.CurVectorValue = CurCellVectorValue;
                return Output;
            }

            FragOutput FCVFieldVisualizerFrag(VertOutput vInput)
            {
                FragOutput Output;

                float ColorScale = length(vInput.CurVectorValue) / MaxLengthValue_FCVField;
                if (ColorScale > ColorVariationCoefficient)
                {
                    ColorScale -= ColorVariationCoefficient;
                    ColorScale /= (1 - ColorVariationCoefficient);
                    Output.PointColor = float4(RenderColorMid + ColorScale * (RenderColorMax - RenderColorMid), 1.0);
                }
                else
                {
                    ColorScale /= ColorVariationCoefficient;
                    Output.PointColor = float4(RenderColorMin + ColorScale * (RenderColorMid - RenderColorMin), 1.0);
                }

                return Output;
            }

            ENDCG
        }
    }
}

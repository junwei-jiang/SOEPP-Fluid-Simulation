using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using EulerFluidEngine;

namespace EulerFluidEngine
{
    public static class CEulerSolverToolInvoker
    {
        private static bool m_Initialized = false;

        private static ComputeShader m_EulerSolverTool = null;

        private static int m_ApplyBuoyancyAKernel = -1;
        private static int m_ApplyBuoyancyBKernel = -1;
        private static int m_DiffuseFieldRedKernel = -1;
        private static int m_DiffuseFieldBlackKernel = -1;
        private static int m_BuildFluidMarkersKernel = -1;
        private static int m_BuildPressureMatrixAKernel = -1;
        private static int m_BuildPressureVectorbKernel = -1;
        private static int m_FdmMatrixVectorMulKernel = -1;
        private static int m_ApplyPotentialGradientKernel = -1;
        private static int m_ApplyPressureGradientKernel = -1;
        private static int m_UpdatePressureKernel = -1;
        private static int m_UpdatePressureGradientKernel = -1;
        private static int m_CorrectPressureBoundaryConditionKernel = -1;
        private static int m_CorrectVelBoundaryConditionKernel = -1;
        private static int m_CalculateKineticEnergyFieldKernel = -1;
        private static int m_AddFluidKernel = -1;
        private static int m_RemoveFluidKernel = -1;
        private static int m_setFluidVelKernel = -1;
        private static int m_BuildExtrapolationMarkersKernel = -1;
        private static int m_ExtrapolatingDataKernel = -1;
        private static int m_BuildExtrapolationMarkersOutwardsKernel = -1;
        private static int m_BuildMarkersAndExtrapolatingDataKernel = -1;
        private static int m_GenerateFluidDomainFromBBoxKernel = -1;
        private static int m_GenerateFluidDomainFromBoundingBoxsKernel = -1;

        public static void init()
        {
            if (m_Initialized) return;

            m_EulerSolverTool = Resources.Load("Shaders/EulerSolverTool") as ComputeShader;

            m_ApplyBuoyancyAKernel = m_EulerSolverTool.FindKernel("applyBuoyancyA");
            m_ApplyBuoyancyBKernel = m_EulerSolverTool.FindKernel("applyBuoyancyB");
            m_DiffuseFieldRedKernel = m_EulerSolverTool.FindKernel("diffuseFieldRed");
            m_DiffuseFieldBlackKernel = m_EulerSolverTool.FindKernel("diffuseFieldBlack");
            m_BuildFluidMarkersKernel = m_EulerSolverTool.FindKernel("buildFluidMarkers");
            m_BuildPressureMatrixAKernel = m_EulerSolverTool.FindKernel("buildPressureMatrixA");
            m_BuildPressureVectorbKernel = m_EulerSolverTool.FindKernel("buildPressureVectorb");
            m_ApplyPotentialGradientKernel = m_EulerSolverTool.FindKernel("applyPotentialGradient");
            m_ApplyPressureGradientKernel = m_EulerSolverTool.FindKernel("applyPressureGradient");
            m_UpdatePressureKernel = m_EulerSolverTool.FindKernel("updatePressure");
            m_UpdatePressureGradientKernel = m_EulerSolverTool.FindKernel("updatePressureGradient");
            m_CorrectPressureBoundaryConditionKernel = m_EulerSolverTool.FindKernel("correctPressureBoundaryCondition");
            m_CorrectVelBoundaryConditionKernel = m_EulerSolverTool.FindKernel("correctVelBoundaryCondition");
            m_CalculateKineticEnergyFieldKernel = m_EulerSolverTool.FindKernel("calculateKineticEnergyField");
            m_AddFluidKernel = m_EulerSolverTool.FindKernel("addFluid");
            m_RemoveFluidKernel = m_EulerSolverTool.FindKernel("removeFluid");
            m_setFluidVelKernel = m_EulerSolverTool.FindKernel("setFluidVel");
            m_FdmMatrixVectorMulKernel = m_EulerSolverTool.FindKernel("fdmMatrixVectorMul");
            m_BuildExtrapolationMarkersKernel = m_EulerSolverTool.FindKernel("buildExtrapolationMarkers");
            m_ExtrapolatingDataKernel = m_EulerSolverTool.FindKernel("extrapolatingData");
            m_BuildExtrapolationMarkersOutwardsKernel = m_EulerSolverTool.FindKernel("buildExtrapolationMarkersOutwards");
            m_BuildMarkersAndExtrapolatingDataKernel = m_EulerSolverTool.FindKernel("buildMarkersAndExtrapolatingData");
            m_GenerateFluidDomainFromBBoxKernel = m_EulerSolverTool.FindKernel("generateFluidDomainFromBBox");
            m_GenerateFluidDomainFromBoundingBoxsKernel = m_EulerSolverTool.FindKernel("generateFluidDomainFromBoundingBoxs");

            m_Initialized = true;
        }

        public static void applyBuoyancyInvoker(float vDeltaT, float vAlpha, float vBeta, CCellCenteredScalarField vDensityField, CCellCenteredScalarField vTemperatureField, CFaceCenteredVectorField voFluidVelField)
        {
            if (!m_Initialized) init();

            Vector3Int Resolution = vDensityField.getResolution();

            int GridTotalThreadNum = (int)(Resolution.x * Resolution.y * Resolution.z);

            m_EulerSolverTool.SetInts("GridResolution", Resolution.x, Resolution.y, Resolution.z);
            m_EulerSolverTool.SetFloat("DeltaT_applyBuoyancyA", vDeltaT);
            m_EulerSolverTool.SetFloat("Alpha_applyBuoyancyA", vAlpha);
            m_EulerSolverTool.SetFloat("Beta_applyBuoyancyA", vBeta);
            m_EulerSolverTool.SetBuffer(m_ApplyBuoyancyAKernel, "vDensityFieldData_applyBuoyancyA", vDensityField.getGridData());
            m_EulerSolverTool.SetBuffer(m_ApplyBuoyancyAKernel, "vTemperatureFieldData_applyBuoyancyA", vTemperatureField.getGridData());
            m_EulerSolverTool.SetBuffer(m_ApplyBuoyancyAKernel, "voFluidVelFieldDataY_applyBuoyancyA", voFluidVelField.getGridDataY());
            CGlobalMacroAndFunc.dispatchKernel(m_EulerSolverTool, m_ApplyBuoyancyAKernel, GridTotalThreadNum);

            m_EulerSolverTool.SetInts("GridResolution", Resolution.x, Resolution.y, Resolution.z);
            m_EulerSolverTool.SetFloat("DeltaT_applyBuoyancyB", vDeltaT);
            m_EulerSolverTool.SetFloat("Alpha_applyBuoyancyB", vAlpha);
            m_EulerSolverTool.SetFloat("Beta_applyBuoyancyB", vBeta);
            m_EulerSolverTool.SetBuffer(m_ApplyBuoyancyBKernel, "vDensityFieldData_applyBuoyancyB", vDensityField.getGridData());
            m_EulerSolverTool.SetBuffer(m_ApplyBuoyancyBKernel, "vTemperatureFieldData_applyBuoyancyB", vTemperatureField.getGridData());
            m_EulerSolverTool.SetBuffer(m_ApplyBuoyancyBKernel, "voFluidVelFieldDataY_applyBuoyancyB", voFluidVelField.getGridDataY());
            CGlobalMacroAndFunc.dispatchKernel(m_EulerSolverTool, m_ApplyBuoyancyBKernel, GridTotalThreadNum);
        }

        public static void diffuseVelFieldInvoker(int vDiffuseNum, Vector3 vDiffuseCoefficient, CFaceCenteredVectorField voFluidVelField, CFaceCenteredVectorField voTempVelField)
        {
            if (!m_Initialized) init();

            voTempVelField.resize(voFluidVelField);

            Vector3Int Resolution = voFluidVelField.getResolution();
            Vector3Int ResolutionX = Resolution + new Vector3Int(1, 0, 0);
            Vector3Int ResolutionY = Resolution + new Vector3Int(0, 1, 0);
            Vector3Int ResolutionZ = Resolution + new Vector3Int(0, 0, 1);

            for(int i = 0; i < vDiffuseNum; i++)
            {
                int TotalThreadNumX = (int)(ResolutionX.x * ResolutionX.y * ResolutionX.z);
                m_EulerSolverTool.SetInts("GridResolution", ResolutionX.x, ResolutionX.y, ResolutionX.z);
                m_EulerSolverTool.SetFloat("DiffuseCoefficient_diffuseFieldRed", vDiffuseCoefficient.x);
                m_EulerSolverTool.SetBuffer(m_DiffuseFieldRedKernel, "vOriginalFieldData_diffuseFieldRed", voFluidVelField.getGridDataX());
                m_EulerSolverTool.SetBuffer(m_DiffuseFieldRedKernel, "voDiffusedFieldData_diffuseFieldRed", voTempVelField.getGridDataX());
                CGlobalMacroAndFunc.dispatchKernel(m_EulerSolverTool, m_DiffuseFieldRedKernel, TotalThreadNumX);
                m_EulerSolverTool.SetInts("GridResolution", ResolutionX.x, ResolutionX.y, ResolutionX.z);
                m_EulerSolverTool.SetFloat("DiffuseCoefficient_diffuseFieldBlack", vDiffuseCoefficient.x);
                m_EulerSolverTool.SetBuffer(m_DiffuseFieldBlackKernel, "vOriginalFieldData_diffuseFieldBlack", voFluidVelField.getGridDataX());
                m_EulerSolverTool.SetBuffer(m_DiffuseFieldBlackKernel, "voDiffusedFieldData_diffuseFieldBlack", voTempVelField.getGridDataX());
                CGlobalMacroAndFunc.dispatchKernel(m_EulerSolverTool, m_DiffuseFieldBlackKernel, TotalThreadNumX);

                int TotalThreadNumY = (int)(ResolutionY.x * ResolutionY.y * ResolutionY.z);
                m_EulerSolverTool.SetInts("GridResolution", ResolutionY.x, ResolutionY.y, ResolutionY.z);
                m_EulerSolverTool.SetFloat("DiffuseCoefficient_diffuseFieldRed", vDiffuseCoefficient.y);
                m_EulerSolverTool.SetBuffer(m_DiffuseFieldRedKernel, "vOriginalFieldData_diffuseFieldRed", voFluidVelField.getGridDataY());
                m_EulerSolverTool.SetBuffer(m_DiffuseFieldRedKernel, "voDiffusedFieldData_diffuseFieldRed", voTempVelField.getGridDataY());
                CGlobalMacroAndFunc.dispatchKernel(m_EulerSolverTool, m_DiffuseFieldRedKernel, TotalThreadNumY);
                m_EulerSolverTool.SetInts("GridResolution", ResolutionY.x, ResolutionY.y, ResolutionY.z);
                m_EulerSolverTool.SetFloat("DiffuseCoefficient_diffuseFieldBlack", vDiffuseCoefficient.y);
                m_EulerSolverTool.SetBuffer(m_DiffuseFieldBlackKernel, "vOriginalFieldData_diffuseFieldBlack", voFluidVelField.getGridDataY());
                m_EulerSolverTool.SetBuffer(m_DiffuseFieldBlackKernel, "voDiffusedFieldData_diffuseFieldBlack", voTempVelField.getGridDataY());
                CGlobalMacroAndFunc.dispatchKernel(m_EulerSolverTool, m_DiffuseFieldBlackKernel, TotalThreadNumY);

                int TotalThreadNumZ = (int)(ResolutionZ.x * ResolutionZ.y * ResolutionZ.z);
                m_EulerSolverTool.SetInts("GridResolution", ResolutionZ.x, ResolutionZ.y, ResolutionZ.z);
                m_EulerSolverTool.SetFloat("DiffuseCoefficient_diffuseFieldRed", vDiffuseCoefficient.z);
                m_EulerSolverTool.SetBuffer(m_DiffuseFieldRedKernel, "vOriginalFieldData_diffuseFieldRed", voFluidVelField.getGridDataZ());
                m_EulerSolverTool.SetBuffer(m_DiffuseFieldRedKernel, "voDiffusedFieldData_diffuseFieldRed", voTempVelField.getGridDataZ());
                CGlobalMacroAndFunc.dispatchKernel(m_EulerSolverTool, m_DiffuseFieldRedKernel, TotalThreadNumZ);
                m_EulerSolverTool.SetInts("GridResolution", ResolutionZ.x, ResolutionZ.y, ResolutionZ.z);
                m_EulerSolverTool.SetFloat("DiffuseCoefficient_diffuseFieldBlack", vDiffuseCoefficient.z);
                m_EulerSolverTool.SetBuffer(m_DiffuseFieldBlackKernel, "vOriginalFieldData_diffuseFieldBlack", voFluidVelField.getGridDataZ());
                m_EulerSolverTool.SetBuffer(m_DiffuseFieldBlackKernel, "voDiffusedFieldData_diffuseFieldBlack", voTempVelField.getGridDataZ());
                CGlobalMacroAndFunc.dispatchKernel(m_EulerSolverTool, m_DiffuseFieldBlackKernel, TotalThreadNumZ);
            }

            voFluidVelField.resize(voTempVelField);
        }

        public static void buildFluidMarkersInvoker(CCellCenteredScalarField vSolidSDFField, CCellCenteredScalarField vFluidSDFField, CCellCenteredScalarField voMarkersField)
        {
            if (!m_Initialized) init();

            CGlobalMacroAndFunc._ASSERTE(vSolidSDFField.getResolution() == voMarkersField.getResolution(), "固体SDF场与标记场的维度不匹配!");
            CGlobalMacroAndFunc._ASSERTE(vFluidSDFField.getResolution() == voMarkersField.getResolution(), "流体SDF场与标记场的维度不匹配!");

            Vector3Int Resolution = vSolidSDFField.getResolution();
            Vector3 Origin = vSolidSDFField.getOrigin();
            Vector3 Spacing = vSolidSDFField.getSpacing();

            int GridTotalThreadNum = (int)(Resolution.x * Resolution.y * Resolution.z);

            m_EulerSolverTool.SetInts("GridResolution", Resolution.x, Resolution.y, Resolution.z);
            m_EulerSolverTool.SetFloats("GridOrigin", Origin.x, Origin.y, Origin.z);
            m_EulerSolverTool.SetFloats("GridSpacing", Spacing.x, Spacing.y, Spacing.z);
            m_EulerSolverTool.SetBuffer(m_BuildFluidMarkersKernel, "vSolidSDFData_buildFluidMarkers", vSolidSDFField.getGridData());
            m_EulerSolverTool.SetBuffer(m_BuildFluidMarkersKernel, "vFluidSDFData_buildFluidMarkers", vFluidSDFField.getGridData());
            m_EulerSolverTool.SetBuffer(m_BuildFluidMarkersKernel, "voMarkersFieldData_buildFluidMarkers", voMarkersField.getGridData());

            CGlobalMacroAndFunc.dispatchKernel(m_EulerSolverTool, m_BuildFluidMarkersKernel, GridTotalThreadNum);
        }

        public static void buildPressureFdmMatrixAInvoker(Vector3Int vResolution, Vector3 vScale, CCellCenteredScalarField vMarkersField, ComputeBuffer voFdmMatrix)
        {
            if (!m_Initialized) init();

            int TotalThreadNum = (int)(vResolution.x * vResolution.y * vResolution.z);

            m_EulerSolverTool.SetInts("GridResolution", vResolution.x, vResolution.y, vResolution.z);
            m_EulerSolverTool.SetFloats("Scale_buildPressureMatrixA", vScale.x, vScale.y, vScale.z);

            m_EulerSolverTool.SetBuffer(m_BuildPressureMatrixAKernel, "vMarkersFieldData_buildPressureMatrixA", vMarkersField.getGridData());
            m_EulerSolverTool.SetBuffer(m_BuildPressureMatrixAKernel, "voFdmMatrixData_buildPressureMatrixA", voFdmMatrix);

            CGlobalMacroAndFunc.dispatchKernel(m_EulerSolverTool, m_BuildPressureMatrixAKernel, TotalThreadNum);
        }

        public static void buildPressureVectorbInvoker
        (
            CFaceCenteredVectorField vFluidVelField,
	        CCellCenteredScalarField vVelDivergenceField,
	        CCellCenteredScalarField vMarkersField,
	        CFaceCenteredVectorField vSolidVelField,
	        ComputeBuffer voVectorb
        )
        {
            if (!m_Initialized) init();

            Vector3Int Resolution = vFluidVelField.getResolution();
            Vector3 Origin = vFluidVelField.getOrigin();
            Vector3 Spacing = vFluidVelField.getSpacing();

            Vector3 Scale = new Vector3(1.0f / Spacing.x, 1.0f / Spacing.y, 1.0f / Spacing.z);

            int TotalThreadNum = (int)(Resolution.x * Resolution.y * Resolution.z);

            m_EulerSolverTool.SetFloats("GridOrigin", Origin.x, Origin.y, Origin.z);
            m_EulerSolverTool.SetFloats("GridSpacing", Spacing.x, Spacing.y, Spacing.z);
            m_EulerSolverTool.SetInts("GridResolution", Resolution.x, Resolution.y, Resolution.z);
            m_EulerSolverTool.SetFloats("Scale_buildPressureVectorb", Scale.x, Scale.y, Scale.z);

            m_EulerSolverTool.SetBuffer(m_BuildPressureVectorbKernel, "voVectorbValue_buildPressureVectorb", voVectorb);
            m_EulerSolverTool.SetBuffer(m_BuildPressureVectorbKernel, "vMarkersFieldData_buildPressureVectorb", vMarkersField.getGridData());
            m_EulerSolverTool.SetBuffer(m_BuildPressureVectorbKernel, "vDivergenceFieldData_buildPressureVectorb", vVelDivergenceField.getGridData());
            m_EulerSolverTool.SetBuffer(m_BuildPressureVectorbKernel, "vFluidVelFieldDataX_buildPressureVectorb", vFluidVelField.getGridDataX());
            m_EulerSolverTool.SetBuffer(m_BuildPressureVectorbKernel, "vFluidVelFieldDataY_buildPressureVectorb", vFluidVelField.getGridDataY());
            m_EulerSolverTool.SetBuffer(m_BuildPressureVectorbKernel, "vFluidVelFieldDataZ_buildPressureVectorb", vFluidVelField.getGridDataZ());
            m_EulerSolverTool.SetBuffer(m_BuildPressureVectorbKernel, "vSolidVelFieldDataX_buildPressureVectorb", vSolidVelField.getGridDataX());
            m_EulerSolverTool.SetBuffer(m_BuildPressureVectorbKernel, "vSolidVelFieldDataY_buildPressureVectorb", vSolidVelField.getGridDataY());
            m_EulerSolverTool.SetBuffer(m_BuildPressureVectorbKernel, "vSolidVelFieldDataZ_buildPressureVectorb", vSolidVelField.getGridDataZ());

            CGlobalMacroAndFunc.dispatchKernel(m_EulerSolverTool, m_BuildPressureVectorbKernel, TotalThreadNum);
        }

        public static void fdmMatrixVectorMulInvoker(Vector3Int vResolution, ComputeBuffer vFdmMatrix, ComputeBuffer vInputVector, ComputeBuffer voOutputVector)
        {
            if (!m_Initialized) init();

            int TotalThreadNum = (int)(vResolution.x * vResolution.y * vResolution.z);

            m_EulerSolverTool.SetInts("GridResolution", vResolution.x, vResolution.y, vResolution.z);
            m_EulerSolverTool.SetInt("TotalThreadNum", TotalThreadNum);

            m_EulerSolverTool.SetBuffer(m_FdmMatrixVectorMulKernel, "vFdmMatrixValue_fdmMatrixVectorMul", vFdmMatrix);
            m_EulerSolverTool.SetBuffer(m_FdmMatrixVectorMulKernel, "vInputVectorValue_fdmMatrixVectorMul", vInputVector);
            m_EulerSolverTool.SetBuffer(m_FdmMatrixVectorMulKernel, "voOutputVectorValue_fdmMatrixVectorMul", voOutputVector);

            CGlobalMacroAndFunc.dispatchKernel(m_EulerSolverTool, m_FdmMatrixVectorMulKernel, TotalThreadNum);
        }

        public static void applyPotentialGradientInvoker
        (
            Vector3Int vResolution,
            Vector3 vScale,
            CCellCenteredScalarField vMarkersField,
	        ComputeBuffer vPotentialFieldData,
            CFaceCenteredVectorField vioFluidVelField,
	        CFaceCenteredVectorField vSolidVelField
        )
        {
            if (!m_Initialized) init();

            int TotalThreadNum = (int)(vResolution.x * vResolution.y * vResolution.z);

            m_EulerSolverTool.SetInts("GridResolution", vResolution.x, vResolution.y, vResolution.z);
            m_EulerSolverTool.SetFloats("Scale_applyPotentialGradient", vScale.x, vScale.y, vScale.z);

            m_EulerSolverTool.SetBuffer(m_ApplyPotentialGradientKernel, "vMarkersFieldData_applyPotentialGradient", vMarkersField.getGridData());
            m_EulerSolverTool.SetBuffer(m_ApplyPotentialGradientKernel, "vPotentialFieldData_applyPotentialGradient", vPotentialFieldData);
            m_EulerSolverTool.SetBuffer(m_ApplyPotentialGradientKernel, "vioFluidVelFieldDataX_applyPotentialGradient", vioFluidVelField.getGridDataX());
            m_EulerSolverTool.SetBuffer(m_ApplyPotentialGradientKernel, "vioFluidVelFieldDataY_applyPotentialGradient", vioFluidVelField.getGridDataY());
            m_EulerSolverTool.SetBuffer(m_ApplyPotentialGradientKernel, "vioFluidVelFieldDataZ_applyPotentialGradient", vioFluidVelField.getGridDataZ());
            m_EulerSolverTool.SetBuffer(m_ApplyPotentialGradientKernel, "vSolidVelFieldDataX_applyPotentialGradient", vSolidVelField.getGridDataX());
            m_EulerSolverTool.SetBuffer(m_ApplyPotentialGradientKernel, "vSolidVelFieldDataY_applyPotentialGradient", vSolidVelField.getGridDataY());
            m_EulerSolverTool.SetBuffer(m_ApplyPotentialGradientKernel, "vSolidVelFieldDataZ_applyPotentialGradient", vSolidVelField.getGridDataZ());

            CGlobalMacroAndFunc.dispatchKernel(m_EulerSolverTool, m_ApplyPotentialGradientKernel, TotalThreadNum);
        }

        public static void applyPressureGradientInvoker
        (
            Vector3Int vResolution,
            Vector3 vScale,
            CCellCenteredScalarField vMarkersField,
            CCellCenteredVectorField vPressureGradientFieldX,
            CCellCenteredVectorField vPressureGradientFieldY,
            CCellCenteredVectorField vPressureGradientFieldZ,
            CFaceCenteredVectorField vioFluidVelField,
            CFaceCenteredVectorField vSolidVelField
        )
        {
            if (!m_Initialized) init();

            int TotalThreadNum = (int)(vResolution.x * vResolution.y * vResolution.z);

            m_EulerSolverTool.SetInts("GridResolution", vResolution.x, vResolution.y, vResolution.z);
            m_EulerSolverTool.SetFloats("Scale_applyPressureGradient", vScale.x, vScale.y, vScale.z);

            m_EulerSolverTool.SetBuffer(m_ApplyPressureGradientKernel, "vMarkersFieldData_applyPressureGradient", vMarkersField.getGridData());
            m_EulerSolverTool.SetBuffer(m_ApplyPressureGradientKernel, "vPressureGradientFieldDataX_applyPressureGradient", vPressureGradientFieldX.getGridDataX());
            m_EulerSolverTool.SetBuffer(m_ApplyPressureGradientKernel, "vPressureGradientFieldDataY_applyPressureGradient", vPressureGradientFieldY.getGridDataY());
            m_EulerSolverTool.SetBuffer(m_ApplyPressureGradientKernel, "vPressureGradientFieldDataZ_applyPressureGradient", vPressureGradientFieldZ.getGridDataZ());
            m_EulerSolverTool.SetBuffer(m_ApplyPressureGradientKernel, "vioFluidVelFieldDataX_applyPressureGradient", vioFluidVelField.getGridDataX());
            m_EulerSolverTool.SetBuffer(m_ApplyPressureGradientKernel, "vioFluidVelFieldDataY_applyPressureGradient", vioFluidVelField.getGridDataY());
            m_EulerSolverTool.SetBuffer(m_ApplyPressureGradientKernel, "vioFluidVelFieldDataZ_applyPressureGradient", vioFluidVelField.getGridDataZ());
            m_EulerSolverTool.SetBuffer(m_ApplyPressureGradientKernel, "vSolidVelFieldDataX_applyPressureGradient", vSolidVelField.getGridDataX());
            m_EulerSolverTool.SetBuffer(m_ApplyPressureGradientKernel, "vSolidVelFieldDataY_applyPressureGradient", vSolidVelField.getGridDataY());
            m_EulerSolverTool.SetBuffer(m_ApplyPressureGradientKernel, "vSolidVelFieldDataZ_applyPressureGradient", vSolidVelField.getGridDataZ());

            CGlobalMacroAndFunc.dispatchKernel(m_EulerSolverTool, m_ApplyPressureGradientKernel, TotalThreadNum);
        }

        public static void updatePressureInvoker
        (
            CCellCenteredScalarField vMarkersField,
            ComputeBuffer vPotentialFieldData,
            CCellCenteredScalarField vPressureField
        )
        {
            if (!m_Initialized) init();

            Vector3Int Resolution = vPressureField.getResolution();

            int TotalThreadNum = (int)(Resolution.x * Resolution.y * Resolution.z);

            m_EulerSolverTool.SetInts("GridResolution", Resolution.x, Resolution.y, Resolution.z);

            m_EulerSolverTool.SetBuffer(m_UpdatePressureKernel, "vMarkersFieldData_updatePressure", vMarkersField.getGridData());
            m_EulerSolverTool.SetBuffer(m_UpdatePressureKernel, "vPotentialFieldData_updatePressure", vPotentialFieldData);
            m_EulerSolverTool.SetBuffer(m_UpdatePressureKernel, "voPressurFieldData_updatePressure", vPressureField.getGridData());
            m_EulerSolverTool.SetBuffer(m_UpdatePressureKernel, "vPressurFieldDataBackup_updatePressure", vPressureField.getGridData());

            CGlobalMacroAndFunc.dispatchKernel(m_EulerSolverTool, m_UpdatePressureKernel, TotalThreadNum);
        }

        public static void updatePressureGradientInvoker
        (
            CCellCenteredScalarField vMarkersField,
            CCellCenteredVectorField vTempPressureGradientField,
            CCellCenteredVectorField voPressureGradientField
        )
        {
            if (!m_Initialized) init();

            Vector3Int Resolution = voPressureGradientField.getResolution();

            int TotalThreadNum = (int)(Resolution.x * Resolution.y * Resolution.z);

            m_EulerSolverTool.SetInts("GridResolution", Resolution.x, Resolution.y, Resolution.z);

            m_EulerSolverTool.SetBuffer(m_UpdatePressureGradientKernel, "vMarkersFieldData_updatePressureGradient", vMarkersField.getGridData());
            m_EulerSolverTool.SetBuffer(m_UpdatePressureGradientKernel, "vTempPressurGradientFieldDataX_updatePressureGradient", vTempPressureGradientField.getGridDataX());
            m_EulerSolverTool.SetBuffer(m_UpdatePressureGradientKernel, "vTempPressurGradientFieldDataY_updatePressureGradient", vTempPressureGradientField.getGridDataY());
            m_EulerSolverTool.SetBuffer(m_UpdatePressureGradientKernel, "vTempPressurGradientFieldDataZ_updatePressureGradient", vTempPressureGradientField.getGridDataZ());
            m_EulerSolverTool.SetBuffer(m_UpdatePressureGradientKernel, "voPressurGradientFieldDataX_updatePressureGradient", voPressureGradientField.getGridDataX());
            m_EulerSolverTool.SetBuffer(m_UpdatePressureGradientKernel, "voPressurGradientFieldDataY_updatePressureGradient", voPressureGradientField.getGridDataY());
            m_EulerSolverTool.SetBuffer(m_UpdatePressureGradientKernel, "voPressurGradientFieldDataZ_updatePressureGradient", voPressureGradientField.getGridDataZ());

            CGlobalMacroAndFunc.dispatchKernel(m_EulerSolverTool, m_UpdatePressureGradientKernel, TotalThreadNum);
        }

        public static void correctPressureBoundaryCondition
        (
            Vector3Int vResolution,
            CCellCenteredScalarField vMarkersField,
            CCellCenteredScalarField vPressureField,
            RedBlack vRBMarker
        )
        {
            if (!m_Initialized) init();

            int TotalThreadNum = (int)(vResolution.x * vResolution.y * vResolution.z);
            int RBMarker = (int)(vRBMarker);
            m_EulerSolverTool.SetInts("GridResolution", vResolution.x, vResolution.y, vResolution.z);

            m_EulerSolverTool.SetBuffer(m_CorrectPressureBoundaryConditionKernel, "vMarkersFieldData_correctPressureBoundaryCondition", vMarkersField.getGridData());
            m_EulerSolverTool.SetBuffer(m_CorrectPressureBoundaryConditionKernel, "vioFluidPressureFieldData_correctPressureBoundaryCondition", vPressureField.getGridData());
            m_EulerSolverTool.SetInt("RBMarker_correctPressureBoundaryCondition", RBMarker);
            CGlobalMacroAndFunc.dispatchKernel(m_EulerSolverTool, m_CorrectPressureBoundaryConditionKernel, TotalThreadNum);
        }

        public static void correctVelBoundaryCondition
        (
            Vector3Int vResolution,
            CCellCenteredScalarField vMarkersField,
            CFaceCenteredVectorField vioFluidVelField,
            CFaceCenteredVectorField vSolidVelField
        )
        {
            if (!m_Initialized) init();

            int TotalThreadNum = (int)(vResolution.x * vResolution.y * vResolution.z);
            m_EulerSolverTool.SetInts("GridResolution", vResolution.x, vResolution.y, vResolution.z);

            m_EulerSolverTool.SetBuffer(m_CorrectVelBoundaryConditionKernel, "vMarkersFieldData_correctVelBoundaryCondition", vMarkersField.getGridData());
            m_EulerSolverTool.SetBuffer(m_CorrectVelBoundaryConditionKernel, "vioFluidVelFieldDataX_correctVelBoundaryCondition", vioFluidVelField.getGridDataX());
            m_EulerSolverTool.SetBuffer(m_CorrectVelBoundaryConditionKernel, "vioFluidVelFieldDataY_correctVelBoundaryCondition", vioFluidVelField.getGridDataY());
            m_EulerSolverTool.SetBuffer(m_CorrectVelBoundaryConditionKernel, "vioFluidVelFieldDataZ_correctVelBoundaryCondition", vioFluidVelField.getGridDataZ());
            m_EulerSolverTool.SetBuffer(m_CorrectVelBoundaryConditionKernel, "vSolidVelFieldDataX_correctVelBoundaryConditiont", vSolidVelField.getGridDataX());
            m_EulerSolverTool.SetBuffer(m_CorrectVelBoundaryConditionKernel, "vSolidVelFieldDataY_correctVelBoundaryCondition", vSolidVelField.getGridDataY());
            m_EulerSolverTool.SetBuffer(m_CorrectVelBoundaryConditionKernel, "vSolidVelFieldDataZ_correctVelBoundaryCondition", vSolidVelField.getGridDataZ());
            CGlobalMacroAndFunc.dispatchKernel(m_EulerSolverTool, m_CorrectVelBoundaryConditionKernel, TotalThreadNum);
        }
        
        public static void calculateKineticEnergyField
        (
            Vector3Int vResolution,
            Vector3 vGridOrigin,
            Vector3 vGridSpacing,
            Vector3 vXYZMin,
            Vector3 vXYZMax,
            CCellCenteredScalarField vMarkersField,
            CCellCenteredVectorField vFluidVelField,
            CCellCenteredScalarField vioKineticEnergyField
        )
        {
            if (!m_Initialized) init();

            int TotalThreadNum = (int)(vResolution.x * vResolution.y * vResolution.z);
            m_EulerSolverTool.SetInts("GridResolution", vResolution.x, vResolution.y, vResolution.z);
            m_EulerSolverTool.SetFloats("GridOrigin", vGridOrigin.x, vGridOrigin.y, vGridOrigin.z);
            m_EulerSolverTool.SetFloats("GridSpacing", vGridSpacing.x, vGridSpacing.y, vGridSpacing.z);

            m_EulerSolverTool.SetBuffer(m_CalculateKineticEnergyFieldKernel, "vMarkersFieldData_calculateKineticEnergyField", vMarkersField.getGridData());
            m_EulerSolverTool.SetFloats("vXYZMin_calculateKineticEnergyField", vXYZMin.x, vXYZMin.y, vXYZMin.z);
            m_EulerSolverTool.SetFloats("vXYZMax_calculateKineticEnergyField", vXYZMax.x, vXYZMax.y, vXYZMax.z);

            m_EulerSolverTool.SetBuffer(m_CalculateKineticEnergyFieldKernel, "vFluidVelFieldDataX_calculateKineticEnergyField", vFluidVelField.getGridDataX());
            m_EulerSolverTool.SetBuffer(m_CalculateKineticEnergyFieldKernel, "vFluidVelFieldDataY_calculateKineticEnergyField", vFluidVelField.getGridDataY());
            m_EulerSolverTool.SetBuffer(m_CalculateKineticEnergyFieldKernel, "vFluidVelFieldDataZ_calculateKineticEnergyField", vFluidVelField.getGridDataZ());
            m_EulerSolverTool.SetBuffer(m_CalculateKineticEnergyFieldKernel, "voFluidKineticEnergyField_calculateKineticEnergyField", vioKineticEnergyField.getGridData());
            CGlobalMacroAndFunc.dispatchKernel(m_EulerSolverTool, m_CalculateKineticEnergyFieldKernel, TotalThreadNum);
        }

        public static void addFluidInvoker
        (
            Vector3Int vResolution,
            CCellCenteredScalarField vioFluidSDFField
        )
        {
            if (!m_Initialized) init();

            int TotalThreadNum = (int)(vResolution.x * vResolution.y * vResolution.z);
            m_EulerSolverTool.SetInts("GridResolution", vResolution.x, vResolution.y, vResolution.z);

            m_EulerSolverTool.SetBuffer(m_AddFluidKernel, "vioFluidSDFField_addFluid", vioFluidSDFField.getGridData());
            CGlobalMacroAndFunc.dispatchKernel(m_EulerSolverTool, m_AddFluidKernel, TotalThreadNum);
        }

        public static void removeFluidInvoker
        (
            Vector3Int vResolution,
            CCellCenteredScalarField vioFluidSDFField
        )
        {
            if (!m_Initialized) init();

            int TotalThreadNum = (int)(vResolution.x * vResolution.y * vResolution.z);
            m_EulerSolverTool.SetInts("GridResolution", vResolution.x, vResolution.y, vResolution.z);

            m_EulerSolverTool.SetBuffer(m_RemoveFluidKernel, "vioFluidSDFField_removeFluid", vioFluidSDFField.getGridData());
            CGlobalMacroAndFunc.dispatchKernel(m_EulerSolverTool, m_RemoveFluidKernel, TotalThreadNum);
        }

        public static void setFluidVelInvoker
        (
            Vector3Int vResolution,
            CFaceCenteredVectorField vioFluidVelField,
            float vInletFluidVel
        )
        {
            if (!m_Initialized) init();

            int TotalThreadNum = (int)(vResolution.x * vResolution.y * vResolution.z);
            m_EulerSolverTool.SetInts("GridResolution", vResolution.x, vResolution.y, vResolution.z);

            m_EulerSolverTool.SetBuffer(m_setFluidVelKernel, "vioFluidVelFieldDataX_setFluidVel", vioFluidVelField.getGridDataX());
            m_EulerSolverTool.SetFloat("VelDataX_setFluidVel", vInletFluidVel);
            CGlobalMacroAndFunc.dispatchKernel(m_EulerSolverTool, m_setFluidVelKernel, TotalThreadNum);
        }

        public static void extrapolatingDataInvoker(CFaceCenteredVectorField vioVectorField, CFaceCenteredVectorField voDisMarkersField, int vExtrapolationDistance)
        {
            if (!m_Initialized) init();

            Vector3Int Resolution = vioVectorField.getResolution();

            CGlobalMacroAndFunc._ASSERTE(Resolution == voDisMarkersField.getResolution(), "外插向量场和对应标记场维度不同！");

            Vector3Int ResolutionX = Resolution + new Vector3Int(1, 0, 0);
            Vector3Int ResolutionY = Resolution + new Vector3Int(0, 1, 0);
            Vector3Int ResolutionZ = Resolution + new Vector3Int(0, 0, 1);

            int TotalThreadNumX = (int)(ResolutionX.x * ResolutionX.y * ResolutionX.z);
            int ThreadsPerBlockX, BlocksPerGridX;
            CGlobalMacroAndFunc.fetchPropBlockGridSize1D(TotalThreadNumX, out ThreadsPerBlockX, out BlocksPerGridX);
            int TotalThreadNumY = (int)(ResolutionY.x * ResolutionY.y * ResolutionY.z);
            int ThreadsPerBlockY, BlocksPerGridY;
            CGlobalMacroAndFunc.fetchPropBlockGridSize1D(TotalThreadNumY, out ThreadsPerBlockY, out BlocksPerGridY);
            int TotalThreadNumZ = (int)(ResolutionZ.x * ResolutionZ.y * ResolutionZ.z);
            int ThreadsPerBlockZ, BlocksPerGridZ;
            CGlobalMacroAndFunc.fetchPropBlockGridSize1D(TotalThreadNumZ, out ThreadsPerBlockZ, out BlocksPerGridZ);

            for (int i = 0; i < vExtrapolationDistance; i++)
            {
                m_EulerSolverTool.SetInt("CurrentDis", i);

                m_EulerSolverTool.SetInts("GridResolution", ResolutionX.x, ResolutionX.y, ResolutionX.z);
                m_EulerSolverTool.SetInt("TotalThreadNum", TotalThreadNumX);
                m_EulerSolverTool.SetBuffer(m_BuildExtrapolationMarkersKernel, "vScalarFieldData", vioVectorField.getGridDataX());
                m_EulerSolverTool.SetBuffer(m_BuildExtrapolationMarkersKernel, "vioDisMarkersFieldData", voDisMarkersField.getGridDataX());
                m_EulerSolverTool.Dispatch(m_BuildExtrapolationMarkersKernel, BlocksPerGridX, 1, 1);

                m_EulerSolverTool.SetInts("GridResolution", ResolutionY.x, ResolutionY.y, ResolutionY.z);
                m_EulerSolverTool.SetInt("TotalThreadNum", TotalThreadNumY);
                m_EulerSolverTool.SetBuffer(m_BuildExtrapolationMarkersKernel, "vScalarFieldData", vioVectorField.getGridDataY());
                m_EulerSolverTool.SetBuffer(m_BuildExtrapolationMarkersKernel, "vioDisMarkersFieldData", voDisMarkersField.getGridDataY());
                m_EulerSolverTool.Dispatch(m_BuildExtrapolationMarkersKernel, BlocksPerGridY, 1, 1);

                m_EulerSolverTool.SetInts("GridResolution", ResolutionZ.x, ResolutionZ.y, ResolutionZ.z);
                m_EulerSolverTool.SetInt("TotalThreadNum", TotalThreadNumZ);
                m_EulerSolverTool.SetBuffer(m_BuildExtrapolationMarkersKernel, "vScalarFieldData", vioVectorField.getGridDataZ());
                m_EulerSolverTool.SetBuffer(m_BuildExtrapolationMarkersKernel, "vioDisMarkersFieldData", voDisMarkersField.getGridDataZ());
                m_EulerSolverTool.Dispatch(m_BuildExtrapolationMarkersKernel, BlocksPerGridZ, 1, 1);
            }

            for (int i = 1; i < vExtrapolationDistance; i++)
            {
                m_EulerSolverTool.SetInt("CurrentDis", i);

                m_EulerSolverTool.SetInts("GridResolution", ResolutionX.x, ResolutionX.y, ResolutionX.z);
                m_EulerSolverTool.SetInt("TotalThreadNum", TotalThreadNumX);
                m_EulerSolverTool.SetBuffer(m_ExtrapolatingDataKernel, "vDisMarkersFieldData", voDisMarkersField.getGridDataX());
                m_EulerSolverTool.SetBuffer(m_ExtrapolatingDataKernel, "vioScalarFieldData", vioVectorField.getGridDataX());
                m_EulerSolverTool.Dispatch(m_ExtrapolatingDataKernel, BlocksPerGridX, 1, 1);

                m_EulerSolverTool.SetInts("GridResolution", ResolutionY.x, ResolutionY.y, ResolutionY.z);
                m_EulerSolverTool.SetInt("TotalThreadNum", TotalThreadNumY);
                m_EulerSolverTool.SetBuffer(m_ExtrapolatingDataKernel, "vDisMarkersFieldData", voDisMarkersField.getGridDataY());
                m_EulerSolverTool.SetBuffer(m_ExtrapolatingDataKernel, "vioScalarFieldData", vioVectorField.getGridDataY());
                m_EulerSolverTool.Dispatch(m_ExtrapolatingDataKernel, BlocksPerGridY, 1, 1);

                m_EulerSolverTool.SetInts("GridResolution", ResolutionZ.x, ResolutionZ.y, ResolutionZ.z);
                m_EulerSolverTool.SetInt("TotalThreadNum", TotalThreadNumZ);
                m_EulerSolverTool.SetBuffer(m_ExtrapolatingDataKernel, "vDisMarkersFieldData", voDisMarkersField.getGridDataZ());
                m_EulerSolverTool.SetBuffer(m_ExtrapolatingDataKernel, "vioScalarFieldData", vioVectorField.getGridDataZ());
                m_EulerSolverTool.Dispatch(m_ExtrapolatingDataKernel, BlocksPerGridZ, 1, 1);
            }
        }

        public static void extrapolatingDataOutwardsInvoker(CFaceCenteredVectorField vioVectorField, CFaceCenteredVectorField voDisMarkersField, int vExtrapolationDistance)
        {
            if (!m_Initialized) init();

            Vector3Int Resolution = vioVectorField.getResolution();

            CGlobalMacroAndFunc._ASSERTE(Resolution == voDisMarkersField.getResolution(), "外插向量场和对应标记场维度不同！");

            Vector3Int ResolutionX = Resolution + new Vector3Int(1, 0, 0);
            Vector3Int ResolutionY = Resolution + new Vector3Int(0, 1, 0);
            Vector3Int ResolutionZ = Resolution + new Vector3Int(0, 0, 1);

            int TotalThreadNumX = (int)(ResolutionX.x * ResolutionX.y * ResolutionX.z);
            int TotalThreadNumY = (int)(ResolutionY.x * ResolutionY.y * ResolutionY.z);
            int TotalThreadNumZ = (int)(ResolutionZ.x * ResolutionZ.y * ResolutionZ.z);

            for (int i = 0; i < vExtrapolationDistance; i++)
            {
                m_EulerSolverTool.SetInt("CurrentDis", i);

                m_EulerSolverTool.SetInts("GridResolution", ResolutionX.x, ResolutionX.y, ResolutionX.z);
                m_EulerSolverTool.SetBuffer(m_BuildExtrapolationMarkersOutwardsKernel, "vScalarFieldData", vioVectorField.getGridDataX());
                m_EulerSolverTool.SetBuffer(m_BuildExtrapolationMarkersOutwardsKernel, "vioDisMarkersFieldData", voDisMarkersField.getGridDataX());
                CGlobalMacroAndFunc.dispatchKernel(m_EulerSolverTool, m_BuildExtrapolationMarkersOutwardsKernel, TotalThreadNumX);

                m_EulerSolverTool.SetInts("GridResolution", ResolutionY.x, ResolutionY.y, ResolutionY.z);
                m_EulerSolverTool.SetBuffer(m_BuildExtrapolationMarkersOutwardsKernel, "vScalarFieldData", vioVectorField.getGridDataY());
                m_EulerSolverTool.SetBuffer(m_BuildExtrapolationMarkersOutwardsKernel, "vioDisMarkersFieldData", voDisMarkersField.getGridDataY());
                CGlobalMacroAndFunc.dispatchKernel(m_EulerSolverTool, m_BuildExtrapolationMarkersOutwardsKernel, TotalThreadNumY);

                m_EulerSolverTool.SetInts("GridResolution", ResolutionZ.x, ResolutionZ.y, ResolutionZ.z);
                m_EulerSolverTool.SetBuffer(m_BuildExtrapolationMarkersOutwardsKernel, "vScalarFieldData", vioVectorField.getGridDataZ());
                m_EulerSolverTool.SetBuffer(m_BuildExtrapolationMarkersOutwardsKernel, "vioDisMarkersFieldData", voDisMarkersField.getGridDataZ());
                CGlobalMacroAndFunc.dispatchKernel(m_EulerSolverTool, m_BuildExtrapolationMarkersOutwardsKernel, TotalThreadNumZ);
            }

            for (int i = 1; i < vExtrapolationDistance; i++)
            {
                m_EulerSolverTool.SetInt("CurrentDis", i);

                m_EulerSolverTool.SetInts("GridResolution", ResolutionX.x, ResolutionX.y, ResolutionX.z);
                m_EulerSolverTool.SetBuffer(m_ExtrapolatingDataKernel, "vDisMarkersFieldData", voDisMarkersField.getGridDataX());
                m_EulerSolverTool.SetBuffer(m_ExtrapolatingDataKernel, "vioScalarFieldData", vioVectorField.getGridDataX());
                CGlobalMacroAndFunc.dispatchKernel(m_EulerSolverTool, m_ExtrapolatingDataKernel, TotalThreadNumX);

                m_EulerSolverTool.SetInts("GridResolution", ResolutionY.x, ResolutionY.y, ResolutionY.z);
                m_EulerSolverTool.SetBuffer(m_ExtrapolatingDataKernel, "vDisMarkersFieldData", voDisMarkersField.getGridDataY());
                m_EulerSolverTool.SetBuffer(m_ExtrapolatingDataKernel, "vioScalarFieldData", vioVectorField.getGridDataY());
                CGlobalMacroAndFunc.dispatchKernel(m_EulerSolverTool, m_ExtrapolatingDataKernel, TotalThreadNumY);

                m_EulerSolverTool.SetInts("GridResolution", ResolutionZ.x, ResolutionZ.y, ResolutionZ.z);
                m_EulerSolverTool.SetBuffer(m_ExtrapolatingDataKernel, "vDisMarkersFieldData", voDisMarkersField.getGridDataZ());
                m_EulerSolverTool.SetBuffer(m_ExtrapolatingDataKernel, "vioScalarFieldData", vioVectorField.getGridDataZ());
                CGlobalMacroAndFunc.dispatchKernel(m_EulerSolverTool, m_ExtrapolatingDataKernel, TotalThreadNumZ);
            }
        }

        public static void buildMarkersAndExtrapolatingDataInvoker(CFaceCenteredVectorField vioVectorField, CFaceCenteredVectorField voDisMarkersField, int vExtrapolationDistance)
        {
            if (!m_Initialized) init();

            Vector3Int Resolution = vioVectorField.getResolution();

            CGlobalMacroAndFunc._ASSERTE(Resolution == voDisMarkersField.getResolution(), "外插向量场和对应标记场维度不同！");

            Vector3Int ResolutionX = Resolution + new Vector3Int(1, 0, 0);
            Vector3Int ResolutionY = Resolution + new Vector3Int(0, 1, 0);
            Vector3Int ResolutionZ = Resolution + new Vector3Int(0, 0, 1);

            int TotalThreadNumX = (int)(ResolutionX.x * ResolutionX.y * ResolutionX.z);
            int TotalThreadNumY = (int)(ResolutionY.x * ResolutionY.y * ResolutionY.z);
            int TotalThreadNumZ = (int)(ResolutionZ.x * ResolutionZ.y * ResolutionZ.z);

            for (int i = 0; i < vExtrapolationDistance; i++)
            {
                m_EulerSolverTool.SetInt("CurrentDis", i);

                m_EulerSolverTool.SetInts("GridResolution", ResolutionX.x, ResolutionX.y, ResolutionX.z);
                m_EulerSolverTool.SetBuffer(m_BuildMarkersAndExtrapolatingDataKernel, "vioScalarFieldData", vioVectorField.getGridDataX());
                m_EulerSolverTool.SetBuffer(m_BuildMarkersAndExtrapolatingDataKernel, "vioDisMarkersFieldData", voDisMarkersField.getGridDataX());
                CGlobalMacroAndFunc.dispatchKernel(m_EulerSolverTool, m_BuildMarkersAndExtrapolatingDataKernel, TotalThreadNumX);

                m_EulerSolverTool.SetInts("GridResolution", ResolutionY.x, ResolutionY.y, ResolutionY.z);
                m_EulerSolverTool.SetBuffer(m_BuildMarkersAndExtrapolatingDataKernel, "vioScalarFieldData", vioVectorField.getGridDataY());
                m_EulerSolverTool.SetBuffer(m_BuildMarkersAndExtrapolatingDataKernel, "vioDisMarkersFieldData", voDisMarkersField.getGridDataY());
                CGlobalMacroAndFunc.dispatchKernel(m_EulerSolverTool, m_BuildMarkersAndExtrapolatingDataKernel, TotalThreadNumY);

                m_EulerSolverTool.SetInts("GridResolution", ResolutionZ.x, ResolutionZ.y, ResolutionZ.z);
                m_EulerSolverTool.SetBuffer(m_BuildMarkersAndExtrapolatingDataKernel, "vioScalarFieldData", vioVectorField.getGridDataZ());
                m_EulerSolverTool.SetBuffer(m_BuildMarkersAndExtrapolatingDataKernel, "vioDisMarkersFieldData", voDisMarkersField.getGridDataZ());
                CGlobalMacroAndFunc.dispatchKernel(m_EulerSolverTool, m_BuildMarkersAndExtrapolatingDataKernel, TotalThreadNumZ);
            }
        }

        public static void generateFluidDomainFromBBoxInvoker
        (
            Vector3 vFluidDomainMin, 
            Vector3 vFluidDomainMax, 
            CCellCenteredScalarField vBoundarysSDFField, 
            CCellCenteredScalarField voFluidDomainField
        )
        {
            if (!m_Initialized) init();

            Vector3Int Resolution = voFluidDomainField.getResolution();
            Vector3 Origin = voFluidDomainField.getOrigin();
            Vector3 Spacing = voFluidDomainField.getSpacing();

            int TotalThreadNum = (int)(Resolution.x * Resolution.y * Resolution.z);

            int ThreadsPerBlock, BlocksPerGrid;
            CGlobalMacroAndFunc.fetchPropBlockGridSize1D(TotalThreadNum, out ThreadsPerBlock, out BlocksPerGrid);

            m_EulerSolverTool.SetFloats("GridOrigin", Origin.x, Origin.y, Origin.z);
            m_EulerSolverTool.SetFloats("GridSpacing", Spacing.x, Spacing.y, Spacing.z);
            m_EulerSolverTool.SetInts("GridResolution", Resolution.x, Resolution.y, Resolution.z);
            m_EulerSolverTool.SetFloats("vFluidDomainMin", vFluidDomainMin.x, vFluidDomainMin.y, vFluidDomainMin.z);
            m_EulerSolverTool.SetFloats("vFluidDomainMax", vFluidDomainMax.x, vFluidDomainMax.y, vFluidDomainMax.z);
            m_EulerSolverTool.SetInt("TotalThreadNum", TotalThreadNum);

            m_EulerSolverTool.SetBuffer(m_GenerateFluidDomainFromBBoxKernel, "vBoundarysSDFFieldData", vBoundarysSDFField.getGridData());
            m_EulerSolverTool.SetBuffer(m_GenerateFluidDomainFromBBoxKernel, "voFluidDomainFieldData", voFluidDomainField.getGridData());

            m_EulerSolverTool.Dispatch(m_GenerateFluidDomainFromBBoxKernel, BlocksPerGrid, 1, 1);
        }

        public static void generateFluidDomainFromBoundingBoxsInvoker
        (
            List<SBoundingBox> vInitialFluidDoamains,
            CCellCenteredScalarField vBoundarysSDFField,
            CCellCenteredScalarField voFluidDomainField
        )
        {
            if (!m_Initialized) init();

            if (vInitialFluidDoamains == null || vInitialFluidDoamains.Count == 0) return;

            Vector3Int Resolution = voFluidDomainField.getResolution();
            Vector3 Origin = voFluidDomainField.getOrigin();
            Vector3 Spacing = voFluidDomainField.getSpacing();
            int NumofBoundingBoxs = vInitialFluidDoamains.Count;

            int TotalThreadNum = (int)(Resolution.x * Resolution.y * Resolution.z);

            ComputeBuffer FluidBoundingBoxsMin = new ComputeBuffer(NumofBoundingBoxs, 3 * sizeof(float));
            ComputeBuffer FluidBoundingBoxsMax = new ComputeBuffer(NumofBoundingBoxs, 3 * sizeof(float));

            float[] BoxsMin = new float[3 * NumofBoundingBoxs], BoxsMax = new float[3 * NumofBoundingBoxs];
            for (int i = 0; i < NumofBoundingBoxs; i++)
            {
                BoxsMin[3 * i] = vInitialFluidDoamains[i].Min.x;
                BoxsMin[3 * i + 1] = vInitialFluidDoamains[i].Min.y;
                BoxsMin[3 * i + 2] = vInitialFluidDoamains[i].Min.z;
                BoxsMax[3 * i] = vInitialFluidDoamains[i].Max.x;
                BoxsMax[3 * i + 1] = vInitialFluidDoamains[i].Max.y;
                BoxsMax[3 * i + 2] = vInitialFluidDoamains[i].Max.z;
            }
            FluidBoundingBoxsMin.SetData(BoxsMin);
            FluidBoundingBoxsMax.SetData(BoxsMax);
            foreach(float num in BoxsMin)
            
            m_EulerSolverTool.SetFloats("GridOrigin", Origin.x, Origin.y, Origin.z);
            m_EulerSolverTool.SetFloats("GridSpacing", Spacing.x, Spacing.y, Spacing.z);
            m_EulerSolverTool.SetInts("GridResolution", Resolution.x, Resolution.y, Resolution.z);
            m_EulerSolverTool.SetInt("vNumofBoundingBoxs", NumofBoundingBoxs);
            m_EulerSolverTool.SetBuffer(m_GenerateFluidDomainFromBoundingBoxsKernel, "vFluidBoundingBoxsMin", FluidBoundingBoxsMin);
            m_EulerSolverTool.SetBuffer(m_GenerateFluidDomainFromBoundingBoxsKernel, "vFluidBoundingBoxsMax", FluidBoundingBoxsMax);
            m_EulerSolverTool.SetBuffer(m_GenerateFluidDomainFromBoundingBoxsKernel, "vBoundarysSDFFieldData", vBoundarysSDFField.getGridData());
            m_EulerSolverTool.SetBuffer(m_GenerateFluidDomainFromBoundingBoxsKernel, "voFluidDomainFieldData", voFluidDomainField.getGridData());

            CGlobalMacroAndFunc.dispatchKernel(m_EulerSolverTool, m_GenerateFluidDomainFromBoundingBoxsKernel, TotalThreadNum);
        }
    }
}
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace EulerFluidEngine
{
    public static class CTaylorVortexInvoker
    {
        private static bool m_Initialized = false;

        private static ComputeShader m_TaylorVortexTool = null;

        private static int m_BuildTaylorVortexMatrixAKernel = -1;
        private static int m_GenerateCurlFieldKernel = -1;
        private static int m_GenerateCurlFieldVortexLeapFroggingKernel = -1;
        private static int m_BuildRhsKernel = -1;
        private static int m_UpdateCurlFieldKernel = -1;
        private static int m_GenerateInitVelFieldXKernel = -1;
        private static int m_GenerateInitVelFieldYKernel = -1;
        private static int m_FillArgumentVelFieldXKernel = -1;
        private static int m_FillArgumentVelFieldYKernel = -1;
        private static int m_GenerateInitVelFieldXYZKernel = -1;
        private static int m_Generate2DCurlKernel = -1;
        private static int m_Generate2DVortKernel = -1;
        private static int m_GenerateDensityFieldVortexLeapFroggingKernel = -1;

        #region Init
        public static void init()
        {
            if (m_Initialized) return;

            m_TaylorVortexTool = Resources.Load("Shaders/TaylorVortex") as ComputeShader;

            m_BuildTaylorVortexMatrixAKernel = m_TaylorVortexTool.FindKernel("buildTaylorVortexMatrixA");
            m_GenerateCurlFieldKernel = m_TaylorVortexTool.FindKernel("generateCurlField");
            m_GenerateCurlFieldVortexLeapFroggingKernel = m_TaylorVortexTool.FindKernel("generateCurlFieldVortexLeapFrogging");
            m_BuildRhsKernel = m_TaylorVortexTool.FindKernel("buildRhs");
            m_UpdateCurlFieldKernel = m_TaylorVortexTool.FindKernel("updateCurlField");
            m_GenerateInitVelFieldXKernel = m_TaylorVortexTool.FindKernel("generateInitVelFieldX");
            m_GenerateInitVelFieldYKernel = m_TaylorVortexTool.FindKernel("generateInitVelFieldY");
            m_GenerateInitVelFieldXYZKernel = m_TaylorVortexTool.FindKernel("generateInitVelFieldXYZ");
            m_FillArgumentVelFieldXKernel = m_TaylorVortexTool.FindKernel("fillArgumentVelFieldX");
            m_FillArgumentVelFieldYKernel = m_TaylorVortexTool.FindKernel("fillArgumentVelFieldY");
            m_Generate2DCurlKernel = m_TaylorVortexTool.FindKernel("generate2DCurl");
            m_Generate2DVortKernel = m_TaylorVortexTool.FindKernel("generate2DVort");
            m_GenerateDensityFieldVortexLeapFroggingKernel = m_TaylorVortexTool.FindKernel("generateDensityFieldVortexLeapFrogging");

            m_Initialized = true;
        }
        #endregion

        public static void buildTaylorVortexMatrixAInvoker(Vector3Int vResolution, Vector3 vScale, ComputeBuffer voFdmMatrixA)
        {
            if (!m_Initialized) init();

            int TotalThreadNum = (int)(vResolution.x * vResolution.y * vResolution.z);

            m_TaylorVortexTool.SetInts("GridResolution", vResolution.x, vResolution.y, vResolution.z);
            m_TaylorVortexTool.SetFloats("Scale_buildTaylorVortexMatrixA", vScale.x, vScale.y, vScale.z);

            m_TaylorVortexTool.SetBuffer(m_BuildTaylorVortexMatrixAKernel, "voFdmMatrixData_buildTaylorVortexMatrixA", voFdmMatrixA);

            CGlobalMacroAndFunc.dispatchKernel(m_TaylorVortexTool, m_BuildTaylorVortexMatrixAKernel, TotalThreadNum);
        }

        public static void generateCurlFieldVortexLeapFroggingInvoker(Vector3Int vResolution, Vector3 vOrigin, Vector3 vSpacing, ComputeBuffer voCurFieldData, float vDistanceA, float vDistanceB)
        {
            if (!m_Initialized) init();

            int TotalThreadNum = (int)(vResolution.x * vResolution.y * vResolution.z);

            m_TaylorVortexTool.SetInts("GridResolution", vResolution.x, vResolution.y, vResolution.z);
            m_TaylorVortexTool.SetFloats("GridSpacing", vSpacing.x, vSpacing.y, vSpacing.z);
            m_TaylorVortexTool.SetFloats("GridOrigin", vOrigin.x, vOrigin.y, vOrigin.z);

            m_TaylorVortexTool.SetFloat("DistanceA_generateCurlFieldVortexLeapFrogging", vDistanceA);
            m_TaylorVortexTool.SetFloat("DistanceB_generateCurlFieldVortexLeapFrogging", vDistanceB);
            m_TaylorVortexTool.SetBuffer(m_GenerateCurlFieldVortexLeapFroggingKernel, "voCurlField_generateCurlFieldVortexLeapFrogging", voCurFieldData);

            CGlobalMacroAndFunc.dispatchKernel(m_TaylorVortexTool, m_GenerateCurlFieldVortexLeapFroggingKernel, TotalThreadNum);
        }

        public static void generateCurlFieldInvoker(Vector3Int vResolution, Vector3 vOrigin, Vector3 vSpacing, ComputeBuffer voCurFieldData, float vDistance)
        {
            if (!m_Initialized) init();

            int TotalThreadNum = (int)(vResolution.x * vResolution.y * vResolution.z);

            m_TaylorVortexTool.SetInts("GridResolution", vResolution.x, vResolution.y, vResolution.z);
            m_TaylorVortexTool.SetFloats("GridSpacing", vSpacing.x, vSpacing.y, vSpacing.z);
            m_TaylorVortexTool.SetFloats("GridOrigin", vOrigin.x, vOrigin.y, vOrigin.z);

            m_TaylorVortexTool.SetFloat("Distance_generateCurlField", vDistance);
            m_TaylorVortexTool.SetBuffer(m_GenerateCurlFieldKernel, "voCurlField_generateCurlField", voCurFieldData);

            CGlobalMacroAndFunc.dispatchKernel(m_TaylorVortexTool, m_GenerateCurlFieldKernel, TotalThreadNum);
        }

        public static void buildRhsInvoker(Vector3Int vResolution, ComputeBuffer vCurFieldData, ComputeBuffer voRhsData)
        {
            if (!m_Initialized) init();

            int TotalThreadNum = (int)(vResolution.x * vResolution.y * vResolution.z);

            m_TaylorVortexTool.SetInts("GridResolution", vResolution.x, vResolution.y, vResolution.z);

            m_TaylorVortexTool.SetBuffer(m_BuildRhsKernel, "vCurlField_buildRhs", vCurFieldData);
            m_TaylorVortexTool.SetBuffer(m_BuildRhsKernel, "voRhsValue_buildRhs", voRhsData);

            CGlobalMacroAndFunc.dispatchKernel(m_TaylorVortexTool, m_BuildRhsKernel, TotalThreadNum);
        }

        public static void updateCurlFieldInvoker(Vector3Int vResolution, ComputeBuffer voCurFieldData, ComputeBuffer vPressureData)
        {
            if (!m_Initialized) init();

            int TotalThreadNum = (int)(vResolution.x * vResolution.y * vResolution.z);

            m_TaylorVortexTool.SetInts("GridResolution", vResolution.x, vResolution.y, vResolution.z);

            m_TaylorVortexTool.SetBuffer(m_UpdateCurlFieldKernel, "vPressureValue_updateCurlField", vPressureData);
            m_TaylorVortexTool.SetBuffer(m_UpdateCurlFieldKernel, "voCurlField_updateCurlField", voCurFieldData);

            CGlobalMacroAndFunc.dispatchKernel(m_TaylorVortexTool, m_UpdateCurlFieldKernel, TotalThreadNum);
        }

        public static void generateInitVelFieldXInvoker(Vector3Int vResolution, Vector3 vSpacing, ComputeBuffer vCurFieldData, ComputeBuffer voInitVelFieldX)
        {
            if (!m_Initialized) init();

            int TotalThreadNum = (int)(vResolution.x * vResolution.y * vResolution.z);

            m_TaylorVortexTool.SetInts("GridResolution", vResolution.x, vResolution.y, vResolution.z);

            m_TaylorVortexTool.SetFloat("vScale_generateInitVelFieldX", 1.0f / vSpacing.x);
            m_TaylorVortexTool.SetBuffer(m_GenerateInitVelFieldXKernel, "vCurlField_generateInitVelFieldX", vCurFieldData);
            m_TaylorVortexTool.SetBuffer(m_GenerateInitVelFieldXKernel, "voVelFieldX_generateInitVelFieldX", voInitVelFieldX);

            CGlobalMacroAndFunc.dispatchKernel(m_TaylorVortexTool, m_GenerateInitVelFieldXKernel, TotalThreadNum);
        }

        public static void generateInitVelFieldYInvoker(Vector3Int vResolution, Vector3 vSpacing, ComputeBuffer vCurFieldData, ComputeBuffer voInitVelFieldY)
        {
            if (!m_Initialized) init();

            int TotalThreadNum = (int)(vResolution.x * vResolution.y * vResolution.z);

            m_TaylorVortexTool.SetInts("GridResolution", vResolution.x, vResolution.y, vResolution.z);

            m_TaylorVortexTool.SetFloat("vScale_generateInitVelFieldY", 1.0f / vSpacing.y);
            m_TaylorVortexTool.SetBuffer(m_GenerateInitVelFieldYKernel, "vCurlField_generateInitVelFieldY", vCurFieldData);
            m_TaylorVortexTool.SetBuffer(m_GenerateInitVelFieldYKernel, "voVelFieldY_generateInitVelFieldY", voInitVelFieldY);

            CGlobalMacroAndFunc.dispatchKernel(m_TaylorVortexTool, m_GenerateInitVelFieldYKernel, TotalThreadNum);
        }

        public static void generateInitVelFieldXYZInvoker(Vector3Int vResolution, Vector3 vSpacing, CCellCenteredVectorField vCurFieldData, CCellCenteredVectorField voInitVelField)
        {
            if (!m_Initialized) init();

            int TotalThreadNum = (int)(vResolution.x * vResolution.y * vResolution.z);

            m_TaylorVortexTool.SetInts("GridResolution", vResolution.x, vResolution.y, vResolution.z);
            m_TaylorVortexTool.SetFloats("GridSpacing", vSpacing.x, vSpacing.y, vSpacing.z);

            m_TaylorVortexTool.SetBuffer(m_GenerateInitVelFieldXYZKernel, "vCurlFieldX_generateInitVelFieldXYZ", vCurFieldData.getGridDataX());
            m_TaylorVortexTool.SetBuffer(m_GenerateInitVelFieldXYZKernel, "vCurlFieldY_generateInitVelFieldXYZ", vCurFieldData.getGridDataY());
            m_TaylorVortexTool.SetBuffer(m_GenerateInitVelFieldXYZKernel, "vCurlFieldZ_generateInitVelFieldXYZ", vCurFieldData.getGridDataZ());
            m_TaylorVortexTool.SetBuffer(m_GenerateInitVelFieldXYZKernel, "voVelFieldX_generateInitVelFieldXYZ", voInitVelField.getGridDataX());
            m_TaylorVortexTool.SetBuffer(m_GenerateInitVelFieldXYZKernel, "voVelFieldY_generateInitVelFieldXYZ", voInitVelField.getGridDataY());
            m_TaylorVortexTool.SetBuffer(m_GenerateInitVelFieldXYZKernel, "voVelFieldZ_generateInitVelFieldXYZ", voInitVelField.getGridDataZ());

            CGlobalMacroAndFunc.dispatchKernel(m_TaylorVortexTool, m_GenerateInitVelFieldXYZKernel, TotalThreadNum);
        }

        //m_generateInitVelFieldXYZKernel

        public static void fillArgumentVelFieldInvoker(CFaceCenteredVectorField vOriginVelField, CFaceCenteredVectorField voArgumentVelField)
        {
            if (!m_Initialized) init();

            Vector3Int Resolution = voArgumentVelField.getResolution();
            Vector3Int ResolutionX = voArgumentVelField.getResolution() + new Vector3Int(1, 0, 0);
            Vector3Int ResolutionY = voArgumentVelField.getResolution() + new Vector3Int(0, 1, 0);

            int TotalThreadNumX = (int)(ResolutionX.x * ResolutionX.y);

            m_TaylorVortexTool.SetInts("GridResolution", ResolutionX.x, ResolutionX.y, ResolutionX.z);

            m_TaylorVortexTool.SetBuffer(m_FillArgumentVelFieldXKernel, "vVelFieldX_fillArgumentVelFieldX", vOriginVelField.getGridDataX());
            m_TaylorVortexTool.SetBuffer(m_FillArgumentVelFieldXKernel, "voVelFieldX_fillArgumentVelFieldX", voArgumentVelField.getGridDataX());

            CGlobalMacroAndFunc.dispatchKernel(m_TaylorVortexTool, m_FillArgumentVelFieldXKernel, TotalThreadNumX);


            int TotalThreadNumY = (int)(ResolutionY.x * ResolutionY.y);

            m_TaylorVortexTool.SetInts("GridResolution", ResolutionY.x, ResolutionY.y, ResolutionY.z);

            m_TaylorVortexTool.SetBuffer(m_FillArgumentVelFieldYKernel, "vVelFieldY_fillArgumentVelFieldY", vOriginVelField.getGridDataY());
            m_TaylorVortexTool.SetBuffer(m_FillArgumentVelFieldYKernel, "voVelFieldY_fillArgumentVelFieldY", voArgumentVelField.getGridDataY());

            CGlobalMacroAndFunc.dispatchKernel(m_TaylorVortexTool, m_FillArgumentVelFieldYKernel, TotalThreadNumY);
        }

        public static void generate2DCurlInvoker(Vector3Int vResolution, Vector3 vSpacing, ComputeBuffer vVelFieldDataX, ComputeBuffer vVelFieldDataY, ComputeBuffer voCurlField)
        {
            if (!m_Initialized) init();

            int TotalThreadNum = (int)(vResolution.x * vResolution.y * vResolution.z);

            m_TaylorVortexTool.SetInts("GridResolution", vResolution.x, vResolution.y, vResolution.z);

            m_TaylorVortexTool.SetFloat("vScale_generate2DCurl", 1.0f / vSpacing.x);
            m_TaylorVortexTool.SetBuffer(m_Generate2DCurlKernel, "vVelFieldX_generate2DCurl", vVelFieldDataX);
            m_TaylorVortexTool.SetBuffer(m_Generate2DCurlKernel, "vVelFieldY_generate2DCurl", vVelFieldDataY);
            m_TaylorVortexTool.SetBuffer(m_Generate2DCurlKernel, "voCurlField_generate2DCurl", voCurlField);

            CGlobalMacroAndFunc.dispatchKernel(m_TaylorVortexTool, m_Generate2DCurlKernel, TotalThreadNum);
        }

        public static void generate2DVortInvoker(CCellCenteredScalarField vCurlField, CCellCenteredScalarField voVortField)
        {
            if (!m_Initialized) init();

            Vector3Int Resolution = voVortField.getResolution();

            int TotalThreadNum = (int)(Resolution.x * Resolution.y * Resolution.z);

            m_TaylorVortexTool.SetInts("GridResolution", Resolution.x, Resolution.y, Resolution.z);

            m_TaylorVortexTool.SetBuffer(m_Generate2DVortKernel, "vCurlField_generate2DVort", vCurlField.getGridData());
            m_TaylorVortexTool.SetBuffer(m_Generate2DVortKernel, "voVortField_generate2DVort", voVortField.getGridData());

            CGlobalMacroAndFunc.dispatchKernel(m_TaylorVortexTool, m_Generate2DVortKernel, TotalThreadNum);
        }

        public static void generateDensityFieldVortexLeapFroggingInvoker(CCellCenteredScalarField voDensityField, float vRho_h, float vRho_w)
        {
            if (!m_Initialized) init();

            Vector3Int Resolution = voDensityField.getResolution();
            Vector3 Origin = voDensityField.getOrigin();
            Vector3 Spacing = voDensityField.getSpacing();

            int TotalThreadNum = (int)(Resolution.x * Resolution.y * Resolution.z);

            m_TaylorVortexTool.SetInts("GridResolution", Resolution.x, Resolution.y, Resolution.z);
            m_TaylorVortexTool.SetFloats("GridSpacing", Spacing.x, Spacing.y, Spacing.z);
            m_TaylorVortexTool.SetFloats("GridOrigin", Origin.x, Origin.y, Origin.z);

            m_TaylorVortexTool.SetFloat("Rho_h_generateDensityFieldVortexLeapFrogging", vRho_h);
            m_TaylorVortexTool.SetFloat("Rho_w_generateDensityFieldVortexLeapFrogging", vRho_w);
            m_TaylorVortexTool.SetBuffer(m_GenerateDensityFieldVortexLeapFroggingKernel, "voDensityField_generateDensityFieldVortexLeapFrogging", voDensityField.getGridData());

            CGlobalMacroAndFunc.dispatchKernel(m_TaylorVortexTool, m_GenerateDensityFieldVortexLeapFroggingKernel, TotalThreadNum);
        }
    }
}
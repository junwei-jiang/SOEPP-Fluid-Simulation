using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace EulerFluidEngine
{
    public static class CVortexBoxInvoer
    {
        private static bool m_Initialized = false;

        private static ComputeShader m_VortexBoxTool = null;

        private static int m_GenerateRhoAndMagFieldKernel = -1;
        private static int m_GenerateInitVelFieldXKernel = -1;
        private static int m_GenerateInitVelFieldYKernel = -1;

        #region Init
        public static void init()
        {
            if (m_Initialized) return;

            m_VortexBoxTool = Resources.Load("Shaders/VortexBox") as ComputeShader;

            m_GenerateRhoAndMagFieldKernel = m_VortexBoxTool.FindKernel("generateRhoAndMagField");
            m_GenerateInitVelFieldXKernel = m_VortexBoxTool.FindKernel("generateInitVelFieldX");
            m_GenerateInitVelFieldYKernel = m_VortexBoxTool.FindKernel("generateInitVelFieldY");

            m_Initialized = true;
        }
        #endregion

        public static void generateRhoAndMagFieldInvoker(float vR, float vCenterX, float vCenterY, CCellCenteredScalarField voRhoField, CCellCenteredScalarField voMagField)
        {
            if (!m_Initialized) init();

            Vector3Int Resolution = voRhoField.getResolution();
            Vector3 Origin = voRhoField.getOrigin();
            Vector3 Spacing = voRhoField.getSpacing();

            int TotalThreadNum = (int)(Resolution.x * Resolution.y * Resolution.z);

            m_VortexBoxTool.SetInts("GridResolution", Resolution.x, Resolution.y, Resolution.z);
            m_VortexBoxTool.SetFloats("GridSpacing", Spacing.x, Spacing.y, Spacing.z);
            m_VortexBoxTool.SetFloats("GridOrigin", Origin.x, Origin.y, Origin.z);

            m_VortexBoxTool.SetFloat("R_generateRhoAndMagField", vR);
            m_VortexBoxTool.SetFloat("CenterX_generateRhoAndMagField", vCenterX);
            m_VortexBoxTool.SetFloat("CenterY_generateRhoAndMagField", vCenterY);
            m_VortexBoxTool.SetBuffer(m_GenerateRhoAndMagFieldKernel, "voRhoField_generateRhoAndMagField", voRhoField.getGridData());
            m_VortexBoxTool.SetBuffer(m_GenerateRhoAndMagFieldKernel, "voMagField_generateRhoAndMagField", voMagField.getGridData());

            CGlobalMacroAndFunc.dispatchKernel(m_VortexBoxTool, m_GenerateRhoAndMagFieldKernel, TotalThreadNum);
        }

        public static void generateInitVelFieldXInvoker(Vector3Int vResolution, Vector3 vOrigin, Vector3 vSpacing, float vNormalize, ComputeBuffer voInitVelFieldX)
        {
            if (!m_Initialized) init();

            int TotalThreadNum = (int)(vResolution.x * vResolution.y * vResolution.z);

            m_VortexBoxTool.SetInts("GridResolution", vResolution.x, vResolution.y, vResolution.z);
            m_VortexBoxTool.SetFloats("GridSpacing", vSpacing.x, vSpacing.y, vSpacing.z);
            m_VortexBoxTool.SetFloats("GridOrigin", vOrigin.x, vOrigin.y, vOrigin.z);

            m_VortexBoxTool.SetFloat("Normalize_generateInitVelFieldX", vNormalize);
            m_VortexBoxTool.SetBuffer(m_GenerateInitVelFieldXKernel, "voVelFieldX_generateInitVelFieldX", voInitVelFieldX);

            CGlobalMacroAndFunc.dispatchKernel(m_VortexBoxTool, m_GenerateInitVelFieldXKernel, TotalThreadNum);
        }

        public static void generateInitVelFieldYInvoker(Vector3Int vResolution, Vector3 vOrigin, Vector3 vSpacing, float vNormalize, ComputeBuffer voInitVelFieldY)
        {
            if (!m_Initialized) init();

            int TotalThreadNum = (int)(vResolution.x * vResolution.y * vResolution.z);

            m_VortexBoxTool.SetInts("GridResolution", vResolution.x, vResolution.y, vResolution.z);
            m_VortexBoxTool.SetFloats("GridSpacing", vSpacing.x, vSpacing.y, vSpacing.z);
            m_VortexBoxTool.SetFloats("GridOrigin", vOrigin.x, vOrigin.y, vOrigin.z);

            m_VortexBoxTool.SetFloat("Normalize_generateInitVelFieldY", vNormalize);
            m_VortexBoxTool.SetBuffer(m_GenerateInitVelFieldYKernel, "voVelFieldY_generateInitVelFieldY", voInitVelFieldY);

            CGlobalMacroAndFunc.dispatchKernel(m_VortexBoxTool, m_GenerateInitVelFieldYKernel, TotalThreadNum);
        }
    }
}
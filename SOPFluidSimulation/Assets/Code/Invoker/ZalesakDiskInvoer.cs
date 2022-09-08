using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace EulerFluidEngine
{
    public static class CZalesakDiskInvoer
    {
        private static bool m_Initialized = false;

        private static ComputeShader m_ZalesakDiskTool = null;

        private static int m_GenerateRhoFieldKernel = -1;
        private static int m_GenerateInitVelFieldXKernel = -1;
        private static int m_GenerateInitVelFieldYKernel = -1;

        #region Init
        public static void init()
        {
            if (m_Initialized) return;

            m_ZalesakDiskTool = Resources.Load("Shaders/ZalesakDisk") as ComputeShader;

            m_GenerateRhoFieldKernel = m_ZalesakDiskTool.FindKernel("generateRhoField");
            m_GenerateInitVelFieldXKernel = m_ZalesakDiskTool.FindKernel("generateInitVelFieldX");
            m_GenerateInitVelFieldYKernel = m_ZalesakDiskTool.FindKernel("generateInitVelFieldY");

            m_Initialized = true;
        }
        #endregion

        public static void generateRhoFieldInvoker(float vR, float vCenterX, float vCenterY, float vWidth, float vHeight, float vRecX, float vRecY, CCellCenteredScalarField voRhoField)
        {
            if (!m_Initialized) init();

            Vector3Int Resolution = voRhoField.getResolution();
            Vector3 Origin = voRhoField.getOrigin();
            Vector3 Spacing = voRhoField.getSpacing();

            int TotalThreadNum = (int)(Resolution.x * Resolution.y * Resolution.z);

            m_ZalesakDiskTool.SetInts("GridResolution", Resolution.x, Resolution.y, Resolution.z);
            m_ZalesakDiskTool.SetFloats("GridSpacing", Spacing.x, Spacing.y, Spacing.z);
            m_ZalesakDiskTool.SetFloats("GridOrigin", Origin.x, Origin.y, Origin.z);

            m_ZalesakDiskTool.SetFloat("R_generateRhoField", vR);
            m_ZalesakDiskTool.SetFloat("CenterX_generateRhoField", vCenterX);
            m_ZalesakDiskTool.SetFloat("CenterY_generateRhoField", vCenterY);
            m_ZalesakDiskTool.SetFloat("Width_generateRhoField", vWidth);
            m_ZalesakDiskTool.SetFloat("Height_generateRhoField", vHeight);
            m_ZalesakDiskTool.SetFloat("RecX_generateRhoField", vRecX);
            m_ZalesakDiskTool.SetFloat("RecY_generateRhoField", vRecY);
            m_ZalesakDiskTool.SetBuffer(m_GenerateRhoFieldKernel, "voRhoField_generateRhoField", voRhoField.getGridData());

            CGlobalMacroAndFunc.dispatchKernel(m_ZalesakDiskTool, m_GenerateRhoFieldKernel, TotalThreadNum);
        }

        public static void generateInitVelFieldXInvoker(Vector3Int vResolution, Vector3 vOrigin, Vector3 vSpacing, ComputeBuffer voInitVelFieldX)
        {
            if (!m_Initialized) init();

            int TotalThreadNum = (int)(vResolution.x * vResolution.y * vResolution.z);

            m_ZalesakDiskTool.SetInts("GridResolution", vResolution.x, vResolution.y, vResolution.z);
            m_ZalesakDiskTool.SetFloats("GridSpacing", vSpacing.x, vSpacing.y, vSpacing.z);
            m_ZalesakDiskTool.SetFloats("GridOrigin", vOrigin.x, vOrigin.y, vOrigin.z);

            m_ZalesakDiskTool.SetBuffer(m_GenerateInitVelFieldXKernel, "voVelFieldX_generateInitVelFieldX", voInitVelFieldX);

            CGlobalMacroAndFunc.dispatchKernel(m_ZalesakDiskTool, m_GenerateInitVelFieldXKernel, TotalThreadNum);
        }

        public static void generateInitVelFieldYInvoker(Vector3Int vResolution, Vector3 vOrigin, Vector3 vSpacing, ComputeBuffer voInitVelFieldY)
        {
            if (!m_Initialized) init();

            int TotalThreadNum = (int)(vResolution.x * vResolution.y * vResolution.z);

            m_ZalesakDiskTool.SetInts("GridResolution", vResolution.x, vResolution.y, vResolution.z);
            m_ZalesakDiskTool.SetFloats("GridSpacing", vSpacing.x, vSpacing.y, vSpacing.z);
            m_ZalesakDiskTool.SetFloats("GridOrigin", vOrigin.x, vOrigin.y, vOrigin.z);

            m_ZalesakDiskTool.SetBuffer(m_GenerateInitVelFieldYKernel, "voVelFieldY_generateInitVelFieldY", voInitVelFieldY);

            CGlobalMacroAndFunc.dispatchKernel(m_ZalesakDiskTool, m_GenerateInitVelFieldYKernel, TotalThreadNum);
        }
    }
}
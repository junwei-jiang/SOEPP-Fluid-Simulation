using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using EulerFluidEngine;

namespace EulerFluidEngine
{
    public static class CBoundaryToolInvoker
    {
        public static void sampleTexture3DWithTransformInvoker
        (
            Texture3D vTexture3D,
            Vector3 vTextureMinPos,
            Vector3 vTextureMaxPos,
            Vector3 vTranslation,
            Vector3 vRotation,
            Vector3 vScale,
            CCellCenteredScalarField voField
        )
        {
            if (!m_Initiated) init();

            Vector3Int Resolution = voField.getResolution();
            Vector3 Origin = voField.getOrigin();
            Vector3 Spacing = voField.getSpacing();

            Vector3 TextureSize = vTextureMaxPos - vTextureMinPos;

            int TotalThreadNum = Resolution.x * Resolution.y * Resolution.z;

            m_BoundaryTool.SetInts("GridResolution", Resolution.x, Resolution.y, Resolution.z);
            m_BoundaryTool.SetFloats("GridOrigin", Origin.x, Origin.y, Origin.z);
            m_BoundaryTool.SetFloats("GridSpacing", Spacing.x, Spacing.y, Spacing.z);
            
            m_BoundaryTool.SetFloats("vTextureMin", vTextureMinPos.x, vTextureMinPos.y, vTextureMinPos.z);
            m_BoundaryTool.SetFloats("vTextureSize", TextureSize.x, TextureSize.y, TextureSize.z);
            m_BoundaryTool.SetTexture(m_SampleTexture3DWithTransformKernel, "vTexture3D", vTexture3D);
            m_BoundaryTool.SetFloats("vTranslation", vTranslation.x, vTranslation.y, vTranslation.z);
            m_BoundaryTool.SetFloats("vRotation", vRotation.x, vRotation.y, vRotation.z);
            m_BoundaryTool.SetFloats("vScale", vScale.x, vScale.y, vScale.z);
            m_BoundaryTool.SetBuffer(m_SampleTexture3DWithTransformKernel, "voFieldData", voField.getGridData());

            CGlobalMacroAndFunc.dispatchKernel(m_BoundaryTool, m_SampleTexture3DWithTransformKernel, TotalThreadNum);
        }

        public static void buildTotalBoundarysSDFInvoker(List<CCellCenteredScalarField> vBoundarysSDF, CCellCenteredScalarField voSolidsMarkerField)
        {
            if (!m_Initiated) init();

            Vector3Int Resolution = voSolidsMarkerField.getResolution();

            for (int i = 0; i < vBoundarysSDF.Count; i++)
            {
                CGlobalMacroAndFunc._ASSERTE(Resolution == vBoundarysSDF[i].getResolution(), "边界SDF场跟边界标记场维度不匹配！");
            }

            int TotalThreadNum = Resolution.x * Resolution.y * Resolution.z;

            int ThreadsPerBlock, BlocksPerGrid;
            CGlobalMacroAndFunc.fetchPropBlockGridSize1D(TotalThreadNum, out ThreadsPerBlock, out BlocksPerGrid);

            voSolidsMarkerField.resize(voSolidsMarkerField.getResolution(), voSolidsMarkerField.getOrigin(), voSolidsMarkerField.getSpacing());

            m_BoundaryTool.SetInts("GridResolution", Resolution.x, Resolution.y, Resolution.z);
            m_BoundaryTool.SetInt("TotalThreadNum", TotalThreadNum);

            m_BoundaryTool.SetBuffer(m_BuildTotalBoundarysSDFKernel, "voSolidsMarkerFieldData", voSolidsMarkerField.getGridData());

            for (int i = 0; i < vBoundarysSDF.Count; i++)
            {
                m_BoundaryTool.SetInt("vCurSolidIndex", i + 1);
                m_BoundaryTool.SetBuffer(m_BuildTotalBoundarysSDFKernel, "vSolidSDFFieldData", vBoundarysSDF[i].getGridData());

                m_BoundaryTool.Dispatch(m_BuildTotalBoundarysSDFKernel, BlocksPerGrid, 1, 1);
            }
        }

        public static void buildSolidsVelFieldInvoker
        (
	        ComputeBuffer vSolidsVel,
            CCellCenteredScalarField vSolidsMarkerField,
            CFaceCenteredVectorField voSolidsVelField
        )
        {
            if (!m_Initiated) init();

            Vector3Int Resolution = vSolidsMarkerField.getResolution();

            CGlobalMacroAndFunc._ASSERTE(Resolution == voSolidsVelField.getResolution(), "边界标记场与边界速度场维度不匹配！");
            CGlobalMacroAndFunc._ASSERTE(vSolidsVel.count % 3 == 0, "边界速度不是三的倍数！");

            int TotalThreadNum = Resolution.x * Resolution.y * Resolution.z;

            m_BoundaryTool.SetInts("GridResolution", Resolution.x, Resolution.y, Resolution.z);

            m_BoundaryTool.SetInt("vTotalSolidNum", vSolidsVel.count / 3);
            m_BoundaryTool.SetBuffer(m_BuildSolidsVelKernel, "vSolidsVel", vSolidsVel);
            m_BoundaryTool.SetBuffer(m_BuildSolidsVelKernel, "vSolidsMarkerFieldData", vSolidsMarkerField.getGridData());
            m_BoundaryTool.SetBuffer(m_BuildSolidsVelKernel, "voSolidsVelFieldDataX", voSolidsVelField.getGridDataX());
            m_BoundaryTool.SetBuffer(m_BuildSolidsVelKernel, "voSolidsVelFieldDataY", voSolidsVelField.getGridDataY());
            m_BoundaryTool.SetBuffer(m_BuildSolidsVelKernel, "voSolidsVelFieldDataZ", voSolidsVelField.getGridDataZ());

            CGlobalMacroAndFunc.dispatchKernel(m_BoundaryTool, m_BuildSolidsVelKernel, TotalThreadNum);
        } 
        
        private static bool m_Initiated = false;
        private static ComputeShader m_BoundaryTool = null;
        private static int m_SampleTexture3DWithTransformKernel = -1;
        private static int m_BuildTotalBoundarysSDFKernel = -1;
        private static int m_BuildSolidsVelKernel = -1;

        public static void init()
        {
            if (m_Initiated) return;
            m_Initiated = true;
            m_BoundaryTool = Resources.Load("Shaders/BoundaryTool") as ComputeShader;

            m_SampleTexture3DWithTransformKernel = m_BoundaryTool.FindKernel("sampleTexture3DWithTransform");
            m_BuildTotalBoundarysSDFKernel = m_BoundaryTool.FindKernel("buildTotalBoundarysSDF");
            m_BuildSolidsVelKernel = m_BoundaryTool.FindKernel("buildSolidsVel");
        }
    }
}
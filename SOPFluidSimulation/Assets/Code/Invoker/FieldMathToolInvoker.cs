using UnityEngine;
using System;

namespace EulerFluidEngine
{
    public static class CFieldMathToolInvoker
    {
        private static bool m_Initialized = false;

        private static ComputeShader m_FieldMathTool = null;

        private static int m_SampleCCSFieldWithPosFieldLinearKernel = -1;
        private static int m_SampleCCSFieldWithPosFieldCubicKernel = -1;
        private static int m_SampleCCSFieldWithPosBufferLinearKernel = -1;
        private static int m_SampleCCSFieldWithPosBufferCubicKernel = -1;
        private static int m_ClampExtremaKernel = -1;
        private static int m_CCSFieldGradientKernel = -1;
        private static int m_CCSFieldLaplacianKernel = -1;
        private static int m_CCVFieldDivergenceKernel = -1;
        private static int m_FCVFieldDivergenceKernel = -1;
        private static int m_CCVFieldCurlKernel = -1;
        private static int m_FCVFieldCurlKernel = -1;
        private static int m_CCVFieldLengthKernel = -1;
        private static int m_FCVFieldLengthKernel = -1;
        private static int m_TransferFCVField2CCVFieldKernel = -1;
        private static int m_TransferCCVField2FCVFieldKernel = -1;
        //TODO
        private static int m_unionSDFKernel = -1;
        private static int m_intersectSDF = -1;
        private static int m_differenceSDF = -1;
        private static int m_SetFCVDataWithBoxKernel = -1;

        #region Init
        public static void init()
        {
            if (m_Initialized) return;

            m_FieldMathTool = Resources.Load("Shaders/FieldMathTool") as ComputeShader;

            m_SampleCCSFieldWithPosFieldLinearKernel = m_FieldMathTool.FindKernel("sampleCCSFieldWithPosFieldLinear");
            m_SampleCCSFieldWithPosFieldCubicKernel = m_FieldMathTool.FindKernel("sampleCCSFieldWithPosFieldCubic");
            m_SampleCCSFieldWithPosBufferLinearKernel = m_FieldMathTool.FindKernel("sampleCCSFieldWithPosBufferLinear");
            m_SampleCCSFieldWithPosBufferCubicKernel = m_FieldMathTool.FindKernel("sampleCCSFieldWithPosBufferCubic");
            m_ClampExtremaKernel = m_FieldMathTool.FindKernel("clampExtrema");
            m_CCSFieldGradientKernel = m_FieldMathTool.FindKernel("CCSFieldGradient");
            m_CCSFieldLaplacianKernel = m_FieldMathTool.FindKernel("CCSFieldLaplacian");
            m_CCVFieldDivergenceKernel = m_FieldMathTool.FindKernel("CCVFieldDivergence");
            m_FCVFieldDivergenceKernel = m_FieldMathTool.FindKernel("FCVFieldDivergence");
            m_CCVFieldCurlKernel = m_FieldMathTool.FindKernel("CCVFieldCurl");
            m_FCVFieldCurlKernel = m_FieldMathTool.FindKernel("FCVFieldCurl");
            m_CCVFieldLengthKernel = m_FieldMathTool.FindKernel("CCVFieldLength");
            m_FCVFieldLengthKernel = m_FieldMathTool.FindKernel("FCVFieldLength");
            m_TransferFCVField2CCVFieldKernel = m_FieldMathTool.FindKernel("transferFCVField2CCVField");
            m_TransferCCVField2FCVFieldKernel = m_FieldMathTool.FindKernel("transferCCVField2FCVField");
            m_unionSDFKernel = m_FieldMathTool.FindKernel("unionSDF");
            m_intersectSDF = m_FieldMathTool.FindKernel("intersectSDF");
            m_differenceSDF = m_FieldMathTool.FindKernel("differenceSDF");
            m_SetFCVDataWithBoxKernel = m_FieldMathTool.FindKernel("setFCVDataWithBox");

            m_Initialized = true;
        }
        #endregion

        #region CoreMethods
        public static void sampleCCSFieldWithPosFieldInvoker
        (
            CCellCenteredScalarField vSrcScalarField,
            CCellCenteredScalarField voDstScalarField,
            CCellCenteredVectorField vSampledAbsPosVectorField,
            ESamplingAlgorithm vSamplingAlgorithm = ESamplingAlgorithm.LINEAR
        )
        {
            if (!m_Initialized) init();

            Vector3Int DstFieldResolution = voDstScalarField.getResolution();
            Vector3Int SrcFieldResolution = vSrcScalarField.getResolution();
            Vector3 SrcFieldOrigin = vSrcScalarField.getOrigin();
            Vector3 SrcFieldSpacing = vSrcScalarField.getSpacing();

            CGlobalMacroAndFunc._ASSERTE(DstFieldResolution == vSampledAbsPosVectorField.getResolution(), "采样位置场和采样目的场维度不匹配!");

            m_FieldMathTool.SetFloats("GridOrigin", SrcFieldOrigin.x, SrcFieldOrigin.y, SrcFieldOrigin.z);
            m_FieldMathTool.SetFloats("GridSpacing", SrcFieldSpacing.x, SrcFieldSpacing.y, SrcFieldSpacing.z);
            m_FieldMathTool.SetInts("GridResolution", SrcFieldResolution.x, SrcFieldResolution.y, SrcFieldResolution.z);

            if(vSamplingAlgorithm == ESamplingAlgorithm.LINEAR)
            {
                m_FieldMathTool.SetInt("DataSpan_sampleCCSFieldWithPosFieldLinear", 1);
                m_FieldMathTool.SetInt("DataOffset_sampleCCSFieldWithPosFieldLinear", 0);
                m_FieldMathTool.SetBuffer(m_SampleCCSFieldWithPosFieldLinearKernel, "vSrcScalarFieldData_sampleCCSFieldWithPosFieldLinear", vSrcScalarField.getGridData());
                m_FieldMathTool.SetBuffer(m_SampleCCSFieldWithPosFieldLinearKernel, "voDstScalarFieldData_sampleCCSFieldWithPosFieldLinear", voDstScalarField.getGridData());
                m_FieldMathTool.SetBuffer(m_SampleCCSFieldWithPosFieldLinearKernel, "vSampledAbsPosDataX_sampleCCSFieldWithPosFieldLinear", vSampledAbsPosVectorField.getGridDataX());
                m_FieldMathTool.SetBuffer(m_SampleCCSFieldWithPosFieldLinearKernel, "vSampledAbsPosDataY_sampleCCSFieldWithPosFieldLinear", vSampledAbsPosVectorField.getGridDataY());
                m_FieldMathTool.SetBuffer(m_SampleCCSFieldWithPosFieldLinearKernel, "vSampledAbsPosDataZ_sampleCCSFieldWithPosFieldLinear", vSampledAbsPosVectorField.getGridDataZ());

                int TotalThreadNum = DstFieldResolution.x * DstFieldResolution.y * DstFieldResolution.z;
                CGlobalMacroAndFunc.dispatchKernel(m_FieldMathTool, m_SampleCCSFieldWithPosFieldLinearKernel, TotalThreadNum);
            }
            else
            {
                m_FieldMathTool.SetInt("SamplingAlg_sampleCCSFieldWithPosFieldCubic", Convert.ToInt32(vSamplingAlgorithm));

                m_FieldMathTool.SetInt("DataSpan_sampleCCSFieldWithPosFieldCubic", 1);
                m_FieldMathTool.SetInt("DataOffset_sampleCCSFieldWithPosFieldCubic", 0);
                m_FieldMathTool.SetBuffer(m_SampleCCSFieldWithPosFieldCubicKernel, "vSrcScalarFieldData_sampleCCSFieldWithPosFieldCubic", vSrcScalarField.getGridData());
                m_FieldMathTool.SetBuffer(m_SampleCCSFieldWithPosFieldCubicKernel, "voDstScalarFieldData_sampleCCSFieldWithPosFieldCubic", voDstScalarField.getGridData());
                m_FieldMathTool.SetBuffer(m_SampleCCSFieldWithPosFieldCubicKernel, "vSampledAbsPosDataX_sampleCCSFieldWithPosFieldCubic", vSampledAbsPosVectorField.getGridDataX());
                m_FieldMathTool.SetBuffer(m_SampleCCSFieldWithPosFieldCubicKernel, "vSampledAbsPosDataY_sampleCCSFieldWithPosFieldCubic", vSampledAbsPosVectorField.getGridDataY());
                m_FieldMathTool.SetBuffer(m_SampleCCSFieldWithPosFieldCubicKernel, "vSampledAbsPosDataZ_sampleCCSFieldWithPosFieldCubic", vSampledAbsPosVectorField.getGridDataZ());

                int TotalThreadNum = DstFieldResolution.x * DstFieldResolution.y * DstFieldResolution.z;
                CGlobalMacroAndFunc.dispatchKernel(m_FieldMathTool, m_SampleCCSFieldWithPosFieldCubicKernel, TotalThreadNum);
            }
        }

        public static void sampleCCSFieldWithPosBufferInvoker
        (
            CCellCenteredScalarField vSrcScalarField,
            ComputeBuffer voDstBuffer,
            ComputeBuffer vSampledAbsPos,
            ESamplingAlgorithm vSamplingAlgorithm = ESamplingAlgorithm.LINEAR
        )
        {
            if (!m_Initialized) init();

            Vector3Int SrcFieldResolution = vSrcScalarField.getResolution();
            Vector3 SrcFieldOrigin = vSrcScalarField.getOrigin();
            Vector3 SrcFieldSpacing = vSrcScalarField.getSpacing();

            CGlobalMacroAndFunc._ASSERTE(voDstBuffer.count == vSampledAbsPos.count / 3, "采样位置Buffer和采样目的Buffer维度不匹配!");

            m_FieldMathTool.SetFloats("GridOrigin", SrcFieldOrigin.x, SrcFieldOrigin.y, SrcFieldOrigin.z);
            m_FieldMathTool.SetFloats("GridSpacing", SrcFieldSpacing.x, SrcFieldSpacing.y, SrcFieldSpacing.z);
            m_FieldMathTool.SetInts("GridResolution", SrcFieldResolution.x, SrcFieldResolution.y, SrcFieldResolution.z);

            if (vSamplingAlgorithm == ESamplingAlgorithm.LINEAR)
            {
                m_FieldMathTool.SetInt("DataSpan_sampleCCSFieldWithPosBufferLinear", 1);
                m_FieldMathTool.SetInt("DataOffset_sampleCCSFieldWithPosBufferLinear", 0);
                m_FieldMathTool.SetBuffer(m_SampleCCSFieldWithPosBufferLinearKernel, "vSrcScalarFieldData_sampleCCSFieldWithPosBufferLinear", vSrcScalarField.getGridData());
                m_FieldMathTool.SetBuffer(m_SampleCCSFieldWithPosBufferLinearKernel, "voDstScalarFieldData_sampleCCSFieldWithPosBufferLinear", voDstBuffer);
                m_FieldMathTool.SetBuffer(m_SampleCCSFieldWithPosBufferLinearKernel, "vSampledAbsPosData_sampleCCSFieldWithPosBufferLinear", vSampledAbsPos);

                int TotalThreadNum = (vSampledAbsPos.count / 3);
                CGlobalMacroAndFunc.dispatchKernel(m_FieldMathTool, m_SampleCCSFieldWithPosBufferLinearKernel, TotalThreadNum);
            }
            else
            {
                m_FieldMathTool.SetInt("SamplingAlg_sampleCCSFieldWithPosBufferCubic", Convert.ToInt32(vSamplingAlgorithm));

                m_FieldMathTool.SetInt("DataSpan_sampleCCSFieldWithPosBufferCubic", 1);
                m_FieldMathTool.SetInt("DataOffset_sampleCCSFieldWithPosBufferCubic", 0);
                m_FieldMathTool.SetBuffer(m_SampleCCSFieldWithPosBufferCubicKernel, "vSrcScalarFieldData_sampleCCSFieldWithPosBufferCubic", vSrcScalarField.getGridData());
                m_FieldMathTool.SetBuffer(m_SampleCCSFieldWithPosBufferCubicKernel, "voDstScalarFieldData_sampleCCSFieldWithPosBufferCubic", voDstBuffer);
                m_FieldMathTool.SetBuffer(m_SampleCCSFieldWithPosBufferCubicKernel, "vSampledAbsPosData_sampleCCSFieldWithPosBufferCubic", vSampledAbsPos);

                int TotalThreadNum = (vSampledAbsPos.count / 3);
                CGlobalMacroAndFunc.dispatchKernel(m_FieldMathTool, m_SampleCCSFieldWithPosBufferCubicKernel, TotalThreadNum);
            }
        }

        public static void sampleCCVFieldWithPosFieldInvoker
        (
            CCellCenteredVectorField vSrcVectorField,
            CCellCenteredVectorField voDstVectorField,
            CCellCenteredVectorField vSampledAbsPosVectorField,
            ESamplingAlgorithm vSamplingAlgorithm = ESamplingAlgorithm.LINEAR
        )
        {
            if (!m_Initialized) init();

            Vector3Int DstFieldResolution = voDstVectorField.getResolution();
            Vector3Int SrcFieldResolution = vSrcVectorField.getResolution();
            Vector3 SrcFieldOrigin = vSrcVectorField.getOrigin();
            Vector3 SrcFieldSpacing = vSrcVectorField.getSpacing();

            CGlobalMacroAndFunc._ASSERTE(DstFieldResolution == vSampledAbsPosVectorField.getResolution(), "采样位置场和采样目的场维度不匹配!");

            int TotalThreadNum = (DstFieldResolution.x * DstFieldResolution.y * DstFieldResolution.z);

            m_FieldMathTool.SetFloats("GridOrigin", SrcFieldOrigin.x, SrcFieldOrigin.y, SrcFieldOrigin.z);
            m_FieldMathTool.SetFloats("GridSpacing", SrcFieldSpacing.x, SrcFieldSpacing.y, SrcFieldSpacing.z);
            m_FieldMathTool.SetInts("GridResolution", SrcFieldResolution.x, SrcFieldResolution.y, SrcFieldResolution.z);

            if (vSamplingAlgorithm == ESamplingAlgorithm.LINEAR)
            {
                m_FieldMathTool.SetInt("DataSpan_sampleCCSFieldWithPosFieldLinear", 1);
                m_FieldMathTool.SetInt("DataOffset_sampleCCSFieldWithPosFieldLinear", 0);
                m_FieldMathTool.SetBuffer(m_SampleCCSFieldWithPosFieldLinearKernel, "vSampledAbsPosDataX_sampleCCSFieldWithPosFieldLinear", vSampledAbsPosVectorField.getGridDataX());
                m_FieldMathTool.SetBuffer(m_SampleCCSFieldWithPosFieldLinearKernel, "vSampledAbsPosDataY_sampleCCSFieldWithPosFieldLinear", vSampledAbsPosVectorField.getGridDataY());
                m_FieldMathTool.SetBuffer(m_SampleCCSFieldWithPosFieldLinearKernel, "vSampledAbsPosDataZ_sampleCCSFieldWithPosFieldLinear", vSampledAbsPosVectorField.getGridDataZ());

                m_FieldMathTool.SetBuffer(m_SampleCCSFieldWithPosFieldLinearKernel, "vSrcScalarFieldData_sampleCCSFieldWithPosFieldLinear", vSrcVectorField.getGridDataX());
                m_FieldMathTool.SetBuffer(m_SampleCCSFieldWithPosFieldLinearKernel, "voDstScalarFieldData_sampleCCSFieldWithPosFieldLinear", voDstVectorField.getGridDataX());
                CGlobalMacroAndFunc.dispatchKernel(m_FieldMathTool, m_SampleCCSFieldWithPosFieldLinearKernel, TotalThreadNum);

                m_FieldMathTool.SetBuffer(m_SampleCCSFieldWithPosFieldLinearKernel, "vSrcScalarFieldData_sampleCCSFieldWithPosFieldLinear", vSrcVectorField.getGridDataY());
                m_FieldMathTool.SetBuffer(m_SampleCCSFieldWithPosFieldLinearKernel, "voDstScalarFieldData_sampleCCSFieldWithPosFieldLinear", voDstVectorField.getGridDataY());
                CGlobalMacroAndFunc.dispatchKernel(m_FieldMathTool, m_SampleCCSFieldWithPosFieldLinearKernel, TotalThreadNum);

                m_FieldMathTool.SetBuffer(m_SampleCCSFieldWithPosFieldLinearKernel, "vSrcScalarFieldData_sampleCCSFieldWithPosFieldLinear", vSrcVectorField.getGridDataZ());
                m_FieldMathTool.SetBuffer(m_SampleCCSFieldWithPosFieldLinearKernel, "voDstScalarFieldData_sampleCCSFieldWithPosFieldLinear", voDstVectorField.getGridDataZ());
                CGlobalMacroAndFunc.dispatchKernel(m_FieldMathTool, m_SampleCCSFieldWithPosFieldLinearKernel, TotalThreadNum);
            }
            else
            {
                m_FieldMathTool.SetInt("SamplingAlg_sampleCCSFieldWithPosFieldCubic", Convert.ToInt32(vSamplingAlgorithm));

                m_FieldMathTool.SetInt("DataSpan_sampleCCSFieldWithPosFieldCubic", 1);
                m_FieldMathTool.SetInt("DataOffset_sampleCCSFieldWithPosFieldCubic", 0);
                m_FieldMathTool.SetBuffer(m_SampleCCSFieldWithPosFieldCubicKernel, "vSampledAbsPosDataX_sampleCCSFieldWithPosFieldCubic", vSampledAbsPosVectorField.getGridDataX());
                m_FieldMathTool.SetBuffer(m_SampleCCSFieldWithPosFieldCubicKernel, "vSampledAbsPosDataY_sampleCCSFieldWithPosFieldCubic", vSampledAbsPosVectorField.getGridDataY());
                m_FieldMathTool.SetBuffer(m_SampleCCSFieldWithPosFieldCubicKernel, "vSampledAbsPosDataZ_sampleCCSFieldWithPosFieldCubic", vSampledAbsPosVectorField.getGridDataZ());

                m_FieldMathTool.SetBuffer(m_SampleCCSFieldWithPosFieldCubicKernel, "vSrcScalarFieldData_sampleCCSFieldWithPosFieldCubic", vSrcVectorField.getGridDataX());
                m_FieldMathTool.SetBuffer(m_SampleCCSFieldWithPosFieldCubicKernel, "voDstScalarFieldData_sampleCCSFieldWithPosFieldCubic", voDstVectorField.getGridDataX());
                CGlobalMacroAndFunc.dispatchKernel(m_FieldMathTool, m_SampleCCSFieldWithPosFieldCubicKernel, TotalThreadNum);

                m_FieldMathTool.SetBuffer(m_SampleCCSFieldWithPosFieldCubicKernel, "vSrcScalarFieldData_sampleCCSFieldWithPosFieldCubic", vSrcVectorField.getGridDataY());
                m_FieldMathTool.SetBuffer(m_SampleCCSFieldWithPosFieldCubicKernel, "voDstScalarFieldData_sampleCCSFieldWithPosFieldCubic", voDstVectorField.getGridDataY());
                CGlobalMacroAndFunc.dispatchKernel(m_FieldMathTool, m_SampleCCSFieldWithPosFieldCubicKernel, TotalThreadNum);

                m_FieldMathTool.SetBuffer(m_SampleCCSFieldWithPosFieldCubicKernel, "vSrcScalarFieldData_sampleCCSFieldWithPosFieldCubic", vSrcVectorField.getGridDataZ());
                m_FieldMathTool.SetBuffer(m_SampleCCSFieldWithPosFieldCubicKernel, "voDstScalarFieldData_sampleCCSFieldWithPosFieldCubic", voDstVectorField.getGridDataZ());
                CGlobalMacroAndFunc.dispatchKernel(m_FieldMathTool, m_SampleCCSFieldWithPosFieldCubicKernel, TotalThreadNum);
            }     
        }

        public static void sampleCCVFieldWithPosBufferInvoker
        (
             CCellCenteredVectorField vSrcVectorField,
             ComputeBuffer voDstData,
             ComputeBuffer vSampledAbsPos,
            ESamplingAlgorithm vSamplingAlgorithm = ESamplingAlgorithm.LINEAR
        )
        {
            if (!m_Initialized) init();

            Vector3Int SrcFieldResolution = vSrcVectorField.getResolution();
            Vector3 SrcFieldOrigin = vSrcVectorField.getOrigin();
            Vector3 SrcFieldSpacing = vSrcVectorField.getSpacing();

            CGlobalMacroAndFunc._ASSERTE(voDstData.count == vSampledAbsPos.count, "采样位置Buffer和采样目的Buffer维度不匹配!");

            int TotalThreadNum = (vSampledAbsPos.count / 3);

            m_FieldMathTool.SetFloats("GridOrigin", SrcFieldOrigin.x, SrcFieldOrigin.y, SrcFieldOrigin.z);
            m_FieldMathTool.SetFloats("GridSpacing", SrcFieldSpacing.x, SrcFieldSpacing.y, SrcFieldSpacing.z);
            m_FieldMathTool.SetInts("GridResolution", SrcFieldResolution.x, SrcFieldResolution.y, SrcFieldResolution.z);

            if (vSamplingAlgorithm == ESamplingAlgorithm.LINEAR)
            {
                m_FieldMathTool.SetInt("DataSpan_sampleCCSFieldWithPosBufferLinear", 3);
                m_FieldMathTool.SetBuffer(m_SampleCCSFieldWithPosBufferLinearKernel, "vSampledAbsPosData_sampleCCSFieldWithPosBufferLinear", vSampledAbsPos);
                m_FieldMathTool.SetBuffer(m_SampleCCSFieldWithPosBufferLinearKernel, "voDstScalarFieldData_sampleCCSFieldWithPosBufferLinear", voDstData);

                m_FieldMathTool.SetInt("DataOffset_sampleCCSFieldWithPosBufferLinear", 0);
                m_FieldMathTool.SetBuffer(m_SampleCCSFieldWithPosBufferLinearKernel, "vSrcScalarFieldData_sampleCCSFieldWithPosBufferLinear", vSrcVectorField.getGridDataX());
                CGlobalMacroAndFunc.dispatchKernel(m_FieldMathTool, m_SampleCCSFieldWithPosBufferLinearKernel, TotalThreadNum);

                m_FieldMathTool.SetInt("DataOffset_sampleCCSFieldWithPosBufferLinear", 1);
                m_FieldMathTool.SetBuffer(m_SampleCCSFieldWithPosBufferLinearKernel, "vSrcScalarFieldData_sampleCCSFieldWithPosBufferLinear", vSrcVectorField.getGridDataY());
                CGlobalMacroAndFunc.dispatchKernel(m_FieldMathTool, m_SampleCCSFieldWithPosBufferLinearKernel, TotalThreadNum);

                m_FieldMathTool.SetInt("DataOffset_sampleCCSFieldWithPosBufferLinear", 2);
                m_FieldMathTool.SetBuffer(m_SampleCCSFieldWithPosBufferLinearKernel, "vSrcScalarFieldData_sampleCCSFieldWithPosBufferLinear", vSrcVectorField.getGridDataZ());
                CGlobalMacroAndFunc.dispatchKernel(m_FieldMathTool, m_SampleCCSFieldWithPosBufferLinearKernel, TotalThreadNum);
            }
            else
            {
                m_FieldMathTool.SetInt("SamplingAlg_sampleCCSFieldWithPosBufferCubic", Convert.ToInt32(vSamplingAlgorithm));

                m_FieldMathTool.SetInt("DataSpan_sampleCCSFieldWithPosBufferCubic", 3);
                m_FieldMathTool.SetBuffer(m_SampleCCSFieldWithPosBufferCubicKernel, "vSampledAbsPosData_sampleCCSFieldWithPosBufferCubic", vSampledAbsPos);
                m_FieldMathTool.SetBuffer(m_SampleCCSFieldWithPosBufferCubicKernel, "voDstScalarFieldData_sampleCCSFieldWithPosBufferCubic", voDstData);

                m_FieldMathTool.SetInt("DataOffset_sampleCCSFieldWithPosBufferCubic", 0);
                m_FieldMathTool.SetBuffer(m_SampleCCSFieldWithPosBufferCubicKernel, "vSrcScalarFieldData_sampleCCSFieldWithPosBufferCubic", vSrcVectorField.getGridDataX());
                CGlobalMacroAndFunc.dispatchKernel(m_FieldMathTool, m_SampleCCSFieldWithPosBufferCubicKernel, TotalThreadNum);

                m_FieldMathTool.SetInt("DataOffset_sampleCCSFieldWithPosBufferCubic", 1);
                m_FieldMathTool.SetBuffer(m_SampleCCSFieldWithPosBufferCubicKernel, "vSrcScalarFieldData_sampleCCSFieldWithPosBufferCubic", vSrcVectorField.getGridDataY());
                CGlobalMacroAndFunc.dispatchKernel(m_FieldMathTool, m_SampleCCSFieldWithPosBufferCubicKernel, TotalThreadNum);

                m_FieldMathTool.SetInt("DataOffset_sampleCCSFieldWithPosBufferCubic", 2);
                m_FieldMathTool.SetBuffer(m_SampleCCSFieldWithPosBufferCubicKernel, "vSrcScalarFieldData_sampleCCSFieldWithPosBufferCubic", vSrcVectorField.getGridDataZ());
                CGlobalMacroAndFunc.dispatchKernel(m_FieldMathTool, m_SampleCCSFieldWithPosBufferCubicKernel, TotalThreadNum);
            }
        }

        public static void sampleFCVFieldWithPosFieldInvoker
        (
             CFaceCenteredVectorField vSrcVectorField,
             CCellCenteredVectorField voDstVectorField,
             CCellCenteredVectorField vSampledAbsPosVectorField,
            ESamplingAlgorithm vSamplingAlgorithm = ESamplingAlgorithm.LINEAR
        )
        {
            if (!m_Initialized) init();

            Vector3Int DstFieldResolution = voDstVectorField.getResolution();
            Vector3Int SrcFieldResolution = vSrcVectorField.getResolution();
            Vector3 SrcFieldOrigin = vSrcVectorField.getOrigin();
            Vector3 SrcFieldSpacing = vSrcVectorField.getSpacing();

            CGlobalMacroAndFunc._ASSERTE(DstFieldResolution == vSampledAbsPosVectorField.getResolution(), "采样位置场和采样目的场维度不匹配!");

            Vector3Int SrcFieldResolutionX = SrcFieldResolution + new Vector3Int(1, 0, 0);
            Vector3 SrcFieldOriginX = SrcFieldOrigin - new Vector3(SrcFieldSpacing.x / 2, 0, 0);
            Vector3 SrcFieldSpacingX = SrcFieldSpacing;
            Vector3Int SrcFieldResolutionY = SrcFieldResolution + new Vector3Int(0, 1, 0);
            Vector3 SrcFieldOriginY = SrcFieldOrigin - new Vector3(0, SrcFieldSpacing.y / 2, 0);
            Vector3 SrcFieldSpacingY = SrcFieldSpacing;
            Vector3Int SrcFieldResolutionZ = SrcFieldResolution + new Vector3Int(0, 0, 1);
            Vector3 SrcFieldOriginZ = SrcFieldOrigin - new Vector3(0, 0, SrcFieldSpacing.z / 2);
            Vector3 SrcFieldSpacingZ = SrcFieldSpacing;

            int TotalThreadNum = (DstFieldResolution.x * DstFieldResolution.y * DstFieldResolution.z);

            if (vSamplingAlgorithm == ESamplingAlgorithm.LINEAR)
            {
                m_FieldMathTool.SetInt("DataSpan_sampleCCSFieldWithPosFieldLinear", 1);
                m_FieldMathTool.SetInt("DataOffset_sampleCCSFieldWithPosFieldLinear", 0);
                m_FieldMathTool.SetBuffer(m_SampleCCSFieldWithPosFieldLinearKernel, "vSampledAbsPosDataX_sampleCCSFieldWithPosFieldLinear", vSampledAbsPosVectorField.getGridDataX());
                m_FieldMathTool.SetBuffer(m_SampleCCSFieldWithPosFieldLinearKernel, "vSampledAbsPosDataY_sampleCCSFieldWithPosFieldLinear", vSampledAbsPosVectorField.getGridDataY());
                m_FieldMathTool.SetBuffer(m_SampleCCSFieldWithPosFieldLinearKernel, "vSampledAbsPosDataZ_sampleCCSFieldWithPosFieldLinear", vSampledAbsPosVectorField.getGridDataZ());

                m_FieldMathTool.SetFloats("GridOrigin", SrcFieldOriginX.x, SrcFieldOriginX.y, SrcFieldOriginX.z);
                m_FieldMathTool.SetFloats("GridSpacing", SrcFieldSpacingX.x, SrcFieldSpacingX.y, SrcFieldSpacingX.z);
                m_FieldMathTool.SetInts("GridResolution", SrcFieldResolutionX.x, SrcFieldResolutionX.y, SrcFieldResolutionX.z);
                m_FieldMathTool.SetBuffer(m_SampleCCSFieldWithPosFieldLinearKernel, "vSrcScalarFieldData_sampleCCSFieldWithPosFieldLinear", vSrcVectorField.getGridDataX());
                m_FieldMathTool.SetBuffer(m_SampleCCSFieldWithPosFieldLinearKernel, "voDstScalarFieldData_sampleCCSFieldWithPosFieldLinear", voDstVectorField.getGridDataX());
                CGlobalMacroAndFunc.dispatchKernel(m_FieldMathTool, m_SampleCCSFieldWithPosFieldLinearKernel, TotalThreadNum);

                m_FieldMathTool.SetFloats("GridOrigin", SrcFieldOriginY.x, SrcFieldOriginY.y, SrcFieldOriginY.z);
                m_FieldMathTool.SetFloats("GridSpacing", SrcFieldSpacingY.x, SrcFieldSpacingY.y, SrcFieldSpacingY.z);
                m_FieldMathTool.SetInts("GridResolution", SrcFieldResolutionY.x, SrcFieldResolutionY.y, SrcFieldResolutionY.z);
                m_FieldMathTool.SetBuffer(m_SampleCCSFieldWithPosFieldLinearKernel, "vSrcScalarFieldData_sampleCCSFieldWithPosFieldLinear", vSrcVectorField.getGridDataY());
                m_FieldMathTool.SetBuffer(m_SampleCCSFieldWithPosFieldLinearKernel, "voDstScalarFieldData_sampleCCSFieldWithPosFieldLinear", voDstVectorField.getGridDataY());
                CGlobalMacroAndFunc.dispatchKernel(m_FieldMathTool, m_SampleCCSFieldWithPosFieldLinearKernel, TotalThreadNum);

                m_FieldMathTool.SetFloats("GridOrigin", SrcFieldOriginZ.x, SrcFieldOriginZ.y, SrcFieldOriginZ.z);
                m_FieldMathTool.SetFloats("GridSpacing", SrcFieldSpacingZ.x, SrcFieldSpacingZ.y, SrcFieldSpacingZ.z);
                m_FieldMathTool.SetInts("GridResolution", SrcFieldResolutionZ.x, SrcFieldResolutionZ.y, SrcFieldResolutionZ.z);
                m_FieldMathTool.SetBuffer(m_SampleCCSFieldWithPosFieldLinearKernel, "vSrcScalarFieldData_sampleCCSFieldWithPosFieldLinear", vSrcVectorField.getGridDataZ());
                m_FieldMathTool.SetBuffer(m_SampleCCSFieldWithPosFieldLinearKernel, "voDstScalarFieldData_sampleCCSFieldWithPosFieldLinear", voDstVectorField.getGridDataZ());
                CGlobalMacroAndFunc.dispatchKernel(m_FieldMathTool, m_SampleCCSFieldWithPosFieldLinearKernel, TotalThreadNum);
            }
            else
            {
                m_FieldMathTool.SetInt("SamplingAlg_sampleCCSFieldWithPosFieldCubic", Convert.ToInt32(vSamplingAlgorithm));

                m_FieldMathTool.SetInt("DataSpan_sampleCCSFieldWithPosFieldCubic", 1);
                m_FieldMathTool.SetInt("DataOffset_sampleCCSFieldWithPosFieldCubic", 0);
                m_FieldMathTool.SetBuffer(m_SampleCCSFieldWithPosFieldCubicKernel, "vSampledAbsPosDataX_sampleCCSFieldWithPosFieldCubic", vSampledAbsPosVectorField.getGridDataX());
                m_FieldMathTool.SetBuffer(m_SampleCCSFieldWithPosFieldCubicKernel, "vSampledAbsPosDataY_sampleCCSFieldWithPosFieldCubic", vSampledAbsPosVectorField.getGridDataY());
                m_FieldMathTool.SetBuffer(m_SampleCCSFieldWithPosFieldCubicKernel, "vSampledAbsPosDataZ_sampleCCSFieldWithPosFieldCubic", vSampledAbsPosVectorField.getGridDataZ());

                m_FieldMathTool.SetFloats("GridOrigin", SrcFieldOriginX.x, SrcFieldOriginX.y, SrcFieldOriginX.z);
                m_FieldMathTool.SetFloats("GridSpacing", SrcFieldSpacingX.x, SrcFieldSpacingX.y, SrcFieldSpacingX.z);
                m_FieldMathTool.SetInts("GridResolution", SrcFieldResolutionX.x, SrcFieldResolutionX.y, SrcFieldResolutionX.z);
                m_FieldMathTool.SetBuffer(m_SampleCCSFieldWithPosFieldCubicKernel, "vSrcScalarFieldData_sampleCCSFieldWithPosFieldCubic", vSrcVectorField.getGridDataX());
                m_FieldMathTool.SetBuffer(m_SampleCCSFieldWithPosFieldCubicKernel, "voDstScalarFieldData_sampleCCSFieldWithPosFieldCubic", voDstVectorField.getGridDataX());
                CGlobalMacroAndFunc.dispatchKernel(m_FieldMathTool, m_SampleCCSFieldWithPosFieldCubicKernel, TotalThreadNum);

                m_FieldMathTool.SetFloats("GridOrigin", SrcFieldOriginY.x, SrcFieldOriginY.y, SrcFieldOriginY.z);
                m_FieldMathTool.SetFloats("GridSpacing", SrcFieldSpacingY.x, SrcFieldSpacingY.y, SrcFieldSpacingY.z);
                m_FieldMathTool.SetInts("GridResolution", SrcFieldResolutionY.x, SrcFieldResolutionY.y, SrcFieldResolutionY.z);
                m_FieldMathTool.SetBuffer(m_SampleCCSFieldWithPosFieldCubicKernel, "vSrcScalarFieldData_sampleCCSFieldWithPosFieldCubic", vSrcVectorField.getGridDataY());
                m_FieldMathTool.SetBuffer(m_SampleCCSFieldWithPosFieldCubicKernel, "voDstScalarFieldData_sampleCCSFieldWithPosFieldCubic", voDstVectorField.getGridDataY());
                CGlobalMacroAndFunc.dispatchKernel(m_FieldMathTool, m_SampleCCSFieldWithPosFieldCubicKernel, TotalThreadNum);

                m_FieldMathTool.SetFloats("GridOrigin", SrcFieldOriginZ.x, SrcFieldOriginZ.y, SrcFieldOriginZ.z);
                m_FieldMathTool.SetFloats("GridSpacing", SrcFieldSpacingZ.x, SrcFieldSpacingZ.y, SrcFieldSpacingZ.z);
                m_FieldMathTool.SetInts("GridResolution", SrcFieldResolutionZ.x, SrcFieldResolutionZ.y, SrcFieldResolutionZ.z);
                m_FieldMathTool.SetBuffer(m_SampleCCSFieldWithPosFieldCubicKernel, "vSrcScalarFieldData_sampleCCSFieldWithPosFieldCubic", vSrcVectorField.getGridDataZ());
                m_FieldMathTool.SetBuffer(m_SampleCCSFieldWithPosFieldCubicKernel, "voDstScalarFieldData_sampleCCSFieldWithPosFieldCubic", voDstVectorField.getGridDataZ());
                CGlobalMacroAndFunc.dispatchKernel(m_FieldMathTool, m_SampleCCSFieldWithPosFieldCubicKernel, TotalThreadNum);
            }
        }

        public static void sampleFCVFieldWithPosBufferInvoker
        (
             CFaceCenteredVectorField vSrcVectorField,
             ComputeBuffer voDstData,
             ComputeBuffer vSampledAbsPos,
            ESamplingAlgorithm vSamplingAlgorithm = ESamplingAlgorithm.LINEAR
        )
        {
            if (!m_Initialized) init();

            CGlobalMacroAndFunc._ASSERTE(voDstData.count == vSampledAbsPos.count, "采样位置Buffer和采样目的Buffer维度不匹配!");

            Vector3Int SrcFieldResolution = vSrcVectorField.getResolution();
            Vector3 SrcFieldOrigin = vSrcVectorField.getOrigin();
            Vector3 SrcFieldSpacing = vSrcVectorField.getSpacing();

            int TotalThreadNum = (vSampledAbsPos.count / 3);

            Vector3Int SrcFieldResolutionX = SrcFieldResolution + new Vector3Int(1, 0, 0);
            Vector3 SrcFieldOriginX = SrcFieldOrigin - new Vector3(SrcFieldSpacing.x / 2, 0, 0);
            Vector3 SrcFieldSpacingX = SrcFieldSpacing;
            Vector3Int SrcFieldResolutionY = SrcFieldResolution + new Vector3Int(0, 1, 0);
            Vector3 SrcFieldOriginY = SrcFieldOrigin - new Vector3(0, SrcFieldSpacing.y / 2, 0);
            Vector3 SrcFieldSpacingY = SrcFieldSpacing;
            Vector3Int SrcFieldResolutionZ = SrcFieldResolution + new Vector3Int(0, 0, 1);
            Vector3 SrcFieldOriginZ = SrcFieldOrigin - new Vector3(0, 0, SrcFieldSpacing.z / 2);
            Vector3 SrcFieldSpacingZ = SrcFieldSpacing;

            if (vSamplingAlgorithm == ESamplingAlgorithm.LINEAR)
            {
                m_FieldMathTool.SetInt("DataSpan_sampleCCSFieldWithPosBufferLinear", 3);
                m_FieldMathTool.SetBuffer(m_SampleCCSFieldWithPosBufferLinearKernel, "vSampledAbsPosData_sampleCCSFieldWithPosBufferLinear", vSampledAbsPos);
                m_FieldMathTool.SetBuffer(m_SampleCCSFieldWithPosBufferLinearKernel, "voDstScalarFieldData_sampleCCSFieldWithPosBufferLinear", voDstData);

                m_FieldMathTool.SetFloats("GridOrigin", SrcFieldOriginX.x, SrcFieldOriginX.y, SrcFieldOriginX.z);
                m_FieldMathTool.SetFloats("GridSpacing", SrcFieldSpacingX.x, SrcFieldSpacingX.y, SrcFieldSpacingX.z);
                m_FieldMathTool.SetInts("GridResolution", SrcFieldResolutionX.x, SrcFieldResolutionX.y, SrcFieldResolutionX.z);
                m_FieldMathTool.SetInt("DataOffset_sampleCCSFieldWithPosBufferLinear", 0);
                m_FieldMathTool.SetBuffer(m_SampleCCSFieldWithPosBufferLinearKernel, "vSrcScalarFieldData_sampleCCSFieldWithPosBufferLinear", vSrcVectorField.getGridDataX());
                CGlobalMacroAndFunc.dispatchKernel(m_FieldMathTool, m_SampleCCSFieldWithPosBufferLinearKernel, TotalThreadNum);

                m_FieldMathTool.SetFloats("GridOrigin", SrcFieldOriginY.x, SrcFieldOriginY.y, SrcFieldOriginY.z);
                m_FieldMathTool.SetFloats("GridSpacing", SrcFieldSpacingY.x, SrcFieldSpacingY.y, SrcFieldSpacingY.z);
                m_FieldMathTool.SetInts("GridResolution", SrcFieldResolutionY.x, SrcFieldResolutionY.y, SrcFieldResolutionY.z);
                m_FieldMathTool.SetInt("DataOffset_sampleCCSFieldWithPosBufferLinear", 1);
                m_FieldMathTool.SetBuffer(m_SampleCCSFieldWithPosBufferLinearKernel, "vSrcScalarFieldData_sampleCCSFieldWithPosBufferLinear", vSrcVectorField.getGridDataY());
                CGlobalMacroAndFunc.dispatchKernel(m_FieldMathTool, m_SampleCCSFieldWithPosBufferLinearKernel, TotalThreadNum);

                m_FieldMathTool.SetFloats("GridOrigin", SrcFieldOriginZ.x, SrcFieldOriginZ.y, SrcFieldOriginZ.z);
                m_FieldMathTool.SetFloats("GridSpacing", SrcFieldSpacingZ.x, SrcFieldSpacingZ.y, SrcFieldSpacingZ.z);
                m_FieldMathTool.SetInts("GridResolution", SrcFieldResolutionZ.x, SrcFieldResolutionZ.y, SrcFieldResolutionZ.z);
                m_FieldMathTool.SetInt("DataOffset_sampleCCSFieldWithPosBufferLinear", 2);
                m_FieldMathTool.SetBuffer(m_SampleCCSFieldWithPosBufferLinearKernel, "vSrcScalarFieldData_sampleCCSFieldWithPosBufferLinear", vSrcVectorField.getGridDataZ());
                CGlobalMacroAndFunc.dispatchKernel(m_FieldMathTool, m_SampleCCSFieldWithPosBufferLinearKernel, TotalThreadNum);
            }
            else
            {
                m_FieldMathTool.SetInt("SamplingAlg_sampleCCSFieldWithPosBufferCubic", Convert.ToInt32(vSamplingAlgorithm));

                m_FieldMathTool.SetInt("DataSpan_sampleCCSFieldWithPosBufferCubic", 3);
                m_FieldMathTool.SetBuffer(m_SampleCCSFieldWithPosBufferCubicKernel, "vSampledAbsPosData_sampleCCSFieldWithPosBufferCubic", vSampledAbsPos);
                m_FieldMathTool.SetBuffer(m_SampleCCSFieldWithPosBufferCubicKernel, "voDstScalarFieldData_sampleCCSFieldWithPosBufferCubic", voDstData);

                m_FieldMathTool.SetFloats("GridOrigin", SrcFieldOriginX.x, SrcFieldOriginX.y, SrcFieldOriginX.z);
                m_FieldMathTool.SetFloats("GridSpacing", SrcFieldSpacingX.x, SrcFieldSpacingX.y, SrcFieldSpacingX.z);
                m_FieldMathTool.SetInts("GridResolution", SrcFieldResolutionX.x, SrcFieldResolutionX.y, SrcFieldResolutionX.z);
                m_FieldMathTool.SetInt("DataOffset_sampleCCSFieldWithPosBufferCubic", 0);
                m_FieldMathTool.SetBuffer(m_SampleCCSFieldWithPosBufferCubicKernel, "vSrcScalarFieldData_sampleCCSFieldWithPosBufferCubic", vSrcVectorField.getGridDataX());
                CGlobalMacroAndFunc.dispatchKernel(m_FieldMathTool, m_SampleCCSFieldWithPosBufferCubicKernel, TotalThreadNum);

                m_FieldMathTool.SetFloats("GridOrigin", SrcFieldOriginY.x, SrcFieldOriginY.y, SrcFieldOriginY.z);
                m_FieldMathTool.SetFloats("GridSpacing", SrcFieldSpacingY.x, SrcFieldSpacingY.y, SrcFieldSpacingY.z);
                m_FieldMathTool.SetInts("GridResolution", SrcFieldResolutionY.x, SrcFieldResolutionY.y, SrcFieldResolutionY.z);
                m_FieldMathTool.SetInt("DataOffset_sampleCCSFieldWithPosBufferCubic", 1);
                m_FieldMathTool.SetBuffer(m_SampleCCSFieldWithPosBufferCubicKernel, "vSrcScalarFieldData_sampleCCSFieldWithPosBufferCubic", vSrcVectorField.getGridDataY());
                CGlobalMacroAndFunc.dispatchKernel(m_FieldMathTool, m_SampleCCSFieldWithPosBufferCubicKernel, TotalThreadNum);

                m_FieldMathTool.SetFloats("GridOrigin", SrcFieldOriginZ.x, SrcFieldOriginZ.y, SrcFieldOriginZ.z);
                m_FieldMathTool.SetFloats("GridSpacing", SrcFieldSpacingZ.x, SrcFieldSpacingZ.y, SrcFieldSpacingZ.z);
                m_FieldMathTool.SetInts("GridResolution", SrcFieldResolutionZ.x, SrcFieldResolutionZ.y, SrcFieldResolutionZ.z);
                m_FieldMathTool.SetInt("DataOffset_sampleCCSFieldWithPosBufferCubic", 2);
                m_FieldMathTool.SetBuffer(m_SampleCCSFieldWithPosBufferCubicKernel, "vSrcScalarFieldData_sampleCCSFieldWithPosBufferCubic", vSrcVectorField.getGridDataZ());
                CGlobalMacroAndFunc.dispatchKernel(m_FieldMathTool, m_SampleCCSFieldWithPosBufferCubicKernel, TotalThreadNum);
            }
        }

        public static void sampleFCVFieldWithPosFieldInvoker
        (
            CFaceCenteredVectorField vSrcVectorField,
            CFaceCenteredVectorField voDstVectorField,
            CCellCenteredVectorField vSampledAbsPosXVectorField,
            CCellCenteredVectorField vSampledAbsPosYVectorField,
            CCellCenteredVectorField vSampledAbsPosZVectorField,
            ESamplingAlgorithm vSamplingAlgorithm = ESamplingAlgorithm.LINEAR
        )
        {
            if (!m_Initialized) init();

            Vector3Int DstFieldResolution = voDstVectorField.getResolution();
            Vector3Int SrcFieldResolution = vSrcVectorField.getResolution();
            Vector3 SrcFieldOrigin = vSrcVectorField.getOrigin();
            Vector3 SrcFieldSpacing = vSrcVectorField.getSpacing();
            Vector3Int DstFieldResolutionX = DstFieldResolution + new Vector3Int(1, 0, 0);
            Vector3Int DstFieldResolutionY = DstFieldResolution + new Vector3Int(0, 1, 0);
            Vector3Int DstFieldResolutionZ = DstFieldResolution + new Vector3Int(0, 0, 1);

            CGlobalMacroAndFunc._ASSERTE(DstFieldResolutionX == vSampledAbsPosXVectorField.getResolution(), "采样位置场和采样目的场维度不匹配!");
            CGlobalMacroAndFunc._ASSERTE(DstFieldResolutionY == vSampledAbsPosYVectorField.getResolution(), "采样位置场和采样目的场维度不匹配!");
            CGlobalMacroAndFunc._ASSERTE(DstFieldResolutionZ == vSampledAbsPosZVectorField.getResolution(), "采样位置场和采样目的场维度不匹配!");

            Vector3Int SrcFieldResolutionX = SrcFieldResolution + new Vector3Int(1, 0, 0);
            Vector3 SrcFieldOriginX = SrcFieldOrigin - new Vector3(SrcFieldSpacing.x / 2, 0, 0);
            Vector3 SrcFieldSpacingX = SrcFieldSpacing;
            Vector3Int SrcFieldResolutionY = SrcFieldResolution + new Vector3Int(0, 1, 0);
            Vector3 SrcFieldOriginY = SrcFieldOrigin - new Vector3(0, SrcFieldSpacing.y / 2, 0);
            Vector3 SrcFieldSpacingY = SrcFieldSpacing;
            Vector3Int SrcFieldResolutionZ = SrcFieldResolution + new Vector3Int(0, 0, 1);
            Vector3 SrcFieldOriginZ = SrcFieldOrigin - new Vector3(0, 0, SrcFieldSpacing.z / 2);
            Vector3 SrcFieldSpacingZ = SrcFieldSpacing;

            int TotalThreadNumX = (DstFieldResolutionX.x * DstFieldResolutionX.y * DstFieldResolutionX.z);
            int TotalThreadNumY = (DstFieldResolutionY.x * DstFieldResolutionY.y * DstFieldResolutionY.z);
            int TotalThreadNumZ = (DstFieldResolutionZ.x * DstFieldResolutionZ.y * DstFieldResolutionZ.z);

            if(vSamplingAlgorithm == ESamplingAlgorithm.LINEAR)
            {
                m_FieldMathTool.SetInt("DataSpan_sampleCCSFieldWithPosFieldLinear", 1);
                m_FieldMathTool.SetInt("DataOffset_sampleCCSFieldWithPosFieldLinear", 0);

                m_FieldMathTool.SetFloats("GridOrigin", SrcFieldOriginX.x, SrcFieldOriginX.y, SrcFieldOriginX.z);
                m_FieldMathTool.SetFloats("GridSpacing", SrcFieldSpacingX.x, SrcFieldSpacingX.y, SrcFieldSpacingX.z);
                m_FieldMathTool.SetInts("GridResolution", SrcFieldResolutionX.x, SrcFieldResolutionX.y, SrcFieldResolutionX.z);
                m_FieldMathTool.SetBuffer(m_SampleCCSFieldWithPosFieldLinearKernel, "vSrcScalarFieldData_sampleCCSFieldWithPosFieldLinear", vSrcVectorField.getGridDataX());
                m_FieldMathTool.SetBuffer(m_SampleCCSFieldWithPosFieldLinearKernel, "voDstScalarFieldData_sampleCCSFieldWithPosFieldLinear", voDstVectorField.getGridDataX());
                m_FieldMathTool.SetBuffer(m_SampleCCSFieldWithPosFieldLinearKernel, "vSampledAbsPosDataX_sampleCCSFieldWithPosFieldLinear", vSampledAbsPosXVectorField.getGridDataX());
                m_FieldMathTool.SetBuffer(m_SampleCCSFieldWithPosFieldLinearKernel, "vSampledAbsPosDataY_sampleCCSFieldWithPosFieldLinear", vSampledAbsPosXVectorField.getGridDataY());
                m_FieldMathTool.SetBuffer(m_SampleCCSFieldWithPosFieldLinearKernel, "vSampledAbsPosDataZ_sampleCCSFieldWithPosFieldLinear", vSampledAbsPosXVectorField.getGridDataZ());
                CGlobalMacroAndFunc.dispatchKernel(m_FieldMathTool, m_SampleCCSFieldWithPosFieldLinearKernel, TotalThreadNumX);

                m_FieldMathTool.SetFloats("GridOrigin", SrcFieldOriginY.x, SrcFieldOriginY.y, SrcFieldOriginY.z);
                m_FieldMathTool.SetFloats("GridSpacing", SrcFieldSpacingY.x, SrcFieldSpacingY.y, SrcFieldSpacingY.z);
                m_FieldMathTool.SetInts("GridResolution", SrcFieldResolutionY.x, SrcFieldResolutionY.y, SrcFieldResolutionY.z);
                m_FieldMathTool.SetBuffer(m_SampleCCSFieldWithPosFieldLinearKernel, "vSrcScalarFieldData_sampleCCSFieldWithPosFieldLinear", vSrcVectorField.getGridDataY());
                m_FieldMathTool.SetBuffer(m_SampleCCSFieldWithPosFieldLinearKernel, "voDstScalarFieldData_sampleCCSFieldWithPosFieldLinear", voDstVectorField.getGridDataY());
                m_FieldMathTool.SetBuffer(m_SampleCCSFieldWithPosFieldLinearKernel, "vSampledAbsPosDataX_sampleCCSFieldWithPosFieldLinear", vSampledAbsPosYVectorField.getGridDataX());
                m_FieldMathTool.SetBuffer(m_SampleCCSFieldWithPosFieldLinearKernel, "vSampledAbsPosDataY_sampleCCSFieldWithPosFieldLinear", vSampledAbsPosYVectorField.getGridDataY());
                m_FieldMathTool.SetBuffer(m_SampleCCSFieldWithPosFieldLinearKernel, "vSampledAbsPosDataZ_sampleCCSFieldWithPosFieldLinear", vSampledAbsPosYVectorField.getGridDataZ());
                CGlobalMacroAndFunc.dispatchKernel(m_FieldMathTool, m_SampleCCSFieldWithPosFieldLinearKernel, TotalThreadNumY);

                m_FieldMathTool.SetFloats("GridOrigin", SrcFieldOriginZ.x, SrcFieldOriginZ.y, SrcFieldOriginZ.z);
                m_FieldMathTool.SetFloats("GridSpacing", SrcFieldSpacingZ.x, SrcFieldSpacingZ.y, SrcFieldSpacingZ.z);
                m_FieldMathTool.SetInts("GridResolution", SrcFieldResolutionZ.x, SrcFieldResolutionZ.y, SrcFieldResolutionZ.z);
                m_FieldMathTool.SetBuffer(m_SampleCCSFieldWithPosFieldLinearKernel, "vSrcScalarFieldData_sampleCCSFieldWithPosFieldLinear", vSrcVectorField.getGridDataZ());
                m_FieldMathTool.SetBuffer(m_SampleCCSFieldWithPosFieldLinearKernel, "voDstScalarFieldData_sampleCCSFieldWithPosFieldLinear", voDstVectorField.getGridDataZ());
                m_FieldMathTool.SetBuffer(m_SampleCCSFieldWithPosFieldLinearKernel, "vSampledAbsPosDataX_sampleCCSFieldWithPosFieldLinear", vSampledAbsPosZVectorField.getGridDataX());
                m_FieldMathTool.SetBuffer(m_SampleCCSFieldWithPosFieldLinearKernel, "vSampledAbsPosDataY_sampleCCSFieldWithPosFieldLinear", vSampledAbsPosZVectorField.getGridDataY());
                m_FieldMathTool.SetBuffer(m_SampleCCSFieldWithPosFieldLinearKernel, "vSampledAbsPosDataZ_sampleCCSFieldWithPosFieldLinear", vSampledAbsPosZVectorField.getGridDataZ());
                CGlobalMacroAndFunc.dispatchKernel(m_FieldMathTool, m_SampleCCSFieldWithPosFieldLinearKernel, TotalThreadNumZ);
            }
            else
            {
                m_FieldMathTool.SetInt("SamplingAlg_sampleCCSFieldWithPosFieldCubic", Convert.ToInt32(vSamplingAlgorithm));

                m_FieldMathTool.SetInt("DataSpan_sampleCCSFieldWithPosFieldCubic", 1);
                m_FieldMathTool.SetInt("DataOffset_sampleCCSFieldWithPosFieldCubic", 0);

                m_FieldMathTool.SetFloats("GridOrigin", SrcFieldOriginX.x, SrcFieldOriginX.y, SrcFieldOriginX.z);
                m_FieldMathTool.SetFloats("GridSpacing", SrcFieldSpacingX.x, SrcFieldSpacingX.y, SrcFieldSpacingX.z);
                m_FieldMathTool.SetInts("GridResolution", SrcFieldResolutionX.x, SrcFieldResolutionX.y, SrcFieldResolutionX.z);
                m_FieldMathTool.SetBuffer(m_SampleCCSFieldWithPosFieldCubicKernel, "vSrcScalarFieldData_sampleCCSFieldWithPosFieldCubic", vSrcVectorField.getGridDataX());
                m_FieldMathTool.SetBuffer(m_SampleCCSFieldWithPosFieldCubicKernel, "voDstScalarFieldData_sampleCCSFieldWithPosFieldCubic", voDstVectorField.getGridDataX());
                m_FieldMathTool.SetBuffer(m_SampleCCSFieldWithPosFieldCubicKernel, "vSampledAbsPosDataX_sampleCCSFieldWithPosFieldCubic", vSampledAbsPosXVectorField.getGridDataX());
                m_FieldMathTool.SetBuffer(m_SampleCCSFieldWithPosFieldCubicKernel, "vSampledAbsPosDataY_sampleCCSFieldWithPosFieldCubic", vSampledAbsPosXVectorField.getGridDataY());
                m_FieldMathTool.SetBuffer(m_SampleCCSFieldWithPosFieldCubicKernel, "vSampledAbsPosDataZ_sampleCCSFieldWithPosFieldCubic", vSampledAbsPosXVectorField.getGridDataZ());
                CGlobalMacroAndFunc.dispatchKernel(m_FieldMathTool, m_SampleCCSFieldWithPosFieldCubicKernel, TotalThreadNumX);

                m_FieldMathTool.SetFloats("GridOrigin", SrcFieldOriginY.x, SrcFieldOriginY.y, SrcFieldOriginY.z);
                m_FieldMathTool.SetFloats("GridSpacing", SrcFieldSpacingY.x, SrcFieldSpacingY.y, SrcFieldSpacingY.z);
                m_FieldMathTool.SetInts("GridResolution", SrcFieldResolutionY.x, SrcFieldResolutionY.y, SrcFieldResolutionY.z);
                m_FieldMathTool.SetBuffer(m_SampleCCSFieldWithPosFieldCubicKernel, "vSrcScalarFieldData_sampleCCSFieldWithPosFieldCubic", vSrcVectorField.getGridDataY());
                m_FieldMathTool.SetBuffer(m_SampleCCSFieldWithPosFieldCubicKernel, "voDstScalarFieldData_sampleCCSFieldWithPosFieldCubic", voDstVectorField.getGridDataY());
                m_FieldMathTool.SetBuffer(m_SampleCCSFieldWithPosFieldCubicKernel, "vSampledAbsPosDataX_sampleCCSFieldWithPosFieldCubic", vSampledAbsPosYVectorField.getGridDataX());
                m_FieldMathTool.SetBuffer(m_SampleCCSFieldWithPosFieldCubicKernel, "vSampledAbsPosDataY_sampleCCSFieldWithPosFieldCubic", vSampledAbsPosYVectorField.getGridDataY());
                m_FieldMathTool.SetBuffer(m_SampleCCSFieldWithPosFieldCubicKernel, "vSampledAbsPosDataZ_sampleCCSFieldWithPosFieldCubic", vSampledAbsPosYVectorField.getGridDataZ());
                CGlobalMacroAndFunc.dispatchKernel(m_FieldMathTool, m_SampleCCSFieldWithPosFieldCubicKernel, TotalThreadNumY);

                m_FieldMathTool.SetFloats("GridOrigin", SrcFieldOriginZ.x, SrcFieldOriginZ.y, SrcFieldOriginZ.z);
                m_FieldMathTool.SetFloats("GridSpacing", SrcFieldSpacingZ.x, SrcFieldSpacingZ.y, SrcFieldSpacingZ.z);
                m_FieldMathTool.SetInts("GridResolution", SrcFieldResolutionZ.x, SrcFieldResolutionZ.y, SrcFieldResolutionZ.z);
                m_FieldMathTool.SetBuffer(m_SampleCCSFieldWithPosFieldCubicKernel, "vSrcScalarFieldData_sampleCCSFieldWithPosFieldCubic", vSrcVectorField.getGridDataZ());
                m_FieldMathTool.SetBuffer(m_SampleCCSFieldWithPosFieldCubicKernel, "voDstScalarFieldData_sampleCCSFieldWithPosFieldCubic", voDstVectorField.getGridDataZ());
                m_FieldMathTool.SetBuffer(m_SampleCCSFieldWithPosFieldCubicKernel, "vSampledAbsPosDataX_sampleCCSFieldWithPosFieldCubic", vSampledAbsPosZVectorField.getGridDataX());
                m_FieldMathTool.SetBuffer(m_SampleCCSFieldWithPosFieldCubicKernel, "vSampledAbsPosDataY_sampleCCSFieldWithPosFieldCubic", vSampledAbsPosZVectorField.getGridDataY());
                m_FieldMathTool.SetBuffer(m_SampleCCSFieldWithPosFieldCubicKernel, "vSampledAbsPosDataZ_sampleCCSFieldWithPosFieldCubic", vSampledAbsPosZVectorField.getGridDataZ());
                CGlobalMacroAndFunc.dispatchKernel(m_FieldMathTool, m_SampleCCSFieldWithPosFieldCubicKernel, TotalThreadNumZ);
            }     
        }

        public static void clampCCSFieldExtremaInvoker
        (
            CCellCenteredScalarField vSrcScalarField,
            CCellCenteredScalarField vioDstScalarField,
            CCellCenteredVectorField vSampledAbsPosVectorField
        )
        {
            if (!m_Initialized) init();

            Vector3Int DstFieldResolution = vioDstScalarField.getResolution();
            Vector3Int SrcFieldResolution = vSrcScalarField.getResolution();
            Vector3 SrcFieldOrigin = vSrcScalarField.getOrigin();
            Vector3 SrcFieldSpacing = vSrcScalarField.getSpacing();

            CGlobalMacroAndFunc._ASSERTE(DstFieldResolution == vSampledAbsPosVectorField.getResolution(), "采样位置场和采样目的场维度不匹配!");

            m_FieldMathTool.SetFloats("GridOrigin", SrcFieldOrigin.x, SrcFieldOrigin.y, SrcFieldOrigin.z);
            m_FieldMathTool.SetFloats("GridSpacing", SrcFieldSpacing.x, SrcFieldSpacing.y, SrcFieldSpacing.z);
            m_FieldMathTool.SetInts("GridResolution", SrcFieldResolution.x, SrcFieldResolution.y, SrcFieldResolution.z);

            m_FieldMathTool.SetInt("DataSpan_clampExtrema", 1);
            m_FieldMathTool.SetInt("DataOffset_clampExtrema", 0);
            m_FieldMathTool.SetBuffer(m_ClampExtremaKernel, "vSrcScalarFieldData_clampExtrema", vSrcScalarField.getGridData());
            m_FieldMathTool.SetBuffer(m_ClampExtremaKernel, "vioDstScalarFieldData_clampExtrema", vioDstScalarField.getGridData());
            m_FieldMathTool.SetBuffer(m_ClampExtremaKernel, "vSampledAbsPosDataX_clampExtrema", vSampledAbsPosVectorField.getGridDataX());
            m_FieldMathTool.SetBuffer(m_ClampExtremaKernel, "vSampledAbsPosDataY_clampExtrema", vSampledAbsPosVectorField.getGridDataY());
            m_FieldMathTool.SetBuffer(m_ClampExtremaKernel, "vSampledAbsPosDataZ_clampExtrema", vSampledAbsPosVectorField.getGridDataZ());

            int TotalThreadNum = DstFieldResolution.x * DstFieldResolution.y * DstFieldResolution.z;
            CGlobalMacroAndFunc.dispatchKernel(m_FieldMathTool, m_ClampExtremaKernel, TotalThreadNum);
        }

        public static void clampCCVFieldExtremaInvoker
        (
            CCellCenteredVectorField vSrcVectorField,
            CCellCenteredVectorField vioDstVectorField,
            CCellCenteredVectorField vSampledAbsPosVectorField
        )
        {
            if (!m_Initialized) init();

            Vector3Int DstFieldResolution = vioDstVectorField.getResolution();
            Vector3Int SrcFieldResolution = vSrcVectorField.getResolution();
            Vector3 SrcFieldOrigin = vSrcVectorField.getOrigin();
            Vector3 SrcFieldSpacing = vSrcVectorField.getSpacing();

            CGlobalMacroAndFunc._ASSERTE(DstFieldResolution == vSampledAbsPosVectorField.getResolution(), "采样位置场和采样目的场维度不匹配!");

            int TotalThreadNum = DstFieldResolution.x * DstFieldResolution.y * DstFieldResolution.z;

            m_FieldMathTool.SetFloats("GridOrigin", SrcFieldOrigin.x, SrcFieldOrigin.y, SrcFieldOrigin.z);
            m_FieldMathTool.SetFloats("GridSpacing", SrcFieldSpacing.x, SrcFieldSpacing.y, SrcFieldSpacing.z);
            m_FieldMathTool.SetInts("GridResolution", SrcFieldResolution.x, SrcFieldResolution.y, SrcFieldResolution.z);
            m_FieldMathTool.SetInt("DataSpan_clampExtrema", 1);
            m_FieldMathTool.SetInt("DataOffset_clampExtrema", 0);
            m_FieldMathTool.SetBuffer(m_ClampExtremaKernel, "vSrcScalarFieldData_clampExtrema", vSrcVectorField.getGridDataX());
            m_FieldMathTool.SetBuffer(m_ClampExtremaKernel, "vioDstScalarFieldData_clampExtrema", vioDstVectorField.getGridDataX());
            m_FieldMathTool.SetBuffer(m_ClampExtremaKernel, "vSampledAbsPosDataX_clampExtrema", vSampledAbsPosVectorField.getGridDataX());
            m_FieldMathTool.SetBuffer(m_ClampExtremaKernel, "vSampledAbsPosDataY_clampExtrema", vSampledAbsPosVectorField.getGridDataY());
            m_FieldMathTool.SetBuffer(m_ClampExtremaKernel, "vSampledAbsPosDataZ_clampExtrema", vSampledAbsPosVectorField.getGridDataZ());
            CGlobalMacroAndFunc.dispatchKernel(m_FieldMathTool, m_ClampExtremaKernel, TotalThreadNum);

            m_FieldMathTool.SetFloats("GridOrigin", SrcFieldOrigin.x, SrcFieldOrigin.y, SrcFieldOrigin.z);
            m_FieldMathTool.SetFloats("GridSpacing", SrcFieldSpacing.x, SrcFieldSpacing.y, SrcFieldSpacing.z);
            m_FieldMathTool.SetInts("GridResolution", SrcFieldResolution.x, SrcFieldResolution.y, SrcFieldResolution.z);
            m_FieldMathTool.SetInt("DataSpan_clampExtrema", 1);
            m_FieldMathTool.SetInt("DataOffset_clampExtrema", 0);
            m_FieldMathTool.SetBuffer(m_ClampExtremaKernel, "vSrcScalarFieldData_clampExtrema", vSrcVectorField.getGridDataY());
            m_FieldMathTool.SetBuffer(m_ClampExtremaKernel, "vioDstScalarFieldData_clampExtrema", vioDstVectorField.getGridDataY());
            m_FieldMathTool.SetBuffer(m_ClampExtremaKernel, "vSampledAbsPosDataX_clampExtrema", vSampledAbsPosVectorField.getGridDataX());
            m_FieldMathTool.SetBuffer(m_ClampExtremaKernel, "vSampledAbsPosDataY_clampExtrema", vSampledAbsPosVectorField.getGridDataY());
            m_FieldMathTool.SetBuffer(m_ClampExtremaKernel, "vSampledAbsPosDataZ_clampExtrema", vSampledAbsPosVectorField.getGridDataZ());
            CGlobalMacroAndFunc.dispatchKernel(m_FieldMathTool, m_ClampExtremaKernel, TotalThreadNum);

            m_FieldMathTool.SetFloats("GridOrigin", SrcFieldOrigin.x, SrcFieldOrigin.y, SrcFieldOrigin.z);
            m_FieldMathTool.SetFloats("GridSpacing", SrcFieldSpacing.x, SrcFieldSpacing.y, SrcFieldSpacing.z);
            m_FieldMathTool.SetInts("GridResolution", SrcFieldResolution.x, SrcFieldResolution.y, SrcFieldResolution.z);
            m_FieldMathTool.SetInt("DataSpan_clampExtrema", 1);
            m_FieldMathTool.SetInt("DataOffset_clampExtrema", 0);
            m_FieldMathTool.SetBuffer(m_ClampExtremaKernel, "vSrcScalarFieldData_clampExtrema", vSrcVectorField.getGridDataZ());
            m_FieldMathTool.SetBuffer(m_ClampExtremaKernel, "vioDstScalarFieldData_clampExtrema", vioDstVectorField.getGridDataZ());
            m_FieldMathTool.SetBuffer(m_ClampExtremaKernel, "vSampledAbsPosDataX_clampExtrema", vSampledAbsPosVectorField.getGridDataX());
            m_FieldMathTool.SetBuffer(m_ClampExtremaKernel, "vSampledAbsPosDataY_clampExtrema", vSampledAbsPosVectorField.getGridDataY());
            m_FieldMathTool.SetBuffer(m_ClampExtremaKernel, "vSampledAbsPosDataZ_clampExtrema", vSampledAbsPosVectorField.getGridDataZ());
            CGlobalMacroAndFunc.dispatchKernel(m_FieldMathTool, m_ClampExtremaKernel, TotalThreadNum);
        }

        public static void clampFCVFieldExtremaInvoker
        (
            CFaceCenteredVectorField vSrcVectorField,
            CFaceCenteredVectorField vioDstVectorField,
            CCellCenteredVectorField vSampledAbsPosXVectorField,
            CCellCenteredVectorField vSampledAbsPosYVectorField,
            CCellCenteredVectorField vSampledAbsPosZVectorField
        )
        {
            if (!m_Initialized) init();

            Vector3Int DstFieldResolution = vioDstVectorField.getResolution();
            Vector3Int SrcFieldResolution = vSrcVectorField.getResolution();
            Vector3 SrcFieldOrigin = vSrcVectorField.getOrigin();
            Vector3 SrcFieldSpacing = vSrcVectorField.getSpacing();
            Vector3Int DstFieldResolutionX = DstFieldResolution + new Vector3Int(1, 0, 0);
            Vector3Int DstFieldResolutionY = DstFieldResolution + new Vector3Int(0, 1, 0);
            Vector3Int DstFieldResolutionZ = DstFieldResolution + new Vector3Int(0, 0, 1);

            CGlobalMacroAndFunc._ASSERTE(DstFieldResolutionX == vSampledAbsPosXVectorField.getResolution(), "采样位置场和采样目的场维度不匹配!");
            CGlobalMacroAndFunc._ASSERTE(DstFieldResolutionY == vSampledAbsPosYVectorField.getResolution(), "采样位置场和采样目的场维度不匹配!");
            CGlobalMacroAndFunc._ASSERTE(DstFieldResolutionZ == vSampledAbsPosZVectorField.getResolution(), "采样位置场和采样目的场维度不匹配!");

            Vector3Int SrcFieldResolutionX = SrcFieldResolution + new Vector3Int(1, 0, 0);
            Vector3 SrcFieldOriginX = SrcFieldOrigin - new Vector3(SrcFieldSpacing.x / 2, 0, 0);
            Vector3 SrcFieldSpacingX = SrcFieldSpacing;
            Vector3Int SrcFieldResolutionY = SrcFieldResolution + new Vector3Int(0, 1, 0);
            Vector3 SrcFieldOriginY = SrcFieldOrigin - new Vector3(0, SrcFieldSpacing.y / 2, 0);
            Vector3 SrcFieldSpacingY = SrcFieldSpacing;
            Vector3Int SrcFieldResolutionZ = SrcFieldResolution + new Vector3Int(0, 0, 1);
            Vector3 SrcFieldOriginZ = SrcFieldOrigin - new Vector3(0, 0, SrcFieldSpacing.z / 2);
            Vector3 SrcFieldSpacingZ = SrcFieldSpacing;

            int TotalThreadNumX = (DstFieldResolutionX.x * DstFieldResolutionX.y * DstFieldResolutionX.z);
            int TotalThreadNumY = (DstFieldResolutionY.x * DstFieldResolutionY.y * DstFieldResolutionY.z);
            int TotalThreadNumZ = (DstFieldResolutionZ.x * DstFieldResolutionZ.y * DstFieldResolutionZ.z);

            m_FieldMathTool.SetInt("DataSpan_clampExtrema", 1);
            m_FieldMathTool.SetInt("DataOffset_clampExtrema", 0);

            m_FieldMathTool.SetFloats("GridOrigin", SrcFieldOriginX.x, SrcFieldOriginX.y, SrcFieldOriginX.z);
            m_FieldMathTool.SetFloats("GridSpacing", SrcFieldSpacingX.x, SrcFieldSpacingX.y, SrcFieldSpacingX.z);
            m_FieldMathTool.SetInts("GridResolution", SrcFieldResolutionX.x, SrcFieldResolutionX.y, SrcFieldResolutionX.z);
            m_FieldMathTool.SetBuffer(m_ClampExtremaKernel, "vSrcScalarFieldData_clampExtrema", vSrcVectorField.getGridDataX());
            m_FieldMathTool.SetBuffer(m_ClampExtremaKernel, "vioDstScalarFieldData_clampExtrema", vioDstVectorField.getGridDataX());
            m_FieldMathTool.SetBuffer(m_ClampExtremaKernel, "vSampledAbsPosDataX_clampExtrema", vSampledAbsPosXVectorField.getGridDataX());
            m_FieldMathTool.SetBuffer(m_ClampExtremaKernel, "vSampledAbsPosDataY_clampExtrema", vSampledAbsPosXVectorField.getGridDataY());
            m_FieldMathTool.SetBuffer(m_ClampExtremaKernel, "vSampledAbsPosDataZ_clampExtrema", vSampledAbsPosXVectorField.getGridDataZ());
            CGlobalMacroAndFunc.dispatchKernel(m_FieldMathTool, m_ClampExtremaKernel, TotalThreadNumX);

            m_FieldMathTool.SetFloats("GridOrigin", SrcFieldOriginY.x, SrcFieldOriginY.y, SrcFieldOriginY.z);
            m_FieldMathTool.SetFloats("GridSpacing", SrcFieldSpacingY.x, SrcFieldSpacingY.y, SrcFieldSpacingY.z);
            m_FieldMathTool.SetInts("GridResolution", SrcFieldResolutionY.x, SrcFieldResolutionY.y, SrcFieldResolutionY.z);
            m_FieldMathTool.SetBuffer(m_ClampExtremaKernel, "vSrcScalarFieldData_clampExtrema", vSrcVectorField.getGridDataY());
            m_FieldMathTool.SetBuffer(m_ClampExtremaKernel, "vioDstScalarFieldData_clampExtrema", vioDstVectorField.getGridDataY());
            m_FieldMathTool.SetBuffer(m_ClampExtremaKernel, "vSampledAbsPosDataX_clampExtrema", vSampledAbsPosYVectorField.getGridDataX());
            m_FieldMathTool.SetBuffer(m_ClampExtremaKernel, "vSampledAbsPosDataY_clampExtrema", vSampledAbsPosYVectorField.getGridDataY());
            m_FieldMathTool.SetBuffer(m_ClampExtremaKernel, "vSampledAbsPosDataZ_clampExtrema", vSampledAbsPosYVectorField.getGridDataZ());
            CGlobalMacroAndFunc.dispatchKernel(m_FieldMathTool, m_ClampExtremaKernel, TotalThreadNumY);

            m_FieldMathTool.SetFloats("GridOrigin", SrcFieldOriginZ.x, SrcFieldOriginZ.y, SrcFieldOriginZ.z);
            m_FieldMathTool.SetFloats("GridSpacing", SrcFieldSpacingZ.x, SrcFieldSpacingZ.y, SrcFieldSpacingZ.z);
            m_FieldMathTool.SetInts("GridResolution", SrcFieldResolutionZ.x, SrcFieldResolutionZ.y, SrcFieldResolutionZ.z);
            m_FieldMathTool.SetBuffer(m_ClampExtremaKernel, "vSrcScalarFieldData_clampExtrema", vSrcVectorField.getGridDataZ());
            m_FieldMathTool.SetBuffer(m_ClampExtremaKernel, "vioDstScalarFieldData_clampExtrema", vioDstVectorField.getGridDataZ());
            m_FieldMathTool.SetBuffer(m_ClampExtremaKernel, "vSampledAbsPosDataX_clampExtrema", vSampledAbsPosZVectorField.getGridDataX());
            m_FieldMathTool.SetBuffer(m_ClampExtremaKernel, "vSampledAbsPosDataY_clampExtrema", vSampledAbsPosZVectorField.getGridDataY());
            m_FieldMathTool.SetBuffer(m_ClampExtremaKernel, "vSampledAbsPosDataZ_clampExtrema", vSampledAbsPosZVectorField.getGridDataZ());
            CGlobalMacroAndFunc.dispatchKernel(m_FieldMathTool, m_ClampExtremaKernel, TotalThreadNumZ);
        }

        public static void gradientInvoker(CCellCenteredScalarField vScalarField, CCellCenteredVectorField voGradientField)
        {
            if (!m_Initialized) init();

            Vector3Int Resolution = vScalarField.getResolution();
            Vector3 Origin = vScalarField.getOrigin();
            Vector3 Spacing = vScalarField.getSpacing();

            CGlobalMacroAndFunc._ASSERTE(voGradientField.getResolution() == Resolution, "源标量场和目的梯度场维度不匹配!");

            m_FieldMathTool.SetFloats("GridOrigin", Origin.x, Origin.y, Origin.z);
            m_FieldMathTool.SetFloats("GridSpacing", Spacing.x, Spacing.y, Spacing.z);
            m_FieldMathTool.SetInts("GridResolution", Resolution.x, Resolution.y, Resolution.z);

            m_FieldMathTool.SetBuffer(m_CCSFieldGradientKernel, "vSrcScalarFieldData_CCSFieldGradient", vScalarField.getGridData());
            m_FieldMathTool.SetBuffer(m_CCSFieldGradientKernel, "voGradientFieldDataX_CCSFieldGradient", voGradientField.getGridDataX());
            m_FieldMathTool.SetBuffer(m_CCSFieldGradientKernel, "voGradientFieldDataY_CCSFieldGradient", voGradientField.getGridDataY());
            m_FieldMathTool.SetBuffer(m_CCSFieldGradientKernel, "voGradientFieldDataZ_CCSFieldGradient", voGradientField.getGridDataZ());

            int TotalThreadNum = (Resolution.x * Resolution.y * Resolution.z);
            CGlobalMacroAndFunc.dispatchKernel(m_FieldMathTool, m_CCSFieldGradientKernel, TotalThreadNum);
        }

        public static void laplacianInvoker(CCellCenteredScalarField vScalarField, CCellCenteredScalarField voLaplacianField)
        {
            if (!m_Initialized) init();

            Vector3Int Resolution = vScalarField.getResolution();
            Vector3 Origin = vScalarField.getOrigin();
            Vector3 Spacing = vScalarField.getSpacing();

            CGlobalMacroAndFunc._ASSERTE(voLaplacianField.getResolution() == Resolution, "源标量场和目的拉普拉斯场维度不匹配!");

            m_FieldMathTool.SetFloats("GridOrigin", Origin.x, Origin.y, Origin.z);
            m_FieldMathTool.SetFloats("GridSpacing", Spacing.x, Spacing.y, Spacing.z);
            m_FieldMathTool.SetInts("GridResolution", Resolution.x, Resolution.y, Resolution.z);

            m_FieldMathTool.SetBuffer(m_CCSFieldLaplacianKernel, "vSrcScalarFieldData_CCSFieldLaplacian", vScalarField.getGridData());
            m_FieldMathTool.SetBuffer(m_CCSFieldLaplacianKernel, "voLaplacianFieldData_CCSFieldLaplacian", voLaplacianField.getGridData());

            int TotalThreadNum = (Resolution.x * Resolution.y * Resolution.z);
            CGlobalMacroAndFunc.dispatchKernel(m_FieldMathTool, m_CCSFieldLaplacianKernel, TotalThreadNum);
        }

        public static void divergenceInvoker(CCellCenteredVectorField vVectorField, CCellCenteredScalarField voDivergenceField)
        {
            if (!m_Initialized) init();

            Vector3Int Resolution = vVectorField.getResolution();
            Vector3 Origin = vVectorField.getOrigin();
            Vector3 Spacing = vVectorField.getSpacing();

            CGlobalMacroAndFunc._ASSERTE(voDivergenceField.getResolution() == Resolution, "源向量场和目的散度场维度不匹配!");

            m_FieldMathTool.SetFloats("GridOrigin", Origin.x, Origin.y, Origin.z);
            m_FieldMathTool.SetFloats("GridSpacing", Spacing.x, Spacing.y, Spacing.z);
            m_FieldMathTool.SetInts("GridResolution", Resolution.x, Resolution.y, Resolution.z);

            m_FieldMathTool.SetBuffer(m_CCVFieldDivergenceKernel, "vSrcVectorFieldDataX_CCVFieldDivergence", vVectorField.getGridDataX());
            m_FieldMathTool.SetBuffer(m_CCVFieldDivergenceKernel, "vSrcVectorFieldDataY_CCVFieldDivergence", vVectorField.getGridDataY());
            m_FieldMathTool.SetBuffer(m_CCVFieldDivergenceKernel, "vSrcVectorFieldDataZ_CCVFieldDivergence", vVectorField.getGridDataZ());
            m_FieldMathTool.SetBuffer(m_CCVFieldDivergenceKernel, "voDivergenceFieldData_CCVFieldDivergence", voDivergenceField.getGridData());

            int TotalThreadNum = (Resolution.x * Resolution.y * Resolution.z);
            CGlobalMacroAndFunc.dispatchKernel(m_FieldMathTool, m_CCVFieldDivergenceKernel, TotalThreadNum);
        }

        public static void divergenceInvoker(CFaceCenteredVectorField vVectorField, CCellCenteredScalarField voDivergenceField)
        {
            if (!m_Initialized) init();

            Vector3Int Resolution = vVectorField.getResolution();
            Vector3 Origin = vVectorField.getOrigin();
            Vector3 Spacing = vVectorField.getSpacing();

            CGlobalMacroAndFunc._ASSERTE(voDivergenceField.getResolution() == Resolution, "源向量场和目的散度场维度不匹配!");

            m_FieldMathTool.SetFloats("GridOrigin", Origin.x, Origin.y, Origin.z);
            m_FieldMathTool.SetFloats("GridSpacing", Spacing.x, Spacing.y, Spacing.z);
            m_FieldMathTool.SetInts("GridResolution", Resolution.x, Resolution.y, Resolution.z);

            m_FieldMathTool.SetBuffer(m_FCVFieldDivergenceKernel, "vSrcVectorFieldDataX_FCVFieldDivergence", vVectorField.getGridDataX());
            m_FieldMathTool.SetBuffer(m_FCVFieldDivergenceKernel, "vSrcVectorFieldDataY_FCVFieldDivergence", vVectorField.getGridDataY());
            m_FieldMathTool.SetBuffer(m_FCVFieldDivergenceKernel, "vSrcVectorFieldDataZ_FCVFieldDivergence", vVectorField.getGridDataZ());
            m_FieldMathTool.SetBuffer(m_FCVFieldDivergenceKernel, "voDivergenceFieldData_FCVFieldDivergence", voDivergenceField.getGridData());

            int TotalThreadNum = (Resolution.x * Resolution.y * Resolution.z);
            CGlobalMacroAndFunc.dispatchKernel(m_FieldMathTool, m_FCVFieldDivergenceKernel, TotalThreadNum);
        }

        public static void curlInvoker(CCellCenteredVectorField vVectorField, CCellCenteredVectorField voCurlField)
        {
            if (!m_Initialized) init();

            Vector3Int Resolution = vVectorField.getResolution();
            Vector3 Origin = vVectorField.getOrigin();
            Vector3 Spacing = vVectorField.getSpacing();

            CGlobalMacroAndFunc._ASSERTE(voCurlField.getResolution() == Resolution, "源向量场和目的旋度场维度不匹配!");

            m_FieldMathTool.SetFloats("GridOrigin", Origin.x, Origin.y, Origin.z);
            m_FieldMathTool.SetFloats("GridSpacing", Spacing.x, Spacing.y, Spacing.z);
            m_FieldMathTool.SetInts("GridResolution", Resolution.x, Resolution.y, Resolution.z);

            m_FieldMathTool.SetBuffer(m_CCVFieldCurlKernel, "vSrcVectorFieldDataX_CCVFieldCurl", vVectorField.getGridDataX());
            m_FieldMathTool.SetBuffer(m_CCVFieldCurlKernel, "vSrcVectorFieldDataY_CCVFieldCurl", vVectorField.getGridDataY());
            m_FieldMathTool.SetBuffer(m_CCVFieldCurlKernel, "vSrcVectorFieldDataZ_CCVFieldCurl", vVectorField.getGridDataZ());
            m_FieldMathTool.SetBuffer(m_CCVFieldCurlKernel, "voCurlFieldDataX_CCVFieldCurl", voCurlField.getGridDataX());
            m_FieldMathTool.SetBuffer(m_CCVFieldCurlKernel, "voCurlFieldDataY_CCVFieldCurl", voCurlField.getGridDataY());
            m_FieldMathTool.SetBuffer(m_CCVFieldCurlKernel, "voCurlFieldDataZ_CCVFieldCurl", voCurlField.getGridDataZ());

            int TotalThreadNum = (Resolution.x * Resolution.y * Resolution.z);
            CGlobalMacroAndFunc.dispatchKernel(m_FieldMathTool, m_CCVFieldCurlKernel, TotalThreadNum);
        }

        public static void curlInvoker(CFaceCenteredVectorField vVectorField, CCellCenteredVectorField voCurlField)
        {
            if (!m_Initialized) init();

            Vector3Int Resolution = vVectorField.getResolution();
            Vector3 Origin = vVectorField.getOrigin();
            Vector3 Spacing = vVectorField.getSpacing();

            CGlobalMacroAndFunc._ASSERTE(voCurlField.getResolution() == Resolution, "源向量场和目的旋度场维度不匹配!");

            m_FieldMathTool.SetFloats("GridOrigin", Origin.x, Origin.y, Origin.z);
            m_FieldMathTool.SetFloats("GridSpacing", Spacing.x, Spacing.y, Spacing.z);
            m_FieldMathTool.SetInts("GridResolution", Resolution.x, Resolution.y, Resolution.z);

            m_FieldMathTool.SetBuffer(m_FCVFieldCurlKernel, "vSrcVectorFieldDataX_FCVFieldCurl", vVectorField.getGridDataX());
            m_FieldMathTool.SetBuffer(m_FCVFieldCurlKernel, "vSrcVectorFieldDataY_FCVFieldCurl", vVectorField.getGridDataY());
            m_FieldMathTool.SetBuffer(m_FCVFieldCurlKernel, "vSrcVectorFieldDataZ_FCVFieldCurl", vVectorField.getGridDataZ());
            m_FieldMathTool.SetBuffer(m_FCVFieldCurlKernel, "voCurlFieldDataX_FCVFieldCurl", voCurlField.getGridDataX());
            m_FieldMathTool.SetBuffer(m_FCVFieldCurlKernel, "voCurlFieldDataY_FCVFieldCurl", voCurlField.getGridDataY());
            m_FieldMathTool.SetBuffer(m_FCVFieldCurlKernel, "voCurlFieldDataZ_FCVFieldCurl", voCurlField.getGridDataZ());

            int TotalThreadNum = (Resolution.x * Resolution.y * Resolution.z);
            CGlobalMacroAndFunc.dispatchKernel(m_FieldMathTool, m_FCVFieldCurlKernel, TotalThreadNum);
        }

        public static void lengthInvoker(CCellCenteredVectorField vVectorField, CCellCenteredScalarField voLengthField)
        {
            if (!m_Initialized) init();

            Vector3Int Resolution = vVectorField.getResolution();
            Vector3 Origin = vVectorField.getOrigin();
            Vector3 Spacing = vVectorField.getSpacing();

            CGlobalMacroAndFunc._ASSERTE(voLengthField.getResolution() == Resolution, "源向量场和目的模场维度不匹配!");

            m_FieldMathTool.SetFloats("GridOrigin", Origin.x, Origin.y, Origin.z);
            m_FieldMathTool.SetFloats("GridSpacing", Spacing.x, Spacing.y, Spacing.z);
            m_FieldMathTool.SetInts("GridResolution", Resolution.x, Resolution.y, Resolution.z);

            m_FieldMathTool.SetBuffer(m_CCVFieldLengthKernel, "vSrcVectorFieldDataX_CCVFieldLength", vVectorField.getGridDataX());
            m_FieldMathTool.SetBuffer(m_CCVFieldLengthKernel, "vSrcVectorFieldDataY_CCVFieldLength", vVectorField.getGridDataY());
            m_FieldMathTool.SetBuffer(m_CCVFieldLengthKernel, "vSrcVectorFieldDataZ_CCVFieldLength", vVectorField.getGridDataZ());
            m_FieldMathTool.SetBuffer(m_CCVFieldLengthKernel, "voLengthFieldData_CCVFieldLength", voLengthField.getGridData());

            int TotalThreadNum = (Resolution.x * Resolution.y * Resolution.z);
            CGlobalMacroAndFunc.dispatchKernel(m_FieldMathTool, m_CCVFieldLengthKernel, TotalThreadNum);
        }

        public static void lengthInvoker(CFaceCenteredVectorField vVectorField, CCellCenteredScalarField voLengthField)
        {
            if (!m_Initialized) init();

            Vector3Int Resolution = vVectorField.getResolution();
            Vector3 Origin = vVectorField.getOrigin();
            Vector3 Spacing = vVectorField.getSpacing();

            CGlobalMacroAndFunc._ASSERTE(voLengthField.getResolution() == Resolution, "源向量场和目的模场维度不匹配!");

            m_FieldMathTool.SetFloats("GridOrigin", Origin.x, Origin.y, Origin.z);
            m_FieldMathTool.SetFloats("GridSpacing", Spacing.x, Spacing.y, Spacing.z);
            m_FieldMathTool.SetInts("GridResolution", Resolution.x, Resolution.y, Resolution.z);

            m_FieldMathTool.SetBuffer(m_FCVFieldLengthKernel, "vSrcVectorFieldDataX_FCVFieldLength", vVectorField.getGridDataX());
            m_FieldMathTool.SetBuffer(m_FCVFieldLengthKernel, "vSrcVectorFieldDataY_FCVFieldLength", vVectorField.getGridDataY());
            m_FieldMathTool.SetBuffer(m_FCVFieldLengthKernel, "vSrcVectorFieldDataZ_FCVFieldLength", vVectorField.getGridDataZ());
            m_FieldMathTool.SetBuffer(m_FCVFieldLengthKernel, "voLengthFieldData_FCVFieldLength", voLengthField.getGridData());

            int TotalThreadNum = (Resolution.x * Resolution.y * Resolution.z);
            CGlobalMacroAndFunc.dispatchKernel(m_FieldMathTool, m_FCVFieldLengthKernel, TotalThreadNum);
        }

        public static void transferFCVField2CCVFieldInvoker(CFaceCenteredVectorField vFCVField, CCellCenteredVectorField voCCVField)
        {
            if (!m_Initialized) init();

            Vector3Int Resolution = vFCVField.getResolution();

            CGlobalMacroAndFunc._ASSERTE(voCCVField.getResolution() == Resolution, "源向量场和目的模场维度不匹配!");

            m_FieldMathTool.SetInts("GridResolution", Resolution.x, Resolution.y, Resolution.z);

            m_FieldMathTool.SetBuffer(m_TransferFCVField2CCVFieldKernel, "vFCVFieldDataX_transferFCVField2CCVField", vFCVField.getGridDataX());
            m_FieldMathTool.SetBuffer(m_TransferFCVField2CCVFieldKernel, "vFCVFieldDataY_transferFCVField2CCVField", vFCVField.getGridDataY());
            m_FieldMathTool.SetBuffer(m_TransferFCVField2CCVFieldKernel, "vFCVFieldDataZ_transferFCVField2CCVField", vFCVField.getGridDataZ());
            m_FieldMathTool.SetBuffer(m_TransferFCVField2CCVFieldKernel, "voCCVFieldDataX_transferFCVField2CCVField", voCCVField.getGridDataX());
            m_FieldMathTool.SetBuffer(m_TransferFCVField2CCVFieldKernel, "voCCVFieldDataY_transferFCVField2CCVField", voCCVField.getGridDataY());
            m_FieldMathTool.SetBuffer(m_TransferFCVField2CCVFieldKernel, "voCCVFieldDataZ_transferFCVField2CCVField", voCCVField.getGridDataZ());

            int TotalThreadNum = (Resolution.x * Resolution.y * Resolution.z);
            CGlobalMacroAndFunc.dispatchKernel(m_FieldMathTool, m_TransferFCVField2CCVFieldKernel, TotalThreadNum);
        }

        public static void transferCCVField2FCVFieldInvoker(CCellCenteredVectorField vCCVField, CFaceCenteredVectorField voFCVField)
        {
            if (!m_Initialized) init();

            Vector3Int Resolution = vCCVField.getResolution();

            CGlobalMacroAndFunc._ASSERTE(voFCVField.getResolution() == Resolution, "源向量场和目的模场维度不匹配!");

            m_FieldMathTool.SetInts("GridResolution", Resolution.x, Resolution.y, Resolution.z);

            m_FieldMathTool.SetBuffer(m_TransferCCVField2FCVFieldKernel, "vCCVFieldDataX_transferCCVField2FCVField", vCCVField.getGridDataX());
            m_FieldMathTool.SetBuffer(m_TransferCCVField2FCVFieldKernel, "vCCVFieldDataY_transferCCVField2FCVField", vCCVField.getGridDataY());
            m_FieldMathTool.SetBuffer(m_TransferCCVField2FCVFieldKernel, "vCCVFieldDataZ_transferCCVField2FCVField", vCCVField.getGridDataZ());
            m_FieldMathTool.SetBuffer(m_TransferCCVField2FCVFieldKernel, "voFCVFieldDataX_transferCCVField2FCVField", voFCVField.getGridDataX());
            m_FieldMathTool.SetBuffer(m_TransferCCVField2FCVFieldKernel, "voFCVFieldDataY_transferCCVField2FCVField", voFCVField.getGridDataY());
            m_FieldMathTool.SetBuffer(m_TransferCCVField2FCVFieldKernel, "voFCVFieldDataZ_transferCCVField2FCVField", voFCVField.getGridDataZ());

            int TotalThreadNum = (Resolution.x * Resolution.y * Resolution.z);
            CGlobalMacroAndFunc.dispatchKernel(m_FieldMathTool, m_TransferCCVField2FCVFieldKernel, TotalThreadNum);
        }

        //TODO
        public static CCellCenteredScalarField unionSDFInvoker(CCellCenteredScalarField vSDF1, CCellCenteredScalarField vSDF2)
        {
            if (!m_Initialized) init();

            Vector3Int Resolution = new Vector3Int(1, 1, 1);
            Vector3 Origin = new Vector3(0, 0, 0);
            Vector3 Spacing = new Vector3(1, 1, 1);

            if (vSDF1.getResolution() == vSDF2.getResolution())
                Resolution = vSDF1.getResolution();
            else
            {
                Debug.LogError("参与并集运算的两个SDF场分辨率不同！");
                return null;
            }

            if (vSDF1.getOrigin() == vSDF2.getOrigin())
                Origin = vSDF1.getOrigin();
            else
                Debug.LogWarning("参与并集运算的两个SDF场原点不同！");

            if (vSDF1.getSpacing() == vSDF2.getSpacing())
                Spacing = vSDF1.getSpacing();
            else
                Debug.LogWarning("参与并集运算的两个SDF场网格尺寸不同！");

            CCellCenteredScalarField ResultSDF = new CCellCenteredScalarField(Resolution, Origin, Spacing);

            int TotalThreadsNum = Resolution.x * Resolution.y * Resolution.z;

            m_FieldMathTool.SetBuffer(m_unionSDFKernel, "vSrcSDFData1", vSDF1.getGridData());
            m_FieldMathTool.SetBuffer(m_unionSDFKernel, "vSrcSDFData2", vSDF2.getGridData());
            m_FieldMathTool.SetBuffer(m_unionSDFKernel, "voDstSDFData", ResultSDF.getGridData());

            CGlobalMacroAndFunc.dispatchKernel(m_FieldMathTool, m_unionSDFKernel, TotalThreadsNum);

            return ResultSDF;
        }

        public static CCellCenteredScalarField intersectSDFInvoker(CCellCenteredScalarField vSDF1, CCellCenteredScalarField vSDF2)
        {
            if (!m_Initialized) init();

            Vector3Int Resolution = new Vector3Int(1, 1, 1);
            Vector3 Origin = new Vector3(0, 0, 0);
            Vector3 Spacing = new Vector3(1, 1, 1);

            if (vSDF1.getResolution() == vSDF2.getResolution())
                Resolution = vSDF1.getResolution();
            else
            {
                Debug.LogError("参与交集运算的两个SDF场分辨率不同！");
                return null;
            }

            if (vSDF1.getOrigin() == vSDF2.getOrigin())
                Origin = vSDF1.getOrigin();
            else
                Debug.LogWarning("参与交集运算的两个SDF场原点不同！");

            if (vSDF1.getSpacing() == vSDF2.getSpacing())
                Spacing = vSDF1.getSpacing();
            else
                Debug.LogWarning("参与交集运算的两个SDF场网格尺寸不同！");

            CCellCenteredScalarField ResultSDF = new CCellCenteredScalarField(Resolution, Origin, Spacing);

            int TotalThreadsNum = Resolution.x * Resolution.y * Resolution.z;

            m_FieldMathTool.SetBuffer(m_intersectSDF, "vSrcSDFData1", vSDF1.getGridData());
            m_FieldMathTool.SetBuffer(m_intersectSDF, "vSrcSDFData2", vSDF2.getGridData());
            m_FieldMathTool.SetBuffer(m_intersectSDF, "voDstSDFData", ResultSDF.getGridData());

            CGlobalMacroAndFunc.dispatchKernel(m_FieldMathTool, m_intersectSDF, TotalThreadsNum);

            return ResultSDF;
        }

        public static CCellCenteredScalarField differenceSDFInvoker(CCellCenteredScalarField vSDF1, CCellCenteredScalarField vSDF2)
        {
            if (!m_Initialized) init();

            Vector3Int Resolution = new Vector3Int(1, 1, 1);
            Vector3 Origin = new Vector3(0, 0, 0);
            Vector3 Spacing = new Vector3(1, 1, 1);

            if (vSDF1.getResolution() == vSDF2.getResolution())
                Resolution = vSDF1.getResolution();
            else
            {
                Debug.LogError("参与差集运算的两个SDF场分辨率不同！");
                return null;
            }

            if (vSDF1.getOrigin() == vSDF2.getOrigin())
                Origin = vSDF1.getOrigin();
            else
                Debug.LogWarning("参与差集运算的两个SDF场原点不同！");

            if (vSDF1.getSpacing() == vSDF2.getSpacing())
                Spacing = vSDF1.getSpacing();
            else
                Debug.LogWarning("参与差集运算的两个SDF场网格尺寸不同！");

            CCellCenteredScalarField ResultSDF = new CCellCenteredScalarField(Resolution, Origin, Spacing);

            int TotalThreadsNum = Resolution.x * Resolution.y * Resolution.z;

            m_FieldMathTool.SetBuffer(m_differenceSDF, "vSrcSDFData1", vSDF1.getGridData());
            m_FieldMathTool.SetBuffer(m_differenceSDF, "vSrcSDFData2", vSDF2.getGridData());
            m_FieldMathTool.SetBuffer(m_differenceSDF, "voDstSDFData", ResultSDF.getGridData());

            CGlobalMacroAndFunc.dispatchKernel(m_FieldMathTool, m_differenceSDF, TotalThreadsNum);

            return ResultSDF;
        }

        public static void SetFCVDataWithBoxInvoker(CFaceCenteredVectorField vioFCVField, SBoundingBox vBoundingBox, Vector3 vVectorValue)
        {
            if (!m_Initialized) init();

            Vector3Int Resolution = vioFCVField.getResolution();

            int TotalThreadsNum = Resolution.x * Resolution.y * Resolution.z;

            m_FieldMathTool.SetInts("GridResolution", Resolution.x, Resolution.y, Resolution.z);
            m_FieldMathTool.SetFloats("vBoxMin", vBoundingBox.Min.x, vBoundingBox.Min.y, vBoundingBox.Min.z);
            m_FieldMathTool.SetFloats("vBoxMax", vBoundingBox.Max.x, vBoundingBox.Max.y, vBoundingBox.Max.z);
            m_FieldMathTool.SetFloats("vVectorValue", vVectorValue.x, vVectorValue.y, vVectorValue.z);
            m_FieldMathTool.SetBuffer(m_SetFCVDataWithBoxKernel, "vioFieldDataX", vioFCVField.getGridDataX());
            m_FieldMathTool.SetBuffer(m_SetFCVDataWithBoxKernel, "vioFieldDataY", vioFCVField.getGridDataY());
            m_FieldMathTool.SetBuffer(m_SetFCVDataWithBoxKernel, "vioFieldDataZ", vioFCVField.getGridDataZ());

            CGlobalMacroAndFunc.dispatchKernel(m_FieldMathTool, m_SetFCVDataWithBoxKernel, TotalThreadsNum);
        }
    }
    #endregion
}
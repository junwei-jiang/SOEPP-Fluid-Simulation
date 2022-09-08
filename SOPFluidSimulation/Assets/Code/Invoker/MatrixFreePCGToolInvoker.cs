using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using EulerFluidEngine;

namespace EulerFluidEngine
{
    public static class CMatrixFreePCGToolInvokers
    {
        private static bool m_Initiated = false;
        private static ComputeShader m_MatrixFreePCGTool = null;
        private static int m_ComputeAlphaKernel = -1;
        private static int m_ComputeBetaKernel = -1;
        private static int m_UpdatePKernel = -1;

        private static ComputeBuffer m_TempResultA = new ComputeBuffer(CGlobalMacroAndFunc.MAX_GRIDS_NUM, sizeof(float));
        private static ComputeBuffer m_TempResultB = new ComputeBuffer(CGlobalMacroAndFunc.MAX_GRIDS_NUM, sizeof(float));
        private static ComputeBuffer m_GroupCounter = new ComputeBuffer(1, sizeof(float), ComputeBufferType.Raw);
        private static ComputeBuffer m_Zero = new ComputeBuffer(1, sizeof(float), ComputeBufferType.Append);

        public static void init()
        {
            if (m_Initiated) return;

            free();

            m_MatrixFreePCGTool = Resources.Load("Shaders/MatrixFreePCGTool") as ComputeShader;

            m_ComputeAlphaKernel = m_MatrixFreePCGTool.FindKernel("computeAlpha");
            m_ComputeBetaKernel = m_MatrixFreePCGTool.FindKernel("computeBeta");
            m_UpdatePKernel = m_MatrixFreePCGTool.FindKernel("updateP");

            m_TempResultA = new ComputeBuffer(CGlobalMacroAndFunc.MAX_GRIDS_NUM, sizeof(float));
            m_TempResultB = new ComputeBuffer(CGlobalMacroAndFunc.MAX_GRIDS_NUM, sizeof(float));
            m_GroupCounter = new ComputeBuffer(1, sizeof(float), ComputeBufferType.Raw);
            m_Zero = new ComputeBuffer(1, sizeof(float), ComputeBufferType.Append);

            m_Zero.SetCounterValue(0);

            Shader.SetGlobalBuffer("TempResultA", m_TempResultA);
            Shader.SetGlobalBuffer("TempResultB", m_TempResultB);
            Shader.SetGlobalBuffer("GroupCounter", m_GroupCounter);

            m_Initiated = true;
        }

        public static void free()
        {
            m_TempResultA.Release();
            m_TempResultB.Release();
            m_GroupCounter.Release();
            m_Zero.Release();

            m_Initiated = false;
        }

        private static void __resetGroupCounter()
        {
            ComputeBuffer.CopyCount(m_Zero, m_GroupCounter, 0);
        }

        public static void computeAlphaInvoker(ComputeBuffer vR, ComputeBuffer vP, ComputeBuffer vAP, ComputeBuffer voAlpha, ComputeBuffer voRkDotRk)
        {
            if (!m_Initiated) init();

            int TotalThreadsNum = vR.count;
            int TotalGroupsNum, ThreadsNumPerGroup;
            CGlobalMacroAndFunc.fetchPropBlockGridSize1D(TotalThreadsNum, 1024, out ThreadsNumPerGroup, out TotalGroupsNum);

            __resetGroupCounter();
            m_MatrixFreePCGTool.SetInt("vTotalGroupsNum", TotalGroupsNum);
            m_MatrixFreePCGTool.SetBuffer(m_ComputeAlphaKernel, "vR", vR);
            m_MatrixFreePCGTool.SetBuffer(m_ComputeAlphaKernel, "vP", vP);
            m_MatrixFreePCGTool.SetBuffer(m_ComputeAlphaKernel, "vAP", vAP);
            m_MatrixFreePCGTool.SetBuffer(m_ComputeAlphaKernel, "voAlpha", voAlpha);
            m_MatrixFreePCGTool.SetBuffer(m_ComputeAlphaKernel, "voRkDotRk", voRkDotRk);

            m_MatrixFreePCGTool.SetInt("TotalThreadNum", TotalThreadsNum);
            m_MatrixFreePCGTool.Dispatch(m_ComputeAlphaKernel, TotalGroupsNum, 1, 1);
        }

        public static bool computeBetaInvoker(float vThreshold, ComputeBuffer vAlpha, ComputeBuffer vP, ComputeBuffer vAP, ComputeBuffer vRkDotRk, ComputeBuffer vioX, ComputeBuffer vioR, ComputeBuffer voBeta)
        {
            if (!m_Initiated) init();

            int TotalThreadsNum = vioR.count;
            int TotalGroupsNum, ThreadsNumPerGroup;
            CGlobalMacroAndFunc.fetchPropBlockGridSize1D(TotalThreadsNum, 1024, out ThreadsNumPerGroup, out TotalGroupsNum);

            __resetGroupCounter();
            m_MatrixFreePCGTool.SetInt("vTotalGroupsNum", TotalGroupsNum);
            m_MatrixFreePCGTool.SetFloat("vThreshold", vThreshold);
            m_MatrixFreePCGTool.SetBuffer(m_ComputeBetaKernel, "vAlpha", vAlpha);
            m_MatrixFreePCGTool.SetBuffer(m_ComputeBetaKernel, "vP", vP);
            m_MatrixFreePCGTool.SetBuffer(m_ComputeBetaKernel, "vAP", vAP);
            m_MatrixFreePCGTool.SetBuffer(m_ComputeBetaKernel, "vRkDotRk", vRkDotRk);
            m_MatrixFreePCGTool.SetBuffer(m_ComputeBetaKernel, "vioX", vioX);
            m_MatrixFreePCGTool.SetBuffer(m_ComputeBetaKernel, "vioR", vioR);
            m_MatrixFreePCGTool.SetBuffer(m_ComputeBetaKernel, "voBeta", voBeta);

            m_MatrixFreePCGTool.SetInt("TotalThreadNum", TotalThreadsNum);
            m_MatrixFreePCGTool.Dispatch(m_ComputeBetaKernel, TotalGroupsNum, 1, 1);

            float[] BetaArray = new float[1];
            voBeta.GetData(BetaArray);
            return BetaArray[0] >= 0;
        }

        public static void updatePInvoker(ComputeBuffer vioP,ComputeBuffer vR,ComputeBuffer vBeta)
        {
            if (!m_Initiated) init();

            int TotalThreadsNum = vioP.count;

            m_MatrixFreePCGTool.SetBuffer(m_UpdatePKernel, "vioP", vioP);
            m_MatrixFreePCGTool.SetBuffer(m_UpdatePKernel, "vR", vR);
            m_MatrixFreePCGTool.SetBuffer(m_UpdatePKernel, "vBeta", vBeta);

            CGlobalMacroAndFunc.dispatchKernel(m_MatrixFreePCGTool, m_UpdatePKernel, TotalThreadsNum);
        }
    }
}
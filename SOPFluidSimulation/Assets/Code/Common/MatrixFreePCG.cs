using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

namespace EulerFluidEngine
{
    public delegate void MatrixVecCGProd(ComputeBuffer vVector, ComputeBuffer voResult, SFdmLinearSystem vUserMatrix);
    public class CMatrixFreePCG
    {
        #region Constructor&Resize
        public CMatrixFreePCG() { }
        ~CMatrixFreePCG() 
        {
            m_ZM1.Release();
            m_R.Release();
            m_P.Release();
            m_AP.Release();
            m_Alpha.Release();

            m_Zero.Release();
        }
        #endregion

        #region Set&Get
        public void init(int vDim, MatrixVecCGProd vMatrixVecCGProd, MatrixVecCGProd vMInvxCGProdFunc)
        {
            m_Dim = vDim;
            m_AxCGProdFunc = vMatrixVecCGProd;
            m_MInvxCGProdFunc = vMInvxCGProdFunc;

            float[] InitValue = new float[vDim];
            m_R = new ComputeBuffer(vDim, sizeof(float));
            m_P = new ComputeBuffer(vDim, sizeof(float));
            m_AP = new ComputeBuffer(vDim, sizeof(float));
            m_ZM1 = new ComputeBuffer(vDim, sizeof(float));
            m_Alpha = new ComputeBuffer(1, sizeof(float));
            m_RkDotRk = new ComputeBuffer(1, sizeof(float));
            m_Beta = new ComputeBuffer(1, sizeof(float));
            m_Zero = new ComputeBuffer(vDim, sizeof(float));

            m_R.SetData(InitValue);
            m_P.SetData(InitValue);
            m_AP.SetData(InitValue);
            m_ZM1.SetData(InitValue);
            m_Zero.SetData(InitValue);
        }
        public void setIterationNum(int vIterationNum)
        {
            m_IterationNum = vIterationNum;
        }

        public void setThreshold(float vThreshold)
        {
            m_Threshold = vThreshold;
        }

        public float setThreshold()
        {
            return m_Threshold;
        }
        #endregion

        #region CoreMethods
        public bool solvePCGInvDia(ComputeBuffer vVectorb, ComputeBuffer voX, SFdmLinearSystem vUserData)
        {
            Debug.Assert(m_AxCGProdFunc != null);
            Debug.Assert(m_MInvxCGProdFunc != null);

            CMathTool.copy(vVectorb, m_R);
            if (Mathf.Sqrt(CMathTool.norm2(m_R)) < m_Threshold)
            {
                CMathTool.copy(m_Zero, voX);
                return true;
            }

            m_AxCGProdFunc(voX, m_R, vUserData);
            CMathTool.scale(m_R, -1.0f);
            CMathTool.plusAlphaX(m_R, vVectorb, 1.0f);

            m_MInvxCGProdFunc(m_R, m_ZM1, vUserData);
            CMathTool.copy(m_ZM1, m_P);
            int ItNum = 0;
            for (int i = 0; i < m_IterationNum; i++)
            {
                float RkDotZk = CMathTool.dot(m_R, m_ZM1);
                m_AxCGProdFunc(m_P, m_AP, vUserData);
                float PkDotAPk = CMathTool.dot(m_P, m_AP);
                float Alpha = RkDotZk / PkDotAPk;

                CMathTool.plusAlphaX(voX, m_P, Alpha);
                CMathTool.plusAlphaX(m_R, m_AP, -Alpha);

                float Error = Mathf.Sqrt(CMathTool.norm2(m_R));
                if (Error < m_Threshold) break;

                m_MInvxCGProdFunc(m_R, m_ZM1, vUserData);

                float NewRkDotZk = CMathTool.dot(m_R, m_ZM1);
                float Beta = NewRkDotZk / RkDotZk;

                CMathTool.scale(m_P, Beta);
                CMathTool.plusAlphaX(m_P, m_ZM1, 1);
                ItNum++;
            }

            return ItNum <= 1;
        }
        public bool solveCG(ComputeBuffer vVectorb, ComputeBuffer voX, SFdmLinearSystem vUserData)
        {
            Debug.Assert(m_AxCGProdFunc != null);

            CMathTool.copy(vVectorb, m_R);
            if (CMathTool.norm2(m_R) < m_Threshold * m_Threshold)
            {
                CMathTool.copy(m_Zero, voX);
                return true;
            }

            m_AxCGProdFunc(voX, m_R, vUserData);
            CMathTool.scale(m_R, -1.0f);
            CMathTool.plusAlphaX(m_R, vVectorb, 1);

            CMathTool.copy(m_R, m_P);
            int ItNum = 0;
            for (int i=0; i < m_IterationNum; i++)
            {
                m_AxCGProdFunc(m_P, m_AP, vUserData);

                float RkDotRk = CMathTool.dot(m_R, m_R);
                float PkDotAPk = CMathTool.dot(m_P, m_AP);
                float Alpha = RkDotRk / PkDotAPk;

                CMathTool.plusAlphaX(voX, m_P, Alpha);
                CMathTool.plusAlphaX(m_R, m_AP, -Alpha);
                float NewRkDotNewRk = CMathTool.dot(m_R, m_R);
                if (NewRkDotNewRk < m_Threshold * m_Threshold) break;
                float Beta = NewRkDotNewRk / RkDotRk;

                CMathTool.scale(m_P, Beta);
                CMathTool.plusAlphaX(m_P, m_R, 1);
                ItNum++;
            }
            if (CMathTool.detectInvalidValue(voX))
            {
                Debug.LogError("方程组无法求解！");
                if (EditorApplication.isPlaying)
                    EditorApplication.ExecuteMenuItem("Edit/Play");
                if (Application.isPlaying)
                    Application.Quit(-1);
            }
            return ItNum <= 2;
        }

        public bool solveCGImproved(ComputeBuffer vVectorb, ComputeBuffer voX, SFdmLinearSystem vUserData)
        {
            Debug.Assert(m_AxCGProdFunc != null);

            CMathTool.copy(vVectorb, m_R);
            if (CMathTool.norm2(m_R) < m_Threshold * m_Threshold)
            {
                CMathTool.copy(m_Zero, voX);
                return true;
            }

            m_AxCGProdFunc(voX, m_R, vUserData);
            CMathTool.scale(m_R, -1.0f);
            CMathTool.plusAlphaX(m_R, vVectorb, 1);
            CMathTool.copy(m_R, m_P);
            int ItNum = 0;
            for (int i = 0; i < m_IterationNum; i++)
            {
                m_AxCGProdFunc(m_P, m_AP, vUserData);

                CMatrixFreePCGToolInvokers.computeAlphaInvoker(m_R, m_P, m_AP, m_Alpha, m_RkDotRk);
                if (!CMatrixFreePCGToolInvokers.computeBetaInvoker(m_Threshold, m_Alpha, m_P, m_AP, m_RkDotRk, voX, m_R, m_Beta))
                    break;
                CMatrixFreePCGToolInvokers.updatePInvoker(m_P, m_R, m_Beta);
                ItNum++;
            }

            if (CMathTool.detectInvalidValue(voX))
            {
                Debug.LogError("方程组无法求解！");
                if (EditorApplication.isPlaying)
                    EditorApplication.ExecuteMenuItem("Edit/Play");
                if (Application.isPlaying)
                    Application.Quit(-1);
            }
            return ItNum <= 2;
        }
        #endregion

        private MatrixVecCGProd m_AxCGProdFunc = null;
        private MatrixVecCGProd m_MInvxCGProdFunc = null;
        private int m_Dim = 0;
        private int m_IterationNum = 1000;
        private float m_Threshold = 1e-2f;

        // CG cache
        private ComputeBuffer m_ZM1;
        private ComputeBuffer m_R;
        private ComputeBuffer m_P;
        private ComputeBuffer m_AP;
        private ComputeBuffer m_Alpha;
        private ComputeBuffer m_RkDotRk;
        private ComputeBuffer m_Beta;

        private ComputeBuffer m_Zero;
    }
}
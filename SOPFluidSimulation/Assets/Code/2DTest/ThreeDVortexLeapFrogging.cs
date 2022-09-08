using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using EulerFluidEngine;

namespace EulerFluidEngine
{
    public class CVortexLeapFrogging
    {
        public CVortexLeapFrogging(Vector3Int vResolution, Vector3 vOrigin, Vector3 vSpacing)
        {
            resizeVortexLeapFrogging(vResolution, vOrigin, vSpacing);
        }

        public void resizeVortexLeapFrogging(Vector3Int vResolution, Vector3 vOrigin, Vector3 vSpacing)
        {
            m_Resolution = new Vector3Int(vResolution.x, vResolution.y, vResolution.z);
            m_Origin = vOrigin;
            m_Spacing = vSpacing;

            Vector3Int ArgumentResolution = m_Resolution + new Vector3Int(1, 1, 1);

            if (m_DensityField == null)
            {
                m_DensityField = new CCellCenteredScalarField(m_Resolution, vOrigin, vSpacing);
            }
            else
            {
                m_DensityField.resize(m_Resolution, vOrigin, vSpacing);
            }

            if (m_CurlField == null)
            {
                m_CurlField = new CCellCenteredVectorField(m_Resolution, vOrigin, vSpacing);
            }
            else
            {
                m_CurlField.resize(m_Resolution, vOrigin, vSpacing);
            }

            if (m_CurlFieldX == null)
            {
                m_CurlFieldX = new CCellCenteredScalarField(m_Resolution, vOrigin, vSpacing);
            }
            else
            {
                m_CurlFieldX.resize(m_Resolution, vOrigin, vSpacing);
            }

            if (m_CurlFieldY == null)
            {
                m_CurlFieldY = new CCellCenteredScalarField(m_Resolution, vOrigin, vSpacing);
            }
            else
            {
                m_CurlFieldY.resize(m_Resolution, vOrigin, vSpacing);
            }

            if (m_CurlFieldZ == null)
            {
                m_CurlFieldZ = new CCellCenteredScalarField(m_Resolution, vOrigin, vSpacing);
            }
            else
            {
                m_CurlFieldZ.resize(m_Resolution, vOrigin, vSpacing);
            }

            if (m_InitVelField == null)
            {
                m_InitVelField = new CFaceCenteredVectorField(m_Resolution, vOrigin, vSpacing);
            }
            else
            {
                m_InitVelField.resize(m_Resolution, vOrigin, vSpacing);
            }

            if (m_TempCCVelField == null)
            {
                m_TempCCVelField = new CCellCenteredVectorField(m_Resolution, vOrigin, vSpacing);
            }
            else
            {
                m_TempCCVelField.resize(m_Resolution, vOrigin, vSpacing);
            }

            m_MaxCurl = 0.0f;

            m_FdmLinearSystem.Resolution = m_Resolution;
            m_FdmLinearSystem.Spacing = vSpacing;

            float[] VectorInitialValueA = new float[4 * m_Resolution.x * m_Resolution.y * m_Resolution.z];
            float[] VectorInitialValueB = new float[m_Resolution.x * m_Resolution.y * m_Resolution.z];

            if (m_FdmLinearSystem.FdmMatrixA != null)
            {
                m_FdmLinearSystem.FdmMatrixA.Release();
            }
            if (m_FdmLinearSystem.FdmVectorx != null)
            {
                m_FdmLinearSystem.FdmVectorx.Release();
            }
            if (m_FdmLinearSystem.FdmVectorb != null)
            {
                m_FdmLinearSystem.FdmVectorb.Release();
            }

            m_FdmLinearSystem.FdmMatrixA = new ComputeBuffer(4 * m_Resolution.x * m_Resolution.y * m_Resolution.z, sizeof(float));
            m_FdmLinearSystem.FdmVectorx = new ComputeBuffer(m_Resolution.x * m_Resolution.y * m_Resolution.z, sizeof(float));
            m_FdmLinearSystem.FdmVectorb = new ComputeBuffer(m_Resolution.x * m_Resolution.y * m_Resolution.z, sizeof(float));
            m_FdmLinearSystem.FdmMatrixA.SetData(VectorInitialValueA);
            m_FdmLinearSystem.FdmVectorx.SetData(VectorInitialValueB);
            m_FdmLinearSystem.FdmVectorb.SetData(VectorInitialValueB);

            m_MartixFreePCGSolver = new CMatrixFreePCG();
            m_MartixFreePCGSolver.init(m_Resolution.x * m_Resolution.y * m_Resolution.z, PressureAxCGProd, PressureMinvxCGProd);
            m_MartixFreePCGSolver.setIterationNum(m_Resolution.x * m_Resolution.y * m_Resolution.z);
        }

        public float getMaxCurl()
        {
            return m_MaxCurl;
        }

        public CCellCenteredScalarField getDensityField()
        {
            return m_DensityField;
        }
        public CFaceCenteredVectorField getVelField()
        {
            return m_InitVelField;
        }

        public void initThreeDVortexLeapFroggingDensityAndVelField()
        {
            __generateCurlFieldAndDensityField();
            __buildMatrixA();
            __buildRhs(m_CurlFieldX);
            m_MartixFreePCGSolver.solveCGImproved(m_FdmLinearSystem.FdmVectorb, m_FdmLinearSystem.FdmVectorx, m_FdmLinearSystem);
            __updateCurlField(m_CurlFieldX);

            __buildMatrixA();
            __buildRhs(m_CurlFieldY);
            m_MartixFreePCGSolver.solveCGImproved(m_FdmLinearSystem.FdmVectorb, m_FdmLinearSystem.FdmVectorx, m_FdmLinearSystem);
            __updateCurlField(m_CurlFieldY);

            __buildMatrixA();
            __buildRhs(m_CurlFieldZ);
            m_MartixFreePCGSolver.solveCGImproved(m_FdmLinearSystem.FdmVectorb, m_FdmLinearSystem.FdmVectorx, m_FdmLinearSystem);
            __updateCurlField(m_CurlFieldZ);

            __generateInitVelField(m_CurlField);
        }

        private void __buildMatrixA()
        {
            Vector3 Scale = new Vector3(1 / (m_FdmLinearSystem.Spacing.x * m_FdmLinearSystem.Spacing.x), 1 / (m_FdmLinearSystem.Spacing.y * m_FdmLinearSystem.Spacing.y), 1 / (m_FdmLinearSystem.Spacing.z * m_FdmLinearSystem.Spacing.z));
            CTaylorVortexInvoker.buildTaylorVortexMatrixAInvoker(m_FdmLinearSystem.Resolution, Scale, m_FdmLinearSystem.FdmMatrixA);
        }

        private void __generateCurlFieldAndDensityField()
        {
            Vector3Int ArgumentResolution = m_Resolution + new Vector3Int(1, 1, 1);
            float[] VorticityVectorFieldDataX = new float[m_Resolution.x * m_Resolution.y * m_Resolution.z];
            float[] VorticityVectorFieldDataY = new float[m_Resolution.x * m_Resolution.y * m_Resolution.z];
            float[] VorticityVectorFieldDataZ = new float[m_Resolution.x * m_Resolution.y * m_Resolution.z];
            float[] DensityFieldData = new float[m_Resolution.x * m_Resolution.y * m_Resolution.z];
            Array.Clear(VorticityVectorFieldDataX, 0, VorticityVectorFieldDataX.Length);
            Array.Clear(VorticityVectorFieldDataY, 0, VorticityVectorFieldDataY.Length);
            Array.Clear(VorticityVectorFieldDataZ, 0, VorticityVectorFieldDataZ.Length);
            Array.Clear(DensityFieldData, 0, DensityFieldData.Length);

            for (int z = 0; z < m_Resolution.z; z++)
            {
                for (int y = 0; y < m_Resolution.y; y++)
                {
                    for (int x = 0; x < m_Resolution.x; x++)
                    {
                        Vector3 CurPos = m_Origin + new Vector3((x + 0.5f) * m_Spacing.x, (y + 0.5f) * m_Spacing.y, (z + 0.5f) * m_Spacing.z);
                        int CurLinearIndex = z * m_Resolution.x * m_Resolution.y + y * m_Resolution.x + x;
                        //int CurLinearIndex = z * ArgumentResolution.x * ArgumentResolution.y + y * ArgumentResolution.x + x;
                        float wx = 0.0f;
                        float wy = 0.0f;
                        float wz = 0.0f;

                        float s = 0.5f;
                        float ss = 0.0f;//-0.9f
                        float sigma = 0.012f;
                        float t = 500.0f;

                        float d = Mathf.Sqrt(CurPos.x * CurPos.x + CurPos.z * CurPos.z);
                        // add vortex ring
                        float rr = (d - s) * (d - s) + (CurPos.y - ss) * (CurPos.y - ss);
                        float mag = Mathf.Exp(-rr / (2.0f * sigma * sigma)) * t / d;
                        wx += mag * CurPos.z;
                        wz += -mag * CurPos.x;
                        // add vortex ring
                        rr = (d - s * 0.7f) * (d - s * 0.7f) + (CurPos.y - ss) * (CurPos.y - ss);
                        mag = Mathf.Exp(-rr / (2.0f * sigma * sigma)) * t / d;
                        wx += mag * CurPos.z;
                        wz += -mag * CurPos.x;
                        VorticityVectorFieldDataX[CurLinearIndex] = wx;
                        VorticityVectorFieldDataY[CurLinearIndex] = wy;
                        VorticityVectorFieldDataZ[CurLinearIndex] = wz;
                        DensityFieldData[CurLinearIndex] = Mathf.Sqrt(wx * wx + wy * wy + wz * wz);
                    }
                }
            }
            m_CurlField.resize(m_Resolution, m_Origin, m_Spacing, VorticityVectorFieldDataX, VorticityVectorFieldDataY, VorticityVectorFieldDataZ);
            m_CurlFieldX.resize(m_Resolution, m_Origin, m_Spacing, VorticityVectorFieldDataX);
            m_CurlFieldY.resize(m_Resolution, m_Origin, m_Spacing, VorticityVectorFieldDataY);
            //m_CurlFieldZ.resize(m_Resolution, m_Origin, m_Spacing, VorticityVectorFieldDataZ);
            //m_CurlField.scale(-1.0f);
            //m_CurlFieldX.scale(-1.0f);
            //m_CurlFieldY.scale(-1.0f);
            //m_CurlFieldZ.scale(-1.0f);
            m_DensityField.resize(m_Resolution, m_Origin, m_Spacing, DensityFieldData);
        }

        private void __buildRhs(CCellCenteredScalarField vCurlField)
        {
            CTaylorVortexInvoker.buildRhsInvoker(m_Resolution, vCurlField.getGridData(), m_FdmLinearSystem.FdmVectorb);
        }

        private void __updateCurlField(CCellCenteredScalarField vioCurlField)
        {
            CTaylorVortexInvoker.updateCurlFieldInvoker(m_Resolution, vioCurlField.getGridData(), m_FdmLinearSystem.FdmVectorx);
        }

        //TODO: 写一个新的kernel处理uvw
        private void __generateInitVelField(CCellCenteredVectorField vCurlField)
        {
            //Vector3Int ArgumentResolution = m_Resolution + new Vector3Int(1, 1, 1);

            CTaylorVortexInvoker.generateInitVelFieldXYZInvoker(m_Resolution, m_Spacing, vCurlField, m_TempCCVelField);
            m_TempCCVelField.transfer2FCVField(m_InitVelField);
        }

        public void generateCurlFieldFrom2DVelField(CFaceCenteredVectorField vVelField, CCellCenteredScalarField vo2DCurlField)
        {
            vo2DCurlField.resize(vo2DCurlField.getResolution(), vo2DCurlField.getOrigin(), vo2DCurlField.getSpacing());
            Vector3Int ArgumentResolution = vVelField.getResolution() + new Vector3Int(1, 1, 0);
            CTaylorVortexInvoker.generate2DCurlInvoker(ArgumentResolution, m_Spacing, vVelField.getGridDataX(), vVelField.getGridDataY(), vo2DCurlField.getGridData());
        }

        public void generateVortFieldFrom2DCurlField(CCellCenteredScalarField v2DCurlField, CCellCenteredScalarField vo2DVortField)
        {
            vo2DVortField.resize(vo2DVortField.getResolution(), vo2DVortField.getOrigin(), vo2DVortField.getSpacing());
            CTaylorVortexInvoker.generate2DVortInvoker(v2DCurlField, vo2DVortField);
        }

        private void PressureAxCGProd(ComputeBuffer vX, ComputeBuffer voResult, SFdmLinearSystem vFdmLinearSystem)
        {
            Vector3Int Resolution = vFdmLinearSystem.Resolution;

            CEulerSolverToolInvoker.fdmMatrixVectorMulInvoker(Resolution, vFdmLinearSystem.FdmMatrixA, vX, voResult);
        }

        private void PressureMinvxCGProd(ComputeBuffer vX, ComputeBuffer voResult, SFdmLinearSystem vFdmLinearSystem)
        {
            CMathTool.copy(vX, voResult);
        }

        private Vector3Int m_Resolution;
        private Vector3 m_Origin;
        private Vector3 m_Spacing;

        private CCellCenteredScalarField m_DensityField;
        private CCellCenteredVectorField m_CurlField;
        private CCellCenteredScalarField m_CurlFieldX;
        private CCellCenteredScalarField m_CurlFieldY;
        private CCellCenteredScalarField m_CurlFieldZ;
        private float m_MaxCurl;

        private CFaceCenteredVectorField m_InitVelField;
        private CCellCenteredVectorField m_TempCCVelField;

        private SFdmLinearSystem m_FdmLinearSystem;
        CMatrixFreePCG m_MartixFreePCGSolver = null;
    }
}


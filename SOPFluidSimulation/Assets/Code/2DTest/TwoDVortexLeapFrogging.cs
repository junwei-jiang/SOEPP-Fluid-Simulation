using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using EulerFluidEngine;

namespace EulerFluidEngine
{
    public class CTwoDVortexLeapFrogging
    {
        public CTwoDVortexLeapFrogging(Vector3Int vResolution, Vector3 vOrigin, Vector3 vSpacing)
        {
            resizeTwoDVortexLeapFrogging(vResolution, vOrigin, vSpacing);
        }

        public void resizeTwoDVortexLeapFrogging(Vector3Int vResolution, Vector3 vOrigin, Vector3 vSpacing)
        {
            m_Resolution = new Vector3Int(vResolution.x, vResolution.y, 1);
            m_Origin = vOrigin;
            m_Spacing = vSpacing;

            Vector3Int ArgumentResolution = m_Resolution + new Vector3Int(1, 1, 0);

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
                m_CurlField = new CCellCenteredScalarField(ArgumentResolution, vOrigin, vSpacing);
            }
            else
            {
                m_CurlField.resize(ArgumentResolution, vOrigin, vSpacing);
            }

            if (m_InitVelField == null)
            {
                m_InitVelField = new CFaceCenteredVectorField(m_Resolution, vOrigin, vSpacing);
            }
            else
            {
                m_InitVelField.resize(m_Resolution, vOrigin, vSpacing);
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

        public void initTwoDVortexLeapFroggingDensityAndVelField(float vDistanceA, float vDistanceB, float vRho_h, float vRho_w)
        {
            __buildMatrixA();
            __generateCurlField(vDistanceA, vDistanceB);
            __buildRhs();
            m_MartixFreePCGSolver.solveCGImproved(m_FdmLinearSystem.FdmVectorb, m_FdmLinearSystem.FdmVectorx, m_FdmLinearSystem);
            __updateCurlField();
            __generateInitVelField();
            __generateDensityField(vRho_h, vRho_w);
        }

        private void __buildMatrixA()
        {
            Vector3 Scale = new Vector3(1 / (m_FdmLinearSystem.Spacing.x * m_FdmLinearSystem.Spacing.x), 1 / (m_FdmLinearSystem.Spacing.y * m_FdmLinearSystem.Spacing.y), 1 / (m_FdmLinearSystem.Spacing.z * m_FdmLinearSystem.Spacing.z));
            CTaylorVortexInvoker.buildTaylorVortexMatrixAInvoker(m_FdmLinearSystem.Resolution, Scale, m_FdmLinearSystem.FdmMatrixA);
        }

        private void __generateCurlField(float vDistanceA, float vDistanceB)
        {
            CTaylorVortexInvoker.generateCurlFieldVortexLeapFroggingInvoker(m_CurlField.getResolution(), m_Origin, m_Spacing, m_CurlField.getGridData(), vDistanceA, vDistanceB);

            m_MaxCurl = CMathTool.getMaxValue(m_CurlField.getGridData());
        }

        private void __buildRhs()
        {
            CTaylorVortexInvoker.buildRhsInvoker(m_Resolution, m_CurlField.getGridData(), m_FdmLinearSystem.FdmVectorb);
        }

        private void __updateCurlField()
        {
            CTaylorVortexInvoker.updateCurlFieldInvoker(m_Resolution, m_CurlField.getGridData(), m_FdmLinearSystem.FdmVectorx);
        }

        private void __generateInitVelField()
        {
            Vector3Int ArgumentResolution = m_Resolution + new Vector3Int(1, 0, 0);
            CTaylorVortexInvoker.generateInitVelFieldXInvoker(ArgumentResolution, m_Spacing, m_CurlField.getGridData(), m_InitVelField.getGridDataX());

            ArgumentResolution = m_Resolution + new Vector3Int(0, 1, 0);
            CTaylorVortexInvoker.generateInitVelFieldYInvoker(ArgumentResolution, m_Spacing, m_CurlField.getGridData(), m_InitVelField.getGridDataY());
        }

        private void __generateDensityField(float vRho_h, float vRho_w)
        {
            CTaylorVortexInvoker.generateDensityFieldVortexLeapFroggingInvoker(m_DensityField, vRho_h, vRho_w);
        }

        //TODO:Œ¨∂»Œ Ã‚
        public void fillArgumentVelField(CFaceCenteredVectorField voVelField)
        {
            CTaylorVortexInvoker.fillArgumentVelFieldInvoker(m_InitVelField, voVelField);
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
        private CCellCenteredScalarField m_CurlField;
        private float m_MaxCurl;

        private CFaceCenteredVectorField m_InitVelField;

        private SFdmLinearSystem m_FdmLinearSystem;
        CMatrixFreePCG m_MartixFreePCGSolver = null;
    }
}


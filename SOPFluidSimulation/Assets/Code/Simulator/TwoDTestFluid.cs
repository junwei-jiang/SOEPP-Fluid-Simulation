using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using EulerFluidEngine;
using SDFr;

namespace EulerFluidEngine
{
    [Serializable]
    public class TwoDTestFluid : MonoBehaviour
    {
        void Start()
        {
            __computeTrueAttributes();
            m_EulerFluid = new CGridFluidSolver(m_DeltaT, m_Resolution, m_Origin, m_Spacing, m_AdvectionAlgorithm);
            m_EulerFluid.addExternalForce(m_ExternalForce);
            m_EulerFluid.getPressureSolver().getPCGSolver().setThreshold(m_CGThreshold);
            m_EulerFluid.setExtrapolatingNums(m_ExtrapolatingNums);

            m_EulerFluid.setSamplingAlgorithm(m_SamplingAlgorithm);
            m_EulerFluid.setAdvectionAccuracy(m_AdvectionAccuracy);
            m_EulerFluid.setPGTransferAlgorithm(m_PGTransferAlgorithm);

            m_FluidDomainField = new CCellCenteredScalarField(m_Resolution, m_Origin, m_Spacing);
            m_SolidDomainField = new CCellCenteredScalarField(m_Resolution, m_Origin, m_Spacing);
            m_FluidDensityField = new CCellCenteredScalarField(m_Resolution, m_Origin, m_Spacing);
            m_FluidSDFField = new CCellCenteredScalarField(m_Resolution, m_Origin, m_Spacing);
            m_FluidNormalField = new CCellCenteredVectorField(m_Resolution, m_Origin, m_Spacing);
            m_FluidVelField = new CFaceCenteredVectorField(m_Resolution, m_Origin, m_Spacing);
            m_FluidVelLengthField = new CCellCenteredScalarField(m_Resolution, m_Origin, m_Spacing);

            //Vector3[] OriginalVelocities = new Vector3[BoundaryObjects.Count];
            //for (int i = 0; i < OriginalVelocities.Length; i++)
            //{
            //    OriginalVelocities[i] = new Vector3(0, 0, 0);
            //}
            //m_EulerFluid.addDynamicBoundarys(BoundaryObjects, OriginalVelocities);

            __generateFluidAndSolidDomain(m_FluidDomainField, m_SolidDomainField);

            m_EulerFluid.setBoundarysType(false);
            m_EulerFluid.setBoundarys(m_SolidDomainField);

            if (m_AdvectionAlgorithm == EAdvectionAlgorithm.SemiLagrangian || m_AdvectionAlgorithm == EAdvectionAlgorithm.MacCormack)
            {
                m_EulerFluid.generateFluid(m_FluidDomainField, m_MaxParticlesNum, m_NumOfParticlesPerGrid, m_MixingCoefficient, m_CFLNumber);
            }
            else if (m_AdvectionAlgorithm == EAdvectionAlgorithm.PIC || m_AdvectionAlgorithm == EAdvectionAlgorithm.FLIP || m_AdvectionAlgorithm == EAdvectionAlgorithm.MixPICAndFLIP)
            {
                m_EulerFluid.generateFluid(m_FluidDomainField, m_MaxParticlesNum, m_NumOfParticlesPerGrid, m_MixingCoefficient, m_CFLNumber);
                m_ParticlesNum = m_EulerFluid.getParticlesAdvectionSolver().getEulerParticles().getNumOfParticles();
            }
            else
            {

            }

            //if (m_AdvectionAlgorithm == EAdvectionAlgorithm.SemiLagrangian)
            //{
            //    m_EulerFluid.generateFluid(m_InitialFluidDomainMin, m_InitialFluidDomainMax, m_MaxParticlesNum, m_NumOfParticlesPerGrid, m_MixingCoefficient, m_CFLNumber);
            //}
            //else if (m_AdvectionAlgorithm == EAdvectionAlgorithm.PIC || m_AdvectionAlgorithm == EAdvectionAlgorithm.FLIP || m_AdvectionAlgorithm == EAdvectionAlgorithm.MixPICandFLIP)
            //{
            //    m_EulerFluid.generateFluid(m_InitialFluidDomainMin, m_InitialFluidDomainMax, m_MaxParticlesNum, m_NumOfParticlesPerGrid, m_MixingCoefficient, m_CFLNumber);
            //    m_ParticlesNum = m_EulerFluid.getParticlesAdvectionSolver().getEulerParticles().getNumOfParticles();
            //}
            //else
            //{

            //}

            if (m_AdvectionAlgorithm == EAdvectionAlgorithm.PIC || m_AdvectionAlgorithm == EAdvectionAlgorithm.FLIP || m_AdvectionAlgorithm == EAdvectionAlgorithm.MixPICAndFLIP)
            {
                m_FluidParticlesVisualizer = new CFluidParticlesVisualizer(m_EulerFluid);
            }

            m_CCSFieldVisualizer = new CCCSFieldVisualizer(m_FluidSDFField);
            //m_CCSFieldVisualizer = new CCCSFieldVisualizer(m_FluidVelLengthField);
            m_CCVFieldVisualizer = new CCCVFieldVisualizer(m_FluidNormalField);
            m_FCVFieldVisualizer = new CFCVFieldVisualizer(m_FluidVelField);

            //__initVelField();
        }

        void Update()
        {
            m_EulerFluid.update(m_PressureAlgorithm);
            m_CurFrame++;
            //m_EulerFluid.updateWithoutPressure();

            //m_EulerFluid.updateTest3();
        }

        /*void OnRenderObject()
        {
            //m_FluidParticlesVisualizer.visualize();

            //m_TaylorVortex.generateCurlFieldFrom2DVelField(m_EulerFluid.getVelocityField(), m_2DCurlField);
            //m_TaylorVortex.generateVortFieldFrom2DCurlField(m_2DCurlField, m_2DVortField);
            m_FluidSDFField.resize(m_EulerFluid.getFluidSDFField());
            //m_FluidVelField = m_EulerFluid.getVelocityField();
            //m_FluidVelField.length(m_FluidVelLengthField);
            m_CCSFieldVisualizer.visualize();

            //CHybridSolverToolInvoker.buildFluidDensityInvoker
            //(
            //    m_EulerFluid.getFluidDomainField(),
            //    m_EulerFluid.getSolidSDFField(),
            //    m_FluidDensityField
            //);
            //CHybridSolverToolInvoker.buildFluidOutsideSDFInvoker
            //(
            //    m_FluidDensityField,
            //    m_EulerFluid.getSolidSDFField(),
            //    m_FluidSDFField,
            //    m_ExtrapolatingNums
            //);
            //m_FluidSDFField.gradient(m_FluidNormalField);
            //m_CCVFieldVisualizer.visualize();

            //TODO:引用换了一个造成的影响？
            //m_FluidVelField = m_EulerFluid.getVelocityField();
            //Debug.Log(CMathTool.getAbsMaxValue(m_FluidVelField.getGridDataX()));
            //m_FCVFieldVisualizer.visualize(m_FluidVelField);
        }*/

        void OnRenderObject()
        {
            if (m_AdvectionAlgorithm == EAdvectionAlgorithm.SemiLagrangian || m_AdvectionAlgorithm == EAdvectionAlgorithm.MacCormack)
            {
                m_FluidSDFField.resize(m_EulerFluid.getFluidPressureField());
                //m_FluidSDFField.resize(m_EulerFluid.getFluidSDFField());
                m_CCSFieldVisualizer.visualize();
            }
            else if (m_AdvectionAlgorithm == EAdvectionAlgorithm.PIC || m_AdvectionAlgorithm == EAdvectionAlgorithm.FLIP || m_AdvectionAlgorithm == EAdvectionAlgorithm.MixPICAndFLIP)
            {
                m_FluidParticlesVisualizer.visualize();
            }
            else
            {

            }
            //m_FluidSDFField.resize(m_EulerFluid.getFluidPressureField());
            //Debug.Log(CMathTool.getAbsMaxValue(m_FluidSDFField.getGridData()));
            //m_CCSFieldVisualizer.visualize();
            //m_FluidParticlesVisualizer.visualize();

            //m_FluidSDFField.resize(m_EulerFluid.getFluidPressureField());
            //Debug.Log(CMathTool.getAbsMaxValue(m_FluidSDFField.getGridData()));
            //m_CCSFieldVisualizer.visualize();

            //m_FluidParticlesVisualizer.visualize();

            //m_TaylorVortex.generateCurlFieldFrom2DVelField(m_EulerFluid.getVelocityField(), m_2DCurlField);
            //m_TaylorVortex.generateVortFieldFrom2DCurlField(m_2DCurlField, m_2DVortField);
            //m_FluidSDFField.resize(m_EulerFluid.getFluidSDFField());
            //m_FluidVelField = m_EulerFluid.getVelocityField();
            //m_FluidVelField.length(m_FluidVelLengthField);
            //m_CCSFieldVisualizer.visualize();

            //CHybridSolverToolInvoker.buildFluidDensityInvoker
            //(
            //    m_EulerFluid.getFluidDomainField(),
            //    m_EulerFluid.getSolidSDFField(),
            //    m_FluidDensityField
            //);
            //CHybridSolverToolInvoker.buildFluidOutsideSDFInvoker
            //(
            //    m_FluidDensityField,
            //    m_EulerFluid.getSolidSDFField(),
            //    m_FluidSDFField,
            //    m_ExtrapolatingNums
            //);
            //m_FluidSDFField.gradient(m_FluidNormalField);
            //m_CCVFieldVisualizer.visualize();

            //TODO:引用换了一个造成的影响？
            //m_FluidVelField = m_EulerFluid.getVelocityField();
            //Debug.Log(CMathTool.getAbsMaxValue(m_FluidVelField.getGridDataX()));
            //m_FCVFieldVisualizer.visualize(m_FluidVelField);
        }

        #region AuxiliaryFunc
        private void __computeTrueAttributes()
        {
            Vector3 SimulationRange = m_SimulationDomainMax - m_SimulationDomainMin;
            m_Spacing = new Vector3(SimulationRange.x / m_Resolution.x, SimulationRange.y / m_Resolution.y, SimulationRange.z / m_Resolution.z);
            m_Origin = m_SimulationDomainMin - m_Spacing;

            m_Resolution += new Vector3Int(2, 2, 2);
        }
        private void __initVelField()
        {
            Vector3Int ResolutionX = m_Resolution + new Vector3Int(1, 0, 0);
            Vector3Int ResolutionY = m_Resolution + new Vector3Int(0, 1, 0);
            Vector3Int ResolutionZ = m_Resolution + new Vector3Int(0, 0, 1);
            float[] VelVectorFieldDataX = new float[ResolutionX.x * ResolutionX.y * ResolutionX.z];
            float[] VelVectorFieldDataY = new float[ResolutionY.x * ResolutionY.y * ResolutionY.z];
            float[] VelVectorFieldDataZ = new float[ResolutionZ.x * ResolutionZ.y * ResolutionZ.z];

            Vector3 VectorZ = new Vector3(0, 0, -1);
            float R = 16;

            for (int i = 0; i < ResolutionX.z; i++)
            {
                for (int j = 0; j < ResolutionX.y; j++)
                {
                    for (int k = 0; k < ResolutionX.x; k++)
                    {
                        Vector3 RelPos = new Vector3(0, 0, 0) - (m_Origin + new Vector3(k * m_Spacing.x, (j + 0.5f) * m_Spacing.y, (i + 0.5f) * m_Spacing.z));
                        float Alpha = RelPos.magnitude / R;
                        Vector3 VelDir = Vector3.Cross(VectorZ, RelPos);
                        VelDir = VelDir.normalized;
                        VelVectorFieldDataX[i * ResolutionX.x * ResolutionX.y + j * ResolutionX.x + k] = -VelDir.x * Alpha;
                    }
                }
            }

            for (int i = 0; i < ResolutionY.z; i++)
            {
                for (int j = 0; j < ResolutionY.y; j++)
                {
                    for (int k = 0; k < ResolutionY.x; k++)
                    {
                        Vector3 RelPos = new Vector3(0, 0, 0) - (m_Origin + new Vector3((k + 0.5f) * m_Spacing.x, j * m_Spacing.y, (i + 0.5f) * m_Spacing.z));
                        float Alpha = RelPos.magnitude / R;
                        Vector3 VelDir = Vector3.Cross(VectorZ, RelPos);
                        VelDir = VelDir.normalized;
                        VelVectorFieldDataY[i * ResolutionY.x * ResolutionY.y + j * ResolutionY.x + k] = -VelDir.y * Alpha;
                    }
                }
            }

            for (int i = 0; i < ResolutionZ.z; i++)
            {
                for (int j = 0; j < ResolutionZ.y; j++)
                {
                    for (int k = 0; k < ResolutionZ.x; k++)
                    {
                        VelVectorFieldDataZ[i * ResolutionZ.x * ResolutionZ.y + j * ResolutionZ.x + k] = 0;
                    }
                }
            }

            CFaceCenteredVectorField FCVVelField = new CFaceCenteredVectorField(m_Resolution, m_Origin, m_Spacing, VelVectorFieldDataX, VelVectorFieldDataY, VelVectorFieldDataZ);

            m_EulerFluid.setVelocityField(FCVVelField);
        }

        private void __generateSamplePos(CCellCenteredVectorField vPosField)
        {
            float[] CCPosFieldDataX = new float[m_Resolution.x * m_Resolution.y * m_Resolution.z];
            float[] CCPosFieldDataY = new float[m_Resolution.x * m_Resolution.y * m_Resolution.z];
            float[] CCPosFieldDataZ = new float[m_Resolution.x * m_Resolution.y * m_Resolution.z];

            for (int i = 0; i < m_Resolution.z; i++)
            {
                for (int j = 0; j < m_Resolution.y; j++)
                {
                    for (int k = 0; k < m_Resolution.x; k++)
                    {
                        CCPosFieldDataX[i * m_Resolution.x * m_Resolution.y + j * m_Resolution.x + k] = (k + 0.5f) * m_Spacing.x + m_Origin.x;
                        CCPosFieldDataY[i * m_Resolution.x * m_Resolution.y + j * m_Resolution.x + k] = (j + 0.5f) * m_Spacing.y + m_Origin.y;
                        CCPosFieldDataZ[i * m_Resolution.x * m_Resolution.y + j * m_Resolution.x + k] = (i + 0.5f) * m_Spacing.z + m_Origin.z;
                    }
                }
            }

            vPosField.resize(m_Resolution, m_Origin, m_Spacing, CCPosFieldDataX, CCPosFieldDataY, CCPosFieldDataZ);
        }
        private void __generateFluidAndSolidDomain(CCellCenteredScalarField vioFluidDomainField, CCellCenteredScalarField vioSolidDomainField)
        {
            float[] FluidDomainFieldData = new float[m_Resolution.x * m_Resolution.y * m_Resolution.z];
            float[] SoildDomainFieldData = new float[m_Resolution.x * m_Resolution.y * m_Resolution.z];

            float R = 16;

            for (int i = 0; i < m_Resolution.z; i++)//z
            {
                for (int j = 0; j < m_Resolution.y; j++)//y
                {
                    for (int k = 0; k < m_Resolution.x; k++)//x
                    {
                        FluidDomainFieldData[i * m_Resolution.x * m_Resolution.y + j * m_Resolution.x + k] = CGlobalMacroAndFunc.FLUID_DOMAIN_VALUE;
                        SoildDomainFieldData[i * m_Resolution.x * m_Resolution.y + j * m_Resolution.x + k] = CGlobalMacroAndFunc.FLUID_DOMAIN_VALUE;
                        Vector3 RelPos = new Vector3(0, 10, 0) - (m_Origin + new Vector3((k + 0.5f) * m_Spacing.x, (j + 0.5f) * m_Spacing.y, (i + 0.5f) * m_Spacing.z));
                        float Alpha = RelPos.magnitude / R;
                        if (Alpha <= 0.3)
                        {
                            FluidDomainFieldData[i * m_Resolution.x * m_Resolution.y + j * m_Resolution.x + k] = -CGlobalMacroAndFunc.FLUID_DOMAIN_VALUE;
                        }
                        if (j < 0.2f * m_Resolution.y)
                        {
                            FluidDomainFieldData[i * m_Resolution.x * m_Resolution.y + j * m_Resolution.x + k] = -CGlobalMacroAndFunc.FLUID_DOMAIN_VALUE;
                        }
                        //if (k > 0 && k <= m_Resolution.x / 4 && j > 0 && j <= m_Resolution.y / 2)
                        //{
                        //    FluidDomainFieldData[i * m_Resolution.x * m_Resolution.y + j * m_Resolution.x + k] = -CGlobalMacroAndFunc.FLUID_DOMAIN_VALUE;
                        //}
                        if (i==0 || i== m_Resolution.z - 1 || j == 0 || j == m_Resolution.y - 1 || k == 0 || k == m_Resolution.x - 1)
                        {
                            SoildDomainFieldData[i * m_Resolution.x * m_Resolution.y + j * m_Resolution.x + k] = -CGlobalMacroAndFunc.FLUID_DOMAIN_VALUE;
                        }
                    }
                }
            }

            vioFluidDomainField.resize(m_Resolution, m_Origin, m_Spacing, FluidDomainFieldData);
            vioSolidDomainField.resize(m_Resolution, m_Origin, m_Spacing, SoildDomainFieldData);
        }
        #endregion

        #region Editable Attributes

        public Vector3 m_SimulationDomainMin;
        public Vector3 m_SimulationDomainMax;
        public Vector3 m_InitialFluidDomainMin;
        public Vector3 m_InitialFluidDomainMax;
        public Vector3Int m_Resolution;
        private Vector3 m_Origin;
        private Vector3 m_Spacing;
        public float m_DeltaT = 0.05f;
        public int m_CurFrame = 0;

        public ESamplingAlgorithm m_SamplingAlgorithm = ESamplingAlgorithm.LINEAR;
        public EAdvectionAccuracy m_AdvectionAccuracy = EAdvectionAccuracy.RK1;
        public EPGTransferAlgorithm m_PGTransferAlgorithm = EPGTransferAlgorithm.LINEAR;
        public EAdvectionAlgorithm m_AdvectionAlgorithm = EAdvectionAlgorithm.MixPICAndFLIP;
        public EPressureAlgorithm m_PressureAlgorithm = EPressureAlgorithm.FirstOrder;
        public float m_MixingCoefficient = 0.01f;
        public int m_MaxParticlesNum = 64 * 64 * 64 * 8;
        public int m_NumOfParticlesPerGrid = 8;
        public float m_CFLNumber = 1.0f;
        private int m_ParticlesNum;

        public Vector3 m_ExternalForce = new Vector3(0, 0, 0);

        public float m_CGThreshold = 1e-4f;

        public int m_ExtrapolatingNums = 20;

        public CGridFluidSolver m_EulerFluid;

        public List<GameObject> BoundaryObjects;

        #region Test Variables
        private CCellCenteredScalarField m_FluidDomainField;
        private CCellCenteredScalarField m_SolidDomainField;
        private CCellCenteredScalarField m_FluidDensityField;
        private CCellCenteredScalarField m_FluidSDFField;
        private CCellCenteredVectorField m_FluidNormalField;
        private CFaceCenteredVectorField m_FluidVelField;
        private CCellCenteredScalarField m_FluidVelLengthField;

        private CFluidParticlesVisualizer m_FluidParticlesVisualizer;
        private CCCSFieldVisualizer m_CCSFieldVisualizer;
        private CCCVFieldVisualizer m_CCVFieldVisualizer;
        private CFCVFieldVisualizer m_FCVFieldVisualizer;
        #endregion

        #endregion
    }

}
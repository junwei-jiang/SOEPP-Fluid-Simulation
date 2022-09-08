using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using EulerFluidEngine;
using SDFr;
using System.IO;
using System.Runtime.InteropServices;

namespace EulerFluidEngine
{
    [Serializable]
    public class VortexCollision : MonoBehaviour
    {
        void Start()
        {
            CGlobalMacroAndFunc.init();

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

            //Vector3[] OriginalVelocities = new Vector3[BoundaryObjects.Count];
            //for (int z = 0; z < OriginalVelocities.Length; z++)
            //{
            //    OriginalVelocities[z] = new Vector3(0, 0, 0);
            //}
            //m_EulerFluid.addDynamicBoundarys(BoundaryObjects, OriginalVelocities);

            __generateFluidAndSolidDomain(m_FluidDomainField, m_SolidDomainField);

            m_EulerFluid.setBoundarysType(false);
            m_EulerFluid.setBoundarys(m_SolidDomainField);

            m_EulerFluid.setVortexCollisionSmokeEmitter(0, 20, 1.0f, 50.0f, new Vector3(0, 0, 0));
            m_EulerFluid.setViscous(true);

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

            if (m_AdvectionAlgorithm == EAdvectionAlgorithm.PIC || m_AdvectionAlgorithm == EAdvectionAlgorithm.FLIP || m_AdvectionAlgorithm == EAdvectionAlgorithm.MixPICAndFLIP)
            {
                m_FluidParticlesVisualizer = new CFluidParticlesVisualizer(m_EulerFluid);
            }

            m_CCSFieldVisualizer = new CCCSFieldVisualizer(m_FluidDensityField);

            var path = m_PathInfo.Parent.FullName + @"\ExperimentData\Test\3D\txt\Frame" + Convert.ToString(m_CurFrame) + ".txt";
            __write2File(path, m_EulerFluid.getFluidDensityField1(), m_EulerFluid.getFluidDensityField2());
        }

        void Update()
        {
            //Debug.Log(CMathTool.getAbsMaxValue(m_EulerFluid.getFluidDensityField().getGridData()));
            m_EulerFluid.update(m_PressureAlgorithm);
            //Debug.Log(CMathTool.getAbsMaxValue(m_EulerFluid.getFluidDensityField().getGridData()));
            m_CurFrame++;
            var path = m_PathInfo.Parent.FullName + @"\ExperimentData\Test\3D\txt\Frame" + Convert.ToString(m_CurFrame) + ".txt";
            __write2File(path, m_EulerFluid.getFluidDensityField1(), m_EulerFluid.getFluidDensityField2());
        }

        void OnRenderObject()
        {
            //m_FluidParticlesVisualizer.visualize();

            //m_FluidSDFField.resize(m_EulerFluid.getFluidSDFField());
            m_FluidDensityField.resize(m_EulerFluid.getFluidDensityField1());
            m_FluidDensityField.plusAlphaX(m_EulerFluid.getFluidDensityField2(), 1.0f);
            m_CCSFieldVisualizer.visualize();
        }

        private void OnDestroy()
        {
            CGlobalMacroAndFunc.free();
        }

        private void __computeTrueAttributes()
        {
            Vector3 SimulationRange = m_SimulationDomainMax - m_SimulationDomainMin;
            m_Spacing = new Vector3(SimulationRange.x / m_Resolution.x, SimulationRange.y / m_Resolution.y, SimulationRange.z / m_Resolution.z);
            m_Origin = m_SimulationDomainMin - m_Spacing;
            m_Resolution += new Vector3Int(2, 2, 2);
        }

        #region AuxiliaryFunc

        private void __generateFluidAndSolidDomain(CCellCenteredScalarField vFluidDomainField, CCellCenteredScalarField vSolidDomainField)
        {
            float[] FluidDomainFieldData = new float[m_Resolution.x * m_Resolution.y * m_Resolution.z];
            float[] SolidDomainFieldData = new float[m_Resolution.x * m_Resolution.y * m_Resolution.z];

            for (int z = 0; z < m_Resolution.z; z++)
            {
                for (int y = 0; y < m_Resolution.y; y++)
                {
                    for (int x = 0; x < m_Resolution.x; x++)
                    {
                        FluidDomainFieldData[z * m_Resolution.x * m_Resolution.y + y * m_Resolution.x + x] = -CGlobalMacroAndFunc.FLUID_DOMAIN_VALUE;
                        SolidDomainFieldData[z * m_Resolution.x * m_Resolution.y + y * m_Resolution.x + x] = CGlobalMacroAndFunc.FLUID_DOMAIN_VALUE;

                        if(y == m_Resolution.y - 1)
                        {
                            //FluidDomainFieldData[z * m_Resolution.x * m_Resolution.y + y * m_Resolution.x + x] = CGlobalMacroAndFunc.FLUID_DOMAIN_VALUE;
                        }
                        //if (x < 5 || x > m_Resolution.x - 6 || y < 5 || y > m_Resolution.y - 6 || z < 5 || z > m_Resolution.z - 6)
                        //{
                        //    FluidDomainFieldData[z * m_Resolution.x * m_Resolution.y + y * m_Resolution.x + x] = CGlobalMacroAndFunc.FLUID_DOMAIN_VALUE;
                        //}
                        if (x == 0 || x == m_Resolution.x - 1 || y == 0 || z == 0 || z == m_Resolution.z - 1)
                        {
                            SolidDomainFieldData[z * m_Resolution.x * m_Resolution.y + y * m_Resolution.x + x] = -CGlobalMacroAndFunc.FLUID_DOMAIN_VALUE;
                            FluidDomainFieldData[z * m_Resolution.x * m_Resolution.y + y * m_Resolution.x + x] = CGlobalMacroAndFunc.FLUID_DOMAIN_VALUE;
                        }
                        //SolidDomainFieldData[z * m_Resolution.x * m_Resolution.y + y * m_Resolution.x + x] = CGlobalMacroAndFunc.FLUID_DOMAIN_VALUE;
                    }
                }
            }

            vFluidDomainField.resize(m_Resolution, m_Origin, m_Spacing, FluidDomainFieldData);
            vSolidDomainField.resize(m_Resolution, m_Origin, m_Spacing, SolidDomainFieldData);
        }

        public void __write2File(string path, CCellCenteredScalarField vOutputField1, CCellCenteredScalarField vOutputField2)
        {
            using (FileStream fs = new FileStream(path, FileMode.OpenOrCreate, FileAccess.Write))
            {
                using (StreamWriter sw = new StreamWriter(fs))
                {
                    Vector3Int Resolution = vOutputField1.getResolution();
                    int TotalDim = Resolution.x * Resolution.y * Resolution.z;
                    float[] FieldValue1 = new float[TotalDim];
                    float[] FieldValue2 = new float[TotalDim];
                    vOutputField1.getGridData().GetData(FieldValue1);
                    vOutputField2.getGridData().GetData(FieldValue2);

                    sw.BaseStream.Seek(0, SeekOrigin.End);
                    for (int i = 0; i < TotalDim; i++)
                    {
                        if(FieldValue1[i] < 0.3)
                        {
                            FieldValue1[i] = 0.0f;
                        }
                        if (FieldValue2[i] < 0.3)
                        {
                            FieldValue2[i] = 0.0f;
                        }
                        if(FieldValue1[i] + FieldValue2[i] > 0.0f)
                        {
                            sw.WriteLine("{0:G} {1:G} {2:G}", i, FieldValue1[i], FieldValue2[i]);
                        }
                    }
                    sw.Flush();
                }
            }
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

        private CFluidParticlesVisualizer m_FluidParticlesVisualizer;
        private CCCSFieldVisualizer m_CCSFieldVisualizer;
        #endregion

        private DirectoryInfo m_PathInfo = new DirectoryInfo(Directory.GetCurrentDirectory());
        #endregion
    }
}
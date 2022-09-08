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
    public class VortexLeapFrogging : MonoBehaviour
    {
        void Start()
        {
            CGlobalMacroAndFunc.init();

            //__computeTrueAttributes();
            m_Resolution = new Vector3Int(128, 128, 128);
            m_Origin = new Vector3(-1, -1, -1);
            m_Spacing = new Vector3(2.0f / 128.0f, 2.0f / 128.0f, 2.0f / 128.0f);
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
            m_FluidScalarField = new CCellCenteredScalarField(m_Resolution, m_Origin, m_Spacing);
            m_FluidVorticityField = new CCellCenteredVectorField(m_Resolution, m_Origin, m_Spacing);

            __generateFluidAndSolidDomain(m_FluidDomainField, m_SolidDomainField);

            //__initVorticityField();
            //m_EulerFluid.settFluidDensityField1(m_FluidDensityField);

            m_EulerFluid.setBoundarysType(false);
            m_EulerFluid.setBoundarys(m_SolidDomainField);

            //m_EulerFluid.setVortexLeapFroggingSmokeEmitter(0, 1000, 1.0f, 50.0f, new Vector3(0, 0, 0));

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

            m_ThreeDVortexLeapFrogging = new CVortexLeapFrogging(m_Resolution, m_Origin, m_Spacing);
            m_ThreeDVortexLeapFrogging.initThreeDVortexLeapFroggingDensityAndVelField();

            m_EulerFluid.setVelocityField(m_ThreeDVortexLeapFrogging.getVelField());
            m_EulerFluid.settFluidDensityField1(m_ThreeDVortexLeapFrogging.getDensityField());
            m_FluidDensityField.resize(m_ThreeDVortexLeapFrogging.getDensityField());

            //m_ColorBar = new CColorBar(m_ThreeDVortexLeapFrogging.getMaxCurl());

            if (m_AdvectionAlgorithm == EAdvectionAlgorithm.PIC || m_AdvectionAlgorithm == EAdvectionAlgorithm.FLIP || m_AdvectionAlgorithm == EAdvectionAlgorithm.MixPICAndFLIP)
            {
                m_FluidParticlesVisualizer = new CFluidParticlesVisualizer(m_EulerFluid);
            }

            m_CCSFieldVisualizer = new CCCSFieldVisualizer(m_FluidDensityField);
            //m_CCSFieldVisualizer = new CCCSFieldVisualizer(m_FluidScalarField);

            //var path = m_PathInfo.Parent.FullName + @"\ExperimentData\Test\3D\txt\Frame" + Convert.ToString(m_CurFrame) + ".txt";
            //__write2File(path, m_EulerFluid.getFluidDensityField1(), m_EulerFluid.getFluidDensityField2());
        }

        void Update()
        {
            m_EulerFluid.update(m_PressureAlgorithm);
            m_CurFrame++;
            //var path = m_PathInfo.Parent.FullName + @"\ExperimentData\Test\3D\txt\Frame" + Convert.ToString(m_CurFrame) + ".txt";
            //__write2File(path, m_EulerFluid.getFluidDensityField1(), m_EulerFluid.getFluidDensityField2());
        }

        void OnRenderObject()
        {
            //m_EulerFluid.getVelocityField().curl(m_FluidVorticityField);
            //m_FluidVorticityField.length(m_FluidScalarField);

            m_FluidDensityField.resize(m_EulerFluid.getFluidDensityField1());
            Debug.Log(CMathTool.getAbsMaxValue(m_FluidDensityField.getGridData()));
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

                        //if (x == 0 || x == m_Resolution.x - 1 || y == 0 || z == 0 || z == m_Resolution.z - 1)
                        if (x <= 10 || x >= m_Resolution.x - 10 || y <= 0 || y >= m_Resolution.y - 10 || z <= 0 || z >= m_Resolution.z - 10)
                        {
                            FluidDomainFieldData[z * m_Resolution.x * m_Resolution.y + y * m_Resolution.x + x] = CGlobalMacroAndFunc.FLUID_DOMAIN_VALUE;
                        }
                        if (x == 0 || x == m_Resolution.x - 1 || y == 0 || z == 0 || z == m_Resolution.z - 1)
                        {
                            SolidDomainFieldData[z * m_Resolution.x * m_Resolution.y + y * m_Resolution.x + x] = -CGlobalMacroAndFunc.FLUID_DOMAIN_VALUE;
                        }
                    }
                }
            }

            vFluidDomainField.resize(m_Resolution, m_Origin, m_Spacing, FluidDomainFieldData);
            vSolidDomainField.resize(m_Resolution, m_Origin, m_Spacing, SolidDomainFieldData);
            //__initVelField();
        }

        private void __initVelField()
        {
            var path = m_PathInfo.Parent.FullName + @"\ExperimentData\VelField.txt";
            string Data = System.IO.File.ReadAllText(path);
            List<string> String_list = new List<string>(Data.Split(','));
            int i = 0, L = String_list.ToArray().Length;
            float[] VelData = new float[L];
            //List<float> VelData_list = new List<float>(L);
            foreach (string s in String_list)
            {
                VelData[i] = Convert.ToSingle(s);
                i++;
            }
            float[] VelFieldXCCV = new float[256 * 256 * 256];
            float[] VelFieldYCCV = new float[256 * 256 * 256];
            float[] VelFieldZCCV = new float[256 * 256 * 256];
            Array.Clear(VelFieldXCCV, 0, VelFieldXCCV.Length);
            Array.Clear(VelFieldYCCV, 0, VelFieldYCCV.Length);
            Array.Clear(VelFieldZCCV, 0, VelFieldZCCV.Length);
            for (int z = 0; z < 256; z++)
            {
                for (int y = 0; y < 256; y++)
                {
                    for (int x = 0; x < 256; x++)
                    {
                        int LinerIndex = 256 * 256 * x + 256 * y + z;
                        VelFieldXCCV[LinerIndex] = VelData[3 * LinerIndex];
                        VelFieldYCCV[LinerIndex] = VelData[3 * LinerIndex + 1];
                        VelFieldZCCV[LinerIndex] = VelData[3 * LinerIndex + 2];
                    }
                }
            }
            //for (int I = 0, J = 0; J < L - 3; J += 3, I++)
            //{
            //    VelFieldXCCV[I] = VelData[J];
            //    VelFieldYCCV[I] = VelData[J + 1];
            //    VelFieldZCCV[I] = VelData[J + 2];
            //}
            Vector3Int Resolution = new Vector3Int(256, 256, 256);
            Vector3 Spacing = new Vector3(2.0f / 256.0f, 2.0f / 256.0f, 2.0f / 256.0f);
            CCellCenteredVectorField TempVelField = new CCellCenteredVectorField(Resolution, m_Origin, Spacing, VelFieldXCCV, VelFieldYCCV, VelFieldZCCV);
            CCellCenteredVectorField CCVelField = new CCellCenteredVectorField(m_Resolution, m_Origin, m_Spacing);
            TempVelField.sampleField(m_EulerFluid.getGridAdvectionSolver().getInputPointPosFieldCC(), CCVelField);

            CFaceCenteredVectorField FCVVelField = new CFaceCenteredVectorField(m_Resolution, m_Origin, m_Spacing);
            CCVelField.transfer2FCVField(FCVVelField);
            m_EulerFluid.setVelocityField(FCVVelField);
        }

        private void __initVorticityField()
        {
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
                        float wx = 0.0f;
                        float wy = 0.0f;
                        float wz = 0.0f;

                        float s = 0.5f;
                        float ss = -0.9f;
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
                        //VorticityVectorFieldDataX[CurLinearIndex] = wx;
                        //VorticityVectorFieldDataY[CurLinearIndex] = wy;
                        //VorticityVectorFieldDataZ[CurLinearIndex] = wz;
                        DensityFieldData[CurLinearIndex] = Mathf.Sqrt(wx * wx + wy * wy + wz * wz);
                    }
                }
            }
            m_FluidDensityField.resize(m_Resolution, m_Origin, m_Spacing, DensityFieldData);
            //m_FluidVorticityField.resize(m_Resolution, m_Origin, m_Spacing, VorticityVectorFieldDataX, VorticityVectorFieldDataY, VorticityVectorFieldDataZ);
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
                        if (FieldValue1[i] < 1e-4)
                        {
                            FieldValue1[i] = 0.0f;
                        }
                        if (FieldValue2[i] < 1e-4)
                        {
                            FieldValue2[i] = 0.0f;
                        }
                        if (FieldValue1[i] + FieldValue2[i] > 0.0f)
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
        private CVortexLeapFrogging m_ThreeDVortexLeapFrogging;
        private CCellCenteredScalarField m_FluidDomainField;
        private CCellCenteredScalarField m_SolidDomainField;
        private CCellCenteredScalarField m_FluidDensityField;
        private CCellCenteredScalarField m_FluidScalarField;
        private CCellCenteredVectorField m_FluidVorticityField;

        private CFluidParticlesVisualizer m_FluidParticlesVisualizer;
        private CCCSFieldVisualizer m_CCSFieldVisualizer;
        #endregion

        private DirectoryInfo m_PathInfo = new DirectoryInfo(Directory.GetCurrentDirectory());
        #endregion
    }
}
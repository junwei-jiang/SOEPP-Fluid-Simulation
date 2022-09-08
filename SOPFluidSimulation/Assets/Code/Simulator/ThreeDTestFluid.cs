using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using EulerFluidEngine;
using SDFr;
using System.IO;
using System.Runtime.InteropServices;
using System.Diagnostics;

namespace EulerFluidEngine
{
    [Serializable]
    public class ThreeDTestFluid : MonoBehaviour
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
            m_FluidDensityField = new CCellCenteredScalarField(m_Resolution, m_Origin, m_Spacing);
            m_FluidSDFField = new CCellCenteredScalarField(m_Resolution, m_Origin, m_Spacing);
            m_FluidNormalField = new CCellCenteredVectorField(m_Resolution, m_Origin, m_Spacing);
            m_CCVFluidVelField = new CCellCenteredVectorField(m_Resolution, m_Origin, m_Spacing);
            m_FluidVelField = new CFaceCenteredVectorField(m_Resolution, m_Origin, m_Spacing);
            m_FluidVelLengthField = new CCellCenteredScalarField(m_Resolution, m_Origin, m_Spacing);
            m_FluidVorticityField = new CCellCenteredVectorField(m_Resolution, m_Origin, m_Spacing);
            m_FluidVorticityLengthField = new CCellCenteredScalarField(m_Resolution, m_Origin, m_Spacing);

            Vector3[] OriginalVelocities = new Vector3[BoundaryObjects.Count];
            for (int z = 0; z < OriginalVelocities.Length; z++)
            {
                OriginalVelocities[z] = new Vector3(0, 0, 0);
            }
            m_EulerFluid.addDynamicBoundarys(BoundaryObjects, OriginalVelocities);

            __generateFluidDomain(m_FluidDomainField);

            if (m_AdvectionAlgorithm == EAdvectionAlgorithm.SemiLagrangian)
            {
                m_EulerFluid.generateFluid(m_InitialFluidDomainMin, m_InitialFluidDomainMax, m_MaxParticlesNum, m_NumOfParticlesPerGrid, m_MixingCoefficient, m_CFLNumber);
            }
            else if (m_AdvectionAlgorithm == EAdvectionAlgorithm.PIC || m_AdvectionAlgorithm == EAdvectionAlgorithm.FLIP || m_AdvectionAlgorithm == EAdvectionAlgorithm.MixPICAndFLIP)
            {
                m_EulerFluid.generateFluid(m_InitialFluidDomainMin, m_InitialFluidDomainMax, m_MaxParticlesNum, m_NumOfParticlesPerGrid, m_MixingCoefficient, m_CFLNumber);
                m_ParticlesNum = m_EulerFluid.getParticlesAdvectionSolver().getEulerParticles().getNumOfParticles();
            }
            else
            {

            }

            m_ParticlesVelocityLength = new ComputeBuffer(m_MaxParticlesNum, sizeof(float));
            m_ParticlesVorticityLength = new ComputeBuffer(m_MaxParticlesNum, sizeof(float));
            m_ParticlesPressure = new ComputeBuffer(m_MaxParticlesNum, sizeof(float));

            //if (m_AdvectionAlgorithm == EAdvectionAlgorithm.SemiLagrangian || m_AdvectionAlgorithm == EAdvectionAlgorithm.MacCormack)
            //{
            //    m_EulerFluid.generateFluid(m_FluidDomainField, m_MaxParticlesNum, m_NumOfParticlesPerGrid, m_MixingCoefficient, m_CFLNumber);
            //}
            //else if (m_AdvectionAlgorithm == EAdvectionAlgorithm.PIC || m_AdvectionAlgorithm == EAdvectionAlgorithm.FLIP || m_AdvectionAlgorithm == EAdvectionAlgorithm.MixPICAndFLIP)
            //{
            //    m_EulerFluid.generateFluid(m_FluidDomainField, m_MaxParticlesNum, m_NumOfParticlesPerGrid, m_MixingCoefficient, m_CFLNumber);
            //    m_ParticlesNum = m_EulerFluid.getParticlesAdvectionSolver().getEulerParticles().getNumOfParticles();
            //}
            //else
            //{

            //}

            if (m_AdvectionAlgorithm == EAdvectionAlgorithm.PIC || m_AdvectionAlgorithm == EAdvectionAlgorithm.FLIP || m_AdvectionAlgorithm == EAdvectionAlgorithm.MixPICAndFLIP)
            {
                m_FluidParticlesVisualizer = new CFluidParticlesVisualizer(m_EulerFluid);
            }

            //m_CCSFieldVisualizer = new CCCSFieldVisualizer(m_FluidVelLengthField);
            m_CCSFieldVisualizer = new CCCSFieldVisualizer(m_FluidSDFField);
            m_CCVFieldVisualizer = new CCCVFieldVisualizer(m_FluidNormalField);
            m_FCVFieldVisualizer = new CFCVFieldVisualizer(m_FluidVelField);

            //var Path = @"D:\My_Project\PythonCode\Test\Frame" + Convert.ToString(m_CurFrame) + ".txt";
            //saveParticlesSourceToFile
            //(
            //    Path,
            //    m_EulerFluid.getParticlesAdvectionSolver().getEulerParticles().getParticlesPos(),
            //    m_EulerFluid.getParticlesAdvectionSolver().getEulerParticles().getParticlesVel(),
            //    m_EulerFluid.getParticlesAdvectionSolver().getEulerParticles().getNumOfParticles()
            // );

            //var PosPath = m_PathInfo.Parent.FullName + @"\ExperimentData\Test\3D\Pos_txt\Frame" + Convert.ToString(m_CurFrame) + ".txt";
            //var VelPath = m_PathInfo.Parent.FullName + @"\ExperimentData\Test\3D\Vel_txt\Frame" + Convert.ToString(m_CurFrame) + ".txt";

            //m_EulerFluid.getVelocityField().transfer2CCVField(m_CCVFluidVelField);
            //CEulerParticlesInvokers.buildFluidDomainInvoker(m_EulerFluid.getParticlesAdvectionSolver().getEulerParticles().getParticlesPos(), m_FluidDomainField);
            //__write2File(VelPath, m_CCVFluidVelField, m_FluidDomainField);

            //m_EulerFluid.getVelocityField().length(m_FluidVelLengthField);
            //m_FluidVelLengthField.sampleField(m_EulerFluid.getParticlesAdvectionSolver().getEulerParticles().getParticlesPos(), m_ParticlesVelocityLength);
            //m_EulerFluid.getVelocityField().curl(m_FluidVorticityField);
            //m_FluidVorticityField.length(m_FluidVorticityLengthField);
            //m_FluidVorticityLengthField.sampleField(m_EulerFluid.getParticlesAdvectionSolver().getEulerParticles().getParticlesPos(), m_ParticlesVorticityLength);
            //m_EulerFluid.getPressureSolver().getPressureField().sampleField(m_EulerFluid.getParticlesAdvectionSolver().getEulerParticles().getParticlesPos(), m_ParticlesPressure);
            //__write2File(PosPath, m_EulerFluid.getParticlesAdvectionSolver().getEulerParticles().getParticlesPos(), m_ParticlesVelocityLength, m_ParticlesVorticityLength, m_ParticlesPressure);
        }

        void Update()
        {
            sw.Reset();
            sw.Start();
            m_EulerFluid.update(m_PressureAlgorithm);
            sw.Stop();
            ComputeOverhead += sw.ElapsedMilliseconds;
            m_EulerFluid.update(m_PressureAlgorithm);
            m_CurFrame++;

            if (m_CurFrame == 250)
            {
                UnityEngine.Debug.Log(ComputeOverhead / m_CurFrame);
            }
            //var Path = @"D:\My_Project\PythonCode\Test\Frame" + Convert.ToString(m_CurFrame) + ".txt";
            //saveParticlesSourceToFile
            //(
            //    Path,
            //    m_EulerFluid.getParticlesAdvectionSolver().getEulerParticles().getParticlesPos(),
            //    m_EulerFluid.getParticlesAdvectionSolver().getEulerParticles().getParticlesVel(),
            //    m_EulerFluid.getParticlesAdvectionSolver().getEulerParticles().getNumOfParticles()
            // );


            //var PosPath = m_PathInfo.Parent.FullName + @"\ExperimentData\Test\3D\Pos_txt\Frame" + Convert.ToString(m_CurFrame) + ".txt";
            //var VelPath = m_PathInfo.Parent.FullName + @"\ExperimentData\Test\3D\Vel_txt\Frame" + Convert.ToString(m_CurFrame) + ".txt";

            //m_EulerFluid.getVelocityField().transfer2CCVField(m_CCVFluidVelField);
            //CEulerParticlesInvokers.buildFluidDomainInvoker(m_EulerFluid.getParticlesAdvectionSolver().getEulerParticles().getParticlesPos(), m_FluidDomainField);
            //__write2File(VelPath, m_CCVFluidVelField, m_FluidDomainField);

            //m_EulerFluid.getVelocityField().length(m_FluidVelLengthField);
            //m_FluidVelLengthField.sampleField(m_EulerFluid.getParticlesAdvectionSolver().getEulerParticles().getParticlesPos(), m_ParticlesVelocityLength);
            //m_EulerFluid.getVelocityField().curl(m_FluidVorticityField);
            //m_FluidVorticityField.length(m_FluidVorticityLengthField);
            //m_FluidVorticityLengthField.sampleField(m_EulerFluid.getParticlesAdvectionSolver().getEulerParticles().getParticlesPos(), m_ParticlesVorticityLength);
            //m_EulerFluid.getPressureSolver().getPressureField().sampleField(m_EulerFluid.getParticlesAdvectionSolver().getEulerParticles().getParticlesPos(), m_ParticlesPressure);
            //__write2File(PosPath, m_EulerFluid.getParticlesAdvectionSolver().getEulerParticles().getParticlesPos(), m_ParticlesVelocityLength, m_ParticlesVorticityLength, m_ParticlesPressure);

        }

        void OnRenderObject()
        {
            m_FluidParticlesVisualizer.visualize();

            //m_FluidSDFField.resize(m_EulerFluid.getFluidSDFField());

            //m_FluidVelField = m_EulerFluid.getVelocityField();
            //m_FluidVelField.length(m_FluidVelLengthField);
            //m_FluidSDFField.resize(m_EulerFluid.getPressureSolver().getPressureField());
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

            //m_FluidVelField = m_EulerFluid.getVelocityField();
            //m_FCVFieldVisualizer.visualize(m_FluidVelField);
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

        private void __generateFluidDomain(CCellCenteredScalarField vFluidDomainField)
        {
            float[] FluidDomainFieldData = new float[m_Resolution.x * m_Resolution.y * m_Resolution.z];

            float R = 16;

            for (int z = 0; z < m_Resolution.z; z++)
            {
                for (int y = 0; y < m_Resolution.y; y++)
                {
                    for (int x = 0; x < m_Resolution.x; x++)
                    {
                        FluidDomainFieldData[z * m_Resolution.x * m_Resolution.y + y * m_Resolution.x + x] = CGlobalMacroAndFunc.FLUID_DOMAIN_VALUE;
                        Vector3 RelPos = new Vector3(0, 3, 0) - (m_Origin + new Vector3((x + 0.5f) * m_Spacing.x, (y + 0.5f) * m_Spacing.y, (z + 0.5f) * m_Spacing.z));
                        float Alpha = RelPos.magnitude / R;
                        if (Alpha <= 0.05)
                        {
                            FluidDomainFieldData[z * m_Resolution.x * m_Resolution.y + y * m_Resolution.x + x] = -CGlobalMacroAndFunc.FLUID_DOMAIN_VALUE;
                        }

                        if (y < 0.3f * m_Resolution.y)
                        {
                            FluidDomainFieldData[z * m_Resolution.x * m_Resolution.y + y * m_Resolution.x + x] = -CGlobalMacroAndFunc.FLUID_DOMAIN_VALUE;
                        }
                    }
                }
            }

            vFluidDomainField.resize(m_Resolution, m_Origin, m_Spacing, FluidDomainFieldData);
        }

        public struct SParticle
        {
            public Vector3 Position;
            public float Speed;
        };

        private bool saveParticlesSourceToFile(string filePath, ComputeBuffer vParticlesPos, ComputeBuffer vParticlesVel, int vParticlesNum)
        {
            FileStream fs = new FileStream(filePath, FileMode.Create);
            BinaryWriter br = new BinaryWriter(fs);

            int partileStructSize = Marshal.SizeOf(typeof(SParticle));
            byte[] byteArr = new byte[partileStructSize * vParticlesNum];
            int startIndex = 0;

            float[] particlesPos = new float[3 * vParticlesNum];
            float[] particlesVel = new float[3 * vParticlesNum];

            vParticlesPos.GetData(particlesPos);
            vParticlesVel.GetData(particlesVel);

            Vector3 TempVel = new Vector3(0, 0, 0);

            for(int i = 0; i < vParticlesNum; i++)
            {
                SParticle particle = new SParticle();
                particle.Position = new Vector3(particlesPos[3 * i], particlesPos[3 * i + 1], particlesPos[3 * i + 2]);
                //particle.Density = 1;
                //particle.AniX = new Vector4(1, 0, 0, 1);
                //particle.AniY = new Vector4(0, 1, 0, 1);
                //particle.AniZ = new Vector4(0, 0, 1, 1);
                TempVel = new Vector3(particlesVel[3 * i], particlesVel[3 * i + 1], particlesVel[3 * i + 2]);
                particle.Speed = TempVel.magnitude;
                IntPtr structIntPtr = Marshal.AllocHGlobal(partileStructSize);
                Marshal.StructureToPtr(particle, structIntPtr, true);
                Marshal.Copy(structIntPtr, byteArr, startIndex, partileStructSize);
                Marshal.FreeHGlobal(structIntPtr);
                startIndex += partileStructSize;
            }

            br.Write(byteArr, 0, partileStructSize * vParticlesNum);
            br.Close();
            fs.Close();
            return true;
        }

        public void __write2File(string path, CCellCenteredVectorField vOutputField, CCellCenteredScalarField vFluidDomainField)
        {
            using (FileStream fs = new FileStream(path, FileMode.OpenOrCreate, FileAccess.Write))
            {
                using (StreamWriter sw = new StreamWriter(fs))
                {
                    Vector3Int Resolution = vOutputField.getResolution();
                    Vector3 Origin = vOutputField.getOrigin();
                    Vector3 Spacing = vOutputField.getSpacing();

                    float[] FieldValue = new float[Resolution.x * Resolution.y * Resolution.z];
                    float[] FieldValueX = new float[Resolution.x * Resolution.y * Resolution.z];
                    float[] FieldValueY = new float[Resolution.x * Resolution.y * Resolution.z];
                    float[] FieldValueZ = new float[Resolution.x * Resolution.y * Resolution.z];
                    vFluidDomainField.getGridData().GetData(FieldValue);
                    vOutputField.getGridDataX().GetData(FieldValueX);
                    vOutputField.getGridDataY().GetData(FieldValueY);
                    vOutputField.getGridDataZ().GetData(FieldValueZ);

                    sw.BaseStream.Seek(0, SeekOrigin.End);

                    sw.WriteLine("{0:G} {1:G} {2:G}", Resolution[0], Resolution[1], Resolution[2]);

                    for (int z = 0; z < Resolution.z; z++)
                    {
                        for (int y = 0; y < Resolution.y; y++)
                        {
                            for (int x = 0; x < Resolution.x; x++)
                            {
                                int CurLinearIndex = z * Resolution.x * Resolution.y + y * Resolution.x + x;

                                if(FieldValue[CurLinearIndex] < -1e-2f)
                                {
                                    sw.WriteLine("{0:G} {1:G} {2:G}", FieldValueX[CurLinearIndex], FieldValueY[CurLinearIndex], FieldValueZ[CurLinearIndex]);
                                }
                                else
                                {
                                    sw.WriteLine("{0:G} {1:G} {2:G}", 0, 0, 0);
                                }
                            }
                        }
                    }

                    sw.Flush();
                }
            }
        }

        public void __write2File(string path, ComputeBuffer vParticlesPos, ComputeBuffer vParticlesVelLength, ComputeBuffer vParticlesVorLength, ComputeBuffer vParticlesPressure)
        {
            using (FileStream fs = new FileStream(path, FileMode.OpenOrCreate, FileAccess.Write))
            {
                using (StreamWriter sw = new StreamWriter(fs))
                {
                    float[] ParticlesPosValue = new float[m_MaxParticlesNum * 3];
                    float[] ParticlesVelLengthValue = new float[m_MaxParticlesNum];
                    float[] ParticlesVorLengthValue = new float[m_MaxParticlesNum];
                    float[] ParticlesPressureValue = new float[m_MaxParticlesNum];
                    vParticlesPos.GetData(ParticlesPosValue);
                    vParticlesVelLength.GetData(ParticlesVelLengthValue);
                    vParticlesVorLength.GetData(ParticlesVorLengthValue);
                    vParticlesPressure.GetData(ParticlesPressureValue);

                    sw.BaseStream.Seek(0, SeekOrigin.End);

                    for (int i = 0; i < m_EulerFluid.getParticlesAdvectionSolver().getEulerParticles().getNumOfParticles(); i++)
                    {
                        sw.WriteLine("{0:G} {1:G} {2:G} {3:G} {4:G} {5:G}", ParticlesPosValue[3 * i], ParticlesPosValue[3 * i + 1], ParticlesPosValue[3 * i + 2], ParticlesVelLengthValue[i], ParticlesVorLengthValue[i], ParticlesPressureValue[i]);
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
        private float ComputeOverhead = 0.0f;
        Stopwatch sw = new Stopwatch();

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
        private DirectoryInfo m_PathInfo = new DirectoryInfo(Directory.GetCurrentDirectory());

        #region Test Variables
        private CCellCenteredScalarField m_FluidDomainField;
        private CCellCenteredScalarField m_FluidDensityField;
        private CCellCenteredScalarField m_FluidSDFField;
        private CCellCenteredVectorField m_FluidNormalField;
        private CFaceCenteredVectorField m_FluidVelField;
        private CCellCenteredVectorField m_CCVFluidVelField;
        private CCellCenteredScalarField m_FluidVelLengthField;
        private CCellCenteredVectorField m_FluidVorticityField;
        private CCellCenteredScalarField m_FluidVorticityLengthField;
        ComputeBuffer m_ParticlesVelocityLength;
        ComputeBuffer m_ParticlesVorticityLength;
        ComputeBuffer m_ParticlesPressure;

        private CFluidParticlesVisualizer m_FluidParticlesVisualizer;
        private CCCSFieldVisualizer m_CCSFieldVisualizer;
        private CCCVFieldVisualizer m_CCVFieldVisualizer;
        private CFCVFieldVisualizer m_FCVFieldVisualizer;
        #endregion
        #endregion
    }
}
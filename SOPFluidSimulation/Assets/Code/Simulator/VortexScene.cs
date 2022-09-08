using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using EulerFluidEngine;
using SDFr;
using System.IO;

namespace EulerFluidEngine
{
    [Serializable]
    public class VortexScene : MonoBehaviour
    {
        void Start()
        {
            m_Resolution = new Vector3Int(256, 256, 1);
            m_Origin = new Vector3(-CGlobalMacroAndFunc.M_PI, -CGlobalMacroAndFunc.M_PI, -CGlobalMacroAndFunc.M_PI / 256.0f);
            m_Spacing = new Vector3(2 * CGlobalMacroAndFunc.M_PI / 256.0f, 2 * CGlobalMacroAndFunc.M_PI / 256.0f, 2 * CGlobalMacroAndFunc.M_PI / 256.0f);
            m_EulerFluid = new CGridFluidSolver(m_DeltaT, m_Resolution, m_Origin, m_Spacing, m_AdvectionAlgorithm);
            m_EulerFluid.addExternalForce(m_ExternalForce);
            m_EulerFluid.getPressureSolver().getPCGSolver().setThreshold(m_CGThreshold);
            m_EulerFluid.setExtrapolatingNums(m_ExtrapolatingNums);

            m_EulerFluid.setSamplingAlgorithm(m_SamplingAlgorithm);
            m_EulerFluid.setAdvectionAccuracy(m_AdvectionAccuracy);
            m_EulerFluid.setPGTransferAlgorithm(m_PGTransferAlgorithm);

            m_FluidDomainField = new CCellCenteredScalarField(m_Resolution, m_Origin, m_Spacing);
            m_SolidDomainField = new CCellCenteredScalarField(m_Resolution, m_Origin, m_Spacing);
            m_2DCurlField = new CCellCenteredScalarField(new Vector3Int(m_Resolution.x + 1, m_Resolution.y + 1, 1), m_Origin, m_Spacing);
            m_2DVortField = new CCellCenteredScalarField(new Vector3Int(m_Resolution.x, m_Resolution.y, 1), m_Origin, m_Spacing);
            m_OriginalCCVFluidVelField = new CCellCenteredVectorField(m_Resolution, m_Origin, m_Spacing);
            m_AdvectedCCVFluidVelField = new CCellCenteredVectorField(m_Resolution, m_Origin, m_Spacing);
            m_CurlFreeCCVFluidVelField = new CCellCenteredVectorField(m_Resolution, m_Origin, m_Spacing);
            m_ProjectedCCVFluidVelField = new CCellCenteredVectorField(m_Resolution, m_Origin, m_Spacing);

            __generateFluidAndSolidDomain(m_FluidDomainField, m_SolidDomainField);

            m_EulerFluid.setBoundarysType(false);
            m_EulerFluid.setBoundarys(m_SolidDomainField);

            if (m_AdvectionAlgorithm == EAdvectionAlgorithm.SemiLagrangian)
            {
                m_EulerFluid.generateFluid(m_FluidDomainField, m_MaxParticlesNum, m_NumOfParticlesPerGrid, m_MixingCoefficient, m_CFLNumber);
            }
            else if (m_AdvectionAlgorithm == EAdvectionAlgorithm.PIC || m_AdvectionAlgorithm == EAdvectionAlgorithm.FLIP || m_AdvectionAlgorithm == EAdvectionAlgorithm.MixPICAndFLIP)
            {
                m_EulerFluid.generateFluid(m_FluidDomainField, m_MaxParticlesNum, m_NumOfParticlesPerGrid, m_MixingCoefficient, m_CFLNumber);
            }
            else
            {

            }

            __initVelField();
        }

        void Update()
        {
            m_CurFrame++;

            m_EulerFluid.getVelocityField().transfer2CCVField(m_OriginalCCVFluidVelField);
            var OriginalPath = @"D:\My_Project\PythonCode\VelVisualization\Vortex\SemiLagrangian\RK1_Linear\Original\txt\Frame" + Convert.ToString(m_CurFrame) + ".txt";
            __write2File(OriginalPath, m_OriginalCCVFluidVelField);

            m_EulerFluid.updateWithoutPressure();

            m_EulerFluid.getVelocityFieldBeforePressure().transfer2CCVField(m_AdvectedCCVFluidVelField);
            var AdvectedPath = @"D:\My_Project\PythonCode\VelVisualization\Vortex\SemiLagrangian\RK1_Linear\Advected\txt\Frame" + Convert.ToString(m_CurFrame) + ".txt";
            __write2File(AdvectedPath, m_AdvectedCCVFluidVelField);

            m_EulerFluid.solvePressure();

            m_EulerFluid.getVelocityField().transfer2CCVField(m_CurlFreeCCVFluidVelField);
            m_CurlFreeCCVFluidVelField.plusAlphaX(m_AdvectedCCVFluidVelField, -1.0f);
            m_CurlFreeCCVFluidVelField.scale(-1.0f);
            var CurlFreePath = @"D:\My_Project\PythonCode\VelVisualization\Vortex\SemiLagrangian\RK1_Linear\CurlFree\txt\Frame" + Convert.ToString(m_CurFrame) + ".txt";
            __write2File(CurlFreePath, m_CurlFreeCCVFluidVelField);
        }

        #region AuxiliaryFunc
        private void __generateFluidAndSolidDomain(CCellCenteredScalarField vFluidDomainField, CCellCenteredScalarField vSolidDomainField)
        {
            float[] FluidDomainFieldData = new float[m_Resolution.x * m_Resolution.y * m_Resolution.z];
            float[] SolidDomainFieldData = new float[m_Resolution.x * m_Resolution.y * m_Resolution.z];

            float R = CGlobalMacroAndFunc.M_PI;

            for (int z = 0; z < m_Resolution.z; z++)
            {
                for (int y = 0; y < m_Resolution.y; y++)
                {
                    for (int x = 0; x < m_Resolution.x; x++)
                    {
                        Vector3 RelPos = new Vector3(0, 0, 0) - (m_Origin + new Vector3((x + 0.5f) * m_Spacing.x, (y + 0.5f) * m_Spacing.y, (z + 0.5f) * m_Spacing.z));
                        float Alpha = RelPos.magnitude / R;
                        if (Alpha <= 1.5f)
                        {
                            FluidDomainFieldData[z * m_Resolution.x * m_Resolution.y + y * m_Resolution.x + x] = -CGlobalMacroAndFunc.FLUID_DOMAIN_VALUE;
                        }
                        else
                        {
                            FluidDomainFieldData[z * m_Resolution.x * m_Resolution.y + y * m_Resolution.x + x] = CGlobalMacroAndFunc.FLUID_DOMAIN_VALUE;
                        }
                        SolidDomainFieldData[z * m_Resolution.x * m_Resolution.y + y * m_Resolution.x + x] = CGlobalMacroAndFunc.FLUID_DOMAIN_VALUE;
                    }
                }
            }

            vFluidDomainField.resize(m_Resolution, m_Origin, m_Spacing, FluidDomainFieldData);
            vSolidDomainField.resize(m_Resolution, m_Origin, m_Spacing, SolidDomainFieldData);
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
            float R = CGlobalMacroAndFunc.M_PI;

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

        public void __write2File(string path, CCellCenteredVectorField vOutputField)
        {
            using (FileStream fs = new FileStream(path, FileMode.OpenOrCreate, FileAccess.Write))
            {
                using (StreamWriter sw = new StreamWriter(fs))
                {
                    Vector3Int Resolution = vOutputField.getResolution();
                    Vector3 Origin = vOutputField.getOrigin();
                    Vector3 Spacing = vOutputField.getSpacing();

                    float[] FieldValueX = new float[Resolution.x * Resolution.y * Resolution.z];
                    float[] FieldValueY = new float[Resolution.x * Resolution.y * Resolution.z];
                    vOutputField.getGridDataX().GetData(FieldValueX);
                    vOutputField.getGridDataY().GetData(FieldValueY);

                    sw.BaseStream.Seek(0, SeekOrigin.End);

                    sw.WriteLine("{0:G} {1:G} {2:G}", Resolution[0], Resolution[1], Resolution[2]);
                    sw.WriteLine("{0:G} {1:G} {2:G}", Origin[0], Origin[1], Origin[2]);
                    sw.WriteLine("{0:G} {1:G} {2:G}", Spacing[0], Spacing[1], Spacing[2]);

                    for(int z = 0; z < Resolution.z; z++)
                    {
                        for(int y = 0; y < Resolution.y; y++)
                        {
                            for(int x = 0; x < Resolution.x; x++)
                            {
                                int CurLinearIndex = z * Resolution.x * Resolution.y + y * Resolution.x + x;

                                if(x % 10 == 0 && y % 10 == 0 && z % 10 == 0)
                                {
                                    sw.WriteLine("{0:G} {1:G} {2:G} {3:G}", Origin.x + (x + 0.5f) * Spacing.x, Origin.y + (y + 0.5f) * Spacing.y, FieldValueX[CurLinearIndex], FieldValueY[CurLinearIndex]);
                                }
                            }
                        }
                    }

                    sw.Flush();
                }
            }
        }
        #endregion

        #region Editable Attributes

        private Vector3Int m_Resolution;
        private Vector3 m_Origin;
        private Vector3 m_Spacing;

        public float m_DeltaT = 0.025f;
        public int m_CurFrame = 0;

        public ESamplingAlgorithm m_SamplingAlgorithm = ESamplingAlgorithm.LINEAR;
        public EAdvectionAccuracy m_AdvectionAccuracy = EAdvectionAccuracy.RK1;
        public EPGTransferAlgorithm m_PGTransferAlgorithm = EPGTransferAlgorithm.LINEAR;
        public EAdvectionAlgorithm m_AdvectionAlgorithm = EAdvectionAlgorithm.MixPICAndFLIP;
        public EPressureAlgorithm m_PressureAlgorithm = EPressureAlgorithm.FirstOrder;
        public float m_CFLNumber = 1.0f;
        public int m_NumOfParticlesPerGrid = 8;
        public int m_MaxParticlesNum = 256 * 256 * 1 * 64;
        public float m_MixingCoefficient = 0.01f;

        public Vector3 m_ExternalForce = new Vector3(0, 0.0f, 0);

        public float m_CGThreshold = 1e-4f;

        public int m_ExtrapolatingNums = 20;

        public CGridFluidSolver m_EulerFluid;

        private CCellCenteredScalarField m_FluidDomainField;
        private CCellCenteredScalarField m_SolidDomainField;
        private CCellCenteredScalarField m_2DCurlField;
        private CCellCenteredScalarField m_2DVortField;
        private CCellCenteredVectorField m_OriginalCCVFluidVelField;
        private CCellCenteredVectorField m_AdvectedCCVFluidVelField;
        private CCellCenteredVectorField m_CurlFreeCCVFluidVelField;
        private CCellCenteredVectorField m_ProjectedCCVFluidVelField;
        #endregion
    }

}
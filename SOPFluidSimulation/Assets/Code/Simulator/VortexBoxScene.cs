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
    public class VortexBoxScene : MonoBehaviour
    {
        void Start()
        {
            m_Resolution = new Vector3Int(512, 512, 1);
            m_Origin = new Vector3(0, 0, 0);
            m_Spacing = new Vector3(1.0f / 512.0f, 1.0f / 512.0f, 1.0f / 512.0f);
            m_EulerFluid = new CGridFluidSolver(m_DeltaT, m_Resolution, m_Origin, m_Spacing, m_AdvectionAlgorithm);
            m_EulerFluid.addExternalForce(m_ExternalForce);
            m_EulerFluid.getPressureSolver().getPCGSolver().setThreshold(m_CGThreshold);
            m_EulerFluid.setExtrapolatingNums(m_ExtrapolatingNums);

            m_EulerFluid.setSamplingAlgorithm(ESamplingAlgorithm.MONOCATMULLROM);
            m_EulerFluid.setAdvectionAccuracy(EAdvectionAccuracy.RK3);
            m_EulerFluid.setPGTransferAlgorithm(EPGTransferAlgorithm.CUBIC);

            m_FluidDomainField = new CCellCenteredScalarField(m_Resolution, m_Origin, m_Spacing);
            m_SolidDomainField = new CCellCenteredScalarField(m_Resolution, m_Origin, m_Spacing);

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

            m_VortexBox = new CVortexBox(m_Resolution, m_Origin, m_Spacing);
            m_VortexBox.initVortexBoxVelField();

            m_EulerFluid.setVelocityField(m_VortexBox.getVelField());
        }

        void Update()
        {
            m_EulerFluid.update();
            m_CurFrame++;
        }

        void OnRenderObject()
        {
            var path = @"D:\My_Project\PythonCode\ZalesakDisk\SemiLagrangian\1½×\txt\Frame" + Convert.ToString(m_CurFrame) + ".txt";
            __write2File(path, m_VortexBox.getRhoField());
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
                        if (x > 10 && x < 246 && y > 10 && y < 246)
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

        public void __write2File(string path, CCellCenteredScalarField vOutputField)
        {
            using (FileStream fs = new FileStream(path, FileMode.OpenOrCreate, FileAccess.Write))
            {
                using (StreamWriter sw = new StreamWriter(fs))
                {
                    Vector3Int Resolution = vOutputField.getResolution();
                    int TotalDim = Resolution.x * Resolution.y * Resolution.z;
                    float[] FieldValue = new float[TotalDim];
                    vOutputField.getGridData().GetData(FieldValue);

                    sw.BaseStream.Seek(0, SeekOrigin.End);
                    for (int i = 0; i < TotalDim; i++)
                    {
                        sw.WriteLine("{0:G} {1:G} {2:G}", FieldValue[i] * 100, FieldValue[i] * 100, FieldValue[i] * 100);
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

        public EAdvectionAlgorithm m_AdvectionAlgorithm = EAdvectionAlgorithm.MixPICAndFLIP;
        public float m_CFLNumber = 1.0f;
        public int m_NumOfParticlesPerGrid = 8;
        public int m_MaxParticlesNum = 256 * 256 * 1 * 64;
        public float m_MixingCoefficient = 0.01f;

        public Vector3 m_ExternalForce = new Vector3(0, 0.0f, 0);

        public float m_CGThreshold = 1e-4f;

        public int m_ExtrapolatingNums = 20;

        public CGridFluidSolver m_EulerFluid;

        private CVortexBox m_VortexBox;

        private CCellCenteredScalarField m_FluidDomainField;
        private CCellCenteredScalarField m_SolidDomainField;
        #endregion
    }

}
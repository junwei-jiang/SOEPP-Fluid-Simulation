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
    public class TwoDSmokeScene : MonoBehaviour
    {
        void Start()
        {
            m_Resolution = new Vector3Int(256, 512, 1);
            m_Origin = new Vector3(0, 0, 0);
            m_Spacing = new Vector3(2 * CGlobalMacroAndFunc.M_PI / m_Resolution.x, 4 * CGlobalMacroAndFunc.M_PI / m_Resolution.y, 2 * CGlobalMacroAndFunc.M_PI / m_Resolution.x);
            m_EulerFluid = new CGridFluidSolver(m_DeltaT, m_Resolution, m_Origin, m_Spacing, m_AdvectionAlgorithm);
            m_EulerFluid.addExternalForce(m_ExternalForce);
            m_EulerFluid.getPressureSolver().getPCGSolver().setThreshold(m_CGThreshold);
            m_EulerFluid.setExtrapolatingNums(m_ExtrapolatingNums);

            m_EulerFluid.setSamplingAlgorithm(m_SamplingAlgorithm);
            m_EulerFluid.setAdvectionAccuracy(m_AdvectionAccuracy);
            m_EulerFluid.setPGTransferAlgorithm(m_PGTransferAlgorithm);

            m_FluidDomainField = new CCellCenteredScalarField(m_Resolution, m_Origin, m_Spacing);
            m_SolidDomainField = new CCellCenteredScalarField(m_Resolution, m_Origin, m_Spacing);
            m_2DCurlField = new CCellCenteredScalarField(m_Resolution, m_Origin, m_Spacing);
            m_2DVortField = new CCellCenteredScalarField(m_Resolution, m_Origin, m_Spacing);
            m_CCVFluidVelField = new CCellCenteredVectorField(m_Resolution, m_Origin, m_Spacing);

            __generateFluidAndSolidDomain(m_FluidDomainField, m_SolidDomainField);

            m_EulerFluid.setBoundarysType(false);
            m_EulerFluid.setBoundarys(m_SolidDomainField);

            m_EulerFluid.setTwoDSmokeEmitter(0, 10, 1.0f, 50.0f, new Vector3(0, 0, 0));

            if (m_AdvectionAlgorithm == EAdvectionAlgorithm.SemiLagrangian || m_AdvectionAlgorithm == EAdvectionAlgorithm.MacCormack)
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

            m_TaylorVortex = new CTaylorVortex(m_Resolution, m_Origin, m_Spacing);

            m_ColorBar = new CColorBar(m_TaylorVortex.getMaxCurl());

            //m_TaylorVortex.generateCurlFieldFrom2DVelField(m_EulerFluid.getVelocityField(), m_2DCurlField);
            //m_TaylorVortex.generateVortFieldFrom2DCurlField(m_2DCurlField, m_2DVortField);

            var path = m_PathInfo.Parent.FullName + @"\ExperimentData\Test\txt\Frame" + Convert.ToString(m_CurFrame) + ".txt";
            //m_ColorBar.Write2File(__generateFilePath(), m_2DVortField);
            //m_ColorBar.Write2File(path, m_2DVortField);

            //m_EulerFluid.getVelocityField().transfer2CCVField(m_CCVFluidVelField);
            //var Path = Directory.GetCurrentDirectory() + @"\PythonCode\VelVisualization\TaylorVortex\Frame" + Convert.ToString(m_CurFrame) + ".txt";
            //__write2File(Path, m_CCVFluidVelField);

            __write2File(path, m_EulerFluid.getFluidDensityField1());
        }

        void Update()
        {
            m_EulerFluid.update(m_PressureAlgorithm);
            m_CurFrame++;

            //m_TaylorVortex.generateCurlFieldFrom2DVelField(m_EulerFluid.getVelocityField(), m_2DCurlField);
            //m_TaylorVortex.generateVortFieldFrom2DCurlField(m_2DCurlField, m_2DVortField);

            var path = m_PathInfo.Parent.FullName + @"\ExperimentData\Test\txt\Frame" + Convert.ToString(m_CurFrame) + ".txt";
            //m_ColorBar.Write2File(__generateFilePath(), m_2DVortField);
            //m_ColorBar.Write2File(path, m_2DVortField);
            __write2File(path, m_EulerFluid.getFluidDensityField1());
        }

        void OnRenderObject()
        {

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
                        if (x > 10 && x < m_Resolution.x - 10 && y > 10 && y < m_Resolution.y - 10)
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

        private void __write2File(string path, CCellCenteredScalarField vOutputField)
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

                    for (int z = 0; z < Resolution.z; z++)
                    {
                        for (int y = 0; y < Resolution.y; y++)
                        {
                            for (int x = 0; x < Resolution.x; x++)
                            {
                                int CurLinearIndex = z * Resolution.x * Resolution.y + y * Resolution.x + x;

                                if (x % 5 == 0 && y % 5 == 0 && z % 5 == 0)
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

        private string __generateFilePath()
        {
            DirectoryInfo PathInfo = new DirectoryInfo(Directory.GetCurrentDirectory());
            if (m_PressureAlgorithm == EPressureAlgorithm.FirstOrder)
            {
                if (m_AdvectionAlgorithm == EAdvectionAlgorithm.SemiLagrangian)
                {
                    return PathInfo.Parent.FullName + @"\ExperimentData\TaylorVortex\1FirstOrderPressure\SemiLagrangian\RK1_Linear\txt\Frame" + Convert.ToString(m_CurFrame) + ".txt";
                }
                else if (m_AdvectionAlgorithm == EAdvectionAlgorithm.MacCormack)
                {
                    return PathInfo.Parent.FullName + @"\ExperimentData\TaylorVortex\1FirstOrderPressure\MacCormack\RK1_Linear\txt\Frame" + Convert.ToString(m_CurFrame) + ".txt";

                }
                else if (m_AdvectionAlgorithm == EAdvectionAlgorithm.MixPICAndFLIP)
                {
                    return PathInfo.Parent.FullName + @"\ExperimentData\TaylorVortex\1FirstOrderPressure\MixPICAndFLIP\RK1_Linear\txt\Frame" + Convert.ToString(m_CurFrame) + ".txt";
                }
                else
                {
                    Debug.LogError("File Path Error!");
                    return " ";
                }
            }
            else if (m_PressureAlgorithm == EPressureAlgorithm.SecondOrder)
            {
                if (m_AdvectionAlgorithm == EAdvectionAlgorithm.SemiLagrangian)
                {
                    return PathInfo.Parent.FullName + @"\ExperimentData\TaylorVortex\2SecondOrderPressure\SemiLagrangian\RK1_Linear\txt\Frame" + Convert.ToString(m_CurFrame) + ".txt";
                }
                else if (m_AdvectionAlgorithm == EAdvectionAlgorithm.MacCormack)
                {
                    return PathInfo.Parent.FullName + @"\ExperimentData\TaylorVortex\2SecondOrderPressure\MacCormack\RK1_Linear\txt\Frame" + Convert.ToString(m_CurFrame) + ".txt";

                }
                else if (m_AdvectionAlgorithm == EAdvectionAlgorithm.MixPICAndFLIP)
                {
                    return PathInfo.Parent.FullName + @"\ExperimentData\TaylorVortex\2SecondOrderPressure\MixPICAndFLIP\RK1_Linear\txt\Frame" + Convert.ToString(m_CurFrame) + ".txt";
                }
                else
                {
                    Debug.LogError("File Path Error!");
                    return " ";
                }
            }
            else if (m_PressureAlgorithm == EPressureAlgorithm.Reflection)
            {
                if (m_DeltaT == 0.05f)
                {
                    if (m_AdvectionAlgorithm == EAdvectionAlgorithm.SemiLagrangian)
                    {
                        return Directory.GetCurrentDirectory() + @"\PythonCode\TaylorVortex\3Reflection\NormalTimeStep\SemiLagrangian\RK1_Linear\txt\Frame" + Convert.ToString(m_CurFrame) + ".txt";
                    }
                    else if (m_AdvectionAlgorithm == EAdvectionAlgorithm.MacCormack)
                    {
                        return Directory.GetCurrentDirectory() + @"\PythonCode\TaylorVortex\3Reflection\NormalTimeStep\MacCormack\RK1_Linear\txt\Frame" + Convert.ToString(m_CurFrame) + ".txt";

                    }
                    else if (m_AdvectionAlgorithm == EAdvectionAlgorithm.MixPICAndFLIP)
                    {
                        return Directory.GetCurrentDirectory() + @"\PythonCode\TaylorVortex\3Reflection\NormalTimeStep\MixPICAndFLIP\RK1_Linear\txt\Frame" + Convert.ToString(m_CurFrame) + ".txt";
                    }
                    else
                    {
                        Debug.LogError("File Path Error!");
                        return " ";
                    }
                }
                else if (m_DeltaT == 0.025f)
                {
                    if (m_AdvectionAlgorithm == EAdvectionAlgorithm.SemiLagrangian)
                    {
                        return PathInfo.Parent.FullName + @"\ExperimentData\TaylorVortex\3Reflection\SuperTimeStep\SemiLagrangian\RK1_Linear\txt\Frame" + Convert.ToString(m_CurFrame) + ".txt";
                    }
                    else if (m_AdvectionAlgorithm == EAdvectionAlgorithm.MacCormack)
                    {
                        return PathInfo.Parent.FullName + @"\ExperimentData\TaylorVortex\3Reflection\SuperTimeStep\MacCormack\RK1_Linear\txt\Frame" + Convert.ToString(m_CurFrame) + ".txt";

                    }
                    else if (m_AdvectionAlgorithm == EAdvectionAlgorithm.MixPICAndFLIP)
                    {
                        return PathInfo.Parent.FullName + @"\ExperimentData\TaylorVortex\3Reflection\SuperTimeStep\MixPICAndFLIP\RK1_Linear\txt\Frame" + Convert.ToString(m_CurFrame) + ".txt";
                    }
                    else
                    {
                        Debug.LogError("File Path Error!");
                        return " ";
                    }
                }
                else
                {
                    Debug.LogError("File Path Error!");
                    return " ";
                }
            }
            else
            {
                Debug.LogError("File Path Error!");
                return " ";
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

        private CTaylorVortex m_TaylorVortex;
        private CColorBar m_ColorBar;

        private CCellCenteredScalarField m_FluidDomainField;
        private CCellCenteredScalarField m_SolidDomainField;
        private CCellCenteredScalarField m_2DCurlField;
        private CCellCenteredScalarField m_2DVortField;

        private CCellCenteredVectorField m_CCVFluidVelField;

        private DirectoryInfo m_PathInfo = new DirectoryInfo(Directory.GetCurrentDirectory());
        #endregion
    }

}
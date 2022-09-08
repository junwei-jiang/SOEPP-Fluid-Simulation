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
    public class TaylorGreenVortex : MonoBehaviour
    {
        void Start()
        {
            m_Resolution = new Vector3Int(1280, 1280, 1);
            m_Spacing = new Vector3(10.0f * CGlobalMacroAndFunc.M_PI / m_Resolution.x, 10.0f * CGlobalMacroAndFunc.M_PI / m_Resolution.y, 10.0f * CGlobalMacroAndFunc.M_PI / m_Resolution.x);
            m_Origin = new Vector3(-4.0f * CGlobalMacroAndFunc.M_PI, -4.0f * CGlobalMacroAndFunc.M_PI, - 0.5f * m_Spacing.z);

            m_XYZMin = new Vector3(0, 0, -4.0f * CGlobalMacroAndFunc.M_PI);
            m_XYZMax = new Vector3(2.0f * CGlobalMacroAndFunc.M_PI, 2.0f * CGlobalMacroAndFunc.M_PI, 10.0f * CGlobalMacroAndFunc.M_PI);

            m_EulerFluid = new CGridFluidSolver(m_DeltaT, m_Resolution, m_Origin, m_Spacing, m_AdvectionAlgorithm);
            m_EulerFluid.addExternalForce(m_ExternalForce);
            m_EulerFluid.getPressureSolver().getPCGSolver().setThreshold(m_CGThreshold);
            m_EulerFluid.setExtrapolatingNums(m_ExtrapolatingNums);

            m_EulerFluid.setSamplingAlgorithm(m_SamplingAlgorithm);
            m_EulerFluid.setAdvectionAccuracy(m_AdvectionAccuracy);
            m_EulerFluid.setPGTransferAlgorithm(m_PGTransferAlgorithm);

            m_FluidDomainField = new CCellCenteredScalarField(m_Resolution, m_Origin, m_Spacing);
            m_SolidDomainField = new CCellCenteredScalarField(m_Resolution, m_Origin, m_Spacing);
            m_InitVelField = new CFaceCenteredVectorField(m_Resolution, m_Origin, m_Spacing);
            m_InitVelCCVField = new CCellCenteredVectorField(m_Resolution, m_Origin, m_Spacing);
            m_CCVFluidVelField = new CCellCenteredVectorField(m_Resolution, m_Origin, m_Spacing);
            m_FluidVelErrorField = new CCellCenteredScalarField(m_Resolution, m_Origin, m_Spacing);
            m_InitPressureField = new CCellCenteredScalarField(m_Resolution, m_Origin, m_Spacing);
            m_2DCurlField = new CCellCenteredScalarField(m_Resolution, m_Origin, m_Spacing);
            m_2DVortField = new CCellCenteredScalarField(m_Resolution, m_Origin, m_Spacing);

            m_RMSError = new float[100000];
            m_KineticEnergy = new float[100000];
            Array.Clear(m_RMSError, 0, m_RMSError.Length);
            Array.Clear(m_KineticEnergy, 0, m_KineticEnergy.Length);

            __generateFluidAndSolidDomain(m_FluidDomainField, m_SolidDomainField);

            m_EulerFluid.setBoundarysType(false);
            m_EulerFluid.setBoundarys(m_SolidDomainField);

            if (m_AdvectionAlgorithm == EAdvectionAlgorithm.SemiLagrangian || m_AdvectionAlgorithm == EAdvectionAlgorithm.MacCormack || m_AdvectionAlgorithm == EAdvectionAlgorithm.Iteration)
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

            __generateInitVelField();
            __generateInitPressureField();
            m_EulerFluid.setVelocityField(m_InitVelField);
            m_InitVelField.transfer2CCVField(m_InitVelCCVField);

            m_TaylorVortex = new CTaylorVortex(m_Resolution, m_Origin, m_Spacing);

            m_ColorBar = new CColorBar(m_TaylorVortex.getMaxCurl());

            //m_TaylorVortex.generateCurlFieldFrom2DVelField(m_EulerFluid.getVelocityField(), m_2DCurlField);
            //m_TaylorVortex.generateVortFieldFrom2DCurlField(m_2DCurlField, m_2DVortField);
            //var path = m_PathInfo.Parent.FullName + @"\ExperimentData\Test\txt\Frame" + Convert.ToString(m_CurFrame) + ".txt";
            //m_ColorBar.Write2File(path, m_2DVortField);

            __generateRMSEFieldAndComputeRMSE();
            m_KineticEnergy[m_CurFrame] = m_EulerFluid.getPressureSolver().calculateKineticEnergy(m_EulerFluid.getVelocityField(), m_XYZMin, m_XYZMax);

            var visualizationPath = m_PathInfo.Parent.FullName + @"\ExperimentData\Test\txt\Frame" + Convert.ToString(m_CurFrame) + ".txt";
            __write2File(visualizationPath, m_FluidVelErrorField);
            var RMSEPath = m_PathInfo.Parent.FullName + @"\ExperimentData\Test\" + "RMSE.txt";
            var KineticEnergyPath = m_PathInfo.Parent.FullName + @"\ExperimentData\Test\" + "KineticEnergy.txt";
            __write2File(RMSEPath, m_RMSError);
            __write2File(KineticEnergyPath, m_KineticEnergy);
        }

        void Update()
        {
            m_EulerFluid.update(m_PressureAlgorithm);
            m_CurFrame++;

            //m_TaylorVortex.generateCurlFieldFrom2DVelField(m_EulerFluid.getVelocityField(), m_2DCurlField);
            //m_TaylorVortex.generateVortFieldFrom2DCurlField(m_2DCurlField, m_2DVortField);
            //var path = m_PathInfo.Parent.FullName + @"\ExperimentData\Test\txt\Frame" + Convert.ToString(m_CurFrame) + ".txt";
            //m_ColorBar.Write2File(path, m_2DVortField);

            __generateRMSEFieldAndComputeRMSE();
            m_KineticEnergy[m_CurFrame] = m_EulerFluid.getPressureSolver().calculateKineticEnergy(m_EulerFluid.getVelocityField(), m_XYZMin, m_XYZMax);

            var visualizationPath = m_PathInfo.Parent.FullName + @"\ExperimentData\Test\txt\Frame" + Convert.ToString(m_CurFrame) + ".txt";
            __write2File(visualizationPath, m_FluidVelErrorField);
            var RMSEPath = m_PathInfo.Parent.FullName + @"\ExperimentData\Test\" + "RMSE.txt";
            var KineticEnergyPath = m_PathInfo.Parent.FullName + @"\ExperimentData\Test\" + "KineticEnergy.txt";
            __write2File(RMSEPath, m_RMSError);
            __write2File(KineticEnergyPath, m_KineticEnergy);
            //Debug.Log(__generateRMSEFieldAndComputeRMSE());
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
                        Vector3 CurPos = m_Origin + new Vector3((x + 0.5f) * m_Spacing.x, (y + 0.5f) * m_Spacing.y, (z + 0.5f) * m_Spacing.z);
                        if (CurPos.x >= -2.0f * CGlobalMacroAndFunc.M_PI && CurPos.x <= 4.0f * CGlobalMacroAndFunc.M_PI && CurPos.y >= -2.0f * CGlobalMacroAndFunc.M_PI && CurPos.y <= 4.0f * CGlobalMacroAndFunc.M_PI)
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

        private void __generateInitVelField()
        {
            float[] TempArrayX = new float[(m_Resolution.x + 1) * m_Resolution.y * m_Resolution.z];
            float[] TempArrayY = new float[m_Resolution.x * (m_Resolution.y + 1) * m_Resolution.z];
            float[] TempArrayZ = new float[m_Resolution.x * m_Resolution.y * (m_Resolution.z + 1)];

            Array.Clear(TempArrayX, 0, TempArrayX.Length);
            Array.Clear(TempArrayY, 0, TempArrayY.Length);
            Array.Clear(TempArrayZ, 0, TempArrayZ.Length);

            Vector3Int ResX = m_Resolution + new Vector3Int(1, 0, 0);
            Vector3Int ResY = m_Resolution + new Vector3Int(0, 1, 0);
            Vector3Int ResZ = m_Resolution + new Vector3Int(0, 0, 1);

            for (int z = 0; z < ResX.z; z++)
            {
                for (int y = 0; y < ResX.y; y++)
                {
                    for (int x = 0; x < ResX.x; x++)
                    {
                        Vector3 CurPos = m_Origin + new Vector3(x * m_Spacing.x, (y + 0.5f) * m_Spacing.y, (z + 0.5f) * m_Spacing.z);
                        if (CurPos.x >= -2.0f * CGlobalMacroAndFunc.M_PI && CurPos.x <= 4.0f * CGlobalMacroAndFunc.M_PI && CurPos.y >= -2.0f * CGlobalMacroAndFunc.M_PI && CurPos.y <= 4.0f * CGlobalMacroAndFunc.M_PI)
                        {
                            int CurLinearIndex = z * ResX.x * ResX.y + y * ResX.x + x;
                            TempArrayX[CurLinearIndex] = Mathf.Sin(CurPos.x) * Mathf.Cos(CurPos.y);
                        }
                    }
                }
            }

            for (int z = 0; z < ResY.z; z++)
            {
                for (int y = 0; y < ResY.y; y++)
                {
                    for (int x = 0; x < ResY.x; x++)
                    {
                        Vector3 CurPos = m_Origin + new Vector3((x + 0.5f) * m_Spacing.x, y * m_Spacing.y, (z + 0.5f) * m_Spacing.z);
                        if (CurPos.x >= -2.0f * CGlobalMacroAndFunc.M_PI && CurPos.x <= 4.0f * CGlobalMacroAndFunc.M_PI && CurPos.y >= -2.0f * CGlobalMacroAndFunc.M_PI && CurPos.y <= 4.0f * CGlobalMacroAndFunc.M_PI)
                        {
                            int CurLinearIndex = z * ResY.x * ResY.y + y * ResY.x + x;
                            TempArrayY[CurLinearIndex] = -Mathf.Cos(CurPos.x) * Mathf.Sin(CurPos.y);
                        }
                    }
                }
            }

            m_InitVelField.resize(m_Resolution, m_Origin, m_Spacing, TempArrayX, TempArrayY, TempArrayZ);
        }

        private void __generateInitPressureField()
        {
            float[] TempArray = new float[m_Resolution.x * m_Resolution.y * m_Resolution.z];

            Array.Clear(TempArray, 0, TempArray.Length);

            for (int z = 0; z < m_Resolution.z; z++)
            {
                for (int y = 0; y < m_Resolution.y; y++)
                {
                    for (int x = 0; x < m_Resolution.x; x++)
                    {
                        Vector3 CurPos = m_Origin + new Vector3((x + 0.5f) * m_Spacing.x, (y + 0.5f) * m_Spacing.y, (z + 0.5f) * m_Spacing.z);
                        if (CurPos.x >= -2.0f * CGlobalMacroAndFunc.M_PI && CurPos.x <= 4.0f * CGlobalMacroAndFunc.M_PI && CurPos.y >= -2.0f * CGlobalMacroAndFunc.M_PI && CurPos.y <= 4.0f * CGlobalMacroAndFunc.M_PI)
                        {
                            int CurLinearIndex = z * m_Resolution.x * m_Resolution.y + y * m_Resolution.x + x;
                            TempArray[CurLinearIndex] = -0.25f * (Mathf.Sin(2.0f * CurPos.x) + Mathf.Cos(2.0f * CurPos.y));
                        }
                    }
                }
            }

            m_InitPressureField.resize(m_Resolution, m_Origin, m_Spacing, TempArray);
        }

        private void __generateRMSEFieldAndComputeRMSE()
        {
            m_EulerFluid.getVelocityField().transfer2CCVField(m_CCVFluidVelField);
            float[] FluidVelFieldDataX = new float[m_Resolution.x * m_Resolution.y * m_Resolution.z];
            float[] InitFluidVelFieldDataX = new float[m_Resolution.x * m_Resolution.y * m_Resolution.z];
            float[] FluidVelFieldDataY = new float[m_Resolution.x * m_Resolution.y * m_Resolution.z];
            float[] InitFluidVelFieldDataY = new float[m_Resolution.x * m_Resolution.y * m_Resolution.z];
            float[] FluidVelFieldDataZ = new float[m_Resolution.x * m_Resolution.y * m_Resolution.z];
            float[] InitFluidVelFieldDataZ = new float[m_Resolution.x * m_Resolution.y * m_Resolution.z];
            float[] FluidVelErrorData = new float[m_Resolution.x * m_Resolution.y * m_Resolution.z];
            //float[] FluidPressureFieldData = new float[m_Resolution.x * m_Resolution.y * m_Resolution.z];
            //float[] InitFluidPressureFieldData = new float[m_Resolution.x * m_Resolution.y * m_Resolution.z];
            m_CCVFluidVelField.getGridDataX().GetData(FluidVelFieldDataX);
            m_CCVFluidVelField.getGridDataY().GetData(FluidVelFieldDataY);
            m_CCVFluidVelField.getGridDataZ().GetData(FluidVelFieldDataZ);
            m_InitVelCCVField.getGridDataX().GetData(InitFluidVelFieldDataX);
            m_InitVelCCVField.getGridDataY().GetData(InitFluidVelFieldDataY);
            m_InitVelCCVField.getGridDataZ().GetData(InitFluidVelFieldDataZ);
            //m_EulerFluid.getFluidPressureField().getGridData().GetData(FluidPressureFieldData);
            //m_InitPressureField.getGridData().GetData(InitFluidPressureFieldData);

            Array.Clear(FluidVelErrorData, 0, FluidVelErrorData.Length);

            float VelTotalError = 0.0f;
            //float PressureTotalError = 0.0f;
            int TotalGrid = 0;

            for (int z = 0; z < m_Resolution.z; z++)
            {
                for (int y = 0; y < m_Resolution.y; y++)
                {
                    for (int x = 0; x < m_Resolution.x; x++)
                    {
                        int CurLinearIndex = z * m_Resolution.x * m_Resolution.y + y * m_Resolution.x + x;
                        Vector3 CurPos = m_Origin + new Vector3((x + 0.5f) * m_Spacing.x, (y + 0.5f) * m_Spacing.y, (z + 0.5f) * m_Spacing.z);
                        if (CurPos.x >= 0.0f && CurPos.x <= 2.0f * CGlobalMacroAndFunc.M_PI && CurPos.y >= 0.0f && CurPos.y <= 2.0f * CGlobalMacroAndFunc.M_PI)
                        {
                            float ErrorX = InitFluidVelFieldDataX[CurLinearIndex] - FluidVelFieldDataX[CurLinearIndex];
                            float ErrorY = InitFluidVelFieldDataY[CurLinearIndex] - FluidVelFieldDataY[CurLinearIndex];
                            //float PressureError = InitFluidPressureFieldData[CurLinearIndex] - FluidPressureFieldData[CurLinearIndex];

                            float ErrorLengthSquare = ErrorX * ErrorX + ErrorY * ErrorY;
                            //float PressureErrorSquare = PressureError * PressureError;
                            FluidVelErrorData[CurLinearIndex] = Mathf.Sqrt(ErrorLengthSquare);

                            VelTotalError += ErrorLengthSquare;
                            //PressureTotalError += PressureErrorSquare;
                            TotalGrid++;
                        }
                    }
                }
            }

            m_FluidVelErrorField.resize(m_Resolution, m_Origin, m_Spacing, FluidVelErrorData);
            m_RMSError[m_CurFrame] = Mathf.Sqrt(VelTotalError / (float)TotalGrid);
            //m_RMSError[2 * m_CurFrame + 1] = Mathf.Sqrt(PressureTotalError / (float)TotalGrid);
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
                    for (int z = 0; z < m_Resolution.z; z++)
                    {
                        for (int y = 0; y < m_Resolution.y; y++)
                        {
                            for (int x = 0; x < m_Resolution.x; x++)
                            {
                                int CurLinearIndex = z * m_Resolution.x * m_Resolution.y + y * m_Resolution.x + x;
                                Vector3 CurPos = m_Origin + new Vector3((x + 0.5f) * m_Spacing.x, (y + 0.5f) * m_Spacing.y, (z + 0.5f) * m_Spacing.z);
                                if (CurPos.x >= 0.0f && CurPos.x <= 2.0f * CGlobalMacroAndFunc.M_PI && CurPos.y >= 0.0f && CurPos.y <= 2.0f * CGlobalMacroAndFunc.M_PI)
                                {
                                    sw.WriteLine("{0:G} {1:G} {2:G}", CurPos.x, CurPos.y, FieldValue[CurLinearIndex]);
                                }
                            }
                        }
                    }
                    sw.Flush();
                }
            }
        }

        private void __write2File(string path, float[] vScalarValue)
        {
            using (FileStream fs = new FileStream(path, FileMode.OpenOrCreate, FileAccess.Write))
            {
                using (StreamWriter sw = new StreamWriter(fs))
                {
                    sw.BaseStream.Seek(0, SeekOrigin.End);
                    sw.WriteLine("{0:G}", vScalarValue[m_CurFrame]);
                    //sw.WriteLine("{0:G} {1:G}", vScalarValue[2 * m_CurFrame], vScalarValue[2 * m_CurFrame + 1]);
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

        private Vector3 m_XYZMin;
        private Vector3 m_XYZMax;

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

        private CFaceCenteredVectorField m_InitVelField;
        private CCellCenteredVectorField m_InitVelCCVField;
        private CCellCenteredVectorField m_CCVFluidVelField;
        private CCellCenteredScalarField m_FluidVelErrorField;
        private CCellCenteredScalarField m_InitPressureField;
        private CCellCenteredScalarField m_2DCurlField;
        private CCellCenteredScalarField m_2DVortField;
        private float[] m_RMSError;
        private float[] m_KineticEnergy;

        private CTaylorVortex m_TaylorVortex;
        private CColorBar m_ColorBar;

        private DirectoryInfo m_PathInfo = new DirectoryInfo(Directory.GetCurrentDirectory());
        #endregion
    }

}

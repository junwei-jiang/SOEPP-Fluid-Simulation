using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using EulerFluidEngine;

namespace EulerFluidEngine
{
    public class CEulerParticles
    {
        #region Constructor&Resize
        public CEulerParticles()
        {
            resize(CGlobalMacroAndFunc.MAX_PARTICLE_NUM);
        }
        public CEulerParticles(int vMaxParitclesNum)
        {
            resize(vMaxParitclesNum);
        }

        public CEulerParticles(int vMaxParticlesNum, int vNumOfParticles, float[] vParticlesPos = null, float[] vParticlesVel = null)
        {
            resize(vMaxParticlesNum, vNumOfParticles, vParticlesPos, vParticlesVel);
        }

        ~CEulerParticles() 
        {
            m_ParticlesNum.Release();

            m_ParticlesPos.Release();
            m_TempParticlesPos.Release();
            m_ParticlesVel.Release();

            m_ParticlesScalarValue.Release();
            m_ParticlesVectorValue.Release();

            m_ParticlesMidPos.Release();
            m_VelFieldCurPosVel.Release();
            m_VelFieldMidPosVel.Release();

            m_NewParticlesPos.Release();
            m_NewParticlesVel.Release();
        }

        public void free()
        {
            m_ParticlesNum.Release();

            m_ParticlesPos.Release();
            m_TempParticlesPos.Release();
            m_ParticlesVel.Release();

            m_ParticlesScalarValue.Release();
            m_ParticlesVectorValue.Release();

            m_ParticlesMidPos.Release();
            m_VelFieldCurPosVel.Release();
            m_VelFieldMidPosVel.Release();

            m_NewParticlesPos.Release();
            m_NewParticlesVel.Release();
        }

        public void resize(int vMaxParticlesNum = CGlobalMacroAndFunc.MAX_PARTICLE_NUM, int vNumOfParticles = 0, float[] vParticlesPos = null, float[] vParticlesVel = null)
        {
            m_MaxParticlesNum = vMaxParticlesNum;
            m_NumOfParticles = vNumOfParticles;

            m_ParticlesNumArray = new int[1];

            Shader.SetGlobalInt("MaxParticlesNum", vMaxParticlesNum);

            if (m_ParticlesNum == null)
            {
                m_ParticlesNum = new ComputeBuffer(1, sizeof(int));
            }
            m_ParticlesNum.SetData(new int[1] { vNumOfParticles });
            Shader.SetGlobalBuffer("ParticlesNum", m_ParticlesNum);

            if (m_ParticlesPos != null)
            {
                m_ParticlesPos.Release();
            }
            m_ParticlesPos = new ComputeBuffer(vMaxParticlesNum * 3, sizeof(float));
            if (vParticlesPos != null)
            {
                m_ParticlesPos.SetData(vParticlesPos, 0, 0, vNumOfParticles * 3);
            }
            if (m_TempParticlesPos != null)
            {
                m_TempParticlesPos.Release();
            }
            m_TempParticlesPos = new ComputeBuffer(vMaxParticlesNum * 3, sizeof(float));
            if (vParticlesPos != null)
            {
                m_TempParticlesPos.SetData(vParticlesPos, 0, 0, vNumOfParticles * 3);
            }

            if (m_ParticlesVel != null)
            {
                m_ParticlesVel.Release();
            }
            m_ParticlesVel = new ComputeBuffer(vMaxParticlesNum * 3, sizeof(float));
            if (vParticlesVel != null)
            {
                m_ParticlesVel.SetData(vParticlesVel, 0, 0, vNumOfParticles * 3);
            }

            if (m_ParticlesScalarValue != null)
            {
                m_ParticlesScalarValue.Release();
            }
            m_ParticlesScalarValue = new ComputeBuffer(vMaxParticlesNum, sizeof(float));

            if (m_ParticlesVectorValue != null)
            {
                m_ParticlesVectorValue.Release();
            }
            m_ParticlesVectorValue = new ComputeBuffer(vMaxParticlesNum * 3, sizeof(float));

            if (m_ParticlesMidPos != null)
            {
                m_ParticlesMidPos.Release();
            }
            m_ParticlesMidPos = new ComputeBuffer(vMaxParticlesNum * 3, sizeof(float));

            if (m_ParticlesThreeFourthsPos != null)
            {
                m_ParticlesThreeFourthsPos.Release();
            }
            m_ParticlesThreeFourthsPos = new ComputeBuffer(vMaxParticlesNum * 3, sizeof(float));

            if (m_VelFieldCurPosVel != null)
            {
                m_VelFieldCurPosVel.Release();
            }
            m_VelFieldCurPosVel = new ComputeBuffer(vMaxParticlesNum * 3, sizeof(float));

            if (m_VelFieldMidPosVel != null)
            {
                m_VelFieldMidPosVel.Release();
            }
            m_VelFieldMidPosVel = new ComputeBuffer(vMaxParticlesNum * 3, sizeof(float));

            if (m_VelFieldThreeFourthsPosVel != null)
            {
                m_VelFieldThreeFourthsPosVel.Release();
            }
            m_VelFieldThreeFourthsPosVel = new ComputeBuffer(vMaxParticlesNum * 3, sizeof(float));

            if (m_NewParticlesPos != null)
            {
                m_NewParticlesPos.Release();
            }
            m_NewParticlesPos = new ComputeBuffer(vMaxParticlesNum * 3, sizeof(float));

            if (m_NewParticlesVel != null)
            {
                m_NewParticlesVel.Release();
            }
            m_NewParticlesVel = new ComputeBuffer(vMaxParticlesNum * 3, sizeof(float));
        }
        #endregion

        #region Get&Set
        private void __updateNumOfParticles()
        {
            m_ParticlesNum.GetData(m_ParticlesNumArray);
            m_NumOfParticles = m_ParticlesNumArray[0];
        }

        public int getNumOfParticles()
        {
            __updateNumOfParticles();
            return m_NumOfParticles;
        }

        public int getMaxParticlesNum()
        {
            return m_MaxParticlesNum;
        }

        public ComputeBuffer getParticlesPos()
        {
            return m_ParticlesPos;
        }

        public ComputeBuffer getParticlesTempPos()
        {
            return m_TempParticlesPos;
        }

        public ComputeBuffer getParticlesVel()
        {
            return m_ParticlesVel;
        }
        #endregion

        #region CoreMethods
        public void addParticles(CCellCenteredScalarField vAdditionalFluidSDF, CCellCenteredScalarField vSolidSDF, int vNumOfPerGrid)
        {
            CEulerParticlesInvokers.generateParticlesInSDFInvoker(vAdditionalFluidSDF, vSolidSDF, m_ParticlesPos, vNumOfPerGrid);
            __updateNumOfParticles();
        }

        public void addParticles(CCellCenteredScalarField vAdditionalFluidSDF, CCellCenteredScalarField vPreviousFluidSDF, CCellCenteredScalarField vSolidSDF, int vNumOfPerGrid)
        {
            CEulerParticlesInvokers.generateParticlesInSDFInvoker(vAdditionalFluidSDF, vSolidSDF, m_ParticlesPos, vNumOfPerGrid, vPreviousFluidSDF);
            __updateNumOfParticles();
        }

        public void addParticles(SBoundingBox vAdditionalFluidBox, CCellCenteredScalarField vSolidSDF, int vNumOfPerGrid)
        {
            CEulerParticlesInvokers.generateParticlesInBoundingBoxInvoker(vAdditionalFluidBox, vSolidSDF, m_ParticlesPos, vNumOfPerGrid);
            __updateNumOfParticles();
        }

        public void deleteOutsideParticles(Vector3Int vResolution, Vector3 vOrigin, Vector3 vSpacing)
        {
            CEulerParticlesInvokers.deleteOutsideParticlesInvoker
            (
                vResolution,
                vOrigin,
                vSpacing,
                m_ParticlesPos,
                m_ParticlesVel,
                m_NewParticlesPos,
                m_NewParticlesVel
            );
            __updateNumOfParticles();

            ComputeBuffer Temp;

            Temp = m_ParticlesPos;
            m_ParticlesPos = m_NewParticlesPos;
            m_NewParticlesPos = Temp;

            Temp = m_ParticlesVel;
            m_ParticlesVel = m_NewParticlesVel;
            m_NewParticlesVel = Temp;
        }

        public void transferParticlesScalarValue2Field(CCellCenteredScalarField voScalarField, CCellCenteredScalarField voWeightField, EPGTransferAlgorithm vPGTransferAlgorithm = EPGTransferAlgorithm.LINEAR)
        {
            CEulerParticlesInvokers.tranferParticles2CCSFieldInvoker(m_ParticlesPos, m_ParticlesScalarValue, voScalarField, voWeightField, vPGTransferAlgorithm);
        }

        public void transferParticlesVectorValue2Field(CCellCenteredVectorField voVectorField, CCellCenteredVectorField voWeightField, EPGTransferAlgorithm vPGTransferAlgorithm = EPGTransferAlgorithm.LINEAR)
        {
            CEulerParticlesInvokers.tranferParticles2CCVFieldInvoker(m_ParticlesPos, m_ParticlesVectorValue, voVectorField, voWeightField, vPGTransferAlgorithm);
        }

        public void transferParticlesVel2Field(CFaceCenteredVectorField voVelField, CFaceCenteredVectorField voWeightField, EPGTransferAlgorithm vPGTransferAlgorithm = EPGTransferAlgorithm.LINEAR)
        {
            CEulerParticlesInvokers.tranferParticles2FCVFieldInvoker(m_ParticlesPos, m_ParticlesVel, voVelField, voWeightField, vPGTransferAlgorithm);
        }

        public void transferScalarField2Particles(CCellCenteredScalarField vScalarField, EPGTransferAlgorithm vPGTransferAlgorithm = EPGTransferAlgorithm.LINEAR)
        {
            CEulerParticlesInvokers.tranferCCSField2ParticlesInvoker(m_ParticlesPos, m_ParticlesScalarValue, vScalarField, vPGTransferAlgorithm);
        }

        public void transferVectorField2Particles(CCellCenteredVectorField vVectorField, EPGTransferAlgorithm vPGTransferAlgorithm = EPGTransferAlgorithm.LINEAR)
        {
            CEulerParticlesInvokers.tranferCCVField2ParticlesInvoker(m_ParticlesPos, m_ParticlesVectorValue, vVectorField, vPGTransferAlgorithm);
        }

        public void transferVelField2Particles(CFaceCenteredVectorField vVelField, EPGTransferAlgorithm vPGTransferAlgorithm = EPGTransferAlgorithm.LINEAR)
        {
            CEulerParticlesInvokers.tranferFCVField2ParticlesInvoker(m_ParticlesPos, m_ParticlesVel, vVelField, vPGTransferAlgorithm);
        }

        public void advectParticlesInVelField
        (
            CFaceCenteredVectorField vVelField, 
            float vDeltaT, 
            float vCFLNumber, 
            EAdvectionAccuracy vAdvectionAccuracy = EAdvectionAccuracy.RK2,
            ESamplingAlgorithm vSamplingAlg = ESamplingAlgorithm.LINEAR
        )
        {
            float SubStepTime = 0.0f;
            bool IsFinishedAdvection = false;
            float MinGridSpacing = Mathf.Min(Mathf.Min(vVelField.getSpacing().x, vVelField.getSpacing().y), vVelField.getSpacing().z);

            while (!IsFinishedAdvection)
            {
                float DeltaSubStepTime = 0.0f;

                vVelField.sampleField(m_ParticlesPos, m_VelFieldCurPosVel, vSamplingAlg);

                float AbsMaxVelComponent = CMathTool.getAbsMaxValue(m_VelFieldCurPosVel);
                if (AbsMaxVelComponent != 0.0f)
                {
                    DeltaSubStepTime = vCFLNumber * MinGridSpacing / AbsMaxVelComponent;
                }
                else
                {
                    DeltaSubStepTime = vDeltaT;
                    Debug.Log("AbsMaxVelComponent == 0.0f !");
                }

                if (SubStepTime + DeltaSubStepTime >= vDeltaT)
                {
                    DeltaSubStepTime = vDeltaT - SubStepTime;
                    IsFinishedAdvection = true;
                }
                else if (SubStepTime + 2 * DeltaSubStepTime >= vDeltaT)
                {
                    DeltaSubStepTime = 0.5f * (vDeltaT - SubStepTime);
                }
                else
                {

                }

                //TODO
                DeltaSubStepTime = vDeltaT;
                IsFinishedAdvection = true;

                if (vAdvectionAccuracy == EAdvectionAccuracy.RK1)
                {
                    CMathTool.plusAlphaX(m_ParticlesPos, m_VelFieldCurPosVel, DeltaSubStepTime);
                }
                else if(vAdvectionAccuracy == EAdvectionAccuracy.RK2)
                {
                    float HalfDeltaSubStepTime = 0.5f * DeltaSubStepTime;

                    CMathTool.copy(m_ParticlesPos, m_ParticlesMidPos);

                    CMathTool.plusAlphaX(m_ParticlesMidPos, m_VelFieldCurPosVel, HalfDeltaSubStepTime);
                    vVelField.sampleField(m_ParticlesMidPos, m_VelFieldMidPosVel, vSamplingAlg);

                    CMathTool.plusAlphaX(m_ParticlesPos, m_VelFieldMidPosVel, DeltaSubStepTime);
                }
                else if(vAdvectionAccuracy == EAdvectionAccuracy.RK3)
                {
                    float HalfDeltaSubStepTime = 0.5f * DeltaSubStepTime;
                    float ThreeFourthsDeltaSubStepTime = 0.75f * DeltaSubStepTime;
                    float TwoNinthsDeltaSubStepTime = 2.0f / 9.0f * DeltaSubStepTime;
                    float ThreeNinthsDeltaSubStepTime = 3.0f / 9.0f * DeltaSubStepTime;
                    float FourNinthsDeltaSubStepTime = 4.0f / 9.0f * DeltaSubStepTime;

                    CMathTool.copy(m_ParticlesPos, m_ParticlesMidPos);
                    CMathTool.copy(m_ParticlesPos, m_ParticlesThreeFourthsPos);

                    CMathTool.plusAlphaX(m_ParticlesMidPos, m_VelFieldCurPosVel, HalfDeltaSubStepTime);
                    vVelField.sampleField(m_ParticlesMidPos, m_VelFieldMidPosVel, vSamplingAlg);

                    CMathTool.plusAlphaX(m_ParticlesThreeFourthsPos, m_VelFieldMidPosVel, ThreeFourthsDeltaSubStepTime);
                    vVelField.sampleField(m_ParticlesThreeFourthsPos, m_VelFieldThreeFourthsPosVel, vSamplingAlg);

                    CMathTool.plusAlphaX(m_ParticlesPos, m_VelFieldCurPosVel, TwoNinthsDeltaSubStepTime);
                    CMathTool.plusAlphaX(m_ParticlesPos, m_VelFieldMidPosVel, ThreeNinthsDeltaSubStepTime);
                    CMathTool.plusAlphaX(m_ParticlesPos, m_VelFieldThreeFourthsPosVel, FourNinthsDeltaSubStepTime);
                }
                else
                {

                }

                SubStepTime += DeltaSubStepTime;
            }
        }

        public void advectParticlesInVelFieldTemp
        (
            CFaceCenteredVectorField vVelField,
            float vDeltaT,
            float vCFLNumber,
            EAdvectionAccuracy vAdvectionAccuracy = EAdvectionAccuracy.RK2,
            ESamplingAlgorithm vSamplingAlg = ESamplingAlgorithm.LINEAR
        )
        {
            float SubStepTime = 0.0f;
            bool IsFinishedAdvection = false;
            float MinGridSpacing = Mathf.Min(Mathf.Min(vVelField.getSpacing().x, vVelField.getSpacing().y), vVelField.getSpacing().z);

            while (!IsFinishedAdvection)
            {
                float DeltaSubStepTime = 0.0f;

                vVelField.sampleField(m_TempParticlesPos, m_VelFieldCurPosVel, vSamplingAlg);

                float AbsMaxVelComponent = CMathTool.getAbsMaxValue(m_VelFieldCurPosVel);
                if (AbsMaxVelComponent != 0.0f)
                {
                    DeltaSubStepTime = vCFLNumber * MinGridSpacing / AbsMaxVelComponent;
                }
                else
                {
                    DeltaSubStepTime = vDeltaT;
                    Debug.Log("AbsMaxVelComponent == 0.0f !");
                }

                if (SubStepTime + DeltaSubStepTime >= vDeltaT)
                {
                    DeltaSubStepTime = vDeltaT - SubStepTime;
                    IsFinishedAdvection = true;
                }
                else if (SubStepTime + 2 * DeltaSubStepTime >= vDeltaT)
                {
                    DeltaSubStepTime = 0.5f * (vDeltaT - SubStepTime);
                }
                else
                {

                }

                //TODO
                DeltaSubStepTime = vDeltaT;
                IsFinishedAdvection = true;

                if (vAdvectionAccuracy == EAdvectionAccuracy.RK1)
                {
                    CMathTool.plusAlphaX(m_TempParticlesPos, m_VelFieldCurPosVel, DeltaSubStepTime);
                }
                else if (vAdvectionAccuracy == EAdvectionAccuracy.RK2)
                {
                    float HalfDeltaSubStepTime = 0.5f * DeltaSubStepTime;

                    CMathTool.copy(m_TempParticlesPos, m_ParticlesMidPos);

                    CMathTool.plusAlphaX(m_ParticlesMidPos, m_VelFieldCurPosVel, HalfDeltaSubStepTime);
                    vVelField.sampleField(m_ParticlesMidPos, m_VelFieldMidPosVel, vSamplingAlg);

                    CMathTool.plusAlphaX(m_TempParticlesPos, m_VelFieldMidPosVel, DeltaSubStepTime);
                }
                else if (vAdvectionAccuracy == EAdvectionAccuracy.RK3)
                {
                    float HalfDeltaSubStepTime = 0.5f * DeltaSubStepTime;
                    float ThreeFourthsDeltaSubStepTime = 0.75f * DeltaSubStepTime;
                    float TwoNinthsDeltaSubStepTime = 2.0f / 9.0f * DeltaSubStepTime;
                    float ThreeNinthsDeltaSubStepTime = 3.0f / 9.0f * DeltaSubStepTime;
                    float FourNinthsDeltaSubStepTime = 4.0f / 9.0f * DeltaSubStepTime;

                    CMathTool.copy(m_TempParticlesPos, m_ParticlesMidPos);
                    CMathTool.copy(m_TempParticlesPos, m_ParticlesThreeFourthsPos);

                    CMathTool.plusAlphaX(m_ParticlesMidPos, m_VelFieldCurPosVel, HalfDeltaSubStepTime);
                    vVelField.sampleField(m_ParticlesMidPos, m_VelFieldMidPosVel, vSamplingAlg);

                    CMathTool.plusAlphaX(m_ParticlesThreeFourthsPos, m_VelFieldMidPosVel, ThreeFourthsDeltaSubStepTime);
                    vVelField.sampleField(m_ParticlesThreeFourthsPos, m_VelFieldThreeFourthsPosVel, vSamplingAlg);

                    CMathTool.plusAlphaX(m_TempParticlesPos, m_VelFieldCurPosVel, TwoNinthsDeltaSubStepTime);
                    CMathTool.plusAlphaX(m_TempParticlesPos, m_VelFieldMidPosVel, ThreeNinthsDeltaSubStepTime);
                    CMathTool.plusAlphaX(m_TempParticlesPos, m_VelFieldThreeFourthsPosVel, FourNinthsDeltaSubStepTime);
                }
                else
                {

                }

                SubStepTime += DeltaSubStepTime;
            }
        }

        //理论上两个版本不完全相同，但是都没有子时间步的情况下是相同的，
        //区别在于改进前的版本使用全局子时间步，而改进后的版本每个粒子计算自己的时间步
        //但这不是优化的地方，优化的点在于将CPU端多次核函数调用变成了一次核函数调用
        //且改进后的版本只支持线性插值，因为高阶插值过于复杂
        public void advectParticlesInVelFieldImproved(CFaceCenteredVectorField vVelField, float vDeltaT, float vCFLNumber, EAdvectionAccuracy vAdvectionAccuracy = EAdvectionAccuracy.RK2)
        {
            CEulerParticlesInvokers.advectParticlesInvoker(m_ParticlesPos, vVelField, vDeltaT, vCFLNumber, vAdvectionAccuracy);
        }

        public void statisticalFluidDensity(CCellCenteredScalarField voFluidDensityField)
        {
            CEulerParticlesInvokers.statisticalFluidDensityInvoker(m_ParticlesPos, voFluidDensityField);
        }
        #endregion

        private int m_NumOfParticles;
        private int m_MaxParticlesNum;

        private int[] m_ParticlesNumArray;

        private ComputeBuffer m_ParticlesNum;

        private ComputeBuffer m_ParticlesPos;
        private ComputeBuffer m_TempParticlesPos;
        private ComputeBuffer m_ParticlesVel;

        private ComputeBuffer m_ParticlesScalarValue;
        private ComputeBuffer m_ParticlesVectorValue;

        private ComputeBuffer m_ParticlesMidPos;
        private ComputeBuffer m_ParticlesThreeFourthsPos;
        private ComputeBuffer m_VelFieldCurPosVel;
        private ComputeBuffer m_VelFieldMidPosVel;
        private ComputeBuffer m_VelFieldThreeFourthsPosVel;

        private ComputeBuffer m_NewParticlesPos;
        private ComputeBuffer m_NewParticlesVel;
    }
}
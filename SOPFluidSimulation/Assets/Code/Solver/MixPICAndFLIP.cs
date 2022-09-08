using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using EulerFluidEngine;

namespace EulerFluidEngine
{
    public class CMixPICAndFLIP
    {
        #region Constructor&Resize
        public CMixPICAndFLIP() { }

        public CMixPICAndFLIP(
            CCellCenteredScalarField vFluidSDF,
            CCellCenteredScalarField vSolidSDF,
            int vMaxParticlesNum,
            int vNumOfPerGrid,
            Vector3Int vResolution,
            Vector3 vOrigin,
            Vector3 vSpacing,
            float vCFLNumber,
            float vMixingCoefficient
        )
        {
            resizeMixPICAndFLIP(vFluidSDF, vSolidSDF, vMaxParticlesNum, vNumOfPerGrid, vResolution, vOrigin, vSpacing, vCFLNumber, vMixingCoefficient);
        }
        ~CMixPICAndFLIP() 
        {
            m_EulerParticles.free();

            m_CCSWeightField.free();
            m_CCVWeightField.free();
            m_FCVWeightField.free();
            m_DeltaVelField.free();

            m_ParticlesDeltaVel.Release();
            m_TempParticlesPICVel.Release();
        }

        public void free()
        {
            m_EulerParticles.free();

            m_CCSWeightField.free();
            m_CCVWeightField.free();
            m_FCVWeightField.free();
            m_DeltaVelField.free();

            m_ParticlesDeltaVel.Release();
            m_TempParticlesPICVel.Release();
        }

        public void resizeMixPICAndFLIP(
            CCellCenteredScalarField vFluidSDF,
            CCellCenteredScalarField vSolidSDF,
            int vMaxParitclesNum,
            int vNumOfPerGrid,
            Vector3Int vResolution,
            Vector3 vOrigin,
            Vector3 vSpacing,
            float vCFLNumber,
            float vMixingCoefficient
        )
        {
            m_PICCoefficient = vMixingCoefficient;
            m_CFLNumber = vCFLNumber;

            if(!m_IsInit)
            {
                m_EulerParticles = new CEulerParticles(vMaxParitclesNum);

                m_EulerParticles.addParticles(vFluidSDF, vSolidSDF, vNumOfPerGrid);

                m_CCSWeightField = new CCellCenteredScalarField(vResolution, vOrigin, vSpacing);
                m_CCVWeightField = new CCellCenteredVectorField(vResolution, vOrigin, vSpacing);
                m_FCVWeightField = new CFaceCenteredVectorField(vResolution, vOrigin, vSpacing);
                m_DeltaVelField = new CFaceCenteredVectorField(vResolution, vOrigin, vSpacing);

                m_ParticlesDeltaVel = new ComputeBuffer(3 * vMaxParitclesNum, sizeof(float));
                m_TempParticlesPICVel = new ComputeBuffer(3 * vMaxParitclesNum, sizeof(float));

                m_IsInit = true;
            }
            else
            {
                m_EulerParticles.resize(vMaxParitclesNum);

                m_EulerParticles.addParticles(vFluidSDF, vSolidSDF, vNumOfPerGrid);

                m_CCSWeightField.resize(vResolution, vOrigin, vSpacing);
                m_CCVWeightField.resize(vResolution, vOrigin, vSpacing);
                m_FCVWeightField.resize(vResolution, vOrigin, vSpacing);
                m_DeltaVelField.resize(vResolution, vOrigin, vSpacing);

                m_ParticlesDeltaVel.Release();
                m_TempParticlesPICVel.Release();
                m_ParticlesDeltaVel = new ComputeBuffer(3 * vMaxParitclesNum, sizeof(float));
                m_TempParticlesPICVel = new ComputeBuffer(3 * vMaxParitclesNum, sizeof(float));
            }

            m_VelFlag = false;
        }
        #endregion

        #region Get&Set
        public CEulerParticles getEulerParticles()
        {
            return m_EulerParticles;
        }
        
        public void setMixingCoefficient(float vMixingCoefficient)
        {
            m_PICCoefficient = vMixingCoefficient;
        }

        public void setCFLNumber(float vCFLNumber)
        {
            m_CFLNumber = vCFLNumber;
        }
        #endregion

        #region Public Method
        public void advect
        (
            CFaceCenteredVectorField vInputField,
            CFaceCenteredVectorField vVelocityField,
            float vDeltaT,
            CFaceCenteredVectorField voOutputField,
            EPGTransferAlgorithm vPGTransferAlgorithm = EPGTransferAlgorithm.LINEAR,
            EAdvectionAccuracy vAdvectionAccuracy = EAdvectionAccuracy.RK2,
            ESamplingAlgorithm vSamplingAlgorithm = ESamplingAlgorithm.LINEAR,
            bool isImproved = false
        )
        {
            CEulerParticlesInvokers.tranferFCVField2ParticlesInvoker(m_EulerParticles.getParticlesPos(), m_TempParticlesPICVel, vInputField, vPGTransferAlgorithm);

            if (!m_VelFlag)
            {
                m_DeltaVelField.resize(vInputField);
                m_VelFlag = true;
            }
            else
            {
                m_DeltaVelField.plusAlphaX(vInputField, -1.0f);
                m_DeltaVelField.scale(-1.0f);
            }
            
            CEulerParticlesInvokers.tranferFCVField2ParticlesInvoker(m_EulerParticles.getParticlesPos(), m_ParticlesDeltaVel, m_DeltaVelField, vPGTransferAlgorithm);
            CMathTool.plusAlphaX(m_EulerParticles.getParticlesVel(), m_ParticlesDeltaVel, 1.0f);
            
            CMathTool.scale(m_EulerParticles.getParticlesVel(), (1.0f - m_PICCoefficient));
            CMathTool.plusAlphaX(m_EulerParticles.getParticlesVel(), m_TempParticlesPICVel, m_PICCoefficient);

            if(isImproved)
            {
                m_EulerParticles.advectParticlesInVelFieldImproved(vVelocityField, vDeltaT, m_CFLNumber, vAdvectionAccuracy);
            }
            else
            {
                m_EulerParticles.advectParticlesInVelField(vVelocityField, vDeltaT, m_CFLNumber, vAdvectionAccuracy, vSamplingAlgorithm);
            }

            m_EulerParticles.transferParticlesVel2Field(voOutputField, m_FCVWeightField, vPGTransferAlgorithm);

            m_DeltaVelField.resize(voOutputField);
        }

        public void addFluid
        (
            CCellCenteredScalarField vAdditionalFluidSDF,
            CCellCenteredScalarField vPreviousFluidSDF,
            CCellCenteredScalarField vSolidSDF,
            int vNumOfPerGrid
        )
        {
            m_EulerParticles.addParticles(vAdditionalFluidSDF, vPreviousFluidSDF, vSolidSDF, vNumOfPerGrid);
        }

        public void addFluid
        (
            CCellCenteredScalarField vAdditionalFluidSDF,
            CCellCenteredScalarField vSolidSDF,
            int vNumOfPerGrid
        )
        {
            m_EulerParticles.addParticles(vAdditionalFluidSDF, vSolidSDF, vNumOfPerGrid);
        }

        public void addFluid(SBoundingBox vAdditionalFluidBox, CCellCenteredScalarField vSolidSDF, int vNumPerGrid)
        {
            m_EulerParticles.addParticles(vAdditionalFluidBox, vSolidSDF, vNumPerGrid);
        }
        #endregion

        private bool m_IsInit = false;

        private float m_PICCoefficient;
        private float m_CFLNumber;

        private CEulerParticles m_EulerParticles;

        private CCellCenteredScalarField m_CCSWeightField;
        private CCellCenteredVectorField m_CCVWeightField;
        private CFaceCenteredVectorField m_FCVWeightField;

        private CFaceCenteredVectorField m_DeltaVelField;
        private ComputeBuffer m_ParticlesDeltaVel;
        private ComputeBuffer m_TempParticlesPICVel;

        private bool m_VelFlag = false;
    }
}
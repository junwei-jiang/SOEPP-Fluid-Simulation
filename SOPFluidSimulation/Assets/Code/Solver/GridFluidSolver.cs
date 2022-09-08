using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using SDFr;
using EulerFluidEngine;

namespace EulerFluidEngine
{
    public class CGridFluidSolver
    {
        #region Constructor&Resize
        public CGridFluidSolver
        (
            float vDeltaT,
            Vector3Int vResolution,
            Vector3 vOrigin,
            Vector3 vSpacing,
            EAdvectionAlgorithm vAdvectionAlgorithm
        )
        {
            resizeGridFluidSolver(vDeltaT, vResolution, vOrigin, vSpacing, vAdvectionAlgorithm);
            Shader.SetGlobalFloat("FluidSurfaceDensity", CGlobalMacroAndFunc.FLUID_SURFACE_DENSITY);
            Shader.SetGlobalFloat("FluidDomainValue", CGlobalMacroAndFunc.FLUID_DOMAIN_VALUE);
        }
        ~CGridFluidSolver()
        {
            free();
        }

        public void free()
        {
            if (m_IsInit)
            {
                m_Boundarys.free();

                m_FluidDomainField.free();
                m_FluidSDFField1.free();
                m_FluidSDFField2.free();
                m_BoundarysSDFField.free();
                m_BoundarysVelField.free();
                m_AdditionalFluidField.free();
                m_FluidDensityField1.free();
                m_FluidDensityField2.free();
                m_FluidTemperatureField.free();
                m_FluidTempScalarField.free();
                m_VelocityField2.free();

                m_ReflectVelocityFieldA.free();
                m_ReflectVelocityFieldB.free();
                m_ReflectVelocityFieldC.free();

                m_TempVelocityField1.free();
                m_TempVelocityField2.free();

                m_ExtrapolatingVelMarkersField.free();

                if (m_GridAdvectionSolver != null)
                {
                    m_GridAdvectionSolver.free();
                }
                if (m_ParticleAdvectionSolver != null)
                {
                    m_ParticleAdvectionSolver.free();
                }
                if (m_ViscositySolver != null)
                {
                    m_ViscositySolver.free();//m_BuoyancySolver
                }
                if (m_PressureSolver != null)
                {
                    m_PressureSolver.free();
                }
                m_PressureSolver.free();
                m_CCSWeightField.free();
                m_CCVWeightField.free();

                m_Emitter = null;
            }
        }

        public void resizeGridFluidSolver
        (
            float vDeltaT,
            Vector3Int vResolution,
            Vector3 vOrigin,
            Vector3 vSpacing,
            EAdvectionAlgorithm vAdvectionAlgorithm
        )
        {
            m_DeltaT = vDeltaT;
            m_GridResolution = vResolution;
            m_GridOrigin = vOrigin;
            m_GridSpacing = vSpacing;

            m_CurSimulationFrame = 0;
            m_ExtrapolatingNums = 1000;
            m_BufferFlag = true;
            m_BoundarysType = true;

            m_AdvectionAlgorithm = vAdvectionAlgorithm;

            if (!m_IsInit)
            {
                m_Boundarys = new CBoundarys(vResolution, vOrigin, vSpacing);
                m_FluidDomainField = new CCellCenteredScalarField(vResolution, vOrigin, vSpacing);
                m_FluidSDFField1 = new CCellCenteredScalarField(vResolution, vOrigin, vSpacing);
                m_FluidSDFField2 = new CCellCenteredScalarField(vResolution, vOrigin, vSpacing);
                m_BoundarysSDFField = new CCellCenteredScalarField(vResolution, vOrigin, vSpacing);
                m_BoundarysVelField = new CFaceCenteredVectorField(vResolution, vOrigin, vSpacing);
                m_AdditionalFluidField = new CCellCenteredScalarField(vResolution, vOrigin, vSpacing);
                m_FluidDensityField1 = new CCellCenteredScalarField(vResolution, vOrigin, vSpacing);
                m_FluidDensityField2 = new CCellCenteredScalarField(vResolution, vOrigin, vSpacing);
                m_FluidTemperatureField = new CCellCenteredScalarField(vResolution, vOrigin, vSpacing);
                m_FluidTempScalarField = new CCellCenteredScalarField(vResolution, vOrigin, vSpacing);
                m_VelocityField1 = new CFaceCenteredVectorField(vResolution, vOrigin, vSpacing);
                m_VelocityField2 = new CFaceCenteredVectorField(vResolution, vOrigin, vSpacing);
                m_ReflectVelocityFieldA = new CFaceCenteredVectorField(vResolution, vOrigin, vSpacing);
                m_ReflectVelocityFieldB = new CFaceCenteredVectorField(vResolution, vOrigin, vSpacing);
                m_ReflectVelocityFieldC = new CFaceCenteredVectorField(vResolution, vOrigin, vSpacing);
                m_TempVelocityField1 = new CFaceCenteredVectorField(vResolution, vOrigin, vSpacing);
                m_TempVelocityField2 = new CFaceCenteredVectorField(vResolution, vOrigin, vSpacing);
                m_TempVelocityField3 = new CFaceCenteredVectorField(vResolution, vOrigin, vSpacing);
                m_VelocityFieldPrevious = new CFaceCenteredVectorField(vResolution, vOrigin, vSpacing);
                m_PressureField = new CCellCenteredScalarField(vResolution, vOrigin, vSpacing);
                m_PressureGradientField = new CCellCenteredVectorField(vResolution, vOrigin, vSpacing);
                m_CCSWeightField = new CCellCenteredScalarField(vResolution, vOrigin, vSpacing);
                m_CCVWeightField = new CCellCenteredVectorField(vResolution, vOrigin, vSpacing);
                m_ExtrapolatingVelMarkersField = new CFaceCenteredVectorField(vResolution, vOrigin, vSpacing);
                m_FluidSources = new List<FluidSource>();
                m_TriggerdDomains = new List<SBoundingBox>();

                m_IsInit = true;
            }
            else
            {
                m_Boundarys.initBoundarys(vResolution, vOrigin, vSpacing);
                m_FluidDomainField.resize(vResolution, vOrigin, vSpacing);
                m_FluidSDFField1.resize(vResolution, vOrigin, vSpacing);
                m_FluidSDFField2.resize(vResolution, vOrigin, vSpacing);
                m_BoundarysSDFField.resize(vResolution, vOrigin, vSpacing);
                m_BoundarysVelField.resize(vResolution, vOrigin, vSpacing);
                m_AdditionalFluidField.resize(vResolution, vOrigin, vSpacing);
                m_FluidDensityField1.resize(vResolution, vOrigin, vSpacing);
                m_FluidDensityField2.resize(vResolution, vOrigin, vSpacing);
                m_FluidTempScalarField.resize(vResolution, vOrigin, vSpacing);
                m_FluidTemperatureField.resize(vResolution, vOrigin, vSpacing);
                m_VelocityField1.resize(vResolution, vOrigin, vSpacing);
                m_VelocityField2.resize(vResolution, vOrigin, vSpacing);
                m_ReflectVelocityFieldA.resize(vResolution, vOrigin, vSpacing);
                m_ReflectVelocityFieldB.resize(vResolution, vOrigin, vSpacing);
                m_ReflectVelocityFieldC.resize(vResolution, vOrigin, vSpacing);
                m_TempVelocityField1.resize(vResolution, vOrigin, vSpacing);
                m_TempVelocityField2.resize(vResolution, vOrigin, vSpacing);
                m_TempVelocityField3.resize(vResolution, vOrigin, vSpacing);
                m_VelocityFieldPrevious.resize(vResolution, vOrigin, vSpacing);
                m_PressureField.resize(vResolution, vOrigin, vSpacing);
                m_PressureGradientField.resize(vResolution, vOrigin, vSpacing);
                m_CCSWeightField.resize(vResolution, vOrigin, vSpacing);
                m_CCVWeightField.resize(vResolution, vOrigin, vSpacing);
                m_ExtrapolatingVelMarkersField.resize(vResolution, vOrigin, vSpacing);
                m_FluidSources.Clear();
                m_TriggerdDomains.Clear();
            }

            m_GridAdvectionSolver = new CSemiLagrangian(vResolution, vOrigin, vSpacing);
            m_ParticleAdvectionSolver = new CMixPICAndFLIP();
            m_ExternalForcesSolver = new CExternalForcesSolver();
            m_ViscositySolver = new CViscositySolver(vResolution, vOrigin, vSpacing);
            m_BuoyancySolver = new CBuoyancySolver();
            m_PressureSolver = new CPressureSolver(vResolution, vOrigin, vSpacing);
        }
        #endregion

        #region Get&Set
        public void setExtrapolatingNums(int vExtrapolatingNums)
        {
            m_ExtrapolatingNums = vExtrapolatingNums;
        }

        public void setVelocityField(CFaceCenteredVectorField vVelocityField)
        {
            m_VelocityField1.resize(vVelocityField);
            m_VelocityField2.resize(vVelocityField);
            m_VelocityFieldPrevious.resize(vVelocityField);
            //m_VelocityField.resize(vVelocityField);
        }

        public void setBoundarysType(bool vBoundarysType)
        {
            m_BoundarysType = vBoundarysType;
        }

        public void setBoundarys(CBoundarys vBoundarys)
        {
            m_Boundarys = vBoundarys;
        }

        public void setBoundarys(CCellCenteredScalarField vBoundarysSDFField)
        {
            m_BoundarysSDFField = vBoundarysSDFField;
        }

        public void setBoundarysVel(CFaceCenteredVectorField vBoundarysVelField)
        {
            m_BoundarysVelField = vBoundarysVelField;
        }

        public void setSamplingAlgorithm(ESamplingAlgorithm vSamplingAlgorithm)
        {
            m_SamplingAlgorithm = vSamplingAlgorithm;
        }

        public void setAdvectionAccuracy(EAdvectionAccuracy vAdvectionAccuracy)
        {
            m_AdvectionAccuracy = vAdvectionAccuracy;
        }

        public void setPGTransferAlgorithm(EPGTransferAlgorithm vPGTransferAlgorithm)
        {
            m_PGTransferAlgorithm = vPGTransferAlgorithm;
        }

        public void setEmitter(CEmitter vEmitter)
        {
            m_Emitter = vEmitter;
        }

        public void setViscous(bool vIsViscous)
        {
            m_IsViscous = vIsViscous;
        }

        public void setBuoyancy(bool vHasBuoyancy)
        {
            m_HasBuoyancy = vHasBuoyancy;
        }

        public Vector3Int getResolution()
        {
            return m_GridResolution;
        }

        public Vector3 getOrigin()
        {
            return m_GridOrigin;
        }

        public Vector3 getSpacing()
        {
            return m_GridSpacing;
        }

        public CCellCenteredScalarField getFluidDomainField()
        {
            return m_FluidDomainField;
        }

        public CCellCenteredScalarField getFluidSDFField()
        {
            if (m_BufferFlag)
            {
                return m_FluidSDFField2;
            }
            else
            {
                return m_FluidSDFField1;
            }
        }

        public CCellCenteredScalarField getSolidSDFField()
        {
            if (m_BoundarysType)
            {
                return m_Boundarys.getTotalBoundarysSDF();
            }
            else
            {
                return m_BoundarysSDFField;
            }
        }

        public CFaceCenteredVectorField getSolidVelField()
        {
            if (m_BoundarysType)
            {
                return m_Boundarys.getTotalBoundarysVel();
            }
            else
            {
                return m_BoundarysVelField;
            }
        }

        public void settFluidDensityField1(CCellCenteredScalarField vDensityField)
        {
            m_FluidDensityField1.resize(vDensityField);
        }

        public CCellCenteredScalarField getFluidDensityField1()
        {
            return m_FluidDensityField1;
        }

        public CCellCenteredScalarField getFluidDensityField2()
        {
            return m_FluidDensityField2;
        }

        public CCellCenteredScalarField getFluidTemperatureField()
        {
            return m_FluidTemperatureField;
        }

        public CCellCenteredScalarField getFluidPressureField()
        {
            return m_PressureSolver.getPressureField();
        }

        public CFaceCenteredVectorField getVelocityField()
        {
            if (m_BufferFlag)
            {
                return m_VelocityField2;
            }
            else
            {
                return m_VelocityField1;
            }
        }

        public CFaceCenteredVectorField getVelocityFieldBeforePressure()
        {
            if (m_BufferFlag)
            {
                return m_VelocityField1;
            }
            else
            {
                return m_VelocityField2;
            }
        }

        public CMixPICAndFLIP getParticlesAdvectionSolver()
        {
            return m_ParticleAdvectionSolver;
        }

        public CSemiLagrangian getGridAdvectionSolver()
        {
            return m_GridAdvectionSolver;
        }

        public CExternalForcesSolver getExternalForcesSolver()
        {
            return m_ExternalForcesSolver;
        }

        public CPressureSolver getPressureSolver()
        {
            return m_PressureSolver;
        }

        public CBoundarys getBoundarys()
        {
            return m_Boundarys;
        }

        public int getCurSimulationTime()
        {
            return m_CurSimulationFrame;
        }

        public CEmitter getEmitter()
        {
            return m_Emitter;
        }
        #endregion

        #region Other Methods
        public void addDynamicBoundarys(List<GameObject> vBoundaryObjects, Vector3[] vVelocities)
        {
            m_Boundarys.addBoundarys(vBoundaryObjects, vVelocities, true);
        }

        public void addStaticBoundarys(List<GameObject> vBoundaryObjects)
        {
            m_Boundarys.addBoundarys(vBoundaryObjects, null, false);
        }

        public void generateFluid(CCellCenteredScalarField vFluidDomainField, int vMaxParticlesNum, int vNumOfParticlesPerGrid = 8, float vMixingCoefficient = 0.01f, float vCFLNumber = 1.0f)
        {
            m_FluidDomainField.resize(vFluidDomainField);

            if (m_AdvectionAlgorithm == EAdvectionAlgorithm.SemiLagrangian || m_AdvectionAlgorithm == EAdvectionAlgorithm.MacCormack || m_AdvectionAlgorithm == EAdvectionAlgorithm.Iteration)
            {
                m_FluidSDFField1.resize(vFluidDomainField);
                m_FluidSDFField2.resize(vFluidDomainField);

                m_GridAdvectionSolver.resizeSemiLagrangian(m_GridResolution, m_GridOrigin, m_GridSpacing);
            }
            else if (m_AdvectionAlgorithm == EAdvectionAlgorithm.PIC || m_AdvectionAlgorithm == EAdvectionAlgorithm.FLIP || m_AdvectionAlgorithm == EAdvectionAlgorithm.MixPICAndFLIP)
            {
                m_ParticleAdvectionSolver.resizeMixPICAndFLIP
                (
                    getFluidDomainField(),
                    getSolidSDFField(),
                    vMaxParticlesNum,
                    vNumOfParticlesPerGrid,
                    m_GridResolution,
                    m_GridOrigin,
                    m_GridSpacing,
                    vCFLNumber,
                    vMixingCoefficient
                 );
            }
            else
            {
                Debug.LogError("Invalid advection solver");
            }
        }

        public void generateFluid(Vector3 vFluidDomainMin, Vector3 vFluidDomainMax, int vMaxParticlesNum, int vNumOfParticlesPerGrid = 8, float vMixingCoefficient = 0.01f, float vCFLNumber = 1.0f)
        {
            if (m_BoundarysType)
            {
                m_Boundarys.updateBoundarys(0);
            }
            CEulerSolverToolInvoker.generateFluidDomainFromBBoxInvoker(vFluidDomainMin, vFluidDomainMax, getSolidSDFField(), m_FluidDomainField);

            if (m_AdvectionAlgorithm == EAdvectionAlgorithm.SemiLagrangian || m_AdvectionAlgorithm == EAdvectionAlgorithm.MacCormack || m_AdvectionAlgorithm == EAdvectionAlgorithm.Iteration)
            {
                m_FluidSDFField1.resize(getFluidDomainField());
                m_FluidSDFField2.resize(getFluidDomainField());

                m_GridAdvectionSolver.resizeSemiLagrangian(m_GridResolution, m_GridOrigin, m_GridSpacing);
            }
            else if (m_AdvectionAlgorithm == EAdvectionAlgorithm.PIC || m_AdvectionAlgorithm == EAdvectionAlgorithm.FLIP || m_AdvectionAlgorithm == EAdvectionAlgorithm.MixPICAndFLIP)
            {
                m_ParticleAdvectionSolver.resizeMixPICAndFLIP
                (
                    getFluidDomainField(),
                    getSolidSDFField(),
                    vMaxParticlesNum,
                    vNumOfParticlesPerGrid,
                    m_GridResolution,
                    m_GridOrigin,
                    m_GridSpacing,
                    vCFLNumber,
                    vMixingCoefficient
                 );
            }
            else
            {
                Debug.LogError("Invalid advection solver");
            }
        }

        public void generateFluid(List<SBoundingBox> vInitialFluidDomains, int vMaxParticlesNum, int vNumOfParticlesPerGrid = 8, float vMixingCoefficient = 0.01f, float vCFLNumber = 1.0f)
        {
            if (m_BoundarysType)
            {
                m_Boundarys.updateBoundarys(0.05f);
            }
            CEulerSolverToolInvoker.generateFluidDomainFromBoundingBoxsInvoker(vInitialFluidDomains, getSolidSDFField(), m_FluidDomainField);

            m_ParticleAdvectionSolver.resizeMixPICAndFLIP
            (
                getFluidDomainField(),
                getSolidSDFField(),
                vMaxParticlesNum,
                vNumOfParticlesPerGrid,
                m_GridResolution,
                m_GridOrigin,
                m_GridSpacing,
                vCFLNumber,
                vMixingCoefficient
             );
        }

        public void setTwoDSmokeEmitter(int vEmitteStartFrame, int vEmitteEndFrame, float vEmittedensity, float vEmitteTemperature, Vector3 vEmitterInitialPos, EmitteVelFunc vEmitteVelFunc = null, EmitterVelFunc vEmitterVelFunc = null)
        {
            m_TwoDSmokeEmitter = new CTwoDSmokeEmitter(vEmitteStartFrame, vEmitteEndFrame, vEmittedensity, vEmitteTemperature, vEmitterInitialPos, vEmitteVelFunc, vEmitterVelFunc);
            m_SmokeEmitterFlag = ESmokeScene.TwoDSmoke;
        }

        public void setThreeDSmokeEmitter(int vEmitteStartFrame, int vEmitteEndFrame, float vEmittedensity, float vEmitteTemperature, Vector3 vEmitterInitialPos)
        {
            m_ThreeDSmokeEmitter = new CThreeDSmokeEmitter(vEmitteStartFrame, vEmitteEndFrame, vEmittedensity, vEmitteTemperature, vEmitterInitialPos);
            m_SmokeEmitterFlag = ESmokeScene.ThreeDSmoke;
        }

        public void setVortexCollisionSmokeEmitter(int vEmitteStartFrame, int vEmitteEndFrame, float vEmittedensity, float vEmitteTemperature, Vector3 vEmitterInitialPos, EmitteVelFunc vEmitteVelFunc = null, EmitterVelFunc vEmitterVelFunc = null)
        {
            m_VortexCollisionSmokeEmitter = new CVortexCollisionSmokeEmitter(vEmitteStartFrame, vEmitteEndFrame, vEmittedensity, vEmitteTemperature, vEmitterInitialPos, vEmitteVelFunc, vEmitterVelFunc);
            m_SmokeEmitterFlag = ESmokeScene.VortexCollision;
        }

        public void setVortexLeapFroggingSmokeEmitter(int vEmitteStartFrame, int vEmitteEndFrame, float vEmittedensity, float vEmitteTemperature, Vector3 vEmitterInitialPos, EmitteVelFunc vEmitteVelFunc = null, EmitterVelFunc vEmitterVelFunc = null)
        {
            m_VortexLeapFroggingSmokeEmitter = new CVortexLeapFroggingSmokeEmitter(vEmitteStartFrame, vEmitteEndFrame, vEmittedensity, vEmitteTemperature, vEmitterInitialPos, vEmitteVelFunc, vEmitterVelFunc);
            m_SmokeEmitterFlag = ESmokeScene.VortexLeapFrogging;
        }

        public void addFluidSource(FluidSource vNewFluidSource)
        {
            m_FluidSources.Add(vNewFluidSource);
        }

        public void addFluidSources(List<FluidSource> vNewFluidSources)
        {
            foreach (FluidSource NewFluidSource in vNewFluidSources)
            {
                if (NewFluidSource != null)
                {
                    m_FluidSources.Add(NewFluidSource);
                }
            }
        }

        public void addExternalForce(Vector3 vExternalForce)
        {
            m_ExternalForcesSolver.addExternalForces(vExternalForce);
        }

        public void addFluid(List<SBoundingBox> vAdditionalFluidDomains, int vNumOfParticlesPerGrid = 8)
        {
            CEulerSolverToolInvoker.generateFluidDomainFromBoundingBoxsInvoker(vAdditionalFluidDomains, getSolidSDFField(), m_AdditionalFluidField);

            m_ParticleAdvectionSolver.addFluid(m_AdditionalFluidField, m_FluidDomainField, getSolidSDFField(), vNumOfParticlesPerGrid);
        }

        public void addFluid(SBoundingBox vAdditionalFluidBox, int vNumOfParticlesPerGrid = 8)
        {
            m_ParticleAdvectionSolver.addFluid(vAdditionalFluidBox, getSolidSDFField(), vNumOfParticlesPerGrid);
        }
        #endregion

        #region Core Methods
        public void update(EPressureAlgorithm vPressureAlgorithm = EPressureAlgorithm.FirstOrder)
        {
            if (vPressureAlgorithm == EPressureAlgorithm.FirstOrder)
            {
                __onAdvanceTimeStepWithFirstOrderPressure(m_DeltaT);
            }
            else if (vPressureAlgorithm == EPressureAlgorithm.FirstOrder_Smoke)
            {
                __onAdvanceTimeStepWithFirstOrderPressure_Smoke(m_DeltaT);
            }
            else if (vPressureAlgorithm == EPressureAlgorithm.SecondOrder)
            {
                __onAdvanceTimeStepWithSecondOrderPressure(m_DeltaT);
            }
            else if (vPressureAlgorithm == EPressureAlgorithm.SecondOrder_Smoke)
            {
                __onAdvanceTimeStepWithSecondOrderPressure_Smoke(m_DeltaT);
            }
            else if (vPressureAlgorithm == EPressureAlgorithm.Reflection)
            {
                __onAdvanceTimeStepWithReflection(m_DeltaT);
            }
            else if (vPressureAlgorithm == EPressureAlgorithm.Reflection_Smoke)
            {
                __onAdvanceTimeStepWithReflection_Smoke(m_DeltaT);
            }
            else if (vPressureAlgorithm == EPressureAlgorithm.SecondOrderReflection)
            {
                __onAdvanceTimeStepWithSecondOrderReflection(m_DeltaT);
            }
            else if (vPressureAlgorithm == EPressureAlgorithm.SecondOrderReflection_Smoke)
            {
                __onAdvanceTimeStepWithSecondOrderReflection_Smoke(m_DeltaT);
            }
            else if (vPressureAlgorithm == EPressureAlgorithm.BDF2)
            {
                __onAdvanceTimeStepWithBDF2(m_DeltaT);
            }
            else if (vPressureAlgorithm == EPressureAlgorithm.BDF2_Smoke)
            {
                __onAdvanceTimeStepWithBDF2_Smoke(m_DeltaT);
            }
            else if (vPressureAlgorithm == EPressureAlgorithm.BDF2AndSecondOrder)
            {
                __onAdvanceTimeStepWithBDF2AndSecondOrderPressure(m_DeltaT);
            }
            else if (vPressureAlgorithm == EPressureAlgorithm.CrankNicolson)
            {
                __onAdvanceTimeStepWithCrankNicolson(m_DeltaT);
            }
            else if (vPressureAlgorithm == EPressureAlgorithm.MaterialAcceleration)
            {
                __onAdvanceTimeStepWithMaterialAcceleration(m_DeltaT);
            }
            else if (vPressureAlgorithm == EPressureAlgorithm.Test)
            {
                //m_TempVelocityField2.resize(m_VelocityField1);
                //__onAdvanceTimeStepWithFirstOrderPressureTest(m_DeltaT);
                //__onAdvanceTimeStepWithSecondOrderPressureTest(m_DeltaT);
                //__onAdvanceTimeStepWithFirstOrderPressureSmoke(m_DeltaT);
            }
            else
            {

            }
        }
        public void update(float vDeltaT, EPressureAlgorithm vPressureAlgorithm = EPressureAlgorithm.FirstOrder)
        {
            if (vPressureAlgorithm == EPressureAlgorithm.FirstOrder)
            {
                __onAdvanceTimeStepWithFirstOrderPressure(vDeltaT);
            }
            else if (vPressureAlgorithm == EPressureAlgorithm.FirstOrder_Smoke)
            {
                __onAdvanceTimeStepWithFirstOrderPressure_Smoke(vDeltaT);
            }
            else if (vPressureAlgorithm == EPressureAlgorithm.SecondOrder)
            {
                __onAdvanceTimeStepWithSecondOrderPressure(vDeltaT);
            }
            else if (vPressureAlgorithm == EPressureAlgorithm.SecondOrder_Smoke)
            {
                __onAdvanceTimeStepWithSecondOrderPressure_Smoke(vDeltaT);
            }
            else if (vPressureAlgorithm == EPressureAlgorithm.Reflection)
            {
                __onAdvanceTimeStepWithReflection(vDeltaT);
            }
            else if (vPressureAlgorithm == EPressureAlgorithm.Reflection_Smoke)
            {
                __onAdvanceTimeStepWithReflection_Smoke(vDeltaT);
            }
            else if (vPressureAlgorithm == EPressureAlgorithm.SecondOrderReflection)
            {
                __onAdvanceTimeStepWithSecondOrderReflection(vDeltaT);
            }
            else if (vPressureAlgorithm == EPressureAlgorithm.SecondOrderReflection_Smoke)
            {
                __onAdvanceTimeStepWithSecondOrderReflection_Smoke(vDeltaT);
            }
            else if (vPressureAlgorithm == EPressureAlgorithm.BDF2)
            {
                __onAdvanceTimeStepWithBDF2(vDeltaT);
            }
            else if (vPressureAlgorithm == EPressureAlgorithm.BDF2_Smoke)
            {
                __onAdvanceTimeStepWithBDF2_Smoke(vDeltaT);
            }
            else if (vPressureAlgorithm == EPressureAlgorithm.BDF2AndSecondOrder)
            {
                __onAdvanceTimeStepWithBDF2AndSecondOrderPressure(vDeltaT);
            }
            else if (vPressureAlgorithm == EPressureAlgorithm.CrankNicolson)
            {
                __onAdvanceTimeStepWithCrankNicolson(vDeltaT);
            }
            else if (vPressureAlgorithm == EPressureAlgorithm.MaterialAcceleration)
            {
                __onAdvanceTimeStepWithMaterialAcceleration(vDeltaT);
            }
            else if (vPressureAlgorithm == EPressureAlgorithm.Test)
            {
                //m_TempVelocityField2.resize(m_VelocityField1) ;
                //__onAdvanceTimeStepWithFirstOrderPressureTest(vDeltaT);
                //__onAdvanceTimeStepWithSecondOrderPressureTest(vDeltaT);
                //__onAdvanceTimeStepWithFirstOrderPressureSmoke(vDeltaT);
            }
            else
            {

            }
        }
        private void __onAdvanceTimeStepWithFirstOrderPressure(float vDeltaT)
        {
            __onBeginAdvanceTimeStep(vDeltaT);

            __computeAdvection(vDeltaT);
            __computeExternalForces(vDeltaT);
            __computeViscosity(vDeltaT);
            __executeHelmholtzHodgDecomposition(vDeltaT);
            __extrapolatingVel();

            __onEndAdvanceTimeStep(vDeltaT);
        }
        private void __onAdvanceTimeStepWithFirstOrderPressure_Smoke(float vDeltaT)
        {
            __onBeginAdvanceTimeStep(vDeltaT);

            __computeAdvectionSmoke(vDeltaT);
            __computeExternalForces(vDeltaT);
            __emitteSmoke(vDeltaT);
            __computeBuoyancy(vDeltaT);
            //CEulerSolverToolInvoker.applyBuoyancyInvoker(vDeltaT, 0.0f, 0.0f, m_FluidDensityField1, m_FluidTemperatureField, getVelocityFieldBeforePressure());
            __computeViscosity(vDeltaT);
            __executeHelmholtzHodgDecomposition(vDeltaT);
            __extrapolatingVel();

            __onEndAdvanceTimeStep(vDeltaT);
        }
        private void __onAdvanceTimeStepWithSecondOrderPressure(float vDeltaT)
        {
            if (m_CurSimulationFrame == 0)
            {
                __onAdvanceTimeStepWithReflection(vDeltaT);
            }
            else
            {
                __onBeginAdvanceTimeStep(vDeltaT);

                m_PressureField.resize(m_PressureSolver.getPressureField());
                if (m_BufferFlag)
                {
                    m_GridAdvectionSolver.advectMacCormack(m_PressureField, m_VelocityField2, 0.5f * vDeltaT, m_FluidTempScalarField, m_AdvectionAccuracy, m_SamplingAlgorithm);
                    m_PressureField.resize(m_FluidTempScalarField);
                }
                else
                {
                    m_GridAdvectionSolver.advectMacCormack(m_PressureField, m_VelocityField1, 0.5f * vDeltaT, m_FluidTempScalarField, m_AdvectionAccuracy, m_SamplingAlgorithm);
                    m_PressureField.resize(m_FluidTempScalarField);
                }
                m_PressureField.gradient(m_PressureSolver.getPressureGradientField());

                //m_PressureSolver.getPressureField().gradient(m_PressureSolver.getPressureGradientField());
                __computeAdvection(vDeltaT);
                __computeAdvectionPressure(vDeltaT);
                //__computeAdvectionPressureGradient(0.5f * vDeltaT);
                __computeExternalForces(vDeltaT);
                __computeViscosity(vDeltaT);
                __computePressure_NoBackTrace(vDeltaT);
                __executeHelmholtzHodgDecomposition(vDeltaT);
                __extrapolatingVel();

                __onEndAdvanceTimeStep(vDeltaT);
            }
        }
        private void __onAdvanceTimeStepWithSecondOrderPressure_Smoke(float vDeltaT)
        {
            if (m_CurSimulationFrame == 0)
            {
                __onAdvanceTimeStepWithReflection_Smoke(vDeltaT);
            }
            else
            {
                __onBeginAdvanceTimeStep(vDeltaT);

                __computeAdvectionSmoke(vDeltaT);
                m_PressureSolver.getPressureField().gradient(m_PressureSolver.getPressureGradientField());
                __computeAdvectionPressure(vDeltaT);
                __computeAdvectionPressureGradient(0.5f * vDeltaT);
                __computeExternalForces(vDeltaT);
                __emitteSmoke(vDeltaT);
                __computeBuoyancy(vDeltaT);
                //CEulerSolverToolInvoker.applyBuoyancyInvoker(vDeltaT, 0.0f, 0.0f, m_FluidDensityField1, m_FluidTemperatureField, getVelocityFieldBeforePressure());
                __computeViscosity(vDeltaT);
                __computePressure_NoBackTrace(vDeltaT);
                __executeHelmholtzHodgDecomposition(vDeltaT);
                __extrapolatingVel();

                __onEndAdvanceTimeStep(vDeltaT);
            }
        }
        private void __onAdvanceTimeStepWithReflection(float vDeltaT)
        {
            __onBeginAdvanceTimeStep(vDeltaT);

            __computeAdvection(0.5f * vDeltaT);
            __computeAdvectionPressure(0.5f * vDeltaT);
            //__computeAdvectionPressureGradient(0.5f * vDeltaT);
            __computeExternalForces(0.5f * vDeltaT);
            __computeViscosity(0.5f * vDeltaT);
            m_ReflectVelocityFieldA.resize(m_VelocityField1);
            __executeHelmholtzHodgDecomposition(0.5f * vDeltaT);
            __extrapolatingVel();
            m_ReflectVelocityFieldA.scale(-1.0f);
            m_ReflectVelocityFieldA.plusAlphaX(m_VelocityField1, 2.0f);
            m_BufferFlag = !m_BufferFlag;
            __computeAdvection_Reflection(0.5f * vDeltaT);
            __computeAdvectionPressure(0.5f * vDeltaT);
            //__computeAdvectionPressureGradient(0.5f * vDeltaT);
            __computeExternalForces(0.5f * vDeltaT);
            __computeViscosity(0.5f * vDeltaT);
            __executeHelmholtzHodgDecomposition(0.5f * vDeltaT);
            __extrapolatingVel_Reflection();
            __onEndAdvanceTimeStep(vDeltaT);
        }
        private void __onAdvanceTimeStepWithSecondOrderReflection(float vDeltaT)
        {
            __onBeginAdvanceTimeStep(vDeltaT);

            m_ReflectVelocityFieldA.resize(m_VelocityField2);
            __computeAdvection(0.5f * vDeltaT);
            m_ReflectVelocityFieldB.resize(m_VelocityField1);
            __computeExternalForces(0.5f * vDeltaT);
            __computeViscosity(0.5f * vDeltaT);
            __executeHelmholtzHodgDecomposition(0.5f * vDeltaT);
            __extrapolatingVel();
            m_ReflectVelocityFieldC.resize(m_VelocityField1);
            m_ReflectVelocityFieldC.scale(2.0f);
            m_ReflectVelocityFieldC.plusAlphaX(m_ReflectVelocityFieldB, -1.0f);
            m_BufferFlag = !m_BufferFlag;
            __computeAdvection_SecondOrderReflection(0.5f * vDeltaT);
            __executeHelmholtzHodgDecomposition(0.5f * vDeltaT);
            __extrapolatingVel_Reflection();
            __onEndAdvanceTimeStep(vDeltaT);
        }
        private void __onAdvanceTimeStepWithReflection_Smoke(float vDeltaT)
        {
            __onBeginAdvanceTimeStep(vDeltaT);

            //__computeAdvection_extrapolated(0.5f * vDeltaT);
            __computeAdvectionSmoke(0.5f * vDeltaT);
            __computeAdvectionPressure(0.5f * vDeltaT);
            __computeExternalForces(0.5f * vDeltaT);
            __emitteSmoke(vDeltaT);
            __computeBuoyancy(0.5f * vDeltaT);
            __computeViscosity(0.5f * vDeltaT);
            m_ReflectVelocityFieldA.resize(m_VelocityField1);
            __executeHelmholtzHodgDecomposition(0.5f * vDeltaT);
            __extrapolatingVel();
            m_ReflectVelocityFieldA.scale(-1.0f);
            m_ReflectVelocityFieldA.plusAlphaX(m_VelocityField1, 2.0f);
            m_BufferFlag = !m_BufferFlag;
            //__computeAdvection_extrapolated(0.5f * vDeltaT);
            __computeAdvectionSmoke_Reflection(0.5f * vDeltaT);
            __computeAdvectionPressure(0.5f * vDeltaT);
            __computeExternalForces(0.5f * vDeltaT);
            __computeBuoyancy(0.5f * vDeltaT);
            __computeViscosity(0.5f * vDeltaT);
            __executeHelmholtzHodgDecomposition(0.5f * vDeltaT);
            __extrapolatingVel();
            __onEndAdvanceTimeStep(vDeltaT);
        }
        private void __onAdvanceTimeStepWithSecondOrderReflection_Smoke(float vDeltaT)
        {
            __onBeginAdvanceTimeStep(vDeltaT);

            m_ReflectVelocityFieldA.resize(m_VelocityField2);
            __computeAdvectionSmoke(0.5f * vDeltaT);
            m_ReflectVelocityFieldB.resize(m_VelocityField1);
            __computeExternalForces(0.5f * vDeltaT);
            __emitteSmoke(vDeltaT);
            __computeBuoyancy(0.5f * vDeltaT);
            __computeViscosity(0.5f * vDeltaT);
            __executeHelmholtzHodgDecomposition(0.5f * vDeltaT);
            __extrapolatingVel();
            m_ReflectVelocityFieldC.resize(m_VelocityField1);
            m_ReflectVelocityFieldC.scale(2.0f);
            m_ReflectVelocityFieldC.plusAlphaX(m_ReflectVelocityFieldB, -1.0f);
            m_BufferFlag = !m_BufferFlag;
            __computeAdvectionSmoke_SecondOrderReflection(0.5f * vDeltaT);
            __executeHelmholtzHodgDecomposition(0.5f * vDeltaT);
            __extrapolatingVel_Reflection();
            __onEndAdvanceTimeStep(vDeltaT);
        }
        private void __onAdvanceTimeStepWithBDF2(float vDeltaT)
        {
            __onBeginAdvanceTimeStep(vDeltaT);

            __computeAdvection_BDF2(vDeltaT);
            __computeExternalForces(vDeltaT / 2.0f);
            __computeViscosity(vDeltaT / 2.0f);
            __executeHelmholtzHodgDecomposition_BDF2(vDeltaT);
            __extrapolatingVel();

            __onEndAdvanceTimeStep(vDeltaT);
        }
        private void __onAdvanceTimeStepWithBDF2_Smoke(float vDeltaT)
        {
            __onBeginAdvanceTimeStep(vDeltaT);

            __computeAdvectionSmoke_BDF2(vDeltaT);
            __computeExternalForces(vDeltaT / 2.0f);
            __emitteSmoke(vDeltaT);
            __computeBuoyancy(vDeltaT / 2.0f);
            __computeViscosity(vDeltaT / 2.0f);
            __executeHelmholtzHodgDecomposition_BDF2(vDeltaT);
            __extrapolatingVel();

            __onEndAdvanceTimeStep(vDeltaT);
        }
        //可能有问题
        private void __onAdvanceTimeStepWithBDF2AndSecondOrderPressure(float vDeltaT)
        {
            __onBeginAdvanceTimeStep(vDeltaT);

            __computeAdvection_BDF2(vDeltaT);
            __computeExternalForces(vDeltaT / 2.0f);
            __computePressureWithBackTrace(vDeltaT / 2.0f);
            __computeViscosity(vDeltaT / 2.0f);
            __executeHelmholtzHodgDecomposition_BDF2(vDeltaT);
            __extrapolatingVel();

            __onEndAdvanceTimeStep(vDeltaT);
        }
        private void __onAdvanceTimeStepWithCrankNicolson(float vDeltaT)
        {
            __onBeginAdvanceTimeStep(vDeltaT);

            __computeAdvection_extrapolated(vDeltaT);
            __computeExternalForces(vDeltaT);
            __computeViscosity(vDeltaT);
            __computePressureWithBackTrace(0.5f * vDeltaT);
            //__computePressure_NoBackTrace(0.5f * vDeltaT);
            __executeHelmholtzHodgDecomposition_CN(vDeltaT);
            __extrapolatingVel();

            __onEndAdvanceTimeStep(vDeltaT);
            //__onBeginAdvanceTimeStep(vDeltaT);

            //__computeAdvection(vDeltaT);
            //__computeExternalForces(vDeltaT);
            //__computeViscosity(vDeltaT);
            //__executeHelmholtzHodgDecomposition_CN(vDeltaT);
            //__extrapolatingVel();

            //__onEndAdvanceTimeStep(vDeltaT);
        }
        private void __onAdvanceTimeStepWithMaterialAcceleration(float vDeltaT)
        {
            __onBeginAdvanceTimeStep(vDeltaT);

            __computeAdvection_MaterialAcceleration(vDeltaT);
            __computeExternalForces(2.0f * vDeltaT / 3.0f);
            //__computePressureWithBackTrace(2.0f * vDeltaT / 3.0f);
            __computeViscosity(vDeltaT);
            __executeHelmholtzHodgDecomposition_MA(vDeltaT);//vDeltaT
            __extrapolatingVel();

            __onEndAdvanceTimeStep(vDeltaT);
        }
        private void __onBeginAdvanceTimeStep(float vDeltaT)
        {
            if (m_BoundarysType)
            {
                m_Boundarys.updateBoundarys(vDeltaT);
            }
            else
            {
                //TODO
            }

            if (m_Emitter != null && m_Emitter.IsActive == true)
            {
                addFluid(m_Emitter.getEmitBox(vDeltaT), m_Emitter.ParticlesNumPerGrid);
            }
        }
        private void __computeAdvectionPressure(float vDeltaT)
        {
            if (m_AdvectionAlgorithm == EAdvectionAlgorithm.SemiLagrangian)
            {
                if (m_BufferFlag)
                {
                    m_GridAdvectionSolver.advect(m_PressureSolver.getPressureField(), m_VelocityField2, vDeltaT, m_PressureField, m_AdvectionAccuracy, m_SamplingAlgorithm);
                    m_PressureSolver.getPressureField().resize(m_PressureField);

                }
                else
                {
                    m_GridAdvectionSolver.advect(m_PressureSolver.getPressureField(), m_VelocityField1, vDeltaT, m_PressureField, m_AdvectionAccuracy, m_SamplingAlgorithm);
                    m_PressureSolver.getPressureField().resize(m_PressureField);
                }
            }
            else if (m_AdvectionAlgorithm == EAdvectionAlgorithm.MacCormack)
            {
                if (m_BufferFlag)
                {
                    m_GridAdvectionSolver.advectMacCormack(m_PressureSolver.getPressureField(), m_VelocityField2, vDeltaT, m_PressureField, m_AdvectionAccuracy, m_SamplingAlgorithm);
                    m_PressureSolver.getPressureField().resize(m_PressureField);
                }
                else
                {
                    m_GridAdvectionSolver.advectMacCormack(m_PressureSolver.getPressureField(), m_VelocityField1, vDeltaT, m_PressureField, m_AdvectionAccuracy, m_SamplingAlgorithm);
                    m_PressureSolver.getPressureField().resize(m_PressureField);
                }
            }
            else if (m_AdvectionAlgorithm == EAdvectionAlgorithm.PIC || m_AdvectionAlgorithm == EAdvectionAlgorithm.FLIP || m_AdvectionAlgorithm == EAdvectionAlgorithm.MixPICAndFLIP)
            {

            }
            else
            {
                Debug.LogError("Invalid advection solver");
            }
        }
        private void __computeAdvectionPressureGradient(float vDeltaT)
        {
            if (m_AdvectionAlgorithm == EAdvectionAlgorithm.SemiLagrangian)
            {
                if (m_BufferFlag)
                {
                    m_GridAdvectionSolver.advect(m_PressureSolver.getPressureGradientField(), m_VelocityField2, vDeltaT, m_PressureGradientField, m_AdvectionAccuracy, m_SamplingAlgorithm);
                    m_PressureSolver.getPressureGradientField().resize(m_PressureGradientField);

                }
                else
                {
                    m_GridAdvectionSolver.advect(m_PressureSolver.getPressureGradientField(), m_VelocityField1, vDeltaT, m_PressureGradientField, m_AdvectionAccuracy, m_SamplingAlgorithm);
                    m_PressureSolver.getPressureGradientField().resize(m_PressureGradientField);
                }
            }
            else if (m_AdvectionAlgorithm == EAdvectionAlgorithm.MacCormack)
            {
                if (m_BufferFlag)
                {
                    m_GridAdvectionSolver.advectMacCormack(m_PressureSolver.getPressureGradientField(), m_VelocityField2, vDeltaT, m_PressureGradientField, m_AdvectionAccuracy, m_SamplingAlgorithm);
                    m_PressureSolver.getPressureGradientField().resize(m_PressureGradientField);
                }
                else
                {
                    m_GridAdvectionSolver.advectMacCormack(m_PressureSolver.getPressureGradientField(), m_VelocityField1, vDeltaT, m_PressureGradientField, m_AdvectionAccuracy, m_SamplingAlgorithm);
                    m_PressureSolver.getPressureGradientField().resize(m_PressureGradientField);
                }
            }
            else if (m_AdvectionAlgorithm == EAdvectionAlgorithm.PIC || m_AdvectionAlgorithm == EAdvectionAlgorithm.FLIP || m_AdvectionAlgorithm == EAdvectionAlgorithm.MixPICAndFLIP)
            {

            }
            else
            {
                Debug.LogError("Invalid advection solver");
            }
        }
        private void __computeAdvection(float vDeltaT)
        {
            float FluidVel = 0.25f;
            if (m_AdvectionAlgorithm == EAdvectionAlgorithm.SemiLagrangian)
            {
                if (m_BufferFlag)
                {
                    //CEulerSolverToolInvoker.addFluidInvoker(m_FluidSDFField2.getResolution(), m_FluidSDFField2);
                    //CEulerSolverToolInvoker.removeFluidInvoker(m_FluidSDFField2.getResolution(), m_FluidSDFField2);
                    //CEulerSolverToolInvoker.setFluidVelInvoker(m_FluidSDFField2.getResolution(), m_VelocityField2, FluidVel);
                    m_GridAdvectionSolver.advect(m_VelocityField2, m_VelocityField2, vDeltaT, m_VelocityField1, m_AdvectionAccuracy, m_SamplingAlgorithm);
                    m_GridAdvectionSolver.advect(m_FluidSDFField2, m_VelocityField2, vDeltaT, m_FluidSDFField1, m_AdvectionAccuracy, m_SamplingAlgorithm);
                    //m_GridAdvectionSolver.advect(m_PressureSolver.getPressureField(), m_VelocityField2, 0.5f * vDeltaT, m_PressureField, m_AdvectionAccuracy, m_SamplingAlgorithm);
                    //m_PressureSolver.getPressureField().resize(m_PressureField);
                    //m_GridAdvectionSolver.advect(m_PressureSolver.getPressureGradientField(), m_VelocityField2, vDeltaT, m_PressureGradientField, m_AdvectionAccuracy, m_SamplingAlgorithm);
                    //m_PressureSolver.getPressureGradientField().resize(m_PressureGradientField);

                }
                else
                {
                    //CEulerSolverToolInvoker.addFluidInvoker(m_FluidSDFField1.getResolution(), m_FluidSDFField1);
                    //CEulerSolverToolInvoker.removeFluidInvoker(m_FluidSDFField1.getResolution(), m_FluidSDFField1);
                    //CEulerSolverToolInvoker.setFluidVelInvoker(m_FluidSDFField1.getResolution(), m_VelocityField1, FluidVel);
                    m_GridAdvectionSolver.advect(m_VelocityField1, m_VelocityField1, vDeltaT, m_VelocityField2, m_AdvectionAccuracy, m_SamplingAlgorithm);
                    m_GridAdvectionSolver.advect(m_FluidSDFField1, m_VelocityField1, vDeltaT, m_FluidSDFField2, m_AdvectionAccuracy, m_SamplingAlgorithm);
                    //m_GridAdvectionSolver.advect(m_PressureSolver.getPressureField(), m_VelocityField1, 0.5f * vDeltaT, m_PressureField, m_AdvectionAccuracy, m_SamplingAlgorithm);
                    //m_PressureSolver.getPressureField().resize(m_PressureField);
                    //m_GridAdvectionSolver.advect(m_PressureSolver.getPressureGradientField(), m_VelocityField1, vDeltaT, m_PressureGradientField, m_AdvectionAccuracy, m_SamplingAlgorithm);
                    //m_PressureSolver.getPressureGradientField().resize(m_PressureGradientField);
                }
            }
            else if (m_AdvectionAlgorithm == EAdvectionAlgorithm.MacCormack)
            {
                if (m_BufferFlag)
                {
                    //CEulerSolverToolInvoker.addFluidInvoker(m_FluidSDFField2.getResolution(), m_FluidSDFField2);
                    //CEulerSolverToolInvoker.removeFluidInvoker(m_FluidSDFField2.getResolution(), m_FluidSDFField2);
                    //CEulerSolverToolInvoker.setFluidVelInvoker(m_FluidSDFField2.getResolution(), m_VelocityField2, FluidVel);
                    m_GridAdvectionSolver.advectMacCormack(m_VelocityField2, m_VelocityField2, vDeltaT, m_VelocityField1, m_AdvectionAccuracy, m_SamplingAlgorithm);
                    m_GridAdvectionSolver.advectMacCormack(m_FluidSDFField2, m_VelocityField2, vDeltaT, m_FluidSDFField1, m_AdvectionAccuracy, m_SamplingAlgorithm);
                    //m_GridAdvectionSolver.advectMacCormack(m_PressureSolver.getPressureField(), m_VelocityField2, 0.5f * vDeltaT, m_PressureField, m_AdvectionAccuracy, m_SamplingAlgorithm);
                    //m_PressureSolver.getPressureField().resize(m_PressureField);
                    //m_GridAdvectionSolver.advectMacCormack(m_PressureSolver.getPressureGradientField(), m_VelocityField2, vDeltaT, m_PressureGradientField, m_AdvectionAccuracy, m_SamplingAlgorithm);
                    //m_PressureSolver.getPressureGradientField().resize(m_PressureGradientField);
                }
                else
                {
                    //CEulerSolverToolInvoker.addFluidInvoker(m_FluidSDFField1.getResolution(), m_FluidSDFField1);
                    //CEulerSolverToolInvoker.removeFluidInvoker(m_FluidSDFField1.getResolution(), m_FluidSDFField1);
                    //CEulerSolverToolInvoker.setFluidVelInvoker(m_FluidSDFField1.getResolution(), m_VelocityField1, FluidVel);
                    m_GridAdvectionSolver.advectMacCormack(m_VelocityField1, m_VelocityField1, vDeltaT, m_VelocityField2, m_AdvectionAccuracy, m_SamplingAlgorithm);
                    m_GridAdvectionSolver.advectMacCormack(m_FluidSDFField1, m_VelocityField1, vDeltaT, m_FluidSDFField2, m_AdvectionAccuracy, m_SamplingAlgorithm);
                    //m_GridAdvectionSolver.advectMacCormack(m_PressureSolver.getPressureField(), m_VelocityField1, 0.5f * vDeltaT, m_PressureField, m_AdvectionAccuracy, m_SamplingAlgorithm);
                    //m_PressureSolver.getPressureField().resize(m_PressureField);
                    //m_GridAdvectionSolver.advectMacCormack(m_PressureSolver.getPressureGradientField(), m_VelocityField1, vDeltaT, m_PressureGradientField, m_AdvectionAccuracy, m_SamplingAlgorithm);
                    //m_PressureSolver.getPressureGradientField().resize(m_PressureGradientField);
                }
            }
            else if (m_AdvectionAlgorithm == EAdvectionAlgorithm.Iteration)
            {
                if (m_BufferFlag)
                {
                    m_GridAdvectionSolver.advectWithIteration(m_FluidSDFField2, m_VelocityFieldPrevious, m_VelocityField2, vDeltaT, m_FluidSDFField1, m_AdvectionAccuracy, m_SamplingAlgorithm);
                    //m_GridAdvectionSolver.advectWithIteration(m_PressureSolver.getPressureField(), m_VelocityFieldPrevious, m_VelocityField2, vDeltaT, m_PressureField, m_AdvectionAccuracy, m_SamplingAlgorithm);
                    //m_PressureSolver.getPressureField().resize(m_PressureField);

                    m_GridAdvectionSolver.advectWithIteration(m_VelocityField2, m_VelocityFieldPrevious, m_VelocityField2, vDeltaT, m_VelocityField1, m_AdvectionAccuracy, m_SamplingAlgorithm);
                    m_VelocityFieldPrevious.resize(m_VelocityField2);
                }
                else
                {
                    m_GridAdvectionSolver.advectWithIteration(m_FluidSDFField1, m_VelocityFieldPrevious, m_VelocityField1, vDeltaT, m_FluidSDFField2, m_AdvectionAccuracy, m_SamplingAlgorithm);
                    //m_GridAdvectionSolver.advectWithIteration(m_PressureSolver.getPressureField(), m_VelocityFieldPrevious, m_VelocityField1, vDeltaT, m_PressureField, m_AdvectionAccuracy, m_SamplingAlgorithm);
                    //m_PressureSolver.getPressureField().resize(m_PressureField);

                    m_GridAdvectionSolver.advectWithIteration(m_VelocityField1, m_VelocityFieldPrevious, m_VelocityField1, vDeltaT, m_VelocityField2, m_AdvectionAccuracy, m_SamplingAlgorithm);
                    m_VelocityFieldPrevious.resize(m_VelocityField1);
                }
            }
            else if (m_AdvectionAlgorithm == EAdvectionAlgorithm.PIC || m_AdvectionAlgorithm == EAdvectionAlgorithm.FLIP || m_AdvectionAlgorithm == EAdvectionAlgorithm.MixPICAndFLIP)
            {
                if (m_BufferFlag)
                {
                    m_ParticleAdvectionSolver.getEulerParticles().transferVectorField2Particles(m_PressureSolver.getPressureGradientField(), m_PGTransferAlgorithm);
                    CMathTool.copy(m_ParticleAdvectionSolver.getEulerParticles().getParticlesPos(), m_ParticleAdvectionSolver.getEulerParticles().getParticlesTempPos());
                    m_ParticleAdvectionSolver.getEulerParticles().advectParticlesInVelFieldTemp(m_VelocityField2, 0.5f * vDeltaT, 1, m_AdvectionAccuracy, m_SamplingAlgorithm);
                    m_ParticleAdvectionSolver.getEulerParticles().transferParticlesVectorValue2Field(m_PressureSolver.getPressureGradientField(), m_CCVWeightField, m_PGTransferAlgorithm);


                    m_ParticleAdvectionSolver.getEulerParticles().transferScalarField2Particles(m_PressureSolver.getPressureField(), m_PGTransferAlgorithm);
                    m_ParticleAdvectionSolver.advect(m_VelocityField2, m_VelocityField2, vDeltaT, m_VelocityField1, m_PGTransferAlgorithm, m_AdvectionAccuracy, m_SamplingAlgorithm);
                    m_ParticleAdvectionSolver.getEulerParticles().transferParticlesScalarValue2Field(m_PressureSolver.getPressureField(), m_CCSWeightField, m_PGTransferAlgorithm);

                }
                else
                {
                    m_ParticleAdvectionSolver.getEulerParticles().transferVectorField2Particles(m_PressureSolver.getPressureGradientField(), m_PGTransferAlgorithm);
                    CMathTool.copy(m_ParticleAdvectionSolver.getEulerParticles().getParticlesPos(), m_ParticleAdvectionSolver.getEulerParticles().getParticlesTempPos());
                    m_ParticleAdvectionSolver.getEulerParticles().advectParticlesInVelFieldTemp(m_VelocityField1, 0.5f * vDeltaT, 1, m_AdvectionAccuracy, m_SamplingAlgorithm);
                    m_ParticleAdvectionSolver.getEulerParticles().transferParticlesVectorValue2Field(m_PressureSolver.getPressureGradientField(), m_CCVWeightField, m_PGTransferAlgorithm);

                    m_ParticleAdvectionSolver.getEulerParticles().transferScalarField2Particles(m_PressureSolver.getPressureField(), m_PGTransferAlgorithm);
                    m_ParticleAdvectionSolver.advect(m_VelocityField1, m_VelocityField1, vDeltaT, m_VelocityField2, m_PGTransferAlgorithm, m_AdvectionAccuracy, m_SamplingAlgorithm);
                    m_ParticleAdvectionSolver.getEulerParticles().transferParticlesScalarValue2Field(m_PressureSolver.getPressureField(), m_CCSWeightField, m_PGTransferAlgorithm);
                }

                m_ParticleAdvectionSolver.getEulerParticles().deleteOutsideParticles(m_GridResolution, m_GridOrigin, m_GridSpacing); ;
            }
            else
            {
                Debug.LogError("Invalid advection solver");
            }
        }
        private void __computeAdvection_extrapolated(float vDeltaT)
        {
            if (m_AdvectionAlgorithm == EAdvectionAlgorithm.SemiLagrangian)
            {
                if (m_BufferFlag)
                {
                    m_TempVelocityField2.resize(m_VelocityField2);
                    m_TempVelocityField2.scale(1.5f);
                    m_TempVelocityField2.plusAlphaX(m_VelocityFieldPrevious, -0.5f);

                    m_GridAdvectionSolver.advect(m_FluidDensityField1, m_TempVelocityField2, vDeltaT, m_FluidTempScalarField, m_AdvectionAccuracy, m_SamplingAlgorithm);
                    m_FluidDensityField1.resize(m_FluidTempScalarField);
                    m_GridAdvectionSolver.advect(m_FluidDensityField2, m_TempVelocityField2, vDeltaT, m_FluidTempScalarField, m_AdvectionAccuracy, m_SamplingAlgorithm);
                    m_FluidDensityField2.resize(m_FluidTempScalarField);
                    m_GridAdvectionSolver.advect(m_FluidTemperatureField, m_TempVelocityField2, vDeltaT, m_FluidTempScalarField, m_AdvectionAccuracy, m_SamplingAlgorithm);
                    m_FluidTemperatureField.resize(m_FluidTempScalarField);

                    m_GridAdvectionSolver.advect(m_VelocityField2, m_TempVelocityField2, vDeltaT, m_VelocityField1, m_AdvectionAccuracy, m_SamplingAlgorithm);
                    m_GridAdvectionSolver.advect(m_FluidSDFField2, m_TempVelocityField2, vDeltaT, m_FluidSDFField1, m_AdvectionAccuracy, m_SamplingAlgorithm);
                    m_VelocityFieldPrevious.resize(m_VelocityField2);
                }
                else
                {
                    m_TempVelocityField1.resize(m_VelocityField1);
                    m_TempVelocityField1.scale(1.5f);
                    m_TempVelocityField1.plusAlphaX(m_VelocityFieldPrevious, -0.5f);

                    m_GridAdvectionSolver.advect(m_FluidDensityField1, m_TempVelocityField1, vDeltaT, m_FluidTempScalarField, m_AdvectionAccuracy, m_SamplingAlgorithm);
                    m_FluidDensityField1.resize(m_FluidTempScalarField);
                    m_GridAdvectionSolver.advect(m_FluidDensityField2, m_TempVelocityField1, vDeltaT, m_FluidTempScalarField, m_AdvectionAccuracy, m_SamplingAlgorithm);
                    m_FluidDensityField2.resize(m_FluidTempScalarField);
                    m_GridAdvectionSolver.advect(m_FluidTemperatureField, m_TempVelocityField1, vDeltaT, m_FluidTempScalarField, m_AdvectionAccuracy, m_SamplingAlgorithm);
                    m_FluidTemperatureField.resize(m_FluidTempScalarField);

                    m_GridAdvectionSolver.advect(m_VelocityField1, m_TempVelocityField1, vDeltaT, m_VelocityField2, m_AdvectionAccuracy, m_SamplingAlgorithm);
                    m_GridAdvectionSolver.advect(m_FluidSDFField1, m_TempVelocityField1, vDeltaT, m_FluidSDFField2, m_AdvectionAccuracy, m_SamplingAlgorithm);
                    m_VelocityFieldPrevious.resize(m_VelocityField1);
                }
            }
            else if (m_AdvectionAlgorithm == EAdvectionAlgorithm.MacCormack)
            {
                if (m_BufferFlag)
                {
                    m_TempVelocityField2.resize(m_VelocityField2);
                    m_TempVelocityField2.scale(1.5f);
                    m_TempVelocityField2.plusAlphaX(m_VelocityFieldPrevious, -0.5f);

                    m_GridAdvectionSolver.advectMacCormack(m_FluidDensityField1, m_TempVelocityField2, vDeltaT, m_FluidTempScalarField, m_AdvectionAccuracy, m_SamplingAlgorithm);
                    m_FluidDensityField1.resize(m_FluidTempScalarField);
                    m_GridAdvectionSolver.advectMacCormack(m_FluidDensityField2, m_TempVelocityField2, vDeltaT, m_FluidTempScalarField, m_AdvectionAccuracy, m_SamplingAlgorithm);
                    m_FluidDensityField2.resize(m_FluidTempScalarField);
                    m_GridAdvectionSolver.advectMacCormack(m_FluidTemperatureField, m_TempVelocityField2, vDeltaT, m_FluidTempScalarField, m_AdvectionAccuracy, m_SamplingAlgorithm);
                    m_FluidTemperatureField.resize(m_FluidTempScalarField);

                    m_GridAdvectionSolver.advectMacCormack(m_VelocityField2, m_TempVelocityField2, vDeltaT, m_VelocityField1, m_AdvectionAccuracy, m_SamplingAlgorithm);
                    m_GridAdvectionSolver.advectMacCormack(m_FluidSDFField2, m_TempVelocityField2, vDeltaT, m_FluidSDFField1, m_AdvectionAccuracy, m_SamplingAlgorithm);
                    m_VelocityFieldPrevious.resize(m_VelocityField2);
                }
                else
                {
                    m_TempVelocityField1.resize(m_VelocityField1);
                    m_TempVelocityField1.scale(1.5f);
                    m_TempVelocityField1.plusAlphaX(m_VelocityFieldPrevious, -0.5f);

                    m_GridAdvectionSolver.advectMacCormack(m_FluidDensityField1, m_TempVelocityField1, vDeltaT, m_FluidTempScalarField, m_AdvectionAccuracy, m_SamplingAlgorithm);
                    m_FluidDensityField1.resize(m_FluidTempScalarField);
                    m_GridAdvectionSolver.advectMacCormack(m_FluidDensityField2, m_TempVelocityField1, vDeltaT, m_FluidTempScalarField, m_AdvectionAccuracy, m_SamplingAlgorithm);
                    m_FluidDensityField2.resize(m_FluidTempScalarField);
                    m_GridAdvectionSolver.advectMacCormack(m_FluidTemperatureField, m_TempVelocityField1, vDeltaT, m_FluidTempScalarField, m_AdvectionAccuracy, m_SamplingAlgorithm);
                    m_FluidTemperatureField.resize(m_FluidTempScalarField);

                    m_GridAdvectionSolver.advectMacCormack(m_VelocityField1, m_TempVelocityField1, vDeltaT, m_VelocityField2, m_AdvectionAccuracy, m_SamplingAlgorithm);
                    m_GridAdvectionSolver.advectMacCormack(m_FluidSDFField1, m_TempVelocityField1, vDeltaT, m_FluidSDFField2, m_AdvectionAccuracy, m_SamplingAlgorithm);
                    m_VelocityFieldPrevious.resize(m_VelocityField1);
                }
            }
            else
            {
                Debug.LogError("Invalid advection solver");
            }
        }
        private void __computeAdvection_extrapolatedPressure(float vDeltaT)
        {
            float FluidVel = 0.05f;
            if (m_AdvectionAlgorithm == EAdvectionAlgorithm.SemiLagrangian)
            {
                if (m_BufferFlag)
                {
                    m_TempVelocityField2.resize(m_VelocityField2);
                    m_TempVelocityField2.scale(1.5f);
                    m_TempVelocityField2.plusAlphaX(m_VelocityFieldPrevious, -0.5f);

                    m_GridAdvectionSolver.advect(m_FluidDensityField1, m_VelocityField2, vDeltaT, m_FluidTempScalarField, m_AdvectionAccuracy, m_SamplingAlgorithm);
                    m_FluidDensityField1.resize(m_FluidTempScalarField);
                    m_GridAdvectionSolver.advect(m_FluidDensityField2, m_VelocityField2, vDeltaT, m_FluidTempScalarField, m_AdvectionAccuracy, m_SamplingAlgorithm);
                    m_FluidDensityField2.resize(m_FluidTempScalarField);
                    m_GridAdvectionSolver.advect(m_FluidTemperatureField, m_VelocityField2, vDeltaT, m_FluidTempScalarField, m_AdvectionAccuracy, m_SamplingAlgorithm);
                    m_FluidTemperatureField.resize(m_FluidTempScalarField);

                    m_GridAdvectionSolver.advect(m_VelocityField2, m_VelocityField2, vDeltaT, m_VelocityField1, m_AdvectionAccuracy, m_SamplingAlgorithm);
                    //m_GridAdvectionSolver.advect(m_FluidSDFField2, m_TempVelocityField2, vDeltaT, m_FluidSDFField1, m_AdvectionAccuracy, m_SamplingAlgorithm);
                    m_GridAdvectionSolver.advect(m_PressureSolver.getPressureField(), m_TempVelocityField2, vDeltaT, m_PressureField, m_AdvectionAccuracy, m_SamplingAlgorithm);
                    m_PressureSolver.getPressureField().resize(m_PressureField);
                    m_VelocityFieldPrevious.resize(m_VelocityField2);
                }
                else
                {
                    m_TempVelocityField1.resize(m_VelocityField1);
                    m_TempVelocityField1.scale(1.5f);
                    m_TempVelocityField1.plusAlphaX(m_VelocityFieldPrevious, -0.5f);

                    m_GridAdvectionSolver.advect(m_FluidDensityField1, m_VelocityField1, vDeltaT, m_FluidTempScalarField, m_AdvectionAccuracy, m_SamplingAlgorithm);
                    m_FluidDensityField1.resize(m_FluidTempScalarField);
                    m_GridAdvectionSolver.advect(m_FluidDensityField2, m_VelocityField1, vDeltaT, m_FluidTempScalarField, m_AdvectionAccuracy, m_SamplingAlgorithm);
                    m_FluidDensityField2.resize(m_FluidTempScalarField);
                    m_GridAdvectionSolver.advect(m_FluidTemperatureField, m_VelocityField1, vDeltaT, m_FluidTempScalarField, m_AdvectionAccuracy, m_SamplingAlgorithm);
                    m_FluidTemperatureField.resize(m_FluidTempScalarField);

                    m_GridAdvectionSolver.advect(m_VelocityField1, m_VelocityField1, vDeltaT, m_VelocityField2, m_AdvectionAccuracy, m_SamplingAlgorithm);
                    //m_GridAdvectionSolver.advect(m_FluidSDFField1, m_TempVelocityField1, vDeltaT, m_FluidSDFField2, m_AdvectionAccuracy, m_SamplingAlgorithm);
                    m_GridAdvectionSolver.advect(m_PressureSolver.getPressureField(), m_TempVelocityField1, vDeltaT, m_PressureField, m_AdvectionAccuracy, m_SamplingAlgorithm);
                    m_PressureSolver.getPressureField().resize(m_PressureField);
                    m_VelocityFieldPrevious.resize(m_VelocityField1);
                }
            }
            else if (m_AdvectionAlgorithm == EAdvectionAlgorithm.MacCormack)
            {
                if (m_BufferFlag)
                {
                    m_TempVelocityField2.resize(m_VelocityField2);
                    m_TempVelocityField2.scale(1.5f);
                    m_TempVelocityField2.plusAlphaX(m_VelocityFieldPrevious, -0.5f);

                    m_GridAdvectionSolver.advectMacCormack(m_FluidDensityField1, m_VelocityField2, vDeltaT, m_FluidTempScalarField, m_AdvectionAccuracy, m_SamplingAlgorithm);
                    m_FluidDensityField1.resize(m_FluidTempScalarField);
                    m_GridAdvectionSolver.advectMacCormack(m_FluidDensityField2, m_VelocityField2, vDeltaT, m_FluidTempScalarField, m_AdvectionAccuracy, m_SamplingAlgorithm);
                    m_FluidDensityField2.resize(m_FluidTempScalarField);
                    m_GridAdvectionSolver.advectMacCormack(m_FluidTemperatureField, m_VelocityField2, vDeltaT, m_FluidTempScalarField, m_AdvectionAccuracy, m_SamplingAlgorithm);
                    m_FluidTemperatureField.resize(m_FluidTempScalarField);

                    m_GridAdvectionSolver.advectMacCormack(m_VelocityField2, m_VelocityField2, vDeltaT, m_VelocityField1, m_AdvectionAccuracy, m_SamplingAlgorithm);
                    //m_GridAdvectionSolver.advectMacCormack(m_FluidSDFField2, m_TempVelocityField2, vDeltaT, m_FluidSDFField1, m_AdvectionAccuracy, m_SamplingAlgorithm);
                    m_GridAdvectionSolver.advectMacCormack(m_PressureSolver.getPressureField(), m_TempVelocityField2, vDeltaT, m_PressureField, m_AdvectionAccuracy, m_SamplingAlgorithm);
                    m_PressureSolver.getPressureField().resize(m_PressureField);
                    m_VelocityFieldPrevious.resize(m_VelocityField2);
                }
                else
                {
                    m_TempVelocityField1.resize(m_VelocityField1);
                    m_TempVelocityField1.scale(1.5f);
                    m_TempVelocityField1.plusAlphaX(m_VelocityFieldPrevious, -0.5f);

                    m_GridAdvectionSolver.advectMacCormack(m_FluidDensityField1, m_VelocityField1, vDeltaT, m_FluidTempScalarField, m_AdvectionAccuracy, m_SamplingAlgorithm);
                    m_FluidDensityField1.resize(m_FluidTempScalarField);
                    m_GridAdvectionSolver.advectMacCormack(m_FluidDensityField2, m_VelocityField1, vDeltaT, m_FluidTempScalarField, m_AdvectionAccuracy, m_SamplingAlgorithm);
                    m_FluidDensityField2.resize(m_FluidTempScalarField);
                    m_GridAdvectionSolver.advectMacCormack(m_FluidTemperatureField, m_VelocityField1, vDeltaT, m_FluidTempScalarField, m_AdvectionAccuracy, m_SamplingAlgorithm);
                    m_FluidTemperatureField.resize(m_FluidTempScalarField);

                    m_GridAdvectionSolver.advectMacCormack(m_VelocityField1, m_VelocityField1, vDeltaT, m_VelocityField2, m_AdvectionAccuracy, m_SamplingAlgorithm);
                    //m_GridAdvectionSolver.advectMacCormack(m_FluidSDFField1, m_TempVelocityField1, vDeltaT, m_FluidSDFField2, m_AdvectionAccuracy, m_SamplingAlgorithm);
                    m_GridAdvectionSolver.advectMacCormack(m_PressureSolver.getPressureField(), m_TempVelocityField1, vDeltaT, m_PressureField, m_AdvectionAccuracy, m_SamplingAlgorithm);
                    m_PressureSolver.getPressureField().resize(m_PressureField);
                    m_VelocityFieldPrevious.resize(m_VelocityField1);
                }
            }
            else
            {
                Debug.LogError("Invalid advection solver");
            }
        }
        private void __computeAdvectionSmoke(float vDeltaT)
        {
            if (m_AdvectionAlgorithm == EAdvectionAlgorithm.SemiLagrangian)
            {
                if (m_BufferFlag)
                {
                    m_GridAdvectionSolver.advect(m_FluidDensityField1, m_VelocityField2, vDeltaT, m_FluidTempScalarField, m_AdvectionAccuracy, m_SamplingAlgorithm);
                    m_FluidDensityField1.resize(m_FluidTempScalarField);
                    m_GridAdvectionSolver.advect(m_FluidDensityField2, m_VelocityField2, vDeltaT, m_FluidTempScalarField, m_AdvectionAccuracy, m_SamplingAlgorithm);
                    m_FluidDensityField2.resize(m_FluidTempScalarField);
                    m_GridAdvectionSolver.advect(m_FluidTemperatureField, m_VelocityField2, vDeltaT, m_FluidTempScalarField, m_AdvectionAccuracy, m_SamplingAlgorithm);
                    m_FluidTemperatureField.resize(m_FluidTempScalarField);
                    m_GridAdvectionSolver.advect(m_VelocityField2, m_VelocityField2, vDeltaT, m_VelocityField1, m_AdvectionAccuracy, m_SamplingAlgorithm);
                }
                else
                {
                    m_GridAdvectionSolver.advect(m_FluidDensityField1, m_VelocityField1, vDeltaT, m_FluidTempScalarField, m_AdvectionAccuracy, m_SamplingAlgorithm);
                    m_FluidDensityField1.resize(m_FluidTempScalarField);
                    m_GridAdvectionSolver.advect(m_FluidDensityField2, m_VelocityField1, vDeltaT, m_FluidTempScalarField, m_AdvectionAccuracy, m_SamplingAlgorithm);
                    m_FluidDensityField2.resize(m_FluidTempScalarField);
                    m_GridAdvectionSolver.advect(m_FluidTemperatureField, m_VelocityField1, vDeltaT, m_FluidTempScalarField, m_AdvectionAccuracy, m_SamplingAlgorithm);
                    m_FluidTemperatureField.resize(m_FluidTempScalarField);
                    m_GridAdvectionSolver.advect(m_VelocityField1, m_VelocityField1, vDeltaT, m_VelocityField2, m_AdvectionAccuracy, m_SamplingAlgorithm);
                }
            }
            else if (m_AdvectionAlgorithm == EAdvectionAlgorithm.MacCormack)
            {
                if (m_BufferFlag)
                {
                    m_GridAdvectionSolver.advectMacCormack(m_FluidDensityField1, m_VelocityField2, vDeltaT, m_FluidTempScalarField, m_AdvectionAccuracy, m_SamplingAlgorithm);
                    m_FluidDensityField1.resize(m_FluidTempScalarField);
                    m_GridAdvectionSolver.advectMacCormack(m_FluidDensityField2, m_VelocityField2, vDeltaT, m_FluidTempScalarField, m_AdvectionAccuracy, m_SamplingAlgorithm);
                    m_FluidDensityField2.resize(m_FluidTempScalarField);
                    m_GridAdvectionSolver.advectMacCormack(m_FluidTemperatureField, m_VelocityField2, vDeltaT, m_FluidTempScalarField, m_AdvectionAccuracy, m_SamplingAlgorithm);
                    m_FluidTemperatureField.resize(m_FluidTempScalarField);
                    m_GridAdvectionSolver.advectMacCormack(m_VelocityField2, m_VelocityField2, vDeltaT, m_VelocityField1, m_AdvectionAccuracy, m_SamplingAlgorithm);
                }
                else
                {
                    m_GridAdvectionSolver.advectMacCormack(m_FluidDensityField1, m_VelocityField1, vDeltaT, m_FluidTempScalarField, m_AdvectionAccuracy, m_SamplingAlgorithm);
                    m_FluidDensityField1.resize(m_FluidTempScalarField);
                    m_GridAdvectionSolver.advectMacCormack(m_FluidDensityField2, m_VelocityField1, vDeltaT, m_FluidTempScalarField, m_AdvectionAccuracy, m_SamplingAlgorithm);
                    m_FluidDensityField2.resize(m_FluidTempScalarField);
                    m_GridAdvectionSolver.advectMacCormack(m_FluidTemperatureField, m_VelocityField1, vDeltaT, m_FluidTempScalarField, m_AdvectionAccuracy, m_SamplingAlgorithm);
                    m_FluidTemperatureField.resize(m_FluidTempScalarField);
                    m_GridAdvectionSolver.advectMacCormack(m_VelocityField1, m_VelocityField1, vDeltaT, m_VelocityField2, m_AdvectionAccuracy, m_SamplingAlgorithm);
                }
            }
            else if (m_AdvectionAlgorithm == EAdvectionAlgorithm.Iteration)
            {
                if (m_BufferFlag)
                {
                    m_GridAdvectionSolver.advectWithIteration(m_FluidDensityField1, m_VelocityField1, m_VelocityField2, vDeltaT, m_FluidTempScalarField, m_AdvectionAccuracy, m_SamplingAlgorithm);
                    m_FluidDensityField1.resize(m_FluidTempScalarField);
                    m_GridAdvectionSolver.advectWithIteration(m_FluidDensityField2, m_VelocityField1, m_VelocityField2, vDeltaT, m_FluidTempScalarField, m_AdvectionAccuracy, m_SamplingAlgorithm);
                    m_FluidDensityField2.resize(m_FluidTempScalarField);
                    m_GridAdvectionSolver.advectWithIteration(m_FluidTemperatureField, m_VelocityField1, m_VelocityField2, vDeltaT, m_FluidTempScalarField, m_AdvectionAccuracy, m_SamplingAlgorithm);
                    m_FluidTemperatureField.resize(m_FluidTempScalarField);
                    m_GridAdvectionSolver.advectWithIteration(m_VelocityField2, m_VelocityField1, m_VelocityField2, vDeltaT, m_VelocityField1, m_AdvectionAccuracy, m_SamplingAlgorithm);
                }
                else
                {
                    m_GridAdvectionSolver.advectWithIteration(m_FluidDensityField1, m_VelocityField2, m_VelocityField1, vDeltaT, m_FluidTempScalarField, m_AdvectionAccuracy, m_SamplingAlgorithm);
                    m_FluidDensityField1.resize(m_FluidTempScalarField);
                    m_GridAdvectionSolver.advectWithIteration(m_FluidDensityField2, m_VelocityField2, m_VelocityField1, vDeltaT, m_FluidTempScalarField, m_AdvectionAccuracy, m_SamplingAlgorithm);
                    m_FluidDensityField2.resize(m_FluidTempScalarField);
                    m_GridAdvectionSolver.advectWithIteration(m_FluidTemperatureField, m_VelocityField2, m_VelocityField1, vDeltaT, m_FluidTempScalarField, m_AdvectionAccuracy, m_SamplingAlgorithm);
                    m_FluidTemperatureField.resize(m_FluidTempScalarField);
                    m_GridAdvectionSolver.advectWithIteration(m_VelocityField1, m_VelocityField2, m_VelocityField1, vDeltaT, m_VelocityField2, m_AdvectionAccuracy, m_SamplingAlgorithm);
                }
            }
            else
            {
                Debug.LogError("Invalid advection solver");
            }
        }
        private void __emitteSmoke(float vDeltaT)
        {
            if (m_SmokeEmitterFlag == ESmokeScene.TwoDSmoke)
            {
                m_TwoDSmokeEmitter.emitteSmoke(m_CurSimulationFrame, vDeltaT, m_FluidDensityField1, m_FluidTemperatureField, getVelocityFieldBeforePressure());
            }
            else if (m_SmokeEmitterFlag == ESmokeScene.VortexCollision)
            {
                m_VortexCollisionSmokeEmitter.emitteSmoke(m_CurSimulationFrame, vDeltaT, m_FluidDensityField1, m_FluidDensityField2, m_FluidTemperatureField, getVelocityFieldBeforePressure());
            }
            else if (m_SmokeEmitterFlag == ESmokeScene.VortexLeapFrogging)
            {
                m_VortexLeapFroggingSmokeEmitter.emitteSmoke(m_CurSimulationFrame, vDeltaT, m_FluidDensityField1, m_FluidDensityField2, m_FluidTemperatureField, getVelocityFieldBeforePressure());
            }
            else if(m_SmokeEmitterFlag == ESmokeScene.ThreeDSmoke)
            {
                m_ThreeDSmokeEmitter.emitteSmoke(m_CurSimulationFrame, vDeltaT, m_FluidDensityField1, m_FluidTemperatureField);
            }
        }
        private void __computeExternalForces(float vDeltaT)
        {
            if (m_BufferFlag)
            {
                m_ExternalForcesSolver.applyExternalForces(m_VelocityField1, vDeltaT);
            }
            else
            {
                m_ExternalForcesSolver.applyExternalForces(m_VelocityField2, vDeltaT);
            }
        }
        private void __computeViscosity(float vDeltaT)
        {
            if (m_IsViscous)
            {
                if (m_BufferFlag)
                {
                    m_ViscositySolver.applyViscosityForces(vDeltaT, m_VelocityField1);
                }
                else
                {
                    m_ViscositySolver.applyViscosityForces(vDeltaT, m_VelocityField2);
                }
            }
            else
            {

            }
        }

        private void __computeBuoyancy(float vDeltaT)
        {
            if (m_HasBuoyancy)
            {
                if (m_BufferFlag)
                {
                    //Debug.Log(CMathTool.getAbsMaxValue(m_VelocityField1.getGridDataY()));
                    m_BuoyancySolver.applyBuoyancy(vDeltaT, m_FluidDensityField1 ,m_FluidTemperatureField, m_VelocityField1);
                    //Debug.Log(CMathTool.getAbsMaxValue(m_VelocityField1.getGridDataY()));
                }
                else
                {
                    //Debug.Log(CMathTool.getAbsMaxValue(m_VelocityField1.getGridDataY()));
                    m_BuoyancySolver.applyBuoyancy(vDeltaT, m_FluidDensityField1, m_FluidTemperatureField, m_VelocityField2);
                    //Debug.Log(CMathTool.getAbsMaxValue(m_VelocityField1.getGridDataY()));
                }
            }
            else
            {

            }
        }

        private void __computePressureWithBackTrace(float vDeltaT)
        {
            if (m_AdvectionAlgorithm == EAdvectionAlgorithm.SemiLagrangian || m_AdvectionAlgorithm == EAdvectionAlgorithm.MacCormack || m_AdvectionAlgorithm == EAdvectionAlgorithm.Iteration)
            {
                if (m_BufferFlag)
                {
                    m_GridAdvectionSolver.fillBackTraceMiddlePosField(vDeltaT, m_VelocityField2, EAdvectionAccuracy.RK3, m_SamplingAlgorithm);
                    m_PressureSolver.applyPressureGradient
                    (
                        vDeltaT,
                        m_GridAdvectionSolver.getBackTraceMidPointPosFieldX(),
                        m_GridAdvectionSolver.getBackTraceMidPointPosFieldY(),
                        m_GridAdvectionSolver.getBackTraceMidPointPosFieldZ(),
                        getSolidSDFField(),
                        m_FluidSDFField1,
                        getSolidVelField(),
                        m_VelocityField1,
                        m_SamplingAlgorithm
                    );
                }
                else
                {
                    m_GridAdvectionSolver.fillBackTraceMiddlePosField(vDeltaT, m_VelocityField1, EAdvectionAccuracy.RK3, m_SamplingAlgorithm);
                    m_PressureSolver.applyPressureGradient
                    (
                        vDeltaT,
                        m_GridAdvectionSolver.getBackTraceMidPointPosFieldX(),
                        m_GridAdvectionSolver.getBackTraceMidPointPosFieldY(),
                        m_GridAdvectionSolver.getBackTraceMidPointPosFieldZ(),
                        getSolidSDFField(),
                        m_FluidSDFField2,
                        getSolidVelField(),
                        m_VelocityField2,
                        m_SamplingAlgorithm
                    );
                }
            }
            else if (m_AdvectionAlgorithm == EAdvectionAlgorithm.PIC || m_AdvectionAlgorithm == EAdvectionAlgorithm.FLIP || m_AdvectionAlgorithm == EAdvectionAlgorithm.MixPICAndFLIP)
            {
                if (m_BufferFlag)
                {
                    m_GridAdvectionSolver.fillBackTraceMiddlePosField(vDeltaT, m_VelocityField2, EAdvectionAccuracy.RK3, m_SamplingAlgorithm);
                    m_PressureSolver.applyPressureGradient
                    (
                        vDeltaT,
                        m_GridAdvectionSolver.getBackTraceMidPointPosFieldX(),
                        m_GridAdvectionSolver.getBackTraceMidPointPosFieldY(),
                        m_GridAdvectionSolver.getBackTraceMidPointPosFieldZ(),
                        getSolidSDFField(),
                        m_ParticleAdvectionSolver.getEulerParticles().getParticlesPos(),
                        getSolidVelField(),
                        m_VelocityField1,
                        m_SamplingAlgorithm
                    );
                }
                else
                {
                    m_GridAdvectionSolver.fillBackTraceMiddlePosField(vDeltaT, m_VelocityField1, EAdvectionAccuracy.RK3, m_SamplingAlgorithm);
                    m_PressureSolver.applyPressureGradient
                    (
                        vDeltaT,
                        m_GridAdvectionSolver.getBackTraceMidPointPosFieldX(),
                        m_GridAdvectionSolver.getBackTraceMidPointPosFieldY(),
                        m_GridAdvectionSolver.getBackTraceMidPointPosFieldZ(),
                        getSolidSDFField(),
                        m_ParticleAdvectionSolver.getEulerParticles().getParticlesPos(),
                        getSolidVelField(),
                        m_VelocityField2,
                        m_SamplingAlgorithm
                    );
                }
            }
            else
            {
                Debug.LogError("Invalid advection solver");
            }
        }
        private void __computePressure_NoBackTrace(float vDeltaT)
        {
            if (m_AdvectionAlgorithm == EAdvectionAlgorithm.SemiLagrangian || m_AdvectionAlgorithm == EAdvectionAlgorithm.MacCormack || m_AdvectionAlgorithm == EAdvectionAlgorithm.Iteration)
            {
                if (m_BufferFlag)
                {
                    m_PressureSolver.applyPressureGradientTest
                    (
                        vDeltaT,
                        m_GridAdvectionSolver.getBackTraceStartPointPosFieldX(),
                        m_GridAdvectionSolver.getBackTraceStartPointPosFieldY(),
                        m_GridAdvectionSolver.getBackTraceStartPointPosFieldZ(),
                        getSolidSDFField(),
                        m_FluidSDFField1,
                        getSolidVelField(),
                        m_VelocityField1,
                        m_SamplingAlgorithm
                    );
                }
                else
                {
                    m_PressureSolver.applyPressureGradientTest
                    (
                        vDeltaT,
                        m_GridAdvectionSolver.getBackTraceStartPointPosFieldX(),
                        m_GridAdvectionSolver.getBackTraceStartPointPosFieldY(),
                        m_GridAdvectionSolver.getBackTraceStartPointPosFieldZ(),
                        getSolidSDFField(),
                        m_FluidSDFField2,
                        getSolidVelField(),
                        m_VelocityField2,
                        m_SamplingAlgorithm
                    );
                }
            }
            else if (m_AdvectionAlgorithm == EAdvectionAlgorithm.PIC || m_AdvectionAlgorithm == EAdvectionAlgorithm.FLIP || m_AdvectionAlgorithm == EAdvectionAlgorithm.MixPICAndFLIP)
            {
                if (m_BufferFlag)
                {
                    m_GridAdvectionSolver.fillBackTraceMiddlePosField(vDeltaT, m_VelocityField2, EAdvectionAccuracy.RK3, m_SamplingAlgorithm);
                    m_PressureSolver.applyPressureGradient
                    (
                        vDeltaT,
                        m_GridAdvectionSolver.getBackTraceStartPointPosFieldX(),
                        m_GridAdvectionSolver.getBackTraceStartPointPosFieldY(),
                        m_GridAdvectionSolver.getBackTraceStartPointPosFieldZ(),
                        getSolidSDFField(),
                        m_ParticleAdvectionSolver.getEulerParticles().getParticlesPos(),
                        getSolidVelField(),
                        m_VelocityField1,
                        m_SamplingAlgorithm
                    );
                }
                else
                {
                    m_GridAdvectionSolver.fillBackTraceMiddlePosField(vDeltaT, m_VelocityField1, EAdvectionAccuracy.RK3, m_SamplingAlgorithm);
                    m_PressureSolver.applyPressureGradient
                    (
                        vDeltaT,
                        m_GridAdvectionSolver.getBackTraceStartPointPosFieldX(),
                        m_GridAdvectionSolver.getBackTraceStartPointPosFieldY(),
                        m_GridAdvectionSolver.getBackTraceStartPointPosFieldZ(),
                        getSolidSDFField(),
                        m_ParticleAdvectionSolver.getEulerParticles().getParticlesPos(),
                        getSolidVelField(),
                        m_VelocityField2,
                        m_SamplingAlgorithm
                    );
                }
            }
            else
            {
                Debug.LogError("Invalid advection solver");
            }
        }
        private void __executeHelmholtzHodgDecomposition(float vDeltaT)
        {
            if (m_AdvectionAlgorithm == EAdvectionAlgorithm.SemiLagrangian || m_AdvectionAlgorithm == EAdvectionAlgorithm.MacCormack || m_AdvectionAlgorithm == EAdvectionAlgorithm.Iteration)
            {
                if (m_BufferFlag)
                {
                    m_PressureSolver.executeHelmholtzHodgDecomposition
                    (
                        m_VelocityField1,
                        vDeltaT,
                        getSolidVelField(),
                        getSolidSDFField(),
                        m_FluidSDFField1
                    );
                }
                else
                {
                    m_PressureSolver.executeHelmholtzHodgDecomposition
                    (
                        m_VelocityField2,
                        vDeltaT,
                        getSolidVelField(),
                        getSolidSDFField(),
                        m_FluidSDFField2
                    );
                }
            }
            else if (m_AdvectionAlgorithm == EAdvectionAlgorithm.PIC || m_AdvectionAlgorithm == EAdvectionAlgorithm.FLIP || m_AdvectionAlgorithm == EAdvectionAlgorithm.MixPICAndFLIP)
            {
                if (m_BufferFlag)
                {
                    m_PressureSolver.executeHelmholtzHodgDecomposition
                    (
                        m_VelocityField1,
                        vDeltaT,
                        getSolidVelField(),
                        getSolidSDFField(),
                        m_ParticleAdvectionSolver.getEulerParticles().getParticlesPos(),
                        m_Emitter
                    );
                }
                else
                {
                    m_PressureSolver.executeHelmholtzHodgDecomposition
                    (
                        m_VelocityField2,
                        vDeltaT,
                        getSolidVelField(),
                        getSolidSDFField(),
                        m_ParticleAdvectionSolver.getEulerParticles().getParticlesPos(),
                        m_Emitter
                    );
                }
            }
            else
            {
                Debug.LogError("Invalid advection solver");
            }
        }
        private void __executeHelmholtzHodgDecomposition_BDF2(float vDeltaT)
        {
            if (m_AdvectionAlgorithm == EAdvectionAlgorithm.SemiLagrangian || m_AdvectionAlgorithm == EAdvectionAlgorithm.MacCormack || m_AdvectionAlgorithm == EAdvectionAlgorithm.Iteration)
            {
                if (m_BufferFlag)
                {
                    m_TempVelocityField3.resize(m_VelocityField1);
                    m_TempVelocityField3.scale(4.0f / 3.0f);
                    m_TempVelocityField3.plusAlphaX(m_TempVelocityField1, -1.0f / 3.0f);
                    m_PressureSolver.executeHelmholtzHodgDecomposition
                    (
                        m_TempVelocityField3,
                        vDeltaT,
                        getSolidVelField(),
                        getSolidSDFField(),
                        m_FluidSDFField1
                    );
                    m_VelocityField1.resize(m_TempVelocityField3);
                }
                else
                {
                    m_TempVelocityField3.resize(m_VelocityField2);
                    m_TempVelocityField3.scale(4.0f / 3.0f);
                    m_TempVelocityField3.plusAlphaX(m_TempVelocityField1, -1.0f / 3.0f);
                    m_PressureSolver.executeHelmholtzHodgDecomposition
                    (
                        m_TempVelocityField3,
                        vDeltaT,
                        getSolidVelField(),
                        getSolidSDFField(),
                        m_FluidSDFField2
                    );
                    m_VelocityField2.resize(m_TempVelocityField3);
                }
            }
            else
            {
                Debug.LogError("Invalid advection solver");
            }
        }
        private void __executeHelmholtzHodgDecomposition_MA(float vDeltaT)
        {
            if (m_AdvectionAlgorithm == EAdvectionAlgorithm.SemiLagrangian || m_AdvectionAlgorithm == EAdvectionAlgorithm.MacCormack || m_AdvectionAlgorithm == EAdvectionAlgorithm.Iteration)
            {
                if (m_BufferFlag)
                {
                    //m_TempVelocityField3.resize(m_VelocityField1);
                    //m_TempVelocityField3.plusAlphaX(m_TempVelocityField2, 1.0f / 3.0f * vDeltaT);
                    m_PressureSolver.executeHelmholtzHodgDecomposition_MA
                    (
                        m_VelocityField1,
                        vDeltaT,
                        getSolidVelField(),
                        getSolidSDFField(),
                        m_FluidSDFField1
                    );
                    //m_VelocityField1.resize(m_TempVelocityField3);
                }
                else
                {
                    //m_TempVelocityField3.resize(m_VelocityField2);
                    //m_TempVelocityField3.plusAlphaX(m_TempVelocityField2, 1.0f / 3.0f * vDeltaT);
                    m_PressureSolver.executeHelmholtzHodgDecomposition_MA
                    (
                        m_VelocityField2,
                        vDeltaT,
                        getSolidVelField(),
                        getSolidSDFField(),
                        m_FluidSDFField2
                    );
                    //m_VelocityField2.resize(m_TempVelocityField3);
                }
            }
        }
        private void __executeHelmholtzHodgDecomposition_CN(float vDeltaT)
        {
            if (m_AdvectionAlgorithm == EAdvectionAlgorithm.SemiLagrangian || m_AdvectionAlgorithm == EAdvectionAlgorithm.MacCormack || m_AdvectionAlgorithm == EAdvectionAlgorithm.Iteration)
            {
                if (m_BufferFlag)
                {
                    m_PressureSolver.executeHelmholtzHodgDecomposition_CN2
                    (
                        m_VelocityField1,
                        vDeltaT,
                        getSolidVelField(),
                        getSolidSDFField(),
                        m_FluidSDFField1
                    );
                }
                else
                {
                    m_PressureSolver.executeHelmholtzHodgDecomposition_CN2
                    (
                        m_VelocityField2,
                        vDeltaT,
                        getSolidVelField(),
                        getSolidSDFField(),
                        m_FluidSDFField2
                    );
                }
            }
        }
        private void __extrapolatingVel()
        {
            if (m_BufferFlag)
            {
                CEulerSolverToolInvoker.buildMarkersAndExtrapolatingDataInvoker(m_VelocityField1, m_ExtrapolatingVelMarkersField, m_ExtrapolatingNums);
                //CEulerSolverToolInvoker.buildMarkersAndExtrapolatingDataInvoker(m_PressureSolver.getPressureField(), m_CCSWeightField, m_ExtrapolatingNums);
                //CEulerSolverToolInvoker.extrapolatingDataOutwardsInvoker(m_VelocityField1, m_ExtrapolatingVelMarkersField, m_ExtrapolatingNums);
            }
            else
            {
                CEulerSolverToolInvoker.buildMarkersAndExtrapolatingDataInvoker(m_VelocityField2, m_ExtrapolatingVelMarkersField, m_ExtrapolatingNums);
                //CEulerSolverToolInvoker.buildMarkersAndExtrapolatingDataInvoker(m_PressureSolver.getPressureField(), m_CCSWeightField, m_ExtrapolatingNums);
                //CEulerSolverToolInvoker.extrapolatingDataOutwardsInvoker(m_VelocityField2, m_ExtrapolatingVelMarkersField, m_ExtrapolatingNums);
            }
        }
        private void __onEndAdvanceTimeStep(float vDeltaT)
        {
            m_BufferFlag = !m_BufferFlag;
            m_CurSimulationFrame += 1;
            Shader.SetGlobalFloat("SimulationTime", m_CurSimulationFrame * vDeltaT);
        }
        public void updateWithoutPressure()
        {
            __onBeginAdvanceTimeStep(m_DeltaT);

            __computeAdvection(m_DeltaT);
            __computeExternalForces(m_DeltaT);
            //__computeViscosity(m_DeltaT);
        }
        public void solvePressure()
        {
            __executeHelmholtzHodgDecomposition(m_DeltaT);
            __extrapolatingVel();

            __onEndAdvanceTimeStep(m_DeltaT);
        }
        #endregion

        #region Reflection Methods
        public void computeAdvection(float vDeltaT, CCellCenteredScalarField vSrcField, CCellCenteredScalarField voDstField)
        {
            if (m_AdvectionAlgorithm == EAdvectionAlgorithm.SemiLagrangian || m_AdvectionAlgorithm == EAdvectionAlgorithm.MacCormack)
            {
                if (m_BufferFlag)
                {
                    m_GridAdvectionSolver.advect(vSrcField, m_VelocityField2, vDeltaT, voDstField, m_AdvectionAccuracy, m_SamplingAlgorithm);
                }
                else
                {
                    m_GridAdvectionSolver.advect(vSrcField, m_VelocityField1, vDeltaT, voDstField, m_AdvectionAccuracy, m_SamplingAlgorithm);
                }
            }
            //else if (m_AdvectionAlgorithm == EAdvectionAlgorithm.PIC || m_AdvectionAlgorithm == EAdvectionAlgorithm.FLIP || m_AdvectionAlgorithm == EAdvectionAlgorithm.MixPICandFLIP)
            //{
            //    if (m_BufferFlag)
            //    {
            //        m_ParticleAdvectionSolver.advect(m_VelocityField2, m_VelocityField2, vDeltaT, m_VelocityField1, m_PGTransferAlgorithm, m_AdvectionAccuracy, m_SamplingAlgorithm);
            //    }
            //    else
            //    {
            //        m_ParticleAdvectionSolver.advect(m_VelocityField1, m_VelocityField1, vDeltaT, m_VelocityField2, m_PGTransferAlgorithm, m_AdvectionAccuracy, m_SamplingAlgorithm);
            //    }

            //    m_ParticleAdvectionSolver.getEulerParticles().deleteOutsideParticles(m_GridResolution, m_GridOrigin, m_GridSpacing); ;

            //    //CEulerParticlesInvokers.buildFluidDomainInvoker(m_ParticleAdvectionSolver.getEulerParticles().getParticlesPos(), m_FluidDomainField);
            //}
            else
            {
                Debug.LogError("Invalid advection solver");
            }
        }
        private void __computeAdvection_Reflection(float vDeltaT)
        {
            if (m_AdvectionAlgorithm == EAdvectionAlgorithm.SemiLagrangian)
            {
                m_GridAdvectionSolver.advect(m_ReflectVelocityFieldA, m_VelocityField1, vDeltaT, m_VelocityField2, m_AdvectionAccuracy, m_SamplingAlgorithm);
                m_GridAdvectionSolver.advect(m_FluidSDFField1, m_VelocityField1, vDeltaT, m_FluidSDFField2, m_AdvectionAccuracy, m_SamplingAlgorithm);
            }
            else if (m_AdvectionAlgorithm == EAdvectionAlgorithm.MacCormack)
            {
                m_GridAdvectionSolver.advectMacCormack(m_ReflectVelocityFieldA, m_VelocityField1, vDeltaT, m_VelocityField2, m_AdvectionAccuracy, m_SamplingAlgorithm);
                m_GridAdvectionSolver.advect(m_FluidSDFField1, m_VelocityField1, vDeltaT, m_FluidSDFField2, m_AdvectionAccuracy, m_SamplingAlgorithm);
            }
            else if (m_AdvectionAlgorithm == EAdvectionAlgorithm.PIC || m_AdvectionAlgorithm == EAdvectionAlgorithm.FLIP || m_AdvectionAlgorithm == EAdvectionAlgorithm.MixPICAndFLIP)
            {
                m_ParticleAdvectionSolver.advect(m_ReflectVelocityFieldA, m_VelocityField1, vDeltaT, m_VelocityField2, m_PGTransferAlgorithm, m_AdvectionAccuracy, m_SamplingAlgorithm);
            }
            else
            {
                Debug.LogError("Invalid advection solver");
            }
        }
        private void __computeAdvection_SecondOrderReflection(float vDeltaT)
        {
            m_TempVelocityField1.resize(m_VelocityField1);
            m_TempVelocityField1.scale(2.0f);
            m_TempVelocityField1.plusAlphaX(m_ReflectVelocityFieldA, -1.0f);
            if (m_AdvectionAlgorithm == EAdvectionAlgorithm.SemiLagrangian)
            {
                m_GridAdvectionSolver.advect(m_ReflectVelocityFieldC, m_TempVelocityField1, vDeltaT, m_VelocityField2, m_AdvectionAccuracy, m_SamplingAlgorithm);
                m_GridAdvectionSolver.advect(m_FluidSDFField1, m_TempVelocityField1, vDeltaT, m_FluidSDFField2, m_AdvectionAccuracy, m_SamplingAlgorithm);
            }
            else if (m_AdvectionAlgorithm == EAdvectionAlgorithm.MacCormack)
            {
                m_GridAdvectionSolver.advectMacCormack(m_ReflectVelocityFieldC, m_TempVelocityField1, vDeltaT, m_VelocityField2, m_AdvectionAccuracy, m_SamplingAlgorithm);
                m_GridAdvectionSolver.advectMacCormack(m_FluidSDFField1, m_TempVelocityField1, vDeltaT, m_FluidSDFField2, m_AdvectionAccuracy, m_SamplingAlgorithm);
            }
            else if (m_AdvectionAlgorithm == EAdvectionAlgorithm.PIC || m_AdvectionAlgorithm == EAdvectionAlgorithm.FLIP || m_AdvectionAlgorithm == EAdvectionAlgorithm.MixPICAndFLIP)
            {
                m_ParticleAdvectionSolver.advect(m_ReflectVelocityFieldC, m_TempVelocityField1, vDeltaT, m_VelocityField2, m_PGTransferAlgorithm, m_AdvectionAccuracy, m_SamplingAlgorithm);
            }
            else
            {
                Debug.LogError("Invalid advection solver");
            }
        }
        private void __computeAdvectionSmoke_SecondOrderReflection(float vDeltaT)
        {
            m_TempVelocityField1.resize(m_VelocityField1);
            m_TempVelocityField1.scale(2.0f);
            m_TempVelocityField1.plusAlphaX(m_ReflectVelocityFieldA, -1.0f);
            if (m_AdvectionAlgorithm == EAdvectionAlgorithm.SemiLagrangian)
            {
                m_GridAdvectionSolver.advect(m_FluidDensityField1, m_TempVelocityField1, vDeltaT, m_FluidTempScalarField, m_AdvectionAccuracy, m_SamplingAlgorithm);
                m_FluidDensityField1.resize(m_FluidTempScalarField);
                m_GridAdvectionSolver.advect(m_FluidDensityField2, m_TempVelocityField1, vDeltaT, m_FluidTempScalarField, m_AdvectionAccuracy, m_SamplingAlgorithm);
                m_FluidDensityField2.resize(m_FluidTempScalarField);
                m_GridAdvectionSolver.advect(m_FluidTemperatureField, m_TempVelocityField1, vDeltaT, m_FluidTempScalarField, m_AdvectionAccuracy, m_SamplingAlgorithm);
                m_FluidTemperatureField.resize(m_FluidTempScalarField);
                m_GridAdvectionSolver.advect(m_PressureSolver.getPressureField(), m_TempVelocityField1, vDeltaT, m_FluidTempScalarField, m_AdvectionAccuracy, m_SamplingAlgorithm);
                m_PressureSolver.getPressureField().resize(m_FluidTempScalarField);
                m_GridAdvectionSolver.advect(m_ReflectVelocityFieldC, m_TempVelocityField1, vDeltaT, m_VelocityField2, m_AdvectionAccuracy, m_SamplingAlgorithm);
            }
            else if (m_AdvectionAlgorithm == EAdvectionAlgorithm.MacCormack)
            {
                m_GridAdvectionSolver.advectMacCormack(m_FluidDensityField1, m_TempVelocityField1, vDeltaT, m_FluidTempScalarField, m_AdvectionAccuracy, m_SamplingAlgorithm);
                m_FluidDensityField1.resize(m_FluidTempScalarField);
                m_GridAdvectionSolver.advectMacCormack(m_FluidDensityField2, m_TempVelocityField1, vDeltaT, m_FluidTempScalarField, m_AdvectionAccuracy, m_SamplingAlgorithm);
                m_FluidDensityField2.resize(m_FluidTempScalarField);
                m_GridAdvectionSolver.advectMacCormack(m_FluidTemperatureField, m_TempVelocityField1, vDeltaT, m_FluidTempScalarField, m_AdvectionAccuracy, m_SamplingAlgorithm);
                m_FluidTemperatureField.resize(m_FluidTempScalarField);
                m_GridAdvectionSolver.advectMacCormack(m_PressureSolver.getPressureField(), m_TempVelocityField1, vDeltaT, m_FluidTempScalarField, m_AdvectionAccuracy, m_SamplingAlgorithm);
                m_PressureSolver.getPressureField().resize(m_FluidTempScalarField);
                m_GridAdvectionSolver.advectMacCormack(m_ReflectVelocityFieldC, m_TempVelocityField1, vDeltaT, m_VelocityField2, m_AdvectionAccuracy, m_SamplingAlgorithm);
            }
            else if (m_AdvectionAlgorithm == EAdvectionAlgorithm.PIC || m_AdvectionAlgorithm == EAdvectionAlgorithm.FLIP || m_AdvectionAlgorithm == EAdvectionAlgorithm.MixPICAndFLIP)
            {
                m_ParticleAdvectionSolver.advect(m_ReflectVelocityFieldC, m_TempVelocityField1, vDeltaT, m_VelocityField2, m_PGTransferAlgorithm, m_AdvectionAccuracy, m_SamplingAlgorithm);
            }
            else
            {
                Debug.LogError("Invalid advection solver");
            }
        }
        private void __computeAdvection_BDF2(float vDeltaT)
        {
            if (m_AdvectionAlgorithm == EAdvectionAlgorithm.SemiLagrangian)
            {
                if (m_BufferFlag)
                {
                    m_GridAdvectionSolver.advect(m_VelocityFieldPrevious, m_VelocityField2, 2.0f * vDeltaT, m_TempVelocityField1, m_AdvectionAccuracy, m_SamplingAlgorithm);
                    m_TempVelocityField2.resize(m_VelocityField2);
                    m_TempVelocityField2.scale(1.5f);
                    m_TempVelocityField2.plusAlphaX(m_VelocityFieldPrevious, -0.5f);
                    m_GridAdvectionSolver.advect(m_VelocityField2, m_TempVelocityField2, vDeltaT, m_VelocityField1, m_AdvectionAccuracy, m_SamplingAlgorithm);
                    m_GridAdvectionSolver.advect(m_FluidSDFField2, m_VelocityField2, vDeltaT, m_FluidSDFField1, m_AdvectionAccuracy, m_SamplingAlgorithm);
                    m_GridAdvectionSolver.advect(m_PressureSolver.getPressureField(), m_VelocityField2, vDeltaT, m_PressureField, m_AdvectionAccuracy, m_SamplingAlgorithm);
                    m_PressureSolver.getPressureField().resize(m_PressureField);
                    m_VelocityFieldPrevious.resize(m_VelocityField2);
                }
                else
                {
                    m_GridAdvectionSolver.advect(m_VelocityFieldPrevious, m_VelocityField1, 2.0f * vDeltaT, m_TempVelocityField1, m_AdvectionAccuracy, m_SamplingAlgorithm);
                    m_TempVelocityField2.resize(m_VelocityField1);
                    m_TempVelocityField2.scale(1.5f);
                    m_TempVelocityField2.plusAlphaX(m_VelocityFieldPrevious, -0.5f);
                    m_GridAdvectionSolver.advect(m_VelocityField1, m_TempVelocityField2, vDeltaT, m_VelocityField2, m_AdvectionAccuracy, m_SamplingAlgorithm);
                    m_GridAdvectionSolver.advect(m_FluidSDFField1, m_VelocityField1, vDeltaT, m_FluidSDFField2, m_AdvectionAccuracy, m_SamplingAlgorithm);
                    m_GridAdvectionSolver.advect(m_PressureSolver.getPressureField(), m_VelocityField1, vDeltaT, m_PressureField, m_AdvectionAccuracy, m_SamplingAlgorithm);
                    m_PressureSolver.getPressureField().resize(m_PressureField);
                    m_VelocityFieldPrevious.resize(m_VelocityField1);
                }
            }
            else if (m_AdvectionAlgorithm == EAdvectionAlgorithm.MacCormack)
            {
                if (m_BufferFlag)
                {
                    m_GridAdvectionSolver.advectMacCormack(m_VelocityFieldPrevious, m_VelocityField2, 2.0f * vDeltaT, m_TempVelocityField1, m_AdvectionAccuracy, m_SamplingAlgorithm);
                    m_TempVelocityField2.resize(m_VelocityField2);
                    m_TempVelocityField2.scale(1.5f);
                    m_TempVelocityField2.plusAlphaX(m_VelocityFieldPrevious, -0.5f);
                    m_GridAdvectionSolver.advectMacCormack(m_VelocityField2, m_TempVelocityField2, vDeltaT, m_VelocityField1, m_AdvectionAccuracy, m_SamplingAlgorithm);
                    m_GridAdvectionSolver.advectMacCormack(m_FluidSDFField2, m_VelocityField2, vDeltaT, m_FluidSDFField1, m_AdvectionAccuracy, m_SamplingAlgorithm);
                    m_GridAdvectionSolver.advectMacCormack(m_PressureSolver.getPressureField(), m_VelocityField2, vDeltaT, m_PressureField, m_AdvectionAccuracy, m_SamplingAlgorithm);
                    m_PressureSolver.getPressureField().resize(m_PressureField);
                    m_VelocityFieldPrevious.resize(m_VelocityField2);
                }
                else
                {
                    m_GridAdvectionSolver.advectMacCormack(m_VelocityFieldPrevious, m_VelocityField1, 2.0f * vDeltaT, m_TempVelocityField1, m_AdvectionAccuracy, m_SamplingAlgorithm);
                    m_TempVelocityField2.resize(m_VelocityField1);
                    m_TempVelocityField2.scale(1.5f);
                    m_TempVelocityField2.plusAlphaX(m_VelocityFieldPrevious, -0.5f);
                    m_GridAdvectionSolver.advectMacCormack(m_VelocityField1, m_TempVelocityField2, vDeltaT, m_VelocityField2, m_AdvectionAccuracy, m_SamplingAlgorithm);
                    m_GridAdvectionSolver.advectMacCormack(m_FluidSDFField1, m_VelocityField1, vDeltaT, m_FluidSDFField2, m_AdvectionAccuracy, m_SamplingAlgorithm);
                    m_GridAdvectionSolver.advectMacCormack(m_PressureSolver.getPressureField(), m_VelocityField1, vDeltaT, m_PressureField, m_AdvectionAccuracy, m_SamplingAlgorithm);
                    m_PressureSolver.getPressureField().resize(m_PressureField);
                    m_VelocityFieldPrevious.resize(m_VelocityField1);
                }
            }
            else
            {

            }
        }
        private void __computeAdvectionSmoke_BDF2(float vDeltaT)
        {
            if (m_AdvectionAlgorithm == EAdvectionAlgorithm.SemiLagrangian)
            {
                if (m_BufferFlag)
                {
                    m_GridAdvectionSolver.advect(m_FluidDensityField1, m_VelocityField2, vDeltaT, m_FluidTempScalarField, m_AdvectionAccuracy, m_SamplingAlgorithm);
                    m_FluidDensityField1.resize(m_FluidTempScalarField);
                    m_GridAdvectionSolver.advect(m_FluidDensityField2, m_VelocityField2, vDeltaT, m_FluidTempScalarField, m_AdvectionAccuracy, m_SamplingAlgorithm);
                    m_FluidDensityField2.resize(m_FluidTempScalarField);
                    m_GridAdvectionSolver.advect(m_FluidTemperatureField, m_VelocityField2, vDeltaT, m_FluidTempScalarField, m_AdvectionAccuracy, m_SamplingAlgorithm);
                    m_FluidTemperatureField.resize(m_FluidTempScalarField);

                    m_GridAdvectionSolver.advect(m_VelocityFieldPrevious, m_VelocityField2, 2.0f * vDeltaT, m_TempVelocityField1, m_AdvectionAccuracy, m_SamplingAlgorithm);
                    m_TempVelocityField2.resize(m_VelocityField2);
                    m_TempVelocityField2.scale(1.5f);
                    m_TempVelocityField2.plusAlphaX(m_VelocityFieldPrevious, -0.5f);
                    m_GridAdvectionSolver.advect(m_VelocityField2, m_TempVelocityField2, vDeltaT, m_VelocityField1, m_AdvectionAccuracy, m_SamplingAlgorithm);
                    m_GridAdvectionSolver.advect(m_PressureSolver.getPressureField(), m_VelocityField2, vDeltaT, m_PressureField, m_AdvectionAccuracy, m_SamplingAlgorithm);
                    m_PressureSolver.getPressureField().resize(m_PressureField);
                    m_VelocityFieldPrevious.resize(m_VelocityField2);
                }
                else
                {
                    m_GridAdvectionSolver.advect(m_FluidDensityField1, m_VelocityField1, vDeltaT, m_FluidTempScalarField, m_AdvectionAccuracy, m_SamplingAlgorithm);
                    m_FluidDensityField1.resize(m_FluidTempScalarField);
                    m_GridAdvectionSolver.advect(m_FluidDensityField2, m_VelocityField1, vDeltaT, m_FluidTempScalarField, m_AdvectionAccuracy, m_SamplingAlgorithm);
                    m_FluidDensityField2.resize(m_FluidTempScalarField);
                    m_GridAdvectionSolver.advect(m_FluidTemperatureField, m_VelocityField1, vDeltaT, m_FluidTempScalarField, m_AdvectionAccuracy, m_SamplingAlgorithm);
                    m_FluidTemperatureField.resize(m_FluidTempScalarField);

                    m_GridAdvectionSolver.advect(m_VelocityFieldPrevious, m_VelocityField1, 2.0f * vDeltaT, m_TempVelocityField1, m_AdvectionAccuracy, m_SamplingAlgorithm);
                    m_TempVelocityField2.resize(m_VelocityField1);
                    m_TempVelocityField2.scale(1.5f);
                    m_TempVelocityField2.plusAlphaX(m_VelocityFieldPrevious, -0.5f);
                    m_GridAdvectionSolver.advect(m_VelocityField1, m_TempVelocityField2, vDeltaT, m_VelocityField2, m_AdvectionAccuracy, m_SamplingAlgorithm);
                    m_GridAdvectionSolver.advect(m_PressureSolver.getPressureField(), m_VelocityField1, vDeltaT, m_PressureField, m_AdvectionAccuracy, m_SamplingAlgorithm);
                    m_PressureSolver.getPressureField().resize(m_PressureField);
                    m_VelocityFieldPrevious.resize(m_VelocityField1);
                }
            }
            else if (m_AdvectionAlgorithm == EAdvectionAlgorithm.MacCormack)
            {
                if (m_BufferFlag)
                {
                    m_GridAdvectionSolver.advectMacCormack(m_FluidDensityField1, m_VelocityField2, vDeltaT, m_FluidTempScalarField, m_AdvectionAccuracy, m_SamplingAlgorithm);
                    m_FluidDensityField1.resize(m_FluidTempScalarField);
                    m_GridAdvectionSolver.advectMacCormack(m_FluidDensityField2, m_VelocityField2, vDeltaT, m_FluidTempScalarField, m_AdvectionAccuracy, m_SamplingAlgorithm);
                    m_FluidDensityField2.resize(m_FluidTempScalarField);
                    m_GridAdvectionSolver.advectMacCormack(m_FluidTemperatureField, m_VelocityField2, vDeltaT, m_FluidTempScalarField, m_AdvectionAccuracy, m_SamplingAlgorithm);
                    m_FluidTemperatureField.resize(m_FluidTempScalarField);

                    m_GridAdvectionSolver.advectMacCormack(m_VelocityFieldPrevious, m_VelocityField2, 2.0f * vDeltaT, m_TempVelocityField1, m_AdvectionAccuracy, m_SamplingAlgorithm);
                    m_TempVelocityField2.resize(m_VelocityField2);
                    m_TempVelocityField2.scale(1.5f);
                    m_TempVelocityField2.plusAlphaX(m_VelocityFieldPrevious, -0.5f);
                    m_GridAdvectionSolver.advectMacCormack(m_VelocityField2, m_TempVelocityField2, vDeltaT, m_VelocityField1, m_AdvectionAccuracy, m_SamplingAlgorithm);
                    m_GridAdvectionSolver.advectMacCormack(m_PressureSolver.getPressureField(), m_VelocityField2, vDeltaT, m_PressureField, m_AdvectionAccuracy, m_SamplingAlgorithm);
                    m_PressureSolver.getPressureField().resize(m_PressureField);
                    m_VelocityFieldPrevious.resize(m_VelocityField2);
                }
                else
                {
                    m_GridAdvectionSolver.advectMacCormack(m_FluidDensityField1, m_VelocityField1, vDeltaT, m_FluidTempScalarField, m_AdvectionAccuracy, m_SamplingAlgorithm);
                    m_FluidDensityField1.resize(m_FluidTempScalarField);
                    m_GridAdvectionSolver.advectMacCormack(m_FluidDensityField2, m_VelocityField1, vDeltaT, m_FluidTempScalarField, m_AdvectionAccuracy, m_SamplingAlgorithm);
                    m_FluidDensityField2.resize(m_FluidTempScalarField);
                    m_GridAdvectionSolver.advectMacCormack(m_FluidTemperatureField, m_VelocityField1, vDeltaT, m_FluidTempScalarField, m_AdvectionAccuracy, m_SamplingAlgorithm);
                    m_FluidTemperatureField.resize(m_FluidTempScalarField);

                    m_GridAdvectionSolver.advectMacCormack(m_VelocityFieldPrevious, m_VelocityField1, 2.0f * vDeltaT, m_TempVelocityField1, m_AdvectionAccuracy, m_SamplingAlgorithm);
                    m_TempVelocityField2.resize(m_VelocityField1);
                    m_TempVelocityField2.scale(1.5f);
                    m_TempVelocityField2.plusAlphaX(m_VelocityFieldPrevious, -0.5f);
                    m_GridAdvectionSolver.advectMacCormack(m_VelocityField1, m_TempVelocityField2, vDeltaT, m_VelocityField2, m_AdvectionAccuracy, m_SamplingAlgorithm);
                    m_GridAdvectionSolver.advectMacCormack(m_PressureSolver.getPressureField(), m_VelocityField1, vDeltaT, m_PressureField, m_AdvectionAccuracy, m_SamplingAlgorithm);
                    m_PressureSolver.getPressureField().resize(m_PressureField);
                    m_VelocityFieldPrevious.resize(m_VelocityField1);
                }
            }
            else
            {

            }
        }
        private void __computeAdvection_MaterialAcceleration(float vDeltaT)
        {
            if (m_AdvectionAlgorithm == EAdvectionAlgorithm.SemiLagrangian)
            {
                if (m_BufferFlag)
                {
                    m_TempVelocityField1.resize(m_VelocityField2);
                    m_TempVelocityField1.plusAlphaX(m_VelocityFieldPrevious, -1);
                    m_TempVelocityField1.scale(1.0f / vDeltaT);
                    m_GridAdvectionSolver.advect(m_VelocityField2, m_VelocityField2, vDeltaT, m_VelocityField1, m_AdvectionAccuracy, m_SamplingAlgorithm);
                    m_GridAdvectionSolver.advect(m_TempVelocityField1, m_VelocityField2, vDeltaT, m_TempVelocityField2, m_AdvectionAccuracy, m_SamplingAlgorithm);
                    //m_GridAdvectionSolver.advect(m_FluidSDFField2, m_VelocityField2, vDeltaT, m_FluidSDFField1, m_AdvectionAccuracy, m_SamplingAlgorithm);
                    m_VelocityFieldPrevious.resize(m_VelocityField2);
                    m_VelocityField1.plusAlphaX(m_TempVelocityField2, vDeltaT / 3.0f);
                }
                else
                {
                    m_TempVelocityField1.resize(m_VelocityField1);
                    m_TempVelocityField1.plusAlphaX(m_VelocityFieldPrevious, -1);
                    m_TempVelocityField1.scale(1.0f / vDeltaT);
                    m_GridAdvectionSolver.advect(m_VelocityField1, m_VelocityField1, vDeltaT, m_VelocityField2, m_AdvectionAccuracy, m_SamplingAlgorithm);
                    m_GridAdvectionSolver.advect(m_TempVelocityField1, m_VelocityField1, vDeltaT, m_TempVelocityField2, m_AdvectionAccuracy, m_SamplingAlgorithm);
                    //m_GridAdvectionSolver.advect(m_FluidSDFField1, m_VelocityField1, vDeltaT, m_FluidSDFField2, m_AdvectionAccuracy, m_SamplingAlgorithm);
                    m_VelocityFieldPrevious.resize(m_VelocityField1);
                    m_VelocityField2.plusAlphaX(m_TempVelocityField2, vDeltaT / 3.0f);
                }
            }
            else if (m_AdvectionAlgorithm == EAdvectionAlgorithm.MacCormack)
            {
                if (m_BufferFlag)
                {
                    m_TempVelocityField1.resize(m_VelocityField2);
                    m_TempVelocityField1.plusAlphaX(m_VelocityFieldPrevious, -1);
                    m_TempVelocityField1.scale(1.0f / vDeltaT);
                    m_GridAdvectionSolver.advectMacCormack(m_VelocityField2, m_VelocityField2, vDeltaT, m_VelocityField1, m_AdvectionAccuracy, m_SamplingAlgorithm);
                    m_GridAdvectionSolver.advectMacCormack(m_TempVelocityField1, m_VelocityField2, vDeltaT, m_TempVelocityField2, m_AdvectionAccuracy, m_SamplingAlgorithm);
                    //m_GridAdvectionSolver.advectMacCormack(m_FluidSDFField2, m_VelocityField2, vDeltaT, m_FluidSDFField1, m_AdvectionAccuracy, m_SamplingAlgorithm);
                    m_VelocityFieldPrevious.resize(m_VelocityField2);
                    m_VelocityField1.plusAlphaX(m_TempVelocityField2, vDeltaT / 3.0f);
                }
                else
                {
                    m_TempVelocityField1.resize(m_VelocityField1);
                    m_TempVelocityField1.plusAlphaX(m_VelocityFieldPrevious, -1);
                    m_TempVelocityField1.scale(1.0f / vDeltaT);
                    m_GridAdvectionSolver.advectMacCormack(m_VelocityField1, m_VelocityField1, vDeltaT, m_VelocityField2, m_AdvectionAccuracy, m_SamplingAlgorithm);
                    m_GridAdvectionSolver.advectMacCormack(m_TempVelocityField1, m_VelocityField1, vDeltaT, m_TempVelocityField2, m_AdvectionAccuracy, m_SamplingAlgorithm);
                    //m_GridAdvectionSolver.advectMacCormack(m_FluidSDFField1, m_VelocityField1, vDeltaT, m_FluidSDFField2, m_AdvectionAccuracy, m_SamplingAlgorithm);
                    m_VelocityFieldPrevious.resize(m_VelocityField1);
                    m_VelocityField2.plusAlphaX(m_TempVelocityField2, vDeltaT / 3.0f);
                }
            }
        }
        private void __computeAdvectionSmoke_Reflection(float vDeltaT)
        {
            if (m_AdvectionAlgorithm == EAdvectionAlgorithm.SemiLagrangian)
            {
                if (m_BufferFlag)
                {
                    m_GridAdvectionSolver.advect(m_FluidDensityField1, m_VelocityField2, vDeltaT, m_FluidTempScalarField, m_AdvectionAccuracy, m_SamplingAlgorithm);
                    m_FluidDensityField1.resize(m_FluidTempScalarField);
                    m_GridAdvectionSolver.advect(m_FluidDensityField2, m_VelocityField2, vDeltaT, m_FluidTempScalarField, m_AdvectionAccuracy, m_SamplingAlgorithm);
                    m_FluidDensityField2.resize(m_FluidTempScalarField);
                    m_GridAdvectionSolver.advect(m_FluidTemperatureField, m_VelocityField2, vDeltaT, m_FluidTempScalarField, m_AdvectionAccuracy, m_SamplingAlgorithm);
                    m_FluidTemperatureField.resize(m_FluidTempScalarField);
                    m_GridAdvectionSolver.advect(m_PressureSolver.getPressureField(), m_VelocityField2, vDeltaT, m_FluidTempScalarField, m_AdvectionAccuracy, m_SamplingAlgorithm);
                    m_PressureSolver.getPressureField().resize(m_FluidTempScalarField);
                    m_GridAdvectionSolver.advect(m_ReflectVelocityFieldA, m_VelocityField2, vDeltaT, m_VelocityField1, m_AdvectionAccuracy, m_SamplingAlgorithm);
                }
                else
                {
                    m_GridAdvectionSolver.advect(m_FluidDensityField1, m_VelocityField1, vDeltaT, m_FluidTempScalarField, m_AdvectionAccuracy, m_SamplingAlgorithm);
                    m_FluidDensityField1.resize(m_FluidTempScalarField);
                    m_GridAdvectionSolver.advect(m_FluidDensityField2, m_VelocityField1, vDeltaT, m_FluidTempScalarField, m_AdvectionAccuracy, m_SamplingAlgorithm);
                    m_FluidDensityField2.resize(m_FluidTempScalarField);
                    m_GridAdvectionSolver.advect(m_FluidTemperatureField, m_VelocityField1, vDeltaT, m_FluidTempScalarField, m_AdvectionAccuracy, m_SamplingAlgorithm);
                    m_FluidTemperatureField.resize(m_FluidTempScalarField);
                    m_GridAdvectionSolver.advect(m_PressureSolver.getPressureField(), m_VelocityField1, vDeltaT, m_FluidTempScalarField, m_AdvectionAccuracy, m_SamplingAlgorithm);
                    m_PressureSolver.getPressureField().resize(m_FluidTempScalarField);
                    m_GridAdvectionSolver.advect(m_ReflectVelocityFieldA, m_VelocityField1, vDeltaT, m_VelocityField2, m_AdvectionAccuracy, m_SamplingAlgorithm);
                }
            }
            else if (m_AdvectionAlgorithm == EAdvectionAlgorithm.MacCormack)
            {
                if (m_BufferFlag)
                {
                    m_GridAdvectionSolver.advectMacCormack(m_FluidDensityField1, m_VelocityField2, vDeltaT, m_FluidTempScalarField, m_AdvectionAccuracy, m_SamplingAlgorithm);
                    m_FluidDensityField1.resize(m_FluidTempScalarField);
                    m_GridAdvectionSolver.advectMacCormack(m_FluidDensityField2, m_VelocityField2, vDeltaT, m_FluidTempScalarField, m_AdvectionAccuracy, m_SamplingAlgorithm);
                    m_FluidDensityField2.resize(m_FluidTempScalarField);
                    m_GridAdvectionSolver.advectMacCormack(m_FluidTemperatureField, m_VelocityField2, vDeltaT, m_FluidTempScalarField, m_AdvectionAccuracy, m_SamplingAlgorithm);
                    m_FluidTemperatureField.resize(m_FluidTempScalarField);
                    m_GridAdvectionSolver.advectMacCormack(m_PressureSolver.getPressureField(), m_VelocityField2, vDeltaT, m_FluidTempScalarField, m_AdvectionAccuracy, m_SamplingAlgorithm);
                    m_PressureSolver.getPressureField().resize(m_FluidTempScalarField);
                    m_GridAdvectionSolver.advectMacCormack(m_ReflectVelocityFieldA, m_VelocityField2, vDeltaT, m_VelocityField1, m_AdvectionAccuracy, m_SamplingAlgorithm);
                }
                else
                {
                    m_GridAdvectionSolver.advectMacCormack(m_FluidDensityField1, m_VelocityField1, vDeltaT, m_FluidTempScalarField, m_AdvectionAccuracy, m_SamplingAlgorithm);
                    m_FluidDensityField1.resize(m_FluidTempScalarField);
                    m_GridAdvectionSolver.advectMacCormack(m_FluidDensityField2, m_VelocityField1, vDeltaT, m_FluidTempScalarField, m_AdvectionAccuracy, m_SamplingAlgorithm);
                    m_FluidDensityField2.resize(m_FluidTempScalarField);
                    m_GridAdvectionSolver.advectMacCormack(m_FluidTemperatureField, m_VelocityField1, vDeltaT, m_FluidTempScalarField, m_AdvectionAccuracy, m_SamplingAlgorithm);
                    m_FluidTemperatureField.resize(m_FluidTempScalarField);
                    m_GridAdvectionSolver.advectMacCormack(m_PressureSolver.getPressureField(), m_VelocityField1, vDeltaT, m_FluidTempScalarField, m_AdvectionAccuracy, m_SamplingAlgorithm);
                    m_PressureSolver.getPressureField().resize(m_FluidTempScalarField);
                    m_GridAdvectionSolver.advectMacCormack(m_ReflectVelocityFieldA, m_VelocityField1, vDeltaT, m_VelocityField2, m_AdvectionAccuracy, m_SamplingAlgorithm);
                }
            }
            else
            {
                Debug.LogError("Invalid advection solver");
            }
        }
        private void __extrapolatingVel_Reflection()
        {
            CEulerSolverToolInvoker.buildMarkersAndExtrapolatingDataInvoker(m_VelocityField2, m_ExtrapolatingVelMarkersField, m_ExtrapolatingNums);
        }
        #endregion

        #region Fluid Attributes

        private Vector3 m_GridOrigin;
        private Vector3 m_GridSpacing;
        private Vector3Int m_GridResolution;

        private float m_DeltaT;
        private int m_CurSimulationFrame = 0;
        private int m_ExtrapolatingNums = 100;
        private bool m_BufferFlag = true;

        private bool m_BoundarysType = true;
        private CBoundarys m_Boundarys;

        private EAdvectionAlgorithm m_AdvectionAlgorithm = EAdvectionAlgorithm.MixPICAndFLIP;
        private ESamplingAlgorithm m_SamplingAlgorithm = ESamplingAlgorithm.LINEAR;
        private EAdvectionAccuracy m_AdvectionAccuracy = EAdvectionAccuracy.RK2;
        private EPGTransferAlgorithm m_PGTransferAlgorithm = EPGTransferAlgorithm.LINEAR;

        private bool m_IsInit = false;

        private bool m_IsViscous = false;
        private bool m_HasBuoyancy = false;

        private CCellCenteredScalarField m_FluidDomainField;
        private CCellCenteredScalarField m_FluidSDFField1;
        private CCellCenteredScalarField m_FluidSDFField2;
        private CCellCenteredScalarField m_BoundarysSDFField;
        private CFaceCenteredVectorField m_BoundarysVelField;
        private CCellCenteredScalarField m_AdditionalFluidField;
        private CCellCenteredScalarField m_FluidDensityField1;
        private CCellCenteredScalarField m_FluidDensityField2;
        private CCellCenteredScalarField m_FluidTemperatureField;
        private CCellCenteredScalarField m_FluidTempScalarField;
        private CFaceCenteredVectorField m_VelocityField1;
        private CFaceCenteredVectorField m_VelocityField2;
        private CFaceCenteredVectorField m_VelocityFieldPrevious;

        private CFaceCenteredVectorField m_ReflectVelocityFieldA;
        private CFaceCenteredVectorField m_ReflectVelocityFieldB;
        private CFaceCenteredVectorField m_ReflectVelocityFieldC;

        private CFaceCenteredVectorField m_TempVelocityField1;
        private CFaceCenteredVectorField m_TempVelocityField2;
        private CFaceCenteredVectorField m_TempVelocityField3;
        private CCellCenteredScalarField m_PressureField;
        private CCellCenteredVectorField m_PressureGradientField;
        private CCellCenteredScalarField m_CCSWeightField;
        private CCellCenteredVectorField m_CCVWeightField;

        private CFaceCenteredVectorField m_ExtrapolatingVelMarkersField;

        private CMixPICAndFLIP m_ParticleAdvectionSolver;
        private CSemiLagrangian m_GridAdvectionSolver;
        private CExternalForcesSolver m_ExternalForcesSolver;
        private CViscositySolver m_ViscositySolver;
        private CBuoyancySolver m_BuoyancySolver;
        private CPressureSolver m_PressureSolver;

        private List<FluidSource> m_FluidSources;
        private List<SBoundingBox> m_TriggerdDomains;

        private CEmitter m_Emitter = null;
        private CTwoDSmokeEmitter m_TwoDSmokeEmitter = null;
        private CThreeDSmokeEmitter m_ThreeDSmokeEmitter = null;
        private CVortexCollisionSmokeEmitter m_VortexCollisionSmokeEmitter = null;
        private CVortexLeapFroggingSmokeEmitter m_VortexLeapFroggingSmokeEmitter = null;
        private ESmokeScene m_SmokeEmitterFlag = ESmokeScene.NULL;
        #endregion
    }
}
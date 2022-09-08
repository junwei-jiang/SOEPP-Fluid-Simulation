using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using EulerFluidEngine;

namespace EulerFluidEngine
{
    public static class CEulerParticlesInvokers
    {
        private static bool m_Initialized = false;
        private static ComputeShader m_EulerParticlesTool = null;

        private static int m_GenerateParticlesInSDFKernel = -1;
        private static int m_GenerateParticlesInSDFExcludePreviousKernel = -1;
        private static int m_GenerateParticlesInBoundingBoxKernel = -1;
        private static int m_TransferParticlesNumToBufferKernel = -1;
        private static int m_DeleteOutsideParticlesKernel = -1;
        private static int m_TransferCCSField2ParticlesKernel = -1;
        private static int m_TransferParticles2CCSFieldKernel = -1;
        private static int m_NormalizeCCSFieldKernel = -1;
        private static int m_AdvectParticlesKernel = -1;
        private static int m_BuildFluidDomainKernel = -1;
        private static int m_StatisticalFluidDensityKernel = -1;
        private static int m_BuildFluidMarkersKernel = -1;
        private static int m_BuildFluidMarkersWithEmitBoxKernel = -1;

        private static ComputeBuffer m_ParticlesDataInt = new ComputeBuffer(CGlobalMacroAndFunc.MAX_PARTICLE_DATA_NUM, sizeof(int));
        private static ComputeBuffer m_FieldDataInt = new ComputeBuffer(CGlobalMacroAndFunc.MAX_GRIDS_NUM, sizeof(int));
        private static ComputeBuffer m_WeightFieldDataInt = new ComputeBuffer(CGlobalMacroAndFunc.MAX_GRIDS_NUM, sizeof(int));
        private static ComputeBuffer m_PreviousParticlesNum = new ComputeBuffer(1, sizeof(int));

        #region Init&Free
        public static void init()
        {
            if (m_Initialized) return;

            free();

            m_EulerParticlesTool = Resources.Load("Shaders/EulerParticlesTool") as ComputeShader;

            m_GenerateParticlesInSDFKernel = m_EulerParticlesTool.FindKernel("generateParticlesInSDF");
            m_GenerateParticlesInSDFExcludePreviousKernel = m_EulerParticlesTool.FindKernel("generateParticlesInSDFExcludePrevious");
            m_GenerateParticlesInBoundingBoxKernel = m_EulerParticlesTool.FindKernel("generateParticlesInBoundingBox");
            m_TransferParticlesNumToBufferKernel = m_EulerParticlesTool.FindKernel("transferParticlesNumToBuffer");
            m_DeleteOutsideParticlesKernel = m_EulerParticlesTool.FindKernel("deleteOutsideParticles");
            m_TransferCCSField2ParticlesKernel = m_EulerParticlesTool.FindKernel("transferCCSField2Particles");
            m_TransferParticles2CCSFieldKernel = m_EulerParticlesTool.FindKernel("transferParticles2CCSField");
            m_NormalizeCCSFieldKernel = m_EulerParticlesTool.FindKernel("normalizeCCSField");
            m_AdvectParticlesKernel = m_EulerParticlesTool.FindKernel("advectParticles");
            m_BuildFluidDomainKernel = m_EulerParticlesTool.FindKernel("buildFluidDomain");
            m_StatisticalFluidDensityKernel = m_EulerParticlesTool.FindKernel("statisticalFluidDensity");
            m_BuildFluidMarkersKernel = m_EulerParticlesTool.FindKernel("buildFluidMarkers");
            m_BuildFluidMarkersWithEmitBoxKernel = m_EulerParticlesTool.FindKernel("buildFluidMarkersWithEmitBox");

            m_ParticlesDataInt = new ComputeBuffer(CGlobalMacroAndFunc.MAX_PARTICLE_DATA_NUM, sizeof(int));
            m_FieldDataInt = new ComputeBuffer(CGlobalMacroAndFunc.MAX_GRIDS_NUM, sizeof(int));
            m_WeightFieldDataInt = new ComputeBuffer(CGlobalMacroAndFunc.MAX_GRIDS_NUM, sizeof(int));
            m_PreviousParticlesNum = new ComputeBuffer(1, sizeof(int));

            m_Initialized = true;
        }
        public static void free()
        {
            m_ParticlesDataInt.Release();
            m_FieldDataInt.Release();
            m_WeightFieldDataInt.Release();
            m_PreviousParticlesNum.Release();

            m_Initialized = false;
        }
        #endregion

        public static void generateParticlesInSDFInvoker
        (
            CCellCenteredScalarField vAdditionalFluidSDF,
            CCellCenteredScalarField vSolidSDF,
            ComputeBuffer vioParticlesPos,
            int vParticlesPerGrid = 8,
            CCellCenteredScalarField vPreviousFluidSDF = null
        )
        {
            if (!m_Initialized) init();

            Vector3Int Resolution = vAdditionalFluidSDF.getResolution();
            Vector3 Origin = vAdditionalFluidSDF.getOrigin();
            Vector3 Spacing = vAdditionalFluidSDF.getSpacing();

            int TotalThreadNum = Resolution.x * Resolution.y * Resolution.z;

            m_EulerParticlesTool.SetInts("GridResolution", Resolution.x, Resolution.y, Resolution.z);
            m_EulerParticlesTool.SetFloats("GridOrigin", Origin.x, Origin.y, Origin.z);
            m_EulerParticlesTool.SetFloats("GridSpacing", Spacing.x, Spacing.y, Spacing.z);

            if (vPreviousFluidSDF != null)
            {
                m_EulerParticlesTool.SetInt("vParticlesPerGrid_generateParticlesInSDFExcludePrevious", vParticlesPerGrid);
                m_EulerParticlesTool.SetBuffer(m_GenerateParticlesInSDFExcludePreviousKernel, "vAdditionalFluidSDFData_generateParticlesInSDFExcludePrevious", vAdditionalFluidSDF.getGridData());
                m_EulerParticlesTool.SetBuffer(m_GenerateParticlesInSDFExcludePreviousKernel, "vSolidSDFData_generateParticlesInSDFExcludePrevious", vSolidSDF.getGridData());
                m_EulerParticlesTool.SetBuffer(m_GenerateParticlesInSDFExcludePreviousKernel, "vioParticlesPos_generateParticlesInSDFExcludePrevious", vioParticlesPos);
                m_EulerParticlesTool.SetBuffer(m_GenerateParticlesInSDFExcludePreviousKernel, "vPreviousFluidSDFData_generateParticlesInSDFExcludePrevious", vPreviousFluidSDF.getGridData());

                CGlobalMacroAndFunc.dispatchKernel(m_EulerParticlesTool, m_GenerateParticlesInSDFExcludePreviousKernel, TotalThreadNum);
            }
            else
            {
                m_EulerParticlesTool.SetInt("vParticlesPerGrid_generateParticlesInSDF", vParticlesPerGrid);
                m_EulerParticlesTool.SetBuffer(m_GenerateParticlesInSDFKernel, "vAdditionalFluidSDFData_generateParticlesInSDF", vAdditionalFluidSDF.getGridData());
                m_EulerParticlesTool.SetBuffer(m_GenerateParticlesInSDFKernel, "vSolidSDFData_generateParticlesInSDF", vSolidSDF.getGridData());
                m_EulerParticlesTool.SetBuffer(m_GenerateParticlesInSDFKernel, "vioParticlesPos_generateParticlesInSDF", vioParticlesPos);

                CGlobalMacroAndFunc.dispatchKernel(m_EulerParticlesTool, m_GenerateParticlesInSDFKernel, TotalThreadNum);
            }
        }

        public static void generateParticlesInBoundingBoxInvoker
        (
            SBoundingBox vAdditionalFluidBox,
            CCellCenteredScalarField vSolidSDF,
            ComputeBuffer vioParticlesPos,
            int vParticlesPerGrid = 8
        )
        {
            if (!m_Initialized) init();

            Vector3Int Resolution = vSolidSDF.getResolution();
            Vector3 Origin = vSolidSDF.getOrigin();
            Vector3 Spacing = vSolidSDF.getSpacing();

            int TotalThreadNum = Resolution.x * Resolution.y * Resolution.z;

            m_EulerParticlesTool.SetInts("GridResolution", Resolution.x, Resolution.y, Resolution.z);
            m_EulerParticlesTool.SetFloats("GridOrigin", Origin.x, Origin.y, Origin.z);
            m_EulerParticlesTool.SetFloats("GridSpacing", Spacing.x, Spacing.y, Spacing.z);

            m_EulerParticlesTool.SetInt("vParticlesPerGrid_generateParticlesInBoundingBox", vParticlesPerGrid);
            m_EulerParticlesTool.SetFloats("vBoxMin_generateParticlesInBoundingBox", vAdditionalFluidBox.Min.x, vAdditionalFluidBox.Min.y, vAdditionalFluidBox.Min.z);
            m_EulerParticlesTool.SetFloats("vBoxMax_generateParticlesInBoundingBox", vAdditionalFluidBox.Max.x, vAdditionalFluidBox.Max.y, vAdditionalFluidBox.Max.z);

            m_EulerParticlesTool.SetBuffer(m_GenerateParticlesInBoundingBoxKernel, "vSolidSDFData_generateParticlesInBoundingBox", vSolidSDF.getGridData());
            m_EulerParticlesTool.SetBuffer(m_GenerateParticlesInBoundingBoxKernel, "vioParticlesPos_generateParticlesInBoundingBox", vioParticlesPos);

            CGlobalMacroAndFunc.dispatchKernel(m_EulerParticlesTool, m_GenerateParticlesInBoundingBoxKernel, TotalThreadNum);
        }

        public static void deleteOutsideParticlesInvoker
        (
            Vector3Int vResolution,
            Vector3 vOrigin,
            Vector3 vSpacing,
            ComputeBuffer vParticlesPos,
            ComputeBuffer vParticlesVel,
            ComputeBuffer voNewParticlesPos,
            ComputeBuffer voNewParticlesVel
        )
        {
            if (!m_Initialized) init();

            int[] arr1 = new int[1], arr2 = new int[1];

            m_EulerParticlesTool.SetBuffer(m_TransferParticlesNumToBufferKernel, "voParticlesNum_transferParticlesNumToBuffer", m_PreviousParticlesNum);

            m_EulerParticlesTool.Dispatch(m_TransferParticlesNumToBufferKernel, 1, 1, 1);

            int ParticlesTotalThreadNum = vParticlesPos.count / 3;

            m_EulerParticlesTool.SetInts("GridResolution", vResolution.x, vResolution.y, vResolution.z);
            m_EulerParticlesTool.SetFloats("GridOrigin", vOrigin.x, vOrigin.y, vOrigin.z);
            m_EulerParticlesTool.SetFloats("GridSpacing", vSpacing.x, vSpacing.y, vSpacing.z);
            m_EulerParticlesTool.SetBuffer(m_DeleteOutsideParticlesKernel, "vPreviousParticlesNum_deleteOutsideParticles", m_PreviousParticlesNum);
            m_EulerParticlesTool.SetBuffer(m_DeleteOutsideParticlesKernel, "vParticlesPos_deleteOutsideParticles", vParticlesPos);
            m_EulerParticlesTool.SetBuffer(m_DeleteOutsideParticlesKernel, "vParticlesVel_deleteOutsideParticles", vParticlesVel);
            m_EulerParticlesTool.SetBuffer(m_DeleteOutsideParticlesKernel, "voNewParticlesPos_deleteOutsideParticles", voNewParticlesPos);
            m_EulerParticlesTool.SetBuffer(m_DeleteOutsideParticlesKernel, "voNewParticlesVel_deleteOutsideParticles", voNewParticlesVel);

            CGlobalMacroAndFunc.dispatchKernel(m_EulerParticlesTool, m_DeleteOutsideParticlesKernel, ParticlesTotalThreadNum);
        }

        public static void tranferCCSField2ParticlesInvoker(
            ComputeBuffer vParticlesPos,
            ComputeBuffer voParticlesScalarValue,
            CCellCenteredScalarField vScalarField,
            EPGTransferAlgorithm vPGTransferAlgorithm = EPGTransferAlgorithm.LINEAR
        )
        {
            if (!m_Initialized) init();

            Vector3Int Resolution = vScalarField.getResolution();
            Vector3 Origin = vScalarField.getOrigin();
            Vector3 Spacing = vScalarField.getSpacing();

            int ParticlesTotalThreadNum = vParticlesPos.count / 3;

            m_EulerParticlesTool.SetInts("GridResolution", Resolution.x, Resolution.y, Resolution.z);
            m_EulerParticlesTool.SetFloats("GridOrigin", Origin.x, Origin.y, Origin.z);
            m_EulerParticlesTool.SetFloats("GridSpacing", Spacing.x, Spacing.y, Spacing.z);
            m_EulerParticlesTool.SetBuffer(m_TransferCCSField2ParticlesKernel, "vParticlesPos_transferCCSField2Particles", vParticlesPos);
            m_EulerParticlesTool.SetBuffer(m_TransferCCSField2ParticlesKernel, "voParticlesScalarValue_transferCCSField2Particles", voParticlesScalarValue);
            m_EulerParticlesTool.SetBuffer(m_TransferCCSField2ParticlesKernel, "vScalarFieldGridData_transferCCSField2Particles", vScalarField.getGridData());
            m_EulerParticlesTool.SetInt("DataSpan_transferCCSField2Particles", 1);
            m_EulerParticlesTool.SetInt("DataOffset_transferCCSField2Particles", 0);
            m_EulerParticlesTool.SetInt("TransferAlg_transferCCSField2Particles", Convert.ToInt32(vPGTransferAlgorithm));

            CGlobalMacroAndFunc.dispatchKernel(m_EulerParticlesTool, m_TransferCCSField2ParticlesKernel, ParticlesTotalThreadNum);
        }

        public static void tranferCCVField2ParticlesInvoker(
            ComputeBuffer vParticlesPos,
            ComputeBuffer voParticlesVectorValue,
            CCellCenteredVectorField vVectorField,
            EPGTransferAlgorithm vPGTransferAlgorithm = EPGTransferAlgorithm.LINEAR
        )
        {
            if (!m_Initialized) init();

            Vector3Int Resolution = vVectorField.getResolution();
            Vector3 Origin = vVectorField.getOrigin();
            Vector3 Spacing = vVectorField.getSpacing();

            int ParticlesTotalThreadNum = vParticlesPos.count / 3;

            m_EulerParticlesTool.SetInts("GridResolution", Resolution.x, Resolution.y, Resolution.z);
            m_EulerParticlesTool.SetFloats("GridOrigin", Origin.x, Origin.y, Origin.z);
            m_EulerParticlesTool.SetFloats("GridSpacing", Spacing.x, Spacing.y, Spacing.z);
            m_EulerParticlesTool.SetBuffer(m_TransferCCSField2ParticlesKernel, "vParticlesPos_transferCCSField2Particles", vParticlesPos);
            m_EulerParticlesTool.SetBuffer(m_TransferCCSField2ParticlesKernel, "voParticlesScalarValue_transferCCSField2Particles", voParticlesVectorValue);
            m_EulerParticlesTool.SetBuffer(m_TransferCCSField2ParticlesKernel, "vScalarFieldGridData_transferCCSField2Particles", vVectorField.getGridDataX());
            m_EulerParticlesTool.SetInt("DataSpan_transferCCSField2Particles", 3);
            m_EulerParticlesTool.SetInt("DataOffset_transferCCSField2Particles", 0);
            m_EulerParticlesTool.SetInt("TransferAlg_transferCCSField2Particles", Convert.ToInt32(vPGTransferAlgorithm));

            CGlobalMacroAndFunc.dispatchKernel(m_EulerParticlesTool, m_TransferCCSField2ParticlesKernel, ParticlesTotalThreadNum);

            m_EulerParticlesTool.SetInts("GridResolution", Resolution.x, Resolution.y, Resolution.z);
            m_EulerParticlesTool.SetFloats("GridOrigin", Origin.x, Origin.y, Origin.z);
            m_EulerParticlesTool.SetFloats("GridSpacing", Spacing.x, Spacing.y, Spacing.z);
            m_EulerParticlesTool.SetBuffer(m_TransferCCSField2ParticlesKernel, "vParticlesPos_transferCCSField2Particles", vParticlesPos);
            m_EulerParticlesTool.SetBuffer(m_TransferCCSField2ParticlesKernel, "voParticlesScalarValue_transferCCSField2Particles", voParticlesVectorValue);
            m_EulerParticlesTool.SetBuffer(m_TransferCCSField2ParticlesKernel, "vScalarFieldGridData_transferCCSField2Particles", vVectorField.getGridDataY());
            m_EulerParticlesTool.SetInt("DataSpan_transferCCSField2Particles", 3);
            m_EulerParticlesTool.SetInt("DataOffset_transferCCSField2Particles", 1);
            m_EulerParticlesTool.SetInt("TransferAlg_transferCCSField2Particles", Convert.ToInt32(vPGTransferAlgorithm));

            CGlobalMacroAndFunc.dispatchKernel(m_EulerParticlesTool, m_TransferCCSField2ParticlesKernel, ParticlesTotalThreadNum);

            m_EulerParticlesTool.SetInts("GridResolution", Resolution.x, Resolution.y, Resolution.z);
            m_EulerParticlesTool.SetFloats("GridOrigin", Origin.x, Origin.y, Origin.z);
            m_EulerParticlesTool.SetFloats("GridSpacing", Spacing.x, Spacing.y, Spacing.z);
            m_EulerParticlesTool.SetBuffer(m_TransferCCSField2ParticlesKernel, "vParticlesPos_transferCCSField2Particles", vParticlesPos);
            m_EulerParticlesTool.SetBuffer(m_TransferCCSField2ParticlesKernel, "voParticlesScalarValue_transferCCSField2Particles", voParticlesVectorValue);
            m_EulerParticlesTool.SetBuffer(m_TransferCCSField2ParticlesKernel, "vScalarFieldGridData_transferCCSField2Particles", vVectorField.getGridDataZ());
            m_EulerParticlesTool.SetInt("DataSpan_transferCCSField2Particles", 3);
            m_EulerParticlesTool.SetInt("DataOffset_transferCCSField2Particles", 2);
            m_EulerParticlesTool.SetInt("TransferAlg_transferCCSField2Particles", Convert.ToInt32(vPGTransferAlgorithm));

            CGlobalMacroAndFunc.dispatchKernel(m_EulerParticlesTool, m_TransferCCSField2ParticlesKernel, ParticlesTotalThreadNum);
        }

        public static void tranferFCVField2ParticlesInvoker(
            ComputeBuffer vParticlesPos,
            ComputeBuffer voParticlesVectorValue,
            CFaceCenteredVectorField vVectorField,
            EPGTransferAlgorithm vPGTransferAlgorithm = EPGTransferAlgorithm.LINEAR
        )
        {
            if (!m_Initialized) init();

            Vector3Int Resolution = vVectorField.getResolution();
            Vector3Int ResolutionX = Resolution + new Vector3Int(1, 0, 0);
            Vector3Int ResolutionY = Resolution + new Vector3Int(0, 1, 0);
            Vector3Int ResolutionZ = Resolution + new Vector3Int(0, 0, 1);
            Vector3 OriginX = vVectorField.getOrigin() - new Vector3(vVectorField.getSpacing().x / 2, 0, 0);
            Vector3 OriginY = vVectorField.getOrigin() - new Vector3(0, vVectorField.getSpacing().y / 2, 0);
            Vector3 OriginZ = vVectorField.getOrigin() - new Vector3(0, 0, vVectorField.getSpacing().z / 2);
            Vector3 Spacing = vVectorField.getSpacing();

            int ParticlesTotalThreadNum = vParticlesPos.count / 3;

            m_EulerParticlesTool.SetInts("GridResolution", ResolutionX.x, ResolutionX.y, ResolutionX.z);
            m_EulerParticlesTool.SetFloats("GridOrigin", OriginX.x, OriginX.y, OriginX.z);
            m_EulerParticlesTool.SetFloats("GridSpacing", Spacing.x, Spacing.y, Spacing.z);
            m_EulerParticlesTool.SetBuffer(m_TransferCCSField2ParticlesKernel, "vParticlesPos_transferCCSField2Particles", vParticlesPos);
            m_EulerParticlesTool.SetBuffer(m_TransferCCSField2ParticlesKernel, "voParticlesScalarValue_transferCCSField2Particles", voParticlesVectorValue);
            m_EulerParticlesTool.SetBuffer(m_TransferCCSField2ParticlesKernel, "vScalarFieldGridData_transferCCSField2Particles", vVectorField.getGridDataX());
            m_EulerParticlesTool.SetInt("DataSpan_transferCCSField2Particles", 3);
            m_EulerParticlesTool.SetInt("DataOffset_transferCCSField2Particles", 0);
            m_EulerParticlesTool.SetInt("TransferAlg_transferCCSField2Particles", Convert.ToInt32(vPGTransferAlgorithm));

            CGlobalMacroAndFunc.dispatchKernel(m_EulerParticlesTool, m_TransferCCSField2ParticlesKernel, ParticlesTotalThreadNum);

            m_EulerParticlesTool.SetInts("GridResolution", ResolutionY.x, ResolutionY.y, ResolutionY.z);
            m_EulerParticlesTool.SetFloats("GridOrigin", OriginY.x, OriginY.y, OriginY.z);
            m_EulerParticlesTool.SetFloats("GridSpacing", Spacing.x, Spacing.y, Spacing.z);
            m_EulerParticlesTool.SetBuffer(m_TransferCCSField2ParticlesKernel, "vParticlesPos_transferCCSField2Particles", vParticlesPos);
            m_EulerParticlesTool.SetBuffer(m_TransferCCSField2ParticlesKernel, "voParticlesScalarValue_transferCCSField2Particles", voParticlesVectorValue);
            m_EulerParticlesTool.SetBuffer(m_TransferCCSField2ParticlesKernel, "vScalarFieldGridData_transferCCSField2Particles", vVectorField.getGridDataY());
            m_EulerParticlesTool.SetInt("DataSpan_transferCCSField2Particles", 3);
            m_EulerParticlesTool.SetInt("DataOffset_transferCCSField2Particles", 1);
            m_EulerParticlesTool.SetInt("TransferAlg_transferCCSField2Particles", Convert.ToInt32(vPGTransferAlgorithm));

            CGlobalMacroAndFunc.dispatchKernel(m_EulerParticlesTool, m_TransferCCSField2ParticlesKernel, ParticlesTotalThreadNum);

            m_EulerParticlesTool.SetInts("GridResolution", ResolutionZ.x, ResolutionZ.y, ResolutionZ.z);
            m_EulerParticlesTool.SetFloats("GridOrigin", OriginZ.x, OriginZ.y, OriginZ.z);
            m_EulerParticlesTool.SetFloats("GridSpacing", Spacing.x, Spacing.y, Spacing.z);
            m_EulerParticlesTool.SetBuffer(m_TransferCCSField2ParticlesKernel, "vParticlesPos_transferCCSField2Particles", vParticlesPos);
            m_EulerParticlesTool.SetBuffer(m_TransferCCSField2ParticlesKernel, "voParticlesScalarValue_transferCCSField2Particles", voParticlesVectorValue);
            m_EulerParticlesTool.SetBuffer(m_TransferCCSField2ParticlesKernel, "vScalarFieldGridData_transferCCSField2Particles", vVectorField.getGridDataZ());
            m_EulerParticlesTool.SetInt("DataSpan_transferCCSField2Particles", 3);
            m_EulerParticlesTool.SetInt("DataOffset_transferCCSField2Particles", 2);
            m_EulerParticlesTool.SetInt("TransferAlg_transferCCSField2Particles", Convert.ToInt32(vPGTransferAlgorithm));

            CGlobalMacroAndFunc.dispatchKernel(m_EulerParticlesTool, m_TransferCCSField2ParticlesKernel, ParticlesTotalThreadNum);
        }

        public static void tranferParticles2CCSFieldInvoker(
            ComputeBuffer vParticlesPos,
            ComputeBuffer vParticlesScalarValue,
            CCellCenteredScalarField voScalarField,
            CCellCenteredScalarField voWeightField,
            EPGTransferAlgorithm vPGTransferAlgorithm = EPGTransferAlgorithm.LINEAR,
            bool vIsNormalization = true
        )
        {
            if (!m_Initialized) init();

            Vector3Int Resolution = voScalarField.getResolution();
            Vector3 Origin = voScalarField.getOrigin();
            Vector3 Spacing = voScalarField.getSpacing();

            int ParticlesTotalThreadNum = vParticlesPos.count / 3;

            voScalarField.resize(voScalarField.getResolution(), voScalarField.getOrigin(), voScalarField.getSpacing());
            voWeightField.resize(voWeightField.getResolution(), voWeightField.getOrigin(), voWeightField.getSpacing());

            CMathTool.transferFloats2Ints(vParticlesScalarValue, m_ParticlesDataInt);

            CMathTool.transferFloats2Ints(voScalarField.getGridData(), m_FieldDataInt);
            CMathTool.transferFloats2Ints(voWeightField.getGridData(), m_WeightFieldDataInt);

            m_EulerParticlesTool.SetInts("GridResolution", Resolution.x, Resolution.y, Resolution.z);
            m_EulerParticlesTool.SetFloats("GridOrigin", Origin.x, Origin.y, Origin.z);
            m_EulerParticlesTool.SetFloats("GridSpacing", Spacing.x, Spacing.y, Spacing.z);
            m_EulerParticlesTool.SetBuffer(m_TransferParticles2CCSFieldKernel, "vParticlesPos_transferParticles2CCSField", vParticlesPos);
            m_EulerParticlesTool.SetBuffer(m_TransferParticles2CCSFieldKernel, "vParticlesScalarValue_transferParticles2CCSField", m_ParticlesDataInt);
            m_EulerParticlesTool.SetBuffer(m_TransferParticles2CCSFieldKernel, "voScalarFieldData_transferParticles2CCSField", m_FieldDataInt);
            m_EulerParticlesTool.SetBuffer(m_TransferParticles2CCSFieldKernel, "voWeightFieldData_transferParticles2CCSField", m_WeightFieldDataInt);
            m_EulerParticlesTool.SetInt("DataSpan_transferParticles2CCSField", 1);
            m_EulerParticlesTool.SetInt("DataOffset_transferParticles2CCSField", 0);
            m_EulerParticlesTool.SetInt("TransferAlg_transferParticles2CCSField", Convert.ToInt32(vPGTransferAlgorithm));

            CGlobalMacroAndFunc.dispatchKernel(m_EulerParticlesTool, m_TransferParticles2CCSFieldKernel, ParticlesTotalThreadNum);

            CMathTool.transferInts2Floats(m_FieldDataInt, voScalarField.getGridData());
            CMathTool.transferInts2Floats(m_WeightFieldDataInt, voWeightField.getGridData());

            if (vIsNormalization)
            {
                int GridTotalThreadNum = Resolution.x * Resolution.y * Resolution.z;

                m_EulerParticlesTool.SetBuffer(m_NormalizeCCSFieldKernel, "vioScalarFieldData_normalizeCCSField", voScalarField.getGridData());
                m_EulerParticlesTool.SetBuffer(m_NormalizeCCSFieldKernel, "vWeightFieldData_normalizeCCSField", voWeightField.getGridData());

                CGlobalMacroAndFunc.dispatchKernel(m_EulerParticlesTool, m_NormalizeCCSFieldKernel, GridTotalThreadNum);
            }
        }

        public static void tranferParticles2CCVFieldInvoker(
            ComputeBuffer vParticlesPos,
            ComputeBuffer vParticlesVectorValue,
            CCellCenteredVectorField voVectorField,
            CCellCenteredVectorField voWeightField,
            EPGTransferAlgorithm vPGTransferAlgorithm = EPGTransferAlgorithm.LINEAR,
            bool vIsNormalization = true
        )
        {
            if (!m_Initialized) init();

            Vector3Int Resolution = voVectorField.getResolution();
            Vector3 Origin = voVectorField.getOrigin();
            Vector3 Spacing = voVectorField.getSpacing();

            int ParticlesTotalThreadNum = (vParticlesPos.count / 3);

            voVectorField.resize(voVectorField.getResolution(), voVectorField.getOrigin(), voVectorField.getSpacing());
            voWeightField.resize(voWeightField.getResolution(), voWeightField.getOrigin(), voWeightField.getSpacing());

            CMathTool.transferFloats2Ints(vParticlesVectorValue, m_ParticlesDataInt);

            CMathTool.transferFloats2Ints(voVectorField.getGridDataX(), m_FieldDataInt);
            CMathTool.transferFloats2Ints(voWeightField.getGridDataX(), m_WeightFieldDataInt);

            m_EulerParticlesTool.SetInts("GridResolution", Resolution.x, Resolution.y, Resolution.z);
            m_EulerParticlesTool.SetFloats("GridOrigin", Origin.x, Origin.y, Origin.z);
            m_EulerParticlesTool.SetFloats("GridSpacing", Spacing.x, Spacing.y, Spacing.z);
            m_EulerParticlesTool.SetBuffer(m_TransferParticles2CCSFieldKernel, "vParticlesPos_transferParticles2CCSField", vParticlesPos);
            m_EulerParticlesTool.SetBuffer(m_TransferParticles2CCSFieldKernel, "vParticlesScalarValue_transferParticles2CCSField", m_ParticlesDataInt);
            m_EulerParticlesTool.SetBuffer(m_TransferParticles2CCSFieldKernel, "voScalarFieldData_transferParticles2CCSField", m_FieldDataInt);
            m_EulerParticlesTool.SetBuffer(m_TransferParticles2CCSFieldKernel, "voWeightFieldData_transferParticles2CCSField", m_WeightFieldDataInt);
            m_EulerParticlesTool.SetInt("DataSpan_transferParticles2CCSField", 3);
            m_EulerParticlesTool.SetInt("DataOffset_transferParticles2CCSField", 0);
            m_EulerParticlesTool.SetInt("TransferAlg_transferParticles2CCSField", Convert.ToInt32(vPGTransferAlgorithm));

            CGlobalMacroAndFunc.dispatchKernel(m_EulerParticlesTool, m_TransferParticles2CCSFieldKernel, ParticlesTotalThreadNum);

            CMathTool.transferInts2Floats(m_FieldDataInt, voVectorField.getGridDataX());
            CMathTool.transferInts2Floats(m_WeightFieldDataInt, voWeightField.getGridDataX());

            CMathTool.transferFloats2Ints(voVectorField.getGridDataY(), m_FieldDataInt);
            CMathTool.transferFloats2Ints(voWeightField.getGridDataY(), m_WeightFieldDataInt);

            m_EulerParticlesTool.SetInts("GridResolution", Resolution.x, Resolution.y, Resolution.z);
            m_EulerParticlesTool.SetFloats("GridOrigin", Origin.x, Origin.y, Origin.z);
            m_EulerParticlesTool.SetFloats("GridSpacing", Spacing.x, Spacing.y, Spacing.z);
            m_EulerParticlesTool.SetBuffer(m_TransferParticles2CCSFieldKernel, "vParticlesPos_transferParticles2CCSField", vParticlesPos);
            m_EulerParticlesTool.SetBuffer(m_TransferParticles2CCSFieldKernel, "vParticlesScalarValue_transferParticles2CCSField", m_ParticlesDataInt);
            m_EulerParticlesTool.SetBuffer(m_TransferParticles2CCSFieldKernel, "voScalarFieldData_transferParticles2CCSField", m_FieldDataInt);
            m_EulerParticlesTool.SetBuffer(m_TransferParticles2CCSFieldKernel, "voWeightFieldData_transferParticles2CCSField", m_WeightFieldDataInt);
            m_EulerParticlesTool.SetInt("DataSpan_transferParticles2CCSField", 3);
            m_EulerParticlesTool.SetInt("DataOffset_transferParticles2CCSField", 1);
            m_EulerParticlesTool.SetInt("TransferAlg_transferParticles2CCSField", Convert.ToInt32(vPGTransferAlgorithm));

            CGlobalMacroAndFunc.dispatchKernel(m_EulerParticlesTool, m_TransferParticles2CCSFieldKernel, ParticlesTotalThreadNum);

            CMathTool.transferInts2Floats(m_FieldDataInt, voVectorField.getGridDataY());
            CMathTool.transferInts2Floats(m_WeightFieldDataInt, voWeightField.getGridDataY());

            CMathTool.transferFloats2Ints(voVectorField.getGridDataZ(), m_FieldDataInt);
            CMathTool.transferFloats2Ints(voWeightField.getGridDataZ(), m_WeightFieldDataInt);

            m_EulerParticlesTool.SetInts("GridResolution", Resolution.x, Resolution.y, Resolution.z);
            m_EulerParticlesTool.SetFloats("GridOrigin", Origin.x, Origin.y, Origin.z);
            m_EulerParticlesTool.SetFloats("GridSpacing", Spacing.x, Spacing.y, Spacing.z);
            m_EulerParticlesTool.SetBuffer(m_TransferParticles2CCSFieldKernel, "vParticlesPos_transferParticles2CCSField", vParticlesPos);
            m_EulerParticlesTool.SetBuffer(m_TransferParticles2CCSFieldKernel, "vParticlesScalarValue_transferParticles2CCSField", m_ParticlesDataInt);
            m_EulerParticlesTool.SetBuffer(m_TransferParticles2CCSFieldKernel, "voScalarFieldData_transferParticles2CCSField", m_FieldDataInt);
            m_EulerParticlesTool.SetBuffer(m_TransferParticles2CCSFieldKernel, "voWeightFieldData_transferParticles2CCSField", m_WeightFieldDataInt);
            m_EulerParticlesTool.SetInt("DataSpan_transferParticles2CCSField", 3);
            m_EulerParticlesTool.SetInt("DataOffset_transferParticles2CCSField", 2);
            m_EulerParticlesTool.SetInt("TransferAlg_transferParticles2CCSField", Convert.ToInt32(vPGTransferAlgorithm));

            CGlobalMacroAndFunc.dispatchKernel(m_EulerParticlesTool, m_TransferParticles2CCSFieldKernel, ParticlesTotalThreadNum);

            CMathTool.transferInts2Floats(m_FieldDataInt, voVectorField.getGridDataZ());
            CMathTool.transferInts2Floats(m_WeightFieldDataInt, voWeightField.getGridDataZ());

            if (vIsNormalization)
            {
                int GridTotalThreadNum = Resolution.x * Resolution.y * Resolution.z;

                m_EulerParticlesTool.SetBuffer(m_NormalizeCCSFieldKernel, "vioScalarFieldData_normalizeCCSField", voVectorField.getGridDataX());
                m_EulerParticlesTool.SetBuffer(m_NormalizeCCSFieldKernel, "vWeightFieldData_normalizeCCSField", voWeightField.getGridDataX());

                CGlobalMacroAndFunc.dispatchKernel(m_EulerParticlesTool, m_NormalizeCCSFieldKernel, GridTotalThreadNum);

                m_EulerParticlesTool.SetBuffer(m_NormalizeCCSFieldKernel, "vioScalarFieldData_normalizeCCSField", voVectorField.getGridDataY());
                m_EulerParticlesTool.SetBuffer(m_NormalizeCCSFieldKernel, "vWeightFieldData_normalizeCCSField", voWeightField.getGridDataY());

                CGlobalMacroAndFunc.dispatchKernel(m_EulerParticlesTool, m_NormalizeCCSFieldKernel, GridTotalThreadNum);

                m_EulerParticlesTool.SetBuffer(m_NormalizeCCSFieldKernel, "vioScalarFieldData_normalizeCCSField", voVectorField.getGridDataZ());
                m_EulerParticlesTool.SetBuffer(m_NormalizeCCSFieldKernel, "vWeightFieldData_normalizeCCSField", voWeightField.getGridDataZ());

                CGlobalMacroAndFunc.dispatchKernel(m_EulerParticlesTool, m_NormalizeCCSFieldKernel, GridTotalThreadNum);
            }
        }

        public static void tranferParticles2FCVFieldInvoker(
            ComputeBuffer vParticlesPos,
            ComputeBuffer vParticlesVectorValue,
            CFaceCenteredVectorField voVectorField,
            CFaceCenteredVectorField voWeightField,
            EPGTransferAlgorithm vPGTransferAlgorithm = EPGTransferAlgorithm.LINEAR,
            bool vIsNormalization = true
        )
        {
            if (!m_Initialized) init();

            Vector3Int Resolution = voVectorField.getResolution();
            Vector3Int ResolutionX = Resolution + new Vector3Int(1, 0, 0);
            Vector3Int ResolutionY = Resolution + new Vector3Int(0, 1, 0);
            Vector3Int ResolutionZ = Resolution + new Vector3Int(0, 0, 1);
            Vector3 OriginX = voVectorField.getOrigin() - new Vector3(voVectorField.getSpacing().x / 2, 0, 0);
            Vector3 OriginY = voVectorField.getOrigin() - new Vector3(0, voVectorField.getSpacing().y / 2, 0);
            Vector3 OriginZ = voVectorField.getOrigin() - new Vector3(0, 0, voVectorField.getSpacing().z / 2);
            Vector3 Spacing = voVectorField.getSpacing();

            int ParticlesTotalThreadNum = vParticlesPos.count / 3;

            voVectorField.resize(voVectorField.getResolution(), voVectorField.getOrigin(), voVectorField.getSpacing());
            voWeightField.resize(voWeightField.getResolution(), voWeightField.getOrigin(), voWeightField.getSpacing());

            CMathTool.transferFloats2Ints(vParticlesVectorValue, m_ParticlesDataInt);

            CMathTool.transferFloats2Ints(voVectorField.getGridDataX(), m_FieldDataInt);
            CMathTool.transferFloats2Ints(voWeightField.getGridDataX(), m_WeightFieldDataInt);

            m_EulerParticlesTool.SetInts("GridResolution", ResolutionX.x, ResolutionX.y, ResolutionX.z);
            m_EulerParticlesTool.SetFloats("GridOrigin", OriginX.x, OriginX.y, OriginX.z);
            m_EulerParticlesTool.SetFloats("GridSpacing", Spacing.x, Spacing.y, Spacing.z);
            m_EulerParticlesTool.SetBuffer(m_TransferParticles2CCSFieldKernel, "vParticlesPos_transferParticles2CCSField", vParticlesPos);
            m_EulerParticlesTool.SetBuffer(m_TransferParticles2CCSFieldKernel, "vParticlesScalarValue_transferParticles2CCSField", m_ParticlesDataInt);
            m_EulerParticlesTool.SetBuffer(m_TransferParticles2CCSFieldKernel, "voScalarFieldData_transferParticles2CCSField", m_FieldDataInt);
            m_EulerParticlesTool.SetBuffer(m_TransferParticles2CCSFieldKernel, "voWeightFieldData_transferParticles2CCSField", m_WeightFieldDataInt);
            m_EulerParticlesTool.SetInt("DataSpan_transferParticles2CCSField", 3);
            m_EulerParticlesTool.SetInt("DataOffset_transferParticles2CCSField", 0);
            m_EulerParticlesTool.SetInt("TransferAlg_transferParticles2CCSField", Convert.ToInt32(vPGTransferAlgorithm));

            CGlobalMacroAndFunc.dispatchKernel(m_EulerParticlesTool, m_TransferParticles2CCSFieldKernel, ParticlesTotalThreadNum);

            CMathTool.transferInts2Floats(m_FieldDataInt, voVectorField.getGridDataX());
            CMathTool.transferInts2Floats(m_WeightFieldDataInt, voWeightField.getGridDataX());

            CMathTool.transferFloats2Ints(voVectorField.getGridDataY(), m_FieldDataInt);
            CMathTool.transferFloats2Ints(voWeightField.getGridDataY(), m_WeightFieldDataInt);

            m_EulerParticlesTool.SetInts("GridResolution", ResolutionY.x, ResolutionY.y, ResolutionY.z);
            m_EulerParticlesTool.SetFloats("GridOrigin", OriginY.x, OriginY.y, OriginY.z);
            m_EulerParticlesTool.SetFloats("GridSpacing", Spacing.x, Spacing.y, Spacing.z);
            m_EulerParticlesTool.SetBuffer(m_TransferParticles2CCSFieldKernel, "vParticlesPos_transferParticles2CCSField", vParticlesPos);
            m_EulerParticlesTool.SetBuffer(m_TransferParticles2CCSFieldKernel, "vParticlesScalarValue_transferParticles2CCSField", m_ParticlesDataInt);
            m_EulerParticlesTool.SetBuffer(m_TransferParticles2CCSFieldKernel, "voScalarFieldData_transferParticles2CCSField", m_FieldDataInt);
            m_EulerParticlesTool.SetBuffer(m_TransferParticles2CCSFieldKernel, "voWeightFieldData_transferParticles2CCSField", m_WeightFieldDataInt);
            m_EulerParticlesTool.SetInt("DataSpan_transferParticles2CCSField", 3);
            m_EulerParticlesTool.SetInt("DataOffset_transferParticles2CCSField", 1);
            m_EulerParticlesTool.SetInt("TransferAlg_transferParticles2CCSField", Convert.ToInt32(vPGTransferAlgorithm));

            CGlobalMacroAndFunc.dispatchKernel(m_EulerParticlesTool, m_TransferParticles2CCSFieldKernel, ParticlesTotalThreadNum);

            CMathTool.transferInts2Floats(m_FieldDataInt, voVectorField.getGridDataY());
            CMathTool.transferInts2Floats(m_WeightFieldDataInt, voWeightField.getGridDataY());

            CMathTool.transferFloats2Ints(voVectorField.getGridDataZ(), m_FieldDataInt);
            CMathTool.transferFloats2Ints(voWeightField.getGridDataZ(), m_WeightFieldDataInt);

            m_EulerParticlesTool.SetInts("GridResolution", ResolutionZ.x, ResolutionZ.y, ResolutionZ.z);
            m_EulerParticlesTool.SetFloats("GridOrigin", OriginZ.x, OriginZ.y, OriginZ.z);
            m_EulerParticlesTool.SetFloats("GridSpacing", Spacing.x, Spacing.y, Spacing.z);
            m_EulerParticlesTool.SetBuffer(m_TransferParticles2CCSFieldKernel, "vParticlesPos_transferParticles2CCSField", vParticlesPos);
            m_EulerParticlesTool.SetBuffer(m_TransferParticles2CCSFieldKernel, "vParticlesScalarValue_transferParticles2CCSField", m_ParticlesDataInt);
            m_EulerParticlesTool.SetBuffer(m_TransferParticles2CCSFieldKernel, "voScalarFieldData_transferParticles2CCSField", m_FieldDataInt);
            m_EulerParticlesTool.SetBuffer(m_TransferParticles2CCSFieldKernel, "voWeightFieldData_transferParticles2CCSField", m_WeightFieldDataInt);
            m_EulerParticlesTool.SetInt("DataSpan_transferParticles2CCSField", 3);
            m_EulerParticlesTool.SetInt("DataOffset_transferParticles2CCSField", 2);
            m_EulerParticlesTool.SetInt("TransferAlg_transferParticles2CCSField", Convert.ToInt32(vPGTransferAlgorithm));

            CGlobalMacroAndFunc.dispatchKernel(m_EulerParticlesTool, m_TransferParticles2CCSFieldKernel, ParticlesTotalThreadNum);

            CMathTool.transferInts2Floats(m_FieldDataInt, voVectorField.getGridDataZ());
            CMathTool.transferInts2Floats(m_WeightFieldDataInt, voWeightField.getGridDataZ());

            if (vIsNormalization)
            {
                int GridTotalThreadNumX = ResolutionX.x * ResolutionX.y * ResolutionX.z;
                int GridTotalThreadNumY = ResolutionY.x * ResolutionY.y * ResolutionY.z;
                int GridTotalThreadNumZ = ResolutionZ.x * ResolutionZ.y * ResolutionZ.z;

                m_EulerParticlesTool.SetBuffer(m_NormalizeCCSFieldKernel, "vioScalarFieldData_normalizeCCSField", voVectorField.getGridDataX());
                m_EulerParticlesTool.SetBuffer(m_NormalizeCCSFieldKernel, "vWeightFieldData_normalizeCCSField", voWeightField.getGridDataX());

                CGlobalMacroAndFunc.dispatchKernel(m_EulerParticlesTool, m_NormalizeCCSFieldKernel, GridTotalThreadNumX);

                m_EulerParticlesTool.SetBuffer(m_NormalizeCCSFieldKernel, "vioScalarFieldData_normalizeCCSField", voVectorField.getGridDataY());
                m_EulerParticlesTool.SetBuffer(m_NormalizeCCSFieldKernel, "vWeightFieldData_normalizeCCSField", voWeightField.getGridDataY());

                CGlobalMacroAndFunc.dispatchKernel(m_EulerParticlesTool, m_NormalizeCCSFieldKernel, GridTotalThreadNumY);

                m_EulerParticlesTool.SetBuffer(m_NormalizeCCSFieldKernel, "vioScalarFieldData_normalizeCCSField", voVectorField.getGridDataZ());
                m_EulerParticlesTool.SetBuffer(m_NormalizeCCSFieldKernel, "vWeightFieldData_normalizeCCSField", voWeightField.getGridDataZ());

                CGlobalMacroAndFunc.dispatchKernel(m_EulerParticlesTool, m_NormalizeCCSFieldKernel, GridTotalThreadNumZ);
            }
        }

        public static void advectParticlesInvoker
        (
            ComputeBuffer vioParticlesPos,
            CFaceCenteredVectorField vVelField,
            float vDeltaT,
            float vCFLNumber,
            EAdvectionAccuracy vAdvectionAccuracy = EAdvectionAccuracy.RK2
        )
        {
            if (!m_Initialized) init();

            Vector3Int Resolution = vVelField.getResolution();
            Vector3 Origin = vVelField.getOrigin();
            Vector3 Spacing = vVelField.getSpacing();

            float MinGridSpacing = Mathf.Min(Mathf.Min(Spacing.x, Spacing.y), Spacing.z);

            Vector3Int ResolutionX = Resolution + new Vector3Int(1, 0, 0);
            Vector3 OriginX = Origin - new Vector3(Spacing.x / 2, 0, 0);
            Vector3Int ResolutionY = Resolution + new Vector3Int(0, 1, 0);
            Vector3 OriginY = Origin - new Vector3(0, Spacing.y / 2, 0);
            Vector3Int ResolutionZ = Resolution + new Vector3Int(0, 0, 1);
            Vector3 OriginZ = Origin - new Vector3(0, 0, Spacing.z / 2);

            int TotalThreadNum = (vioParticlesPos.count / 3);

            m_EulerParticlesTool.SetFloat("vDeltaT_advectParticles", vDeltaT);
            m_EulerParticlesTool.SetFloat("vCFLNumber_advectParticles", vCFLNumber);
            m_EulerParticlesTool.SetFloat("vMinGridSpacing_advectParticles", MinGridSpacing);
            m_EulerParticlesTool.SetFloats("vGridResolutionX_advectParticles", ResolutionX.x, ResolutionX.y, ResolutionX.z);
            m_EulerParticlesTool.SetFloats("vGridResolutionY_advectParticles", ResolutionY.x, ResolutionY.y, ResolutionY.z);
            m_EulerParticlesTool.SetFloats("vGridResolutionZ_advectParticles", ResolutionZ.x, ResolutionZ.y, ResolutionZ.z);
            m_EulerParticlesTool.SetFloats("vGridOriginX_advectParticles", OriginX.x, OriginX.y, OriginX.z);
            m_EulerParticlesTool.SetFloats("vGridOriginY_advectParticles", OriginY.x, OriginY.y, OriginY.z);
            m_EulerParticlesTool.SetFloats("vGridOriginZ_advectParticles", OriginZ.x, OriginZ.y, OriginZ.z);
            m_EulerParticlesTool.SetFloats("GridSpacing", Spacing.x, Spacing.y, Spacing.z);
            m_EulerParticlesTool.SetBuffer(m_AdvectParticlesKernel, "vVelFieldDataX_advectParticles", vVelField.getGridDataX());
            m_EulerParticlesTool.SetBuffer(m_AdvectParticlesKernel, "vVelFieldDataY_advectParticles", vVelField.getGridDataY());
            m_EulerParticlesTool.SetBuffer(m_AdvectParticlesKernel, "vVelFieldDataZ_advectParticles", vVelField.getGridDataZ());
            m_EulerParticlesTool.SetBuffer(m_AdvectParticlesKernel, "vioParticlesPos_advectParticles", vioParticlesPos);
            m_EulerParticlesTool.SetInt("AdvectionAcc_advectParticles", Convert.ToInt32(vAdvectionAccuracy));

            CGlobalMacroAndFunc.dispatchKernel(m_EulerParticlesTool, m_AdvectParticlesKernel, TotalThreadNum);
        }

        public static void buildFluidDomainInvoker(ComputeBuffer vParticlesPos, CCellCenteredScalarField voFluidDomainField)
        {
            if (!m_Initialized) init();

            Vector3Int Resolution = voFluidDomainField.getResolution();
            Vector3 Origin = voFluidDomainField.getOrigin();
            Vector3 Spacing = voFluidDomainField.getSpacing();

            int ParticlesTotalThreadNum = (int)(vParticlesPos.count / 3);

            voFluidDomainField.resize(Resolution, Origin, Spacing);

            m_EulerParticlesTool.SetInts("GridResolution", Resolution.x, Resolution.y, Resolution.z);
            m_EulerParticlesTool.SetFloats("GridOrigin", Origin.x, Origin.y, Origin.z);
            m_EulerParticlesTool.SetFloats("GridSpacing", Spacing.x, Spacing.y, Spacing.z);
            m_EulerParticlesTool.SetBuffer(m_BuildFluidDomainKernel, "vParticlesPos_buildFluidDomain", vParticlesPos);
            m_EulerParticlesTool.SetBuffer(m_BuildFluidDomainKernel, "vFluidDomainFieldData_buildFluidDomain", voFluidDomainField.getGridData());

            CGlobalMacroAndFunc.dispatchKernel(m_EulerParticlesTool, m_BuildFluidDomainKernel, ParticlesTotalThreadNum);
        }

        public static void statisticalFluidDensityInvoker(ComputeBuffer vParticlesPos, CCellCenteredScalarField voFluidDensityField)
        {
            if (!m_Initialized) init();

            Vector3Int Resolution = voFluidDensityField.getResolution();
            Vector3 Origin = voFluidDensityField.getOrigin();
            Vector3 Spacing = voFluidDensityField.getSpacing();

            int ParticlesTotalThreadNum = (int)(vParticlesPos.count / 3);

            voFluidDensityField.resize(Resolution, Origin, Spacing);

            m_EulerParticlesTool.SetInts("GridResolution", Resolution.x, Resolution.y, Resolution.z);
            m_EulerParticlesTool.SetFloats("GridOrigin", Origin.x, Origin.y, Origin.z);
            m_EulerParticlesTool.SetFloats("GridSpacing", Spacing.x, Spacing.y, Spacing.z);
            m_EulerParticlesTool.SetBuffer(m_StatisticalFluidDensityKernel, "vParticlesPos_statisticalFluidDensity", vParticlesPos);
            m_EulerParticlesTool.SetBuffer(m_StatisticalFluidDensityKernel, "voParticlesDensity_statisticalFluidDensity", voFluidDensityField.getGridData());

            CGlobalMacroAndFunc.dispatchKernel(m_EulerParticlesTool, m_StatisticalFluidDensityKernel, ParticlesTotalThreadNum);
        }

        public static void buildFluidMarkersInvoker(CCellCenteredScalarField vSolidSDFField, ComputeBuffer vParticlesPos, CCellCenteredScalarField voMarkersField)
        {
            if (!m_Initialized) init();

            CGlobalMacroAndFunc._ASSERTE(vSolidSDFField.getResolution() == voMarkersField.getResolution(), "固体SDF场与标记场的维度不匹配!");

            Vector3Int Resolution = vSolidSDFField.getResolution();
            Vector3 Origin = vSolidSDFField.getOrigin();
            Vector3 Spacing = vSolidSDFField.getSpacing();

            statisticalFluidDensityInvoker(vParticlesPos, voMarkersField);

            int GridTotalThreadNum = (int)(Resolution.x * Resolution.y * Resolution.z);

            m_EulerParticlesTool.SetBuffer(m_BuildFluidMarkersKernel, "vSolidSDFData_buildFluidMarkers", vSolidSDFField.getGridData());
            m_EulerParticlesTool.SetBuffer(m_BuildFluidMarkersKernel, "voMarkersFieldData_buildFluidMarkers", voMarkersField.getGridData());

            CGlobalMacroAndFunc.dispatchKernel(m_EulerParticlesTool, m_BuildFluidMarkersKernel, GridTotalThreadNum);
        }

        public static void buildFluidMarkersWithEmitBoxInvoker(CCellCenteredScalarField vSolidSDFField, SBoundingBox vEmitBox, ComputeBuffer vParticlesPos, CCellCenteredScalarField voMarkersField)
        {
            if (!m_Initialized) init();

            CGlobalMacroAndFunc._ASSERTE(vSolidSDFField.getResolution() == voMarkersField.getResolution(), "固体SDF场与标记场的维度不匹配!");

            Vector3Int Resolution = vSolidSDFField.getResolution();
            Vector3 Origin = vSolidSDFField.getOrigin();
            Vector3 Spacing = vSolidSDFField.getSpacing();

            statisticalFluidDensityInvoker(vParticlesPos, voMarkersField);

            int GridTotalThreadNum = (int)(Resolution.x * Resolution.y * Resolution.z);

            int GridThreadsPerBlock, GridBlocksPerGrid;
            CGlobalMacroAndFunc.fetchPropBlockGridSize1D(GridTotalThreadNum, out GridThreadsPerBlock, out GridBlocksPerGrid);

            m_EulerParticlesTool.SetInts("GridResolution", Resolution.x, Resolution.y, Resolution.z);
            m_EulerParticlesTool.SetFloats("GridOrigin", Origin.x, Origin.y, Origin.z);
            m_EulerParticlesTool.SetFloats("GridSpacing", Spacing.x, Spacing.y, Spacing.z);

            m_EulerParticlesTool.SetFloats("vBoxMin_buildFluidMarkersWithEmitBox", vEmitBox.Min.x, vEmitBox.Min.y, vEmitBox.Min.z);
            m_EulerParticlesTool.SetFloats("vBoxMax_buildFluidMarkersWithEmitBox", vEmitBox.Max.x, vEmitBox.Max.y, vEmitBox.Max.z);
            m_EulerParticlesTool.SetBuffer(m_BuildFluidMarkersWithEmitBoxKernel, "vSolidSDFData_buildFluidMarkersWithEmitBox", vSolidSDFField.getGridData());
            m_EulerParticlesTool.SetBuffer(m_BuildFluidMarkersWithEmitBoxKernel, "voMarkersFieldData_buildFluidMarkersWithEmitBox", voMarkersField.getGridData());

            CGlobalMacroAndFunc.dispatchKernel(m_EulerParticlesTool, m_BuildFluidMarkersWithEmitBoxKernel, GridTotalThreadNum);
        }
    }
}
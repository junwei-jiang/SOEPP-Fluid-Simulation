using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using EulerFluidEngine;

namespace EulerFluidEngine
{
    public class CSemiLagrangian
    {
        #region Constructor&Resize
        public CSemiLagrangian() { }

        public CSemiLagrangian(Vector3Int vResolution, Vector3 vOrigin, Vector3 vSpacing)
        {
            resizeSemiLagrangian(vResolution, vOrigin, vSpacing);
        }
        ~CSemiLagrangian()
        {
            free();
        }

        public void free()
        {
            if(m_AdvectionInputPointPosFieldCC != null)
                m_AdvectionInputPointPosFieldCC.free();
            if (m_AdvectionOutputPointPosFieldCC != null)
                m_AdvectionOutputPointPosFieldCC.free();
            if (m_AdvectionInputPointPosXFieldFC != null) 
                m_AdvectionInputPointPosXFieldFC.free();
            if (m_AdvectionInputPointPosYFieldFC != null) 
                m_AdvectionInputPointPosYFieldFC.free();
            if (m_AdvectionInputPointPosZFieldFC != null) 
                m_AdvectionInputPointPosZFieldFC.free();
            if (m_AdvectionOutputPointPosXFieldFC != null) 
                m_AdvectionOutputPointPosXFieldFC.free();
            if (m_AdvectionOutputPointPosYFieldFC != null) 
                m_AdvectionOutputPointPosYFieldFC.free();
            if (m_AdvectionOutputPointPosZFieldFC != null) 
                m_AdvectionOutputPointPosZFieldFC.free();
            if (m_MacCormackAdvectionOutputPointPosXFieldFC != null)
                m_MacCormackAdvectionOutputPointPosXFieldFC.free();
            if (m_MacCormackAdvectionOutputPointPosYFieldFC != null)
                m_MacCormackAdvectionOutputPointPosYFieldFC.free();
            if (m_MacCormackAdvectionOutputPointPosZFieldFC != null)
                m_MacCormackAdvectionOutputPointPosZFieldFC.free();

            if (m_BackTraceInputPointVelField != null) 
                m_BackTraceInputPointVelField.free();
            if (m_BackTraceMidPointPosField != null) 
                m_BackTraceMidPointPosField.free();
            if (m_BackTraceMidPointVelField != null) 
                m_BackTraceMidPointVelField.free();
            if (m_BackTraceTwoThirdsPointPosField != null) 
                m_BackTraceTwoThirdsPointPosField.free();
            if (m_BackTraceTwoThirdsPointVelField != null) 
                m_BackTraceTwoThirdsPointVelField.free();

            if (m_BackTraceInputPointVelFieldX != null) 
                m_BackTraceInputPointVelFieldX.free();
            if (m_BackTraceMidPointPosFieldX != null) 
                m_BackTraceMidPointPosFieldX.free();
            if (m_BackTraceMidPointVelFieldX != null) 
                m_BackTraceMidPointVelFieldX.free();
            if (m_BackTraceTwoThirdsPointPosFieldX != null) 
                m_BackTraceTwoThirdsPointPosFieldX.free();
            if (m_BackTraceTwoThirdsPointVelFieldX != null) 
                m_BackTraceTwoThirdsPointVelFieldX.free();

            if (m_BackTraceInputPointVelFieldY != null) 
                m_BackTraceInputPointVelFieldY.free();
            if (m_BackTraceMidPointPosFieldY != null) 
                m_BackTraceMidPointPosFieldY.free();
            if (m_BackTraceMidPointVelFieldY != null) 
                m_BackTraceMidPointVelFieldY.free();
            if (m_BackTraceTwoThirdsPointPosFieldY != null) 
                m_BackTraceTwoThirdsPointPosFieldY.free();
            if (m_BackTraceTwoThirdsPointVelFieldY != null) 
                m_BackTraceTwoThirdsPointVelFieldY.free();

            if (m_BackTraceInputPointVelFieldZ != null) 
                m_BackTraceInputPointVelFieldZ.free();
            if (m_BackTraceMidPointPosFieldZ != null) 
                m_BackTraceMidPointPosFieldZ.free();
            if (m_BackTraceMidPointVelFieldZ != null) 
                m_BackTraceMidPointVelFieldZ.free();
            if (m_BackTraceTwoThirdsPointPosFieldZ != null) 
                m_BackTraceTwoThirdsPointPosFieldZ.free();
            if (m_BackTraceTwoThirdsPointVelFieldZ != null) 
                m_BackTraceTwoThirdsPointVelFieldZ.free();

            if (m_MacCormackForwardScalarField != null)
                m_MacCormackForwardScalarField.free();
            if (m_MacCormackForwardVectorField != null)
                m_MacCormackForwardVectorField.free();
            if (m_MacCormackForwardVelField != null)
                m_MacCormackForwardVelField.free();
        }

        public void resizeSemiLagrangian(Vector3Int vResolution, Vector3 vOrigin, Vector3 vSpacing)
        {
            m_Resolution = new Vector3Int(vResolution.x, vResolution.y, vResolution.z);
            Vector3Int vResolutionX = new Vector3Int(vResolution.x + 1, vResolution.y, vResolution.z);
            Vector3Int vResolutionY = new Vector3Int(vResolution.x, vResolution.y + 1, vResolution.z);
            Vector3Int vResolutionZ = new Vector3Int(vResolution.x, vResolution.y, vResolution.z + 1);

            int Size = vResolution.x * vResolution.y * vResolution.z;
            int SizeX = vResolutionX.x * vResolutionX.y * vResolutionX.z;
            int SizeY = vResolutionY.x * vResolutionY.y * vResolutionY.z;
            int SizeZ = vResolutionZ.x * vResolutionZ.y * vResolutionZ.z;

            float[] CCPosFieldDataX = new float[Size];
            float[] CCPosFieldDataY = new float[Size];
            float[] CCPosFieldDataZ = new float[Size];
            float[] FCPosFieldDataXX = new float[SizeX];
            float[] FCPosFieldDataXY = new float[SizeX];
            float[] FCPosFieldDataXZ = new float[SizeX];
            float[] FCPosFieldDataYX = new float[SizeY];
            float[] FCPosFieldDataYY = new float[SizeY];
            float[] FCPosFieldDataYZ = new float[SizeY];
            float[] FCPosFieldDataZX = new float[SizeZ];
            float[] FCPosFieldDataZY = new float[SizeZ];
            float[] FCPosFieldDataZZ = new float[SizeZ];

            for (int z = 0; z < vResolution.z; z++)
            {
                for (int y = 0; y < vResolution.y; y++)
                {
                    for (int x = 0; x < vResolution.x; x++)
                    {
                        CCPosFieldDataX[z * vResolution.x * vResolution.y + y * vResolution.x + x] = (x + 0.5f) * vSpacing.x + vOrigin.x;
                        CCPosFieldDataY[z * vResolution.x * vResolution.y + y * vResolution.x + x] = (y + 0.5f) * vSpacing.y + vOrigin.y;
                        CCPosFieldDataZ[z * vResolution.x * vResolution.y + y * vResolution.x + x] = (z + 0.5f) * vSpacing.z + vOrigin.z;
                    }
                }
            }

            for (int z = 0; z < vResolutionX.z; z++)
            {
                for (int y = 0; y < vResolutionX.y; y++)
                {
                    for (int x = 0; x < vResolutionX.x; x++)
                    {
                        FCPosFieldDataXX[z * vResolutionX.x * vResolutionX.y + y * vResolutionX.x + x] = x * vSpacing.x + vOrigin.x;
                        FCPosFieldDataXY[z * vResolutionX.x * vResolutionX.y + y * vResolutionX.x + x] = (y + 0.5f) * vSpacing.y + vOrigin.y;
                        FCPosFieldDataXZ[z * vResolutionX.x * vResolutionX.y + y * vResolutionX.x + x] = (z + 0.5f) * vSpacing.z + vOrigin.z;
                    }
                }
            }

            for (int z = 0; z < vResolutionY.z; z++)
            {
                for (int y = 0; y < vResolutionY.y; y++)
                {
                    for (int x = 0; x < vResolutionY.x; x++)
                    {
                        FCPosFieldDataYX[z * vResolutionY.x * vResolutionY.y + y * vResolutionY.x + x] = (x + 0.5f) * vSpacing.x + vOrigin.x;
                        FCPosFieldDataYY[z * vResolutionY.x * vResolutionY.y + y * vResolutionY.x + x] = y * vSpacing.y + vOrigin.y;
                        FCPosFieldDataYZ[z * vResolutionY.x * vResolutionY.y + y * vResolutionY.x + x] = (z + 0.5f) * vSpacing.z + vOrigin.z;
                    }
                }
            }

            for (int z = 0; z < vResolutionZ.z; z++)
            {
                for (int y = 0; y < vResolutionZ.y; y++)
                {
                    for (int x = 0; x < vResolutionZ.x; x++)
                    {
                        FCPosFieldDataZX[z * vResolutionZ.x * vResolutionZ.y + y * vResolutionZ.x + x] = (x + 0.5f) * vSpacing.x + vOrigin.x;
                        FCPosFieldDataZY[z * vResolutionZ.x * vResolutionZ.y + y * vResolutionZ.x + x] = (y + 0.5f) * vSpacing.y + vOrigin.y;
                        FCPosFieldDataZZ[z * vResolutionZ.x * vResolutionZ.y + y * vResolutionZ.x + x] = z * vSpacing.z + vOrigin.z;
                    }
                }
            }

            free();

            m_AdvectionInputPointPosFieldCC = new CCellCenteredVectorField(vResolution, vOrigin, vSpacing, CCPosFieldDataX, CCPosFieldDataY, CCPosFieldDataZ);
            m_AdvectionOutputPointPosFieldCC = new CCellCenteredVectorField(vResolution, vOrigin, vSpacing, CCPosFieldDataX, CCPosFieldDataY, CCPosFieldDataZ);
            m_AdvectionInputPointPosXFieldFC = new CCellCenteredVectorField(vResolutionX, vOrigin, vSpacing, FCPosFieldDataXX, FCPosFieldDataXY, FCPosFieldDataXZ);
            m_AdvectionInputPointPosYFieldFC = new CCellCenteredVectorField(vResolutionY, vOrigin, vSpacing, FCPosFieldDataYX, FCPosFieldDataYY, FCPosFieldDataYZ);
            m_AdvectionInputPointPosZFieldFC = new CCellCenteredVectorField(vResolutionZ, vOrigin, vSpacing, FCPosFieldDataZX, FCPosFieldDataZY, FCPosFieldDataZZ);
            m_AdvectionOutputPointPosXFieldFC = new CCellCenteredVectorField(vResolutionX, vOrigin, vSpacing, FCPosFieldDataXX, FCPosFieldDataXY, FCPosFieldDataXZ);
            m_AdvectionOutputPointPosYFieldFC = new CCellCenteredVectorField(vResolutionY, vOrigin, vSpacing, FCPosFieldDataYX, FCPosFieldDataYY, FCPosFieldDataYZ);
            m_AdvectionOutputPointPosZFieldFC = new CCellCenteredVectorField(vResolutionZ, vOrigin, vSpacing, FCPosFieldDataZX, FCPosFieldDataZY, FCPosFieldDataZZ);
            m_MacCormackAdvectionOutputPointPosXFieldFC = new CCellCenteredVectorField(vResolutionX, vOrigin, vSpacing, FCPosFieldDataXX, FCPosFieldDataXY, FCPosFieldDataXZ);
            m_MacCormackAdvectionOutputPointPosYFieldFC = new CCellCenteredVectorField(vResolutionY, vOrigin, vSpacing, FCPosFieldDataYX, FCPosFieldDataYY, FCPosFieldDataYZ);
            m_MacCormackAdvectionOutputPointPosZFieldFC = new CCellCenteredVectorField(vResolutionZ, vOrigin, vSpacing, FCPosFieldDataZX, FCPosFieldDataZY, FCPosFieldDataZZ); 

            m_BackTraceInputPointVelField = new CCellCenteredVectorField(vResolution, vOrigin, vSpacing);
            m_BackTraceMidPointPosField = new CCellCenteredVectorField(vResolution, vOrigin, vSpacing);
            m_BackTraceMidPointVelField = new CCellCenteredVectorField(vResolution, vOrigin, vSpacing);
            m_BackTraceTwoThirdsPointPosField = new CCellCenteredVectorField(vResolution, vOrigin, vSpacing);
            m_BackTraceTwoThirdsPointVelField = new CCellCenteredVectorField(vResolution, vOrigin, vSpacing);

            m_BackTraceInputPointVelFieldX = new CCellCenteredVectorField(vResolutionX, vOrigin, vSpacing);
            m_BackTraceMidPointPosFieldX = new CCellCenteredVectorField(vResolutionX, vOrigin, vSpacing);
            m_BackTraceMidPointVelFieldX = new CCellCenteredVectorField(vResolutionX, vOrigin, vSpacing);
            m_BackTraceTwoThirdsPointPosFieldX = new CCellCenteredVectorField(vResolutionX, vOrigin, vSpacing);
            m_BackTraceTwoThirdsPointVelFieldX = new CCellCenteredVectorField(vResolutionX, vOrigin, vSpacing);

            m_BackTraceInputPointVelFieldY = new CCellCenteredVectorField(vResolutionY, vOrigin, vSpacing);
            m_BackTraceMidPointPosFieldY = new CCellCenteredVectorField(vResolutionY, vOrigin, vSpacing);
            m_BackTraceMidPointVelFieldY = new CCellCenteredVectorField(vResolutionY, vOrigin, vSpacing);
            m_BackTraceTwoThirdsPointPosFieldY = new CCellCenteredVectorField(vResolutionY, vOrigin, vSpacing);
            m_BackTraceTwoThirdsPointVelFieldY = new CCellCenteredVectorField(vResolutionY, vOrigin, vSpacing);

            m_BackTraceInputPointVelFieldZ = new CCellCenteredVectorField(vResolutionZ, vOrigin, vSpacing);
            m_BackTraceMidPointPosFieldZ = new CCellCenteredVectorField(vResolutionZ, vOrigin, vSpacing);
            m_BackTraceMidPointVelFieldZ = new CCellCenteredVectorField(vResolutionZ, vOrigin, vSpacing);
            m_BackTraceTwoThirdsPointPosFieldZ = new CCellCenteredVectorField(vResolutionZ, vOrigin, vSpacing);
            m_BackTraceTwoThirdsPointVelFieldZ = new CCellCenteredVectorField(vResolutionZ, vOrigin, vSpacing);

            m_MacCormackForwardScalarField = new CCellCenteredScalarField (vResolution, vOrigin, vSpacing);
            m_MacCormackForwardVectorField = new CCellCenteredVectorField (vResolution, vOrigin, vSpacing);
            m_MacCormackForwardVelField = new CFaceCenteredVectorField(vResolution, vOrigin, vSpacing);
        }
        #endregion

        #region Get&Set
        public CCellCenteredVectorField getBackTraceStartPointPosFieldX()
        {
            return m_AdvectionInputPointPosXFieldFC;
        }
        public CCellCenteredVectorField getBackTraceStartPointPosFieldY()
        {
            return m_AdvectionInputPointPosYFieldFC;
        }
        public CCellCenteredVectorField getBackTraceStartPointPosFieldZ()
        {
            return m_AdvectionInputPointPosZFieldFC;
        }
        public CCellCenteredVectorField getBackTraceMidPointPosFieldX()
        {
            return m_BackTraceMidPointPosFieldX;
        }
        public CCellCenteredVectorField getBackTraceMidPointPosFieldY()
        {
            return m_BackTraceMidPointPosFieldY;
        }
        public CCellCenteredVectorField getBackTraceMidPointPosFieldZ()
        {
            return m_BackTraceMidPointPosFieldZ;
        }
        public CCellCenteredVectorField getInputPointPosFieldCC()
        {
            return m_AdvectionInputPointPosFieldCC;
        }
        #endregion

        #region CoreMethods

        #region Semi-Lagrangian
        public void advect
        (
            CCellCenteredScalarField vInputField,
            CFaceCenteredVectorField vVelocityField,
            float vDeltaT,
            CCellCenteredScalarField voOutputField,
            EAdvectionAccuracy vAdvectionAccuracy = EAdvectionAccuracy.RK2,
            ESamplingAlgorithm vSamplingAlgorithm = ESamplingAlgorithm.LINEAR
        )
        {
            backTrace(m_AdvectionInputPointPosFieldCC, vVelocityField, vDeltaT, m_AdvectionOutputPointPosFieldCC, vAdvectionAccuracy, vSamplingAlgorithm);
            vInputField.sampleField(m_AdvectionOutputPointPosFieldCC, voOutputField, vSamplingAlgorithm);
        }

        public void advect
        (
            CCellCenteredVectorField vInputField,
            CFaceCenteredVectorField vVelocityField,
            float vDeltaT,
            CCellCenteredVectorField voOutputField,
            EAdvectionAccuracy vAdvectionAccuracy = EAdvectionAccuracy.RK2,
            ESamplingAlgorithm vSamplingAlgorithm = ESamplingAlgorithm.LINEAR
        )
        {
            backTrace(m_AdvectionInputPointPosFieldCC, vVelocityField, vDeltaT, m_AdvectionOutputPointPosFieldCC, vAdvectionAccuracy, vSamplingAlgorithm);
            vInputField.sampleField(m_AdvectionOutputPointPosFieldCC, voOutputField, vSamplingAlgorithm);
        }

        public void advect
        (
            CFaceCenteredVectorField vInputField,
            CFaceCenteredVectorField vVelocityField,
            float vDeltaT,
            CFaceCenteredVectorField voOutputField,
            EAdvectionAccuracy vAdvectionAccuracy = EAdvectionAccuracy.RK2,
            ESamplingAlgorithm vSamplingAlgorithm = ESamplingAlgorithm.LINEAR
        )
        {
            backTrace(m_AdvectionInputPointPosXFieldFC, vVelocityField, vDeltaT, m_AdvectionOutputPointPosXFieldFC, vAdvectionAccuracy, vSamplingAlgorithm);
            backTrace(m_AdvectionInputPointPosYFieldFC, vVelocityField, vDeltaT, m_AdvectionOutputPointPosYFieldFC, vAdvectionAccuracy, vSamplingAlgorithm);
            backTrace(m_AdvectionInputPointPosZFieldFC, vVelocityField, vDeltaT, m_AdvectionOutputPointPosZFieldFC, vAdvectionAccuracy, vSamplingAlgorithm);
            vInputField.sampleField(m_AdvectionOutputPointPosXFieldFC, m_AdvectionOutputPointPosYFieldFC, m_AdvectionOutputPointPosZFieldFC, voOutputField, vSamplingAlgorithm);
        }
        #endregion

        #region MacCormack
        public void advectMacCormack
        (
            CCellCenteredScalarField vInputField,
            CFaceCenteredVectorField vVelocityField,
            float vDeltaT,
            CCellCenteredScalarField voOutputField,
            EAdvectionAccuracy vAdvectionAccuracy = EAdvectionAccuracy.RK2,
            ESamplingAlgorithm vSamplingAlgorithm = ESamplingAlgorithm.LINEAR
        )
        {
            backTrace(m_AdvectionInputPointPosFieldCC, vVelocityField, vDeltaT, m_AdvectionOutputPointPosFieldCC, vAdvectionAccuracy, vSamplingAlgorithm);
            vInputField.sampleField(m_AdvectionOutputPointPosFieldCC, voOutputField, vSamplingAlgorithm);

            backTrace(m_AdvectionInputPointPosFieldCC, vVelocityField, -vDeltaT, m_AdvectionOutputPointPosFieldCC, vAdvectionAccuracy, vSamplingAlgorithm);
            voOutputField.sampleField(m_AdvectionOutputPointPosFieldCC, m_MacCormackForwardScalarField, vSamplingAlgorithm);

            m_MacCormackForwardScalarField.scale(-1.0f);
            m_MacCormackForwardScalarField.plusAlphaX(vInputField, 1.0f);
            m_MacCormackForwardScalarField.scale(0.5f);
            voOutputField.plusAlphaX(m_MacCormackForwardScalarField, 1.0f);

            backTrace(m_AdvectionInputPointPosFieldCC, vVelocityField, vDeltaT, m_AdvectionOutputPointPosFieldCC, vAdvectionAccuracy, vSamplingAlgorithm);
            CFieldMathToolInvoker.clampCCSFieldExtremaInvoker(vInputField, voOutputField, m_AdvectionOutputPointPosFieldCC);
        }

        public void advectMacCormack
        (
            CCellCenteredVectorField vInputField,
            CFaceCenteredVectorField vVelocityField,
            float vDeltaT,
            CCellCenteredVectorField voOutputField,
            EAdvectionAccuracy vAdvectionAccuracy = EAdvectionAccuracy.RK2,
            ESamplingAlgorithm vSamplingAlgorithm = ESamplingAlgorithm.LINEAR
        )
        {
            backTrace(m_AdvectionInputPointPosFieldCC, vVelocityField, vDeltaT, m_AdvectionOutputPointPosFieldCC, vAdvectionAccuracy, vSamplingAlgorithm);
            vInputField.sampleField(m_AdvectionOutputPointPosFieldCC, voOutputField, vSamplingAlgorithm);

            backTrace(m_AdvectionInputPointPosFieldCC, vVelocityField, -vDeltaT, m_AdvectionOutputPointPosFieldCC, vAdvectionAccuracy, vSamplingAlgorithm);
            voOutputField.sampleField(m_AdvectionOutputPointPosFieldCC, m_MacCormackForwardVectorField, vSamplingAlgorithm);

            m_MacCormackForwardVectorField.scale(-1.0f);
            m_MacCormackForwardVectorField.plusAlphaX(vInputField, 1.0f);
            m_MacCormackForwardVectorField.scale(0.5f);
            voOutputField.plusAlphaX(m_MacCormackForwardVectorField, 1.0f);

            backTrace(m_AdvectionInputPointPosFieldCC, vVelocityField, vDeltaT, m_AdvectionOutputPointPosFieldCC, vAdvectionAccuracy, vSamplingAlgorithm);
            CFieldMathToolInvoker.clampCCVFieldExtremaInvoker(vInputField, voOutputField, m_AdvectionOutputPointPosFieldCC);
        }

        public void advectMacCormack
        (
            CFaceCenteredVectorField vInputField,
            CFaceCenteredVectorField vVelocityField,
            float vDeltaT,
            CFaceCenteredVectorField voOutputField,
            EAdvectionAccuracy vAdvectionAccuracy = EAdvectionAccuracy.RK2,
            ESamplingAlgorithm vSamplingAlgorithm = ESamplingAlgorithm.LINEAR
        )
        {
            backTrace(m_AdvectionInputPointPosXFieldFC, vVelocityField, vDeltaT, m_AdvectionOutputPointPosXFieldFC, vAdvectionAccuracy, vSamplingAlgorithm);
            backTrace(m_AdvectionInputPointPosYFieldFC, vVelocityField, vDeltaT, m_AdvectionOutputPointPosYFieldFC, vAdvectionAccuracy, vSamplingAlgorithm);
            backTrace(m_AdvectionInputPointPosZFieldFC, vVelocityField, vDeltaT, m_AdvectionOutputPointPosZFieldFC, vAdvectionAccuracy, vSamplingAlgorithm);
            vInputField.sampleField(m_AdvectionOutputPointPosXFieldFC, m_AdvectionOutputPointPosYFieldFC, m_AdvectionOutputPointPosZFieldFC, voOutputField, vSamplingAlgorithm);

            backTrace(m_AdvectionInputPointPosXFieldFC, vVelocityField, -vDeltaT, m_AdvectionOutputPointPosXFieldFC, vAdvectionAccuracy, vSamplingAlgorithm);
            backTrace(m_AdvectionInputPointPosYFieldFC, vVelocityField, -vDeltaT, m_AdvectionOutputPointPosYFieldFC, vAdvectionAccuracy, vSamplingAlgorithm);
            backTrace(m_AdvectionInputPointPosZFieldFC, vVelocityField, -vDeltaT, m_AdvectionOutputPointPosZFieldFC, vAdvectionAccuracy, vSamplingAlgorithm);
            voOutputField.sampleField(m_AdvectionOutputPointPosXFieldFC, m_AdvectionOutputPointPosYFieldFC, m_AdvectionOutputPointPosZFieldFC, m_MacCormackForwardVelField, vSamplingAlgorithm);
            
            m_MacCormackForwardVelField.scale(-1.0f);
            m_MacCormackForwardVelField.plusAlphaX(vInputField, 1.0f);
            m_MacCormackForwardVelField.scale(0.5f);
            voOutputField.plusAlphaX(m_MacCormackForwardVelField, 1.0f);

            backTrace(m_AdvectionInputPointPosXFieldFC, vVelocityField, vDeltaT, m_AdvectionOutputPointPosXFieldFC, vAdvectionAccuracy, vSamplingAlgorithm);
            backTrace(m_AdvectionInputPointPosYFieldFC, vVelocityField, vDeltaT, m_AdvectionOutputPointPosYFieldFC, vAdvectionAccuracy, vSamplingAlgorithm);
            backTrace(m_AdvectionInputPointPosZFieldFC, vVelocityField, vDeltaT, m_AdvectionOutputPointPosZFieldFC, vAdvectionAccuracy, vSamplingAlgorithm);
            CFieldMathToolInvoker.clampFCVFieldExtremaInvoker(vInputField, voOutputField, m_AdvectionOutputPointPosXFieldFC, m_AdvectionOutputPointPosYFieldFC, m_AdvectionOutputPointPosZFieldFC);
        }
        #endregion

        #region Iteration
        public void advectWithIteration
        (
            CCellCenteredScalarField vInputField,
            CFaceCenteredVectorField vPreVelocityField,
            CFaceCenteredVectorField vCurVelocityField,
            float vDeltaT,
            CCellCenteredScalarField voOutputField,
            EAdvectionAccuracy vAdvectionAccuracy = EAdvectionAccuracy.RK2,
            ESamplingAlgorithm vSamplingAlgorithm = ESamplingAlgorithm.LINEAR
        )
        {
            backTraceWithIteration(m_AdvectionInputPointPosFieldCC, vPreVelocityField, vCurVelocityField, vDeltaT, m_AdvectionOutputPointPosFieldCC, vAdvectionAccuracy, vSamplingAlgorithm);
            vInputField.sampleField(m_AdvectionOutputPointPosFieldCC, voOutputField, vSamplingAlgorithm);
        }

        public void advectWithIteration
        (
            CCellCenteredVectorField vInputField,
            CFaceCenteredVectorField vPreVelocityField,
            CFaceCenteredVectorField vCurVelocityField,
            float vDeltaT,
            CCellCenteredVectorField voOutputField,
            EAdvectionAccuracy vAdvectionAccuracy = EAdvectionAccuracy.RK2,
            ESamplingAlgorithm vSamplingAlgorithm = ESamplingAlgorithm.LINEAR
        )
        {
            backTraceWithIteration(m_AdvectionInputPointPosFieldCC, vPreVelocityField, vCurVelocityField, vDeltaT, m_AdvectionOutputPointPosFieldCC, vAdvectionAccuracy, vSamplingAlgorithm);
            vInputField.sampleField(m_AdvectionOutputPointPosFieldCC, voOutputField, vSamplingAlgorithm);
        }

        public void advectWithIteration
        (
            CFaceCenteredVectorField vInputField,
            CFaceCenteredVectorField vPreVelocityField,
            CFaceCenteredVectorField vCurVelocityField,
            float vDeltaT,
            CFaceCenteredVectorField voOutputField,
            EAdvectionAccuracy vAdvectionAccuracy = EAdvectionAccuracy.RK2,
            ESamplingAlgorithm vSamplingAlgorithm = ESamplingAlgorithm.LINEAR
        )
        {
            backTraceWithIteration(m_AdvectionInputPointPosXFieldFC, vPreVelocityField, vCurVelocityField, vDeltaT, m_AdvectionOutputPointPosXFieldFC, vAdvectionAccuracy, vSamplingAlgorithm);
            backTraceWithIteration(m_AdvectionInputPointPosYFieldFC, vPreVelocityField, vCurVelocityField, vDeltaT, m_AdvectionOutputPointPosYFieldFC, vAdvectionAccuracy, vSamplingAlgorithm);
            backTraceWithIteration(m_AdvectionInputPointPosZFieldFC, vPreVelocityField, vCurVelocityField, vDeltaT, m_AdvectionOutputPointPosZFieldFC, vAdvectionAccuracy, vSamplingAlgorithm);
            vInputField.sampleField(m_AdvectionOutputPointPosXFieldFC, m_AdvectionOutputPointPosYFieldFC, m_AdvectionOutputPointPosZFieldFC, voOutputField, vSamplingAlgorithm);
        }
        #endregion

        public void backTrace
        (
            CCellCenteredVectorField vInputPosField,
            CFaceCenteredVectorField vVelocityField,
            float vDeltaT,
            CCellCenteredVectorField voOutputPosField,
            EAdvectionAccuracy vAdvectionAccuracy = EAdvectionAccuracy.RK2,
            ESamplingAlgorithm vSamplingAlgorithm = ESamplingAlgorithm.LINEAR
        )
        {
            CGlobalMacroAndFunc._ASSERTE(voOutputPosField.getResolution() == vInputPosField.getResolution());

            if (vInputPosField.getResolution() == m_Resolution)
            {
                voOutputPosField.resize(vInputPosField);

                vVelocityField.sampleField(vInputPosField, m_BackTraceInputPointVelField, vSamplingAlgorithm);

                if (vAdvectionAccuracy == EAdvectionAccuracy.RK1)
                {
                    voOutputPosField.plusAlphaX(m_BackTraceInputPointVelField, -vDeltaT);
                }
                else if (vAdvectionAccuracy == EAdvectionAccuracy.RK2)
                {
                    m_BackTraceMidPointPosField.resize(vInputPosField);
                    m_BackTraceMidPointPosField.plusAlphaX(m_BackTraceInputPointVelField, -0.5f * vDeltaT);
                    vVelocityField.sampleField(m_BackTraceMidPointPosField, m_BackTraceMidPointVelField, vSamplingAlgorithm);
                    voOutputPosField.plusAlphaX(m_BackTraceMidPointVelField, -vDeltaT);
                }
                else if (vAdvectionAccuracy == EAdvectionAccuracy.RK3)
                {
                    m_BackTraceMidPointPosField.resize(vInputPosField);
                    m_BackTraceMidPointPosField.plusAlphaX(m_BackTraceInputPointVelField, -0.5f * vDeltaT);
                    vVelocityField.sampleField(m_BackTraceMidPointPosField, m_BackTraceMidPointVelField, vSamplingAlgorithm);
                    m_BackTraceTwoThirdsPointPosField.resize(vInputPosField);
                    m_BackTraceTwoThirdsPointPosField.plusAlphaX(m_BackTraceMidPointVelField, -0.75f * vDeltaT);
                    vVelocityField.sampleField(m_BackTraceTwoThirdsPointPosField, m_BackTraceTwoThirdsPointVelField, vSamplingAlgorithm);
                    voOutputPosField.plusAlphaX(m_BackTraceInputPointVelField, -(2.0f / 9.0f) * vDeltaT);
                    voOutputPosField.plusAlphaX(m_BackTraceMidPointVelField, -(3.0f / 9.0f) * vDeltaT);
                    voOutputPosField.plusAlphaX(m_BackTraceTwoThirdsPointVelField, -(4.0f / 9.0f) * vDeltaT);
                }
                else
                {

                }
            }
            else if (vInputPosField.getResolution() == m_Resolution + new Vector3Int(1, 0, 0))
            {
                voOutputPosField.resize(vInputPosField);

                vVelocityField.sampleField(vInputPosField, m_BackTraceInputPointVelFieldX, vSamplingAlgorithm);

                if (vAdvectionAccuracy == EAdvectionAccuracy.RK1)
                {
                    voOutputPosField.plusAlphaX(m_BackTraceInputPointVelFieldX, -vDeltaT);
                }
                else if (vAdvectionAccuracy == EAdvectionAccuracy.RK2)
                {
                    m_BackTraceMidPointPosFieldX.resize(vInputPosField);
                    m_BackTraceMidPointPosFieldX.plusAlphaX(m_BackTraceInputPointVelFieldX, -0.5f * vDeltaT);
                    vVelocityField.sampleField(m_BackTraceMidPointPosFieldX, m_BackTraceMidPointVelFieldX, vSamplingAlgorithm);
                    voOutputPosField.plusAlphaX(m_BackTraceMidPointVelFieldX, -vDeltaT);
                }
                else if (vAdvectionAccuracy == EAdvectionAccuracy.RK3)
                {
                    m_BackTraceMidPointPosFieldX.resize(vInputPosField);
                    m_BackTraceMidPointPosFieldX.plusAlphaX(m_BackTraceInputPointVelFieldX, -0.5f * vDeltaT);
                    vVelocityField.sampleField(m_BackTraceMidPointPosFieldX, m_BackTraceMidPointVelFieldX, vSamplingAlgorithm);
                    m_BackTraceTwoThirdsPointPosFieldX.resize(vInputPosField);
                    m_BackTraceTwoThirdsPointPosFieldX.plusAlphaX(m_BackTraceMidPointVelFieldX, -0.75f * vDeltaT);
                    vVelocityField.sampleField(m_BackTraceTwoThirdsPointPosFieldX, m_BackTraceTwoThirdsPointVelFieldX, vSamplingAlgorithm);
                    voOutputPosField.plusAlphaX(m_BackTraceInputPointVelFieldX, -(2.0f / 9.0f) * vDeltaT);
                    voOutputPosField.plusAlphaX(m_BackTraceMidPointVelFieldX, -(3.0f / 9.0f) * vDeltaT);
                    voOutputPosField.plusAlphaX(m_BackTraceTwoThirdsPointVelFieldX, -(4.0f / 9.0f) * vDeltaT);
                }
                else
                {

                }
            }
            else if (vInputPosField.getResolution() == m_Resolution + new Vector3Int(0, 1, 0))
            {
                voOutputPosField.resize(vInputPosField);

                vVelocityField.sampleField(vInputPosField, m_BackTraceInputPointVelFieldY, vSamplingAlgorithm);

                if (vAdvectionAccuracy == EAdvectionAccuracy.RK1)
                {
                    voOutputPosField.plusAlphaX(m_BackTraceInputPointVelFieldY, -vDeltaT);
                }
                else if (vAdvectionAccuracy == EAdvectionAccuracy.RK2)
                {
                    m_BackTraceMidPointPosFieldY.resize(vInputPosField);
                    m_BackTraceMidPointPosFieldY.plusAlphaX(m_BackTraceInputPointVelFieldY, -0.5f * vDeltaT);
                    vVelocityField.sampleField(m_BackTraceMidPointPosFieldY, m_BackTraceMidPointVelFieldY, vSamplingAlgorithm);
                    voOutputPosField.plusAlphaX(m_BackTraceMidPointVelFieldY, -vDeltaT);
                }
                else if (vAdvectionAccuracy == EAdvectionAccuracy.RK3)
                {
                    m_BackTraceMidPointPosFieldY.resize(vInputPosField);
                    m_BackTraceMidPointPosFieldY.plusAlphaX(m_BackTraceInputPointVelFieldY, -0.5f * vDeltaT);
                    vVelocityField.sampleField(m_BackTraceMidPointPosFieldY, m_BackTraceMidPointVelFieldY, vSamplingAlgorithm);
                    m_BackTraceTwoThirdsPointPosFieldY.resize(vInputPosField);
                    m_BackTraceTwoThirdsPointPosFieldY.plusAlphaX(m_BackTraceMidPointVelFieldY, -0.75f * vDeltaT);
                    vVelocityField.sampleField(m_BackTraceTwoThirdsPointPosFieldY, m_BackTraceTwoThirdsPointVelFieldY, vSamplingAlgorithm);
                    voOutputPosField.plusAlphaX(m_BackTraceInputPointVelFieldY, -(2.0f / 9.0f) * vDeltaT);
                    voOutputPosField.plusAlphaX(m_BackTraceMidPointVelFieldY, -(3.0f / 9.0f) * vDeltaT);
                    voOutputPosField.plusAlphaX(m_BackTraceTwoThirdsPointVelFieldY, -(4.0f / 9.0f) * vDeltaT);
                }
                else
                {

                }
            }
            else if (vInputPosField.getResolution() == m_Resolution + new Vector3Int(0, 0, 1))
            {
                voOutputPosField.resize(vInputPosField);

                vVelocityField.sampleField(vInputPosField, m_BackTraceInputPointVelFieldZ, vSamplingAlgorithm);

                if (vAdvectionAccuracy == EAdvectionAccuracy.RK1)
                {
                    voOutputPosField.plusAlphaX(m_BackTraceInputPointVelFieldZ, -vDeltaT);
                }
                else if (vAdvectionAccuracy == EAdvectionAccuracy.RK2)
                {
                    m_BackTraceMidPointPosFieldZ.resize(vInputPosField);
                    m_BackTraceMidPointPosFieldZ.plusAlphaX(m_BackTraceInputPointVelFieldZ, -0.5f * vDeltaT);
                    vVelocityField.sampleField(m_BackTraceMidPointPosFieldZ, m_BackTraceMidPointVelFieldZ, vSamplingAlgorithm);
                    voOutputPosField.plusAlphaX(m_BackTraceMidPointVelFieldZ, -vDeltaT);
                }
                else if (vAdvectionAccuracy == EAdvectionAccuracy.RK3)
                {
                    m_BackTraceMidPointPosFieldZ.resize(vInputPosField);
                    m_BackTraceMidPointPosFieldZ.plusAlphaX(m_BackTraceInputPointVelFieldZ, -0.5f * vDeltaT);
                    vVelocityField.sampleField(m_BackTraceMidPointPosFieldZ, m_BackTraceMidPointVelFieldZ, vSamplingAlgorithm);
                    m_BackTraceTwoThirdsPointPosFieldZ.resize(vInputPosField);
                    m_BackTraceTwoThirdsPointPosFieldZ.plusAlphaX(m_BackTraceMidPointVelFieldZ, -0.75f * vDeltaT);
                    vVelocityField.sampleField(m_BackTraceTwoThirdsPointPosFieldZ, m_BackTraceTwoThirdsPointVelFieldZ, vSamplingAlgorithm);
                    voOutputPosField.plusAlphaX(m_BackTraceInputPointVelFieldZ, -(2.0f / 9.0f) * vDeltaT);
                    voOutputPosField.plusAlphaX(m_BackTraceMidPointVelFieldZ, -(3.0f / 9.0f) * vDeltaT);
                    voOutputPosField.plusAlphaX(m_BackTraceTwoThirdsPointVelFieldZ, -(4.0f / 9.0f) * vDeltaT);
                }
                else
                {

                }
            }
            else
            {
                return;
            }
        }

        public void fillBackTraceMiddlePosField
        (
            float vDeltaT,
            CFaceCenteredVectorField vVelocityField,
            EAdvectionAccuracy vAdvectionAccuracy = EAdvectionAccuracy.RK2,
            ESamplingAlgorithm vSamplingAlgorithm = ESamplingAlgorithm.LINEAR
        )
        {
            #region X
            m_AdvectionOutputPointPosXFieldFC.resize(m_AdvectionInputPointPosXFieldFC);

            vVelocityField.sampleField(m_AdvectionInputPointPosXFieldFC, m_BackTraceInputPointVelFieldX, vSamplingAlgorithm);

            if (vAdvectionAccuracy == EAdvectionAccuracy.RK1)
            {
                m_AdvectionOutputPointPosXFieldFC.plusAlphaX(m_BackTraceInputPointVelFieldX, -vDeltaT);
            }
            else if (vAdvectionAccuracy == EAdvectionAccuracy.RK2)
            {
                m_BackTraceMidPointPosFieldX.resize(m_AdvectionInputPointPosXFieldFC);
                m_BackTraceMidPointPosFieldX.plusAlphaX(m_BackTraceInputPointVelFieldX, -0.5f * vDeltaT);
                vVelocityField.sampleField(m_BackTraceMidPointPosFieldX, m_BackTraceMidPointVelFieldX, vSamplingAlgorithm);
                m_AdvectionOutputPointPosXFieldFC.plusAlphaX(m_BackTraceMidPointVelFieldX, -vDeltaT);
            }
            else if (vAdvectionAccuracy == EAdvectionAccuracy.RK3)
            {
                m_BackTraceMidPointPosFieldX.resize(m_AdvectionInputPointPosXFieldFC);
                m_BackTraceMidPointPosFieldX.plusAlphaX(m_BackTraceInputPointVelFieldX, -0.5f * vDeltaT);
                vVelocityField.sampleField(m_BackTraceMidPointPosFieldX, m_BackTraceMidPointVelFieldX, vSamplingAlgorithm);
                m_BackTraceTwoThirdsPointPosFieldX.resize(m_AdvectionInputPointPosXFieldFC);
                m_BackTraceTwoThirdsPointPosFieldX.plusAlphaX(m_BackTraceMidPointVelFieldX, -0.75f * vDeltaT);
                vVelocityField.sampleField(m_BackTraceTwoThirdsPointPosFieldX, m_BackTraceTwoThirdsPointVelFieldX, vSamplingAlgorithm);
                m_AdvectionOutputPointPosXFieldFC.plusAlphaX(m_BackTraceInputPointVelFieldX, -(2.0f / 9.0f) * vDeltaT);
                m_AdvectionOutputPointPosXFieldFC.plusAlphaX(m_BackTraceMidPointVelFieldX, -(3.0f / 9.0f) * vDeltaT);
                m_AdvectionOutputPointPosXFieldFC.plusAlphaX(m_BackTraceTwoThirdsPointVelFieldX, -(4.0f / 9.0f) * vDeltaT);
            }
            else
            {

            }
            #endregion

            #region Y
            m_AdvectionOutputPointPosYFieldFC.resize(m_AdvectionInputPointPosYFieldFC);

            vVelocityField.sampleField(m_AdvectionInputPointPosYFieldFC, m_BackTraceInputPointVelFieldY, vSamplingAlgorithm);

            if (vAdvectionAccuracy == EAdvectionAccuracy.RK1)
            {
                m_AdvectionOutputPointPosYFieldFC.plusAlphaX(m_BackTraceInputPointVelFieldY, -vDeltaT);
            }
            else if (vAdvectionAccuracy == EAdvectionAccuracy.RK2)
            {
                m_BackTraceMidPointPosFieldY.resize(m_AdvectionInputPointPosYFieldFC);
                m_BackTraceMidPointPosFieldY.plusAlphaX(m_BackTraceInputPointVelFieldY, -0.5f * vDeltaT);
                vVelocityField.sampleField(m_BackTraceMidPointPosFieldY, m_BackTraceMidPointVelFieldY, vSamplingAlgorithm);
                m_AdvectionOutputPointPosYFieldFC.plusAlphaX(m_BackTraceMidPointVelFieldY, -vDeltaT);
            }
            else if (vAdvectionAccuracy == EAdvectionAccuracy.RK3)
            {
                m_BackTraceMidPointPosFieldY.resize(m_AdvectionInputPointPosYFieldFC);
                m_BackTraceMidPointPosFieldY.plusAlphaX(m_BackTraceInputPointVelFieldY, -0.5f * vDeltaT);
                vVelocityField.sampleField(m_BackTraceMidPointPosFieldY, m_BackTraceMidPointVelFieldY, vSamplingAlgorithm);
                m_BackTraceTwoThirdsPointPosFieldY.resize(m_AdvectionInputPointPosYFieldFC);
                m_BackTraceTwoThirdsPointPosFieldY.plusAlphaX(m_BackTraceMidPointVelFieldY, -0.75f * vDeltaT);
                vVelocityField.sampleField(m_BackTraceTwoThirdsPointPosFieldY, m_BackTraceTwoThirdsPointVelFieldY, vSamplingAlgorithm);
                m_AdvectionOutputPointPosYFieldFC.plusAlphaX(m_BackTraceInputPointVelFieldY, -(2.0f / 9.0f) * vDeltaT);
                m_AdvectionOutputPointPosYFieldFC.plusAlphaX(m_BackTraceMidPointVelFieldY, -(3.0f / 9.0f) * vDeltaT);
                m_AdvectionOutputPointPosYFieldFC.plusAlphaX(m_BackTraceTwoThirdsPointVelFieldY, -(4.0f / 9.0f) * vDeltaT);
            }
            else
            {

            }
            #endregion

            #region Z
            m_AdvectionOutputPointPosZFieldFC.resize(m_AdvectionInputPointPosZFieldFC);

            vVelocityField.sampleField(m_AdvectionInputPointPosZFieldFC, m_BackTraceInputPointVelFieldZ, vSamplingAlgorithm);

            if (vAdvectionAccuracy == EAdvectionAccuracy.RK1)
            {
                m_AdvectionOutputPointPosZFieldFC.plusAlphaX(m_BackTraceInputPointVelFieldZ, -vDeltaT);
            }
            else if (vAdvectionAccuracy == EAdvectionAccuracy.RK2)
            {
                m_BackTraceMidPointPosFieldZ.resize(m_AdvectionInputPointPosZFieldFC);
                m_BackTraceMidPointPosFieldZ.plusAlphaX(m_BackTraceInputPointVelFieldZ, -0.5f * vDeltaT);
                vVelocityField.sampleField(m_BackTraceMidPointPosFieldZ, m_BackTraceMidPointVelFieldZ, vSamplingAlgorithm);
                m_AdvectionOutputPointPosZFieldFC.plusAlphaX(m_BackTraceMidPointVelFieldZ, -vDeltaT);
            }
            else if (vAdvectionAccuracy == EAdvectionAccuracy.RK3)
            {
                m_BackTraceMidPointPosFieldZ.resize(m_AdvectionInputPointPosZFieldFC);
                m_BackTraceMidPointPosFieldZ.plusAlphaX(m_BackTraceInputPointVelFieldZ, -0.5f * vDeltaT);
                vVelocityField.sampleField(m_BackTraceMidPointPosFieldZ, m_BackTraceMidPointVelFieldZ, vSamplingAlgorithm);
                m_BackTraceTwoThirdsPointPosFieldZ.resize(m_AdvectionInputPointPosZFieldFC);
                m_BackTraceTwoThirdsPointPosFieldZ.plusAlphaX(m_BackTraceMidPointVelFieldZ, -0.75f * vDeltaT);
                vVelocityField.sampleField(m_BackTraceTwoThirdsPointPosFieldZ, m_BackTraceTwoThirdsPointVelFieldZ, vSamplingAlgorithm);
                m_AdvectionOutputPointPosZFieldFC.plusAlphaX(m_BackTraceInputPointVelFieldZ, -(2.0f / 9.0f) * vDeltaT);
                m_AdvectionOutputPointPosZFieldFC.plusAlphaX(m_BackTraceMidPointVelFieldZ, -(3.0f / 9.0f) * vDeltaT);
                m_AdvectionOutputPointPosZFieldFC.plusAlphaX(m_BackTraceTwoThirdsPointVelFieldZ, -(4.0f / 9.0f) * vDeltaT);
            }
            else
            {

            }
            #endregion
        }

        public void backTraceWithIteration
        (
            CCellCenteredVectorField vInputPosField,
            CFaceCenteredVectorField vPreVelocityField,
            CFaceCenteredVectorField vCurVelocityField,
            float vDeltaT,
            CCellCenteredVectorField voOutputPosField,
            EAdvectionAccuracy vAdvectionAccuracy = EAdvectionAccuracy.RK2,
            ESamplingAlgorithm vSamplingAlgorithm = ESamplingAlgorithm.LINEAR
        )
        {
            CGlobalMacroAndFunc._ASSERTE(voOutputPosField.getResolution() == vInputPosField.getResolution());

            int IterNum = 20;

            //CCellCenteredVectorField TempAlphaField = new CCellCenteredVectorField(vInputPosField.getResolution(), vInputPosField.getOrigin(), vInputPosField.getSpacing());
            //CCellCenteredVectorField TempBackTraceIterVelField = new CCellCenteredVectorField(vInputPosField.getResolution(), vInputPosField.getOrigin(), vInputPosField.getSpacing());
            //CCellCenteredVectorField TempBackTraceIterPosField = new CCellCenteredVectorField(vInputPosField.getResolution(), vInputPosField.getOrigin(), vInputPosField.getSpacing());

            //CFaceCenteredVectorField TempExVelField = new CFaceCenteredVectorField(vCurVelocityField);
            //TempExVelField.scale(1.5f);
            //TempExVelField.plusAlphaX(vPreVelocityField, -0.5f);

            //TempExVelField.sampleField(vInputPosField, TempBackTraceIterVelField, vSamplingAlgorithm);
            //TempAlphaField.resize(TempBackTraceIterVelField);
            //TempAlphaField.scale(vDeltaT);

            //for (int i = 0; i < IterNum; i++)
            //{
            //    TempBackTraceIterPosField.resize(vInputPosField);
            //    TempBackTraceIterPosField.plusAlphaX(TempAlphaField, -0.5f);
            //    TempExVelField.sampleField(TempBackTraceIterPosField, TempBackTraceIterVelField, vSamplingAlgorithm);

            //    TempAlphaField.resize(TempBackTraceIterVelField);
            //    TempAlphaField.scale(vDeltaT);
            //}

            //voOutputPosField.resize(vInputPosField);
            //voOutputPosField.plusAlphaX(TempAlphaField, -1.0f);

            CCellCenteredVectorField TempAlphaField = new CCellCenteredVectorField(vInputPosField.getResolution(), vInputPosField.getOrigin(), vInputPosField.getSpacing());
            CCellCenteredVectorField TempBackTraceIterPreVelField = new CCellCenteredVectorField(vInputPosField.getResolution(), vInputPosField.getOrigin(), vInputPosField.getSpacing());
            CCellCenteredVectorField TempBackTraceIterCurVelField = new CCellCenteredVectorField(vInputPosField.getResolution(), vInputPosField.getOrigin(), vInputPosField.getSpacing());
            CCellCenteredVectorField TempBackTraceIterPosField = new CCellCenteredVectorField(vInputPosField.getResolution(), vInputPosField.getOrigin(), vInputPosField.getSpacing());

            vPreVelocityField.sampleField(vInputPosField, TempBackTraceIterPreVelField, vSamplingAlgorithm);
            vCurVelocityField.sampleField(vInputPosField, TempBackTraceIterCurVelField, vSamplingAlgorithm);

            TempAlphaField.resize(TempBackTraceIterCurVelField);
            TempAlphaField.scale(1.5f);
            TempAlphaField.plusAlphaX(TempBackTraceIterPreVelField, -0.5f);
            TempAlphaField.scale(vDeltaT);

            for (int i = 0; i < IterNum; i++)
            {
                TempBackTraceIterPosField.resize(vInputPosField);
                TempBackTraceIterPosField.plusAlphaX(TempAlphaField, -0.5f);
                vPreVelocityField.sampleField(TempBackTraceIterPosField, TempBackTraceIterPreVelField, vSamplingAlgorithm);
                vCurVelocityField.sampleField(TempBackTraceIterPosField, TempBackTraceIterCurVelField, vSamplingAlgorithm);

                TempAlphaField.resize(TempBackTraceIterCurVelField);
                TempAlphaField.scale(1.5f);
                TempAlphaField.plusAlphaX(TempBackTraceIterPreVelField, -0.5f);
                TempAlphaField.scale(vDeltaT);
            }

            voOutputPosField.resize(vInputPosField);
            voOutputPosField.plusAlphaX(TempAlphaField, -1.0f);

            TempAlphaField.free();
            TempBackTraceIterPreVelField.free();
            TempBackTraceIterCurVelField.free();
            TempBackTraceIterPosField.free();
            #region ×¢ÊÍ
            //if (vInputPosField.getResolution() == m_Resolution)
            //{
            //    voOutputPosField.resize(vInputPosField);

            //    CCellCenteredVectorField TempAlphaField = new CCellCenteredVectorField(vInputPosField.getResolution(), vInputPosField.getOrigin(), vInputPosField.getSpacing());
            //    CCellCenteredVectorField TempBackTraceIterPreVelField = new CCellCenteredVectorField(vInputPosField.getResolution(), vInputPosField.getOrigin(), vInputPosField.getSpacing());
            //    CCellCenteredVectorField TempBackTraceIterCurVelField = new CCellCenteredVectorField(vInputPosField.getResolution(), vInputPosField.getOrigin(), vInputPosField.getSpacing());
            //    CCellCenteredVectorField TempBackTraceIterPosField = new CCellCenteredVectorField(vInputPosField.getResolution(), vInputPosField.getOrigin(), vInputPosField.getSpacing());

            //    vPreVelocityField.sampleField(vInputPosField, TempBackTraceIterPreVelField, vSamplingAlgorithm);
            //    vCurVelocityField.sampleField(vInputPosField, TempBackTraceIterCurVelField, vSamplingAlgorithm);

            //    TempAlphaField.resize(TempBackTraceIterCurVelField);
            //    TempAlphaField.scale(1.5f);
            //    TempAlphaField.plusAlphaX(TempBackTraceIterPreVelField, -0.5f);
            //    TempAlphaField.scale(vDeltaT);

            //    for(int i = 0; i < IterNum; i++)
            //    {
            //        TempBackTraceIterPosField.resize(vInputPosField);
            //        TempBackTraceIterPosField.plusAlphaX(TempAlphaField, -0.5f);
            //        vPreVelocityField.sampleField(TempBackTraceIterPosField, TempBackTraceIterPreVelField, vSamplingAlgorithm);
            //        vCurVelocityField.sampleField(TempBackTraceIterPosField, TempBackTraceIterPreVelField, vSamplingAlgorithm);

            //        TempAlphaField.resize(TempBackTraceIterCurVelField);
            //        TempAlphaField.scale(1.5f);
            //        TempAlphaField.plusAlphaX(TempBackTraceIterPreVelField, -0.5f);
            //        TempAlphaField.scale(vDeltaT);
            //    }

            //    voOutputPosField.resize(vInputPosField);
            //    voOutputPosField.plusAlphaX(TempAlphaField, -1.0f);
            //}
            //else if (vInputPosField.getResolution() == m_Resolution + new Vector3Int(1, 0, 0))
            //{
            //    voOutputPosField.resize(vInputPosField);

            //    CCellCenteredVectorField TempAlphaField = new CCellCenteredVectorField(vInputPosField.getResolution(), vInputPosField.getOrigin(), vInputPosField.getSpacing());
            //    CCellCenteredVectorField TempBackTraceIterPreVelField = new CCellCenteredVectorField(vInputPosField.getResolution(), vInputPosField.getOrigin(), vInputPosField.getSpacing());
            //    CCellCenteredVectorField TempBackTraceIterCurVelField = new CCellCenteredVectorField(vInputPosField.getResolution(), vInputPosField.getOrigin(), vInputPosField.getSpacing());
            //    CCellCenteredVectorField TempBackTraceIterPosField = new CCellCenteredVectorField(vInputPosField.getResolution(), vInputPosField.getOrigin(), vInputPosField.getSpacing());

            //    vPreVelocityField.sampleField(vInputPosField, TempBackTraceIterPreVelField, vSamplingAlgorithm);
            //    vCurVelocityField.sampleField(vInputPosField, TempBackTraceIterCurVelField, vSamplingAlgorithm);

            //    TempAlphaField.resize(TempBackTraceIterCurVelField);
            //    TempAlphaField.scale(1.5f);
            //    TempAlphaField.plusAlphaX(TempBackTraceIterPreVelField, -0.5f);
            //    TempAlphaField.scale(vDeltaT);

            //    for (int i = 0; i < IterNum; i++)
            //    {
            //        TempBackTraceIterPosField.resize(vInputPosField);
            //        TempBackTraceIterPosField.plusAlphaX(TempAlphaField, -0.5f);
            //        vPreVelocityField.sampleField(TempBackTraceIterPosField, TempBackTraceIterPreVelField, vSamplingAlgorithm);
            //        vCurVelocityField.sampleField(TempBackTraceIterPosField, TempBackTraceIterPreVelField, vSamplingAlgorithm);

            //        TempAlphaField.resize(TempBackTraceIterCurVelField);
            //        TempAlphaField.scale(1.5f);
            //        TempAlphaField.plusAlphaX(TempBackTraceIterPreVelField, -0.5f);
            //        TempAlphaField.scale(vDeltaT);
            //    }

            //    voOutputPosField.resize(vInputPosField);
            //    voOutputPosField.plusAlphaX(TempAlphaField, -1.0f);
            //}
            //else if (vInputPosField.getResolution() == m_Resolution + new Vector3Int(0, 1, 0))
            //{
            //    voOutputPosField.resize(vInputPosField);

            //    vVelocityField.sampleField(vInputPosField, m_BackTraceInputPointVelFieldY, vSamplingAlgorithm);

            //    if (vAdvectionAccuracy == EAdvectionAccuracy.RK1)
            //    {
            //        voOutputPosField.plusAlphaX(m_BackTraceInputPointVelFieldY, -vDeltaT);
            //    }
            //    else if (vAdvectionAccuracy == EAdvectionAccuracy.RK2)
            //    {
            //        m_BackTraceMidPointPosFieldY.resize(vInputPosField);
            //        m_BackTraceMidPointPosFieldY.plusAlphaX(m_BackTraceInputPointVelFieldY, -0.5f * vDeltaT);
            //        vVelocityField.sampleField(m_BackTraceMidPointPosFieldY, m_BackTraceMidPointVelFieldY, vSamplingAlgorithm);
            //        voOutputPosField.plusAlphaX(m_BackTraceMidPointVelFieldY, -vDeltaT);
            //    }
            //    else if (vAdvectionAccuracy == EAdvectionAccuracy.RK3)
            //    {
            //        m_BackTraceMidPointPosFieldY.resize(vInputPosField);
            //        m_BackTraceMidPointPosFieldY.plusAlphaX(m_BackTraceInputPointVelFieldY, -0.5f * vDeltaT);
            //        vVelocityField.sampleField(m_BackTraceMidPointPosFieldY, m_BackTraceMidPointVelFieldY, vSamplingAlgorithm);
            //        m_BackTraceTwoThirdsPointPosFieldY.resize(vInputPosField);
            //        m_BackTraceTwoThirdsPointPosFieldY.plusAlphaX(m_BackTraceMidPointVelFieldY, -0.75f * vDeltaT);
            //        vVelocityField.sampleField(m_BackTraceTwoThirdsPointPosFieldY, m_BackTraceTwoThirdsPointVelFieldY, vSamplingAlgorithm);
            //        voOutputPosField.plusAlphaX(m_BackTraceInputPointVelFieldY, -(2.0f / 9.0f) * vDeltaT);
            //        voOutputPosField.plusAlphaX(m_BackTraceMidPointVelFieldY, -(3.0f / 9.0f) * vDeltaT);
            //        voOutputPosField.plusAlphaX(m_BackTraceTwoThirdsPointVelFieldY, -(4.0f / 9.0f) * vDeltaT);
            //    }
            //    else
            //    {

            //    }
            //}
            //else if (vInputPosField.getResolution() == m_Resolution + new Vector3Int(0, 0, 1))
            //{
            //    voOutputPosField.resize(vInputPosField);

            //    vVelocityField.sampleField(vInputPosField, m_BackTraceInputPointVelFieldZ, vSamplingAlgorithm);

            //    if (vAdvectionAccuracy == EAdvectionAccuracy.RK1)
            //    {
            //        voOutputPosField.plusAlphaX(m_BackTraceInputPointVelFieldZ, -vDeltaT);
            //    }
            //    else if (vAdvectionAccuracy == EAdvectionAccuracy.RK2)
            //    {
            //        m_BackTraceMidPointPosFieldZ.resize(vInputPosField);
            //        m_BackTraceMidPointPosFieldZ.plusAlphaX(m_BackTraceInputPointVelFieldZ, -0.5f * vDeltaT);
            //        vVelocityField.sampleField(m_BackTraceMidPointPosFieldZ, m_BackTraceMidPointVelFieldZ, vSamplingAlgorithm);
            //        voOutputPosField.plusAlphaX(m_BackTraceMidPointVelFieldZ, -vDeltaT);
            //    }
            //    else if (vAdvectionAccuracy == EAdvectionAccuracy.RK3)
            //    {
            //        m_BackTraceMidPointPosFieldZ.resize(vInputPosField);
            //        m_BackTraceMidPointPosFieldZ.plusAlphaX(m_BackTraceInputPointVelFieldZ, -0.5f * vDeltaT);
            //        vVelocityField.sampleField(m_BackTraceMidPointPosFieldZ, m_BackTraceMidPointVelFieldZ, vSamplingAlgorithm);
            //        m_BackTraceTwoThirdsPointPosFieldZ.resize(vInputPosField);
            //        m_BackTraceTwoThirdsPointPosFieldZ.plusAlphaX(m_BackTraceMidPointVelFieldZ, -0.75f * vDeltaT);
            //        vVelocityField.sampleField(m_BackTraceTwoThirdsPointPosFieldZ, m_BackTraceTwoThirdsPointVelFieldZ, vSamplingAlgorithm);
            //        voOutputPosField.plusAlphaX(m_BackTraceInputPointVelFieldZ, -(2.0f / 9.0f) * vDeltaT);
            //        voOutputPosField.plusAlphaX(m_BackTraceMidPointVelFieldZ, -(3.0f / 9.0f) * vDeltaT);
            //        voOutputPosField.plusAlphaX(m_BackTraceTwoThirdsPointVelFieldZ, -(4.0f / 9.0f) * vDeltaT);
            //    }
            //    else
            //    {

            //    }
            //}
            //else
            //{
            //    return;
            //}
            #endregion
        }
        #endregion

        private Vector3Int m_Resolution;

        private CCellCenteredVectorField m_AdvectionInputPointPosFieldCC;
		private CCellCenteredVectorField m_AdvectionOutputPointPosFieldCC;
		private CCellCenteredVectorField m_AdvectionInputPointPosXFieldFC;
		private CCellCenteredVectorField m_AdvectionInputPointPosYFieldFC;
		private CCellCenteredVectorField m_AdvectionInputPointPosZFieldFC;
		private CCellCenteredVectorField m_AdvectionOutputPointPosXFieldFC;
		private CCellCenteredVectorField m_AdvectionOutputPointPosYFieldFC;
		private CCellCenteredVectorField m_AdvectionOutputPointPosZFieldFC;
        private CCellCenteredVectorField m_MacCormackAdvectionOutputPointPosXFieldFC;
        private CCellCenteredVectorField m_MacCormackAdvectionOutputPointPosYFieldFC;
        private CCellCenteredVectorField m_MacCormackAdvectionOutputPointPosZFieldFC;

        private CCellCenteredVectorField m_BackTraceInputPointVelField;
		private CCellCenteredVectorField m_BackTraceMidPointPosField;
		private CCellCenteredVectorField m_BackTraceMidPointVelField;
		private CCellCenteredVectorField m_BackTraceTwoThirdsPointPosField;
		private CCellCenteredVectorField m_BackTraceTwoThirdsPointVelField;

		private CCellCenteredVectorField m_BackTraceInputPointVelFieldX;
		private CCellCenteredVectorField m_BackTraceMidPointPosFieldX;
		private CCellCenteredVectorField m_BackTraceMidPointVelFieldX;
		private CCellCenteredVectorField m_BackTraceTwoThirdsPointPosFieldX;
		private CCellCenteredVectorField m_BackTraceTwoThirdsPointVelFieldX;

		private CCellCenteredVectorField m_BackTraceInputPointVelFieldY;
		private CCellCenteredVectorField m_BackTraceMidPointPosFieldY;
		private CCellCenteredVectorField m_BackTraceMidPointVelFieldY;
		private CCellCenteredVectorField m_BackTraceTwoThirdsPointPosFieldY;
		private CCellCenteredVectorField m_BackTraceTwoThirdsPointVelFieldY;

		private CCellCenteredVectorField m_BackTraceInputPointVelFieldZ;
		private CCellCenteredVectorField m_BackTraceMidPointPosFieldZ;
		private CCellCenteredVectorField m_BackTraceMidPointVelFieldZ;
		private CCellCenteredVectorField m_BackTraceTwoThirdsPointPosFieldZ;
		private CCellCenteredVectorField m_BackTraceTwoThirdsPointVelFieldZ;

        private CCellCenteredScalarField m_MacCormackForwardScalarField;
        private CCellCenteredVectorField m_MacCormackForwardVectorField;
        private CFaceCenteredVectorField m_MacCormackForwardVelField;
	}
}
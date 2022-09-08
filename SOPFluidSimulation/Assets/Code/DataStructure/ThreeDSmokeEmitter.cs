using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace EulerFluidEngine
{
    public class CThreeDSmokeEmitter
    {
        public CThreeDSmokeEmitter()
        {
            m_EmitteStartFrame = -1;
            m_EmitteEndFrame = -1;
            m_Emittedensity = 0.0f;
            m_EmitteTemperature = 0.0f;
            m_EmitterCurrentPos = Vector3.zero;
        }

        public CThreeDSmokeEmitter(int vEmitteStartFrame, int vEmitteEndFrame, float vEmittedensity, float vEmitteTemperature, Vector3 vEmitterInitialPos)
        {
            ResizeThreeDSmokeEmitter(vEmitteStartFrame, vEmitteEndFrame, vEmittedensity, vEmitteTemperature, vEmitterInitialPos);
        }

        public void ResizeThreeDSmokeEmitter(int vEmitteStartFrame, int vEmitteEndFrame, float vEmittedensity, float vEmitteTemperature, Vector3 vEmitterInitialPos, EmitteVelFunc vEmitteVelFunc = null, EmitterVelFunc vEmitterVelFunc = null)
        {
            m_EmitteStartFrame = vEmitteStartFrame;
            m_EmitteEndFrame = vEmitteEndFrame;
            m_Emittedensity = vEmittedensity;
            m_EmitteTemperature = vEmitteTemperature;
            m_EmitterCurrentPos = vEmitterInitialPos;

            if (vEmitteVelFunc != null)
            {
                m_EmitteVelFunc = vEmitteVelFunc;
            }
            else
            {
                m_EmitteVelFunc = __DefaultEmitteVelFunc;
            }

            if (vEmitterVelFunc != null)
            {
                m_EmitterVelFunc = vEmitterVelFunc;
            }
            else
            {
                m_EmitterVelFunc = __DefaultEmitterVelFunc;
            }


        }

        private Vector3 __DefaultEmitterVelFunc(int vCurSimulationFrame)
        {
            return new Vector3(0, 0, 0);
        }

        private Vector3 __DefaultEmitteVelFunc(Vector3 vPos)
        {
            return new Vector3(0, 0, 0);
        }

        private void __updateEmitterPos(int vCurSimulationFrame, float vDeltaT)
        {
            m_EmitterCurrentPos += m_EmitterVelFunc(vCurSimulationFrame) * vDeltaT;
        }

        public void emitteSmoke(int vCurSimulationFrame, float vDeltaT, CCellCenteredScalarField voDensityField, CCellCenteredScalarField voTemperatureField)
        {
            __updateEmitterPos(vCurSimulationFrame, vDeltaT);

            if (vCurSimulationFrame >= m_EmitteStartFrame && vCurSimulationFrame < m_EmitteEndFrame)
            {
                //Debug.Log(CMathTool.getAbsMaxValue(voDensityField.getGridData()));
                CSmokeEmitterInvoker.emitte3DSmokeDensityAndTemperatureInvoker(m_Emittedensity, m_EmitteTemperature, voDensityField, voTemperatureField);
                //Debug.Log(CMathTool.getAbsMaxValue(voDensityField.getGridData()));
            }
        }

        private int m_EmitteStartFrame;
        private int m_EmitteEndFrame;
        private float m_Emittedensity = 0.0f;
        private float m_EmitteTemperature;
        private Vector3 m_EmitterCurrentPos;

        EmitteVelFunc m_EmitteVelFunc;
        EmitterVelFunc m_EmitterVelFunc;
    }
}
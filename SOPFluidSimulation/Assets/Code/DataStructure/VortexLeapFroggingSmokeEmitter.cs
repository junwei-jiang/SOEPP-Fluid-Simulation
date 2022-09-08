using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace EulerFluidEngine
{
    public class CVortexLeapFroggingSmokeEmitter
    {
        public CVortexLeapFroggingSmokeEmitter()
        {
            m_EmitteStartFrame = -1;
            m_EmitteEndFrame = -1;
            m_Emittedensity = 0.0f;
            m_EmitteTemperature = 0.0f;
            m_EmitterCurrentPos = Vector3.zero;

            m_EmitteVelFunc = __DefaultEmitteVelFunc;
            m_EmitterVelFunc = __DefaultEmitterVelFunc;
        }

        public CVortexLeapFroggingSmokeEmitter(int vEmitteStartFrame, int vEmitteEndFrame, float vEmittedensity, float vEmitteTemperature, Vector3 vEmitterInitialPos, EmitteVelFunc vEmitteVelFunc = null, EmitterVelFunc vEmitterVelFunc = null)
        {
            ResizeVortexLeapFroggingSmokeEmitter(vEmitteStartFrame, vEmitteEndFrame, vEmittedensity, vEmitteTemperature, vEmitterInitialPos, vEmitteVelFunc, vEmitterVelFunc);
        }

        public void ResizeVortexLeapFroggingSmokeEmitter(int vEmitteStartFrame, int vEmitteEndFrame, float vEmittedensity, float vEmitteTemperature, Vector3 vEmitterInitialPos, EmitteVelFunc vEmitteVelFunc = null, EmitterVelFunc vEmitterVelFunc = null)
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

        public void emitteSmoke(int vCurSimulationFrame, float vDeltaT, CCellCenteredScalarField voDensityField1, CCellCenteredScalarField voDensityField2, CCellCenteredScalarField voTemperatureField, CFaceCenteredVectorField voVelField)
        {
            __updateEmitterPos(vCurSimulationFrame, vDeltaT);

            if (vCurSimulationFrame >= m_EmitteStartFrame && vCurSimulationFrame < m_EmitteEndFrame)
            {
                CSmokeEmitterInvoker.emitteVortexLeapFroggingDensityAndTemperatureInvoker(m_Emittedensity, m_EmitteTemperature, voDensityField1, voTemperatureField, true);
                CSmokeEmitterInvoker.emitteVortexLeapFroggingDensityAndTemperatureInvoker(m_Emittedensity, m_EmitteTemperature, voDensityField2, voTemperatureField, false);
                CSmokeEmitterInvoker.emitteVortexLeapFroggingVelInvoker(voVelField);
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
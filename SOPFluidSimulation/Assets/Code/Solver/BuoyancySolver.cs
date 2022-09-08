using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace EulerFluidEngine
{
    public class CBuoyancySolver
    {
        public CBuoyancySolver(float vAlpha = 0.0f, float vBeta = 5.0f)
        {
            setBuoyancy(vAlpha, vBeta);
        }

        ~CBuoyancySolver() { }

        public void setBuoyancy(float vAlpha = 0.0f, float vBeta = 5.0f)
        {
            m_Alpha = vAlpha;
            m_Beta = vBeta;
        }

        public void applyBuoyancy(float vDeltaT, CCellCenteredScalarField vDensityField, CCellCenteredScalarField vTemperatureField,CFaceCenteredVectorField vioVelField)
        {
            CEulerSolverToolInvoker.applyBuoyancyInvoker(vDeltaT, m_Alpha, m_Beta, vDensityField, vTemperatureField, vioVelField);
        }

        private float m_Alpha = 0.0f;
        private float m_Beta = 0.0f;
    }
}
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace EulerFluidEngine
{
    public class CViscositySolver
    {
        public CViscositySolver(Vector3Int vResolution, Vector3 vOrigin, Vector3 vSpacing, float vViscosity = 1e-6f)
        {
            resizeViscositySolver(vResolution, vOrigin, vSpacing, vViscosity);
        }

        ~CViscositySolver()
        {
            free();
        }

        public void free()
        {
            m_TempField.free();
        }

        public void resizeViscositySolver(Vector3Int vResolution, Vector3 vOrigin, Vector3 vSpacing, float vViscosity = 1e-6f)
        {
            if(m_TempField == null)
            {
                m_TempField = new CFaceCenteredVectorField(vResolution, vOrigin, vSpacing);
            }
            else
            {
                m_TempField.resize(vResolution, vOrigin, vSpacing);
            }

            setViscosity(vViscosity);
        }

        public void setViscosity(float vViscosity)
        {
            m_Viscosity = vViscosity;
        }

        public void applyViscosityForces(float vDeltaT, CFaceCenteredVectorField vioVelField)
        {
            Vector3 Spacing = vioVelField.getSpacing();
            Vector3 DiffuseCoff = m_Viscosity * new Vector3(vDeltaT / (Spacing.x * Spacing.x), vDeltaT/ (Spacing.y * Spacing.y), vDeltaT / (Spacing.z * Spacing.z));
            CEulerSolverToolInvoker.diffuseVelFieldInvoker(20, DiffuseCoff, vioVelField, m_TempField);
        }

        private float m_Viscosity = 1e-6f;
        private CFaceCenteredVectorField m_TempField;
    }
}
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using EulerFluidEngine;

namespace EulerFluidEngine
{
    public class CExternalForcesSolver
    {
		public CExternalForcesSolver()
        {
            resizeExternalForcesSolver();
        }

        ~CExternalForcesSolver()
        {

        }

        public void resizeExternalForcesSolver()
        {
			m_ExternalForces = new Vector3(0, 0, 0);
		}

        public void addExternalForces(Vector3 vExternalForces)
        {
            m_ExternalForces += vExternalForces;
        }

        public void setExternalForces(Vector3 vExternalForces)
        {
            m_ExternalForces = vExternalForces;
        }

		public void applyExternalForces
		(
			CFaceCenteredVectorField vioVelField,
			float vDeltaT
	    )
        {
            Vector3 m_TotalExternalForces = m_Gravity + m_ExternalForces;
            CMathTool.plus(vioVelField.getGridDataX(), m_TotalExternalForces.x * vDeltaT);
            CMathTool.plus(vioVelField.getGridDataY(), m_TotalExternalForces.y * vDeltaT);
            CMathTool.plus(vioVelField.getGridDataZ(), m_TotalExternalForces.z * vDeltaT);
        }

        private Vector3 m_Gravity = new Vector3(0, -9.8f, 0);
        private Vector3 m_ExternalForces = new Vector3(0, 0, 0);
    }
}
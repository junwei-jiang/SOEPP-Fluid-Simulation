using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace EulerFluidEngine
{
    public class FluidSource : MonoBehaviour
    {
        public SBoundingBox Domain = new SBoundingBox();
        public float DeltaT = 1;

        [SerializeField]
        private bool m_IsActive = true;
        private float m_AccumulatedTime = 0;
        private bool m_IsTriggered = false;

        public FluidSource() { }

        public FluidSource(SBoundingBox vDomain, float vDeltaT, bool vIsActive)
        {
            Domain = vDomain;
            DeltaT = vDeltaT;
            m_IsActive = vIsActive;
        }


        public void Update()
        {
            if (m_IsActive)
            {
                m_AccumulatedTime += Time.deltaTime;
                if (m_AccumulatedTime >= DeltaT)
                {
                    m_AccumulatedTime -= DeltaT;
                    m_IsTriggered = true;
                }
            }
        }

        public bool isTriggerd()
        {
            if(m_IsTriggered)
            {
                m_IsTriggered = false;
                return true;
            }
            else
            {
                return false;
            }
        }

        public void resetAndActive()
        {
            m_AccumulatedTime = 0;
            m_IsTriggered = false;

            m_IsActive = true;
        }

        public void setActive(bool vIsActive)
        {
            m_IsActive = vIsActive;
        }
    }
}

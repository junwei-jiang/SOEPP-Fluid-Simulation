using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using EulerFluidEngine;
using SDFr;

namespace EulerFluidEngine
{
    [Serializable]
    public class EulerFluid : MonoBehaviour
    {
        void Start()
        {
            __computeTrueAttributes();
            m_EulerFluid = new CGridFluidSolver(m_DeltaT, m_Resolution, m_Origin, m_Spacing, m_AdvectionAlgorithm);
            m_EulerFluid.addExternalForce(m_ExternalForce);
            m_EulerFluid.getPressureSolver().getPCGSolver().setThreshold(m_CGThreshold);
            m_EulerFluid.setExtrapolatingNums(m_ExtrapolatingNums);

            Vector3[] OriginalVelocities = new Vector3[BoundaryObjects.Count];
            for (int i = 0; i < OriginalVelocities.Length; i++)
            {
                OriginalVelocities[i] = new Vector3(0, 0, 0);
            }
            m_EulerFluid.addDynamicBoundarys(BoundaryObjects, OriginalVelocities);

            m_EulerFluid.generateFluid(m_InitialFluidDomainMin, m_InitialFluidDomainMax, m_MaxParticlesNum, m_NumOfParticlesPerGrid, m_MixingCoefficient, m_CFLNumber);
            
            m_ParticlesNum = m_EulerFluid.getParticlesAdvectionSolver().getEulerParticles().getNumOfParticles();

            VisualMaterial.SetFloat("_ParticlesRadius", 0.025f);
        }

        void Update()
        {
            m_EulerFluid.update();

            VisualMaterial.SetBuffer("ParticlesPos", m_EulerFluid.getParticlesAdvectionSolver().getEulerParticles().getParticlesPos());
            VisualMaterial.SetBuffer("ParticlesVel", m_EulerFluid.getParticlesAdvectionSolver().getEulerParticles().getParticlesVel());
        }

        void OnRenderObject()
        {
            VisualMaterial.SetPass(0);
            Graphics.DrawProceduralNow(MeshTopology.Quads, (int)m_ParticlesNum * 4);
        }

        private void __computeTrueAttributes()
        {
            Vector3 SimulationRange = m_SimulationDomainMax - m_SimulationDomainMin;
            m_Spacing = new Vector3(SimulationRange.x / m_Resolution.x, SimulationRange.y / m_Resolution.y, SimulationRange.z / m_Resolution.z);
            m_Origin = m_SimulationDomainMin - m_Spacing;
            m_Resolution += new Vector3Int(2, 2, 2);
        }

        #region Editable Attributes

        public Vector3 m_SimulationDomainMin;
        public Vector3 m_SimulationDomainMax;
        public Vector3 m_InitialFluidDomainMin;
        public Vector3 m_InitialFluidDomainMax;
        public Vector3Int m_Resolution;
        private Vector3 m_Origin;
        private Vector3 m_Spacing;
        public float m_DeltaT = 0.05f;

        public EAdvectionAlgorithm m_AdvectionAlgorithm = EAdvectionAlgorithm.MixPICAndFLIP;
        public float m_MixingCoefficient = 0.01f;
        public int m_MaxParticlesNum = 64 * 64 * 64 * 8;
        public int m_NumOfParticlesPerGrid = 8;
        public float m_CFLNumber = 1.0f;
        private int m_ParticlesNum;

        public Vector3 m_ExternalForce = new Vector3(0, 0, 0);

        public float m_CGThreshold = 1e-4f;

        public int m_ExtrapolatingNums = 20;

        public CGridFluidSolver m_EulerFluid;

        public Material VisualMaterial;

        public List<GameObject> BoundaryObjects;
        #endregion
    }

}